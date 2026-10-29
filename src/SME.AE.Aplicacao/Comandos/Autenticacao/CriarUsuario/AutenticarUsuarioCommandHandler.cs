using MediatR;
using SME.AE.Aplicacao.Comandos.Autenticacao.AutenticarUsuario;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Extensoes;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using SME.AE.Aplicacao.Consultas.ObterUsuarioCoreSSO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.Autenticacao.CriarUsuario
{
    public class AutenticarUsuarioCommandHandler : IRequestHandler<AutenticarUsuarioCommand, RespostaApi>
    {
        private readonly IUsuarioRepository _repository;
        private readonly IUsuarioCoreSSORepositorio _repositoryCoreSSO;
        private readonly IMediator _mediator;

        public AutenticarUsuarioCommandHandler(
            IUsuarioRepository repository,
            IUsuarioCoreSSORepositorio repositoryCoreSSO,
            IMediator mediator)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _repositoryCoreSSO = repositoryCoreSSO ?? throw new ArgumentNullException(nameof(repositoryCoreSSO));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<RespostaApi> Handle(AutenticarUsuarioCommand request, CancellationToken cancellationToken)
        {
            var validacao = await new AutenticarUsuarioUseCaseValidatior().ValidateAsync(request, cancellationToken);
            if (!validacao.IsValid)
                return RespostaApi.Falha(validacao.Errors);

            var usuarioCoreSSO = await _mediator.Send(new ObterUsuarioCoreSSOQuery(request.Cpf), cancellationToken);
            var usuarioApp = await _repository.ObterPorCpf(request.Cpf);

            var primeiroAcesso = usuarioCoreSSO == null || usuarioApp == null || usuarioApp.PrimeiroAcesso;

            var erroValidacaoAcesso = ValidarCredenciaisOuDataDeNascimento(request, usuarioCoreSSO, primeiroAcesso);
            if (erroValidacaoAcesso != null) return erroValidacaoAcesso;

            var usuarioAlunos = await ObterAlunosDoResponsavelAsync(request.Cpf, cancellationToken);

            if (usuarioAlunos == null || !usuarioAlunos.Any())
            {
                await ExcluirEDesativarUsuarioInvalidoAsync(request.Cpf, usuarioApp, usuarioCoreSSO);
                return RespostaApi.Falha("Este CPF não está relacionado como responsável de um aluno ativo na rede municipal.");
            }

            if (primeiroAcesso)
            {
                var erroValidacaoPrimeiroAcesso = ValidarRegrasParaPrimeiroAcesso(request, usuarioAlunos);
                if (erroValidacaoPrimeiroAcesso != null) return erroValidacaoPrimeiroAcesso;
            }

            var grupos = await _repositoryCoreSSO.SelecionarGrupos();
            primeiroAcesso = primeiroAcesso || VerificarSeUsuarioEstaSemGrupo(usuarioCoreSSO, grupos);

            await AtualizarCoreSSOSenecessarioAsync(usuarioCoreSSO, request.Senha);

            var usuarioParaSeBasear = usuarioAlunos.OrderByDescending(a => a.DataAtualizacao).First();
            usuarioApp = await CriaOuAtualizaUsuarioAppAsync(request.Cpf, usuarioApp, usuarioParaSeBasear, primeiroAcesso);
            usuarioApp.PrimeiroAcesso = usuarioApp.PrimeiroAcesso || primeiroAcesso;

            var atualizarDadosCadastrais = VerificarAtualizacaoCadastral(usuarioParaSeBasear);
            return MapearResposta(usuarioParaSeBasear, usuarioApp, primeiroAcesso, atualizarDadosCadastrais || primeiroAcesso);
        }

        #region Métodos de Validação

        private static RespostaApi ValidarCredenciaisOuDataDeNascimento(AutenticarUsuarioCommand request, RetornoUsuarioCoreSSO usuarioCoreSSO, bool primeiroAcesso)
        {
            if (!primeiroAcesso)
            {
                if (!Criptografia.EqualsSenha(request.Senha, usuarioCoreSSO.Senha, usuarioCoreSSO.TipoCriptografia))
                    return RespostaApi.Falha("Usuário ou senha incorretos.");
            }
            else
            {
                if (!TentarExtrairDataNascimentoDaSenha(request.Senha, out var dataNascimentoParsed))
                    return RespostaApi.Falha("Data de nascimento inválida.");

                request.DataNascimento = dataNascimentoParsed;
            }

            return null;
        }

        private static RespostaApi ValidarRegrasParaPrimeiroAcesso(AutenticarUsuarioCommand request, IEnumerable<DadosResponsavelAluno> usuarioAlunos)
        {
            var alunoCorrespondente = usuarioAlunos.FirstOrDefault(w => w.DataNascimentoAluno == request.DataNascimento);

            if (alunoCorrespondente == null)
                return RespostaApi.Falha("Data de Nascimento inválida.");

            if (alunoCorrespondente.TipoSigilo == (int)AlunoTipoSigilo.Restricao)
                return RespostaApi.Falha("Usuário não cadastrado, qualquer dúvida procure a unidade escolar.");

            return null;
        }

        private static bool VerificarSeUsuarioEstaSemGrupo(RetornoUsuarioCoreSSO usuarioCoreSSO, IEnumerable<Guid> grupos)
        {
            if (usuarioCoreSSO?.Grupos == null || !usuarioCoreSSO.Grupos.Any())
                return true;

            return !grupos.Any(g => usuarioCoreSSO.Grupos.Contains(g));
        }

        #endregion

        #region Métodos Auxiliares

        private async Task<IEnumerable<DadosResponsavelAluno>> ObterAlunosDoResponsavelAsync(string cpf, CancellationToken cancellationToken)
        {
            return await _mediator.Send(new ObterDadosResponsavelQuery(cpf), cancellationToken);
        }

        private static bool TentarExtrairDataNascimentoDaSenha(string senha, out DateTime dataNascimento)
        {
            var senhaLimpa = new string((senha ?? string.Empty).Where(char.IsDigit).ToArray());
            return DateTime.TryParseExact(senhaLimpa, "ddMMyyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dataNascimento);
        }

        private async Task ExcluirEDesativarUsuarioInvalidoAsync(string cpf, Dominio.Entidades.Usuario usuarioApp, RetornoUsuarioCoreSSO usuarioCoreSSO)
        {
            if (usuarioApp != null)
                await _repository.ExcluirUsuario(cpf);

            if (usuarioCoreSSO != null)
                await _repositoryCoreSSO.AlterarStatusUsuario(usuarioCoreSSO.UsuId, StatusUsuarioCoreSSO.Inativo);
        }

        private async Task AtualizarCoreSSOSenecessarioAsync(RetornoUsuarioCoreSSO usuarioCoreSSO, string senhaOriginal)
        {
            if (usuarioCoreSSO == null) return;

            if (usuarioCoreSSO.Status == (int)StatusUsuarioCoreSSO.Inativo)
                await _repositoryCoreSSO.AlterarStatusUsuario(usuarioCoreSSO.UsuId, StatusUsuarioCoreSSO.Ativo);

            if (usuarioCoreSSO.TipoCriptografia != TipoCriptografia.TripleDES)
            {
                var senhaCriptografada = Criptografia.CriptografarSenhaTripleDES(senhaOriginal);
                await _repositoryCoreSSO.AtualizarCriptografiaUsuario(usuarioCoreSSO.UsuId, senhaCriptografada);
            }
        }

        private async Task<Dominio.Entidades.Usuario> CriaOuAtualizaUsuarioAppAsync(string cpf, Dominio.Entidades.Usuario usuarioApp, DadosResponsavelAluno usuarioEol, bool primeiroAcesso)
        {
            if (usuarioApp != null)
            {
                usuarioApp.AtualizarLogin(primeiroAcesso);
                await _repository.AltualizarUltimoAcessoPrimeiroUsuario(usuarioApp);
            }
            else
            {
                var novoUsuario = new Dominio.Entidades.Usuario
                {
                    Cpf = cpf,
                    Excluido = false,
                    UltimoLogin = DateTime.Now,
                    PrimeiroAcesso = primeiroAcesso
                };
                await _repository.SalvarAsync(novoUsuario);
            }

            return await _repository.ObterUsuarioNaoExcluidoPorCpf(cpf);
        }

        private static bool VerificarAtualizacaoCadastral(DadosResponsavelAluno usuario)
        {
            return usuario.DataNascimento == null ||
                   string.IsNullOrWhiteSpace(usuario.NomeMae) ||
                   string.IsNullOrWhiteSpace(usuario.Email) ||
                   string.IsNullOrWhiteSpace(usuario.NumeroCelular);
        }

        private RespostaApi MapearResposta(DadosResponsavelAluno usuarioEol, Dominio.Entidades.Usuario usuarioApp, bool primeiroAcesso, bool atualizarDadosCadastrais)
        {
            return RespostaApi.Sucesso(new RespostaAutenticar
            {
                Cpf = usuarioEol.Cpf,
                Email = usuarioEol.Email,
                Id = usuarioApp.Id,
                Nome = usuarioEol.Nome,
                DataNascimento = usuarioEol.DataNascimento,
                NomeMae = usuarioEol.NomeMae,
                PrimeiroAcesso = primeiroAcesso,
                AtualizarDadosCadastrais = atualizarDadosCadastrais,
                Celular = usuarioEol.ObterCelularComDDD(),
                Token = "",
                UltimaAtualizacao = usuarioEol.DataAtualizacao
            });
        }

        #endregion
    }
}