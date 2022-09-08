using FluentValidation.Results;
using MediatR;
using SME.AE.Aplicacao.Comandos.Autenticacao.AutenticarUsuario;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Extensoes;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Interfaces.Servicos;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Consultas.ObterUsuarioCoreSSO;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.Autenticacao.CriarUsuario
{
    public class AutenticarUsuarioCommandHandler : IRequestHandler<AutenticarUsuarioCommand, RespostaApi>
    {
        private readonly IAutenticacaoService _autenticacaoService;
        private readonly IUsuarioRepository _repository;
        private readonly IUsuarioCoreSSORepositorio _repositoryCoreSSO;
        private readonly IMediator mediator;

        public AutenticarUsuarioCommandHandler(IAutenticacaoService autenticacaoService, IUsuarioRepository repository, IUsuarioCoreSSORepositorio repositoryCoreSSO, IMediator mediator)
        {
            _autenticacaoService = autenticacaoService;
            _repositoryCoreSSO = repositoryCoreSSO;
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _repository = repository;
        }

        public async Task<RespostaApi> Handle(AutenticarUsuarioCommand request, CancellationToken cancellationToken)
        {
            bool primeiroAcesso = false;            

            var validator = new AutenticarUsuarioUseCaseValidatior();
            var validacao = validator.Validate(request);

            if (!validacao.IsValid)
                return RespostaApi.Falha(validacao.Errors);

            //verificar se o usuário está cadastrado no CoreSSO
            var usuarioCoreSSO = await mediator.Send(new ObterUsuarioCoreSSOQuery(request.Cpf));

            string senhaCriptografada = string.Empty;

            //se for primeiro acesso
            if (usuarioCoreSSO == null)
            {
                primeiroAcesso = true;
                var senha = Regex.Replace(request.Senha, @"\-\/", "");

                try
                {
                    request.DataNascimento = DateTime.ParseExact(senha, "ddMMyyyy", CultureInfo.InvariantCulture);
                }
                catch
                {
                    validacao.Errors.Add(new ValidationFailure("Usuário", "Data de nascimento inválida."));
                    return RespostaApi.Falha(validacao.Errors);
                }
            }

            //buscar o usuario 
            var usuarioRetorno = await _repository.ObterPorCpf(request.Cpf);

            //verificar se as senhas são iguais
            if (usuarioRetorno != null)
            {
                primeiroAcesso = usuarioRetorno.PrimeiroAcesso;

                if (!usuarioRetorno.PrimeiroAcesso)
                {
                    if (usuarioCoreSSO != null && (!Criptografia.EqualsSenha(request.Senha, usuarioCoreSSO.Senha, usuarioCoreSSO.TipoCriptografia)))
                    {
                        validacao.Errors.Add(new ValidationFailure("Usuário", "Usuário ou senha incorretos."));
                        return RespostaApi.Falha(validacao.Errors);
                    }
                }
                else
                {
                    var senha = Regex.Replace(request.Senha, @"\-\/", "");

                    try
                    {
                        request.DataNascimento = DateTime.ParseExact(senha, "ddMMyyyy", CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        validacao.Errors.Add(new ValidationFailure("Usuário", "Data de nascimento inválida."));
                        return RespostaApi.Falha(validacao.Errors);
                    }
                }
            } else
            {
                primeiroAcesso = true;
                var senha = Regex.Replace(request.Senha, @"\-\/", "");

                try
                {
                    request.DataNascimento = DateTime.ParseExact(senha, "ddMMyyyy", CultureInfo.InvariantCulture);
                }
                catch
                {
                    validacao.Errors.Add(new ValidationFailure("Usuário", "Data de nascimento inválida."));
                    return RespostaApi.Falha(validacao.Errors);
                }
            }

            //selecionar alunos do responsável buscando apenas pelo cpf
            var usuarioAlunos = await _autenticacaoService.SelecionarAlunosResponsavel(request.Cpf);

            //caso nao tenha nenhum filho matriculado, retornar falha e inativá-lo no coresso
            if (usuarioAlunos == null || !usuarioAlunos.Any())
            {
                validacao.Errors.Add(new ValidationFailure("Usuário", "Este CPF não está relacionado como responsável de um aluno ativo na rede municipal."));
                ExcluiUsuarioSeExistir(request, usuarioRetorno);

                if (usuarioCoreSSO != null)
                    await _repositoryCoreSSO.AlterarStatusUsuario(usuarioCoreSSO.UsuId, StatusUsuarioCoreSSO.Inativo);

                return RespostaApi.Falha(validacao.Errors);
            }

            //se for primeiro acesso, a senha validar se a senha inputada é alguma data de nascimento de algum aluno do responsável
            if (primeiroAcesso && (!usuarioAlunos.Any(w => w.DataNascimento == request.DataNascimento)))
            {
                validacao.Errors.Add(new ValidationFailure("Usuário", "Data de Nascimento inválida."));
                //ExcluiUsuarioSeExistir(request, usuarioRetorno);
                return RespostaApi.Falha(validacao.Errors);
            }

            if (primeiroAcesso && (usuarioAlunos.Any(w => w.DataNascimento == request.DataNascimento && w.TipoSigilo == (int)AlunoTipoSigilo.Restricao)))
            {
                validacao.Errors.Add(new ValidationFailure("Usuário", "Usuário não cadastrado, qualquer dúvida procure a unidade escolar."));
                return RespostaApi.Falha(validacao.Errors);
            }

            //necessário implementar unit of work para transacionar essas operações
            var grupos = await _repositoryCoreSSO.SelecionarGrupos();

            var usuarioParaSeBasear = usuarioAlunos
                .OrderByDescending(a => a.DataAtualizacao)
                .FirstOrDefault();
         
            primeiroAcesso = primeiroAcesso || !grupos.Any(x => usuarioCoreSSO.Grupos.Any(z => z.Equals(x)));

            //verificar se o usuário está incluído em todos os grupos            
            if (usuarioCoreSSO != null && usuarioCoreSSO.Status == (int)StatusUsuarioCoreSSO.Inativo)
                await _repositoryCoreSSO.AlterarStatusUsuario(usuarioCoreSSO.UsuId, StatusUsuarioCoreSSO.Ativo);

            if (usuarioCoreSSO != null && usuarioCoreSSO.TipoCriptografia != TipoCriptografia.TripleDES)
            {
                senhaCriptografada = Criptografia.CriptografarSenhaTripleDES(request.Senha);
                await _repositoryCoreSSO.AtualizarCriptografiaUsuario(usuarioCoreSSO.UsuId, senhaCriptografada);
            }

            usuarioRetorno = await CriaUsuarioEhSeJaExistirAtualizaUltimoLogin(request, usuarioRetorno, usuarioParaSeBasear, primeiroAcesso);
            
            usuarioRetorno.PrimeiroAcesso = usuarioRetorno.PrimeiroAcesso || primeiroAcesso;

            var atualizarDadosCadastrais = VerificarAtualizacaoCadastral(usuarioParaSeBasear);

            
            return MapearResposta(usuarioParaSeBasear, usuarioRetorno, primeiroAcesso, atualizarDadosCadastrais || primeiroAcesso);
        }

        private bool VerificarAtualizacaoCadastral(RetornoUsuarioEol usuario)
        {
            return usuario.DataNascimentoResponsavel == null || string.IsNullOrWhiteSpace(usuario.NomeMae) ||
                   string.IsNullOrWhiteSpace(usuario.Email) || string.IsNullOrWhiteSpace(usuario.Celular);
        }

        private async Task<Dominio.Entidades.Usuario> CriaUsuarioEhSeJaExistirAtualizaUltimoLogin(AutenticarUsuarioCommand request, Dominio.Entidades.Usuario usuarioRetorno, RetornoUsuarioEol usuario, bool primeiroAcesso)
        {
            usuario.Cpf = request.Cpf;

            if (usuarioRetorno != null)
            {
                usuarioRetorno.AtualizarLogin(primeiroAcesso);

                await _repository.AltualizarUltimoAcessoPrimeiroUsuario(usuarioRetorno);
            }
            else
            {
                await _repository.SalvarAsync(MapearDominioUsuario(usuario, primeiroAcesso));
            }

            return await _repository.ObterUsuarioNaoExcluidoPorCpf(request.Cpf);
        }

        private void ExcluiUsuarioSeExistir(AutenticarUsuarioCommand request, Dominio.Entidades.Usuario usuarioRetorno)
        {
            if (usuarioRetorno != null)
                _repository.ExcluirUsuario(request.Cpf);
        }

        private Dominio.Entidades.Usuario MapearDominioUsuario(RetornoUsuarioEol usuarioEol, bool primeiroAcesso)
        {
            var usuario = new Dominio.Entidades.Usuario
            {
                Cpf = usuarioEol.Cpf,
                Excluido = false,
                UltimoLogin = DateTime.Now,
                PrimeiroAcesso = primeiroAcesso
            };

            return usuario;
        }

        private RespostaApi MapearResposta(RetornoUsuarioEol usuarioEol, Dominio.Entidades.Usuario usuarioApp, bool primeiroAcesso, bool atualizarDadosCadastrais)
        {
            RespostaAutenticar usuario = new RespostaAutenticar
            {
                Cpf = usuarioEol.Cpf,
                Email = usuarioEol.Email,
                Id = usuarioApp.Id,
                Nome = usuarioEol.Nome,
                DataNascimento = usuarioEol.DataNascimentoResponsavel,
                NomeMae = usuarioEol.NomeMae,
                PrimeiroAcesso = primeiroAcesso,
                AtualizarDadosCadastrais = atualizarDadosCadastrais,
                Celular = usuarioEol.ObterCelularComDDD(),
                Token = "",
                UltimaAtualizacao = usuarioEol.DataAtualizacao
            };

            return RespostaApi.Sucesso(usuario);
        }
    }
}
