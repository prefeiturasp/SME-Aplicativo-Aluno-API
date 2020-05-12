using FluentValidation.Results;
using MediatR;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Interfaces;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using SME.AE.Aplicacao.Comum.Interfaces.Geral;
using SME.AE.Aplicacao.Comum.Interfaces.Servicos;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System.Collections.Generic;
using SME.AE.Aplicacao.Comum.Extensoes;

namespace SME.AE.Aplicacao.Comandos.Autenticacao.AutenticarUsuario
{
    public class AutenticarUsuarioCommand : IRequest<RespostaApi>
    {
        public AutenticarUsuarioCommand(string cpf, string senha)
        {
            Cpf = cpf;
            Senha = senha;
            senha = Regex.Replace(senha, @"\-\/", "");
            DataNascimento = DateTime.ParseExact(senha, "ddMMyyyy", CultureInfo.InvariantCulture);
        }

        public string Cpf { get; set; }
        public DateTime DataNascimento { get; set; }
        public String Senha { get; set; }

        public class AutenticarUsuarioCommandHandler : IRequestHandler<AutenticarUsuarioCommand, RespostaApi>
        {
            private readonly IAplicacaoContext _context;
            private readonly IAutenticacaoService _autenticacaoService;
            private readonly IUsuarioRepository _repository;
            private readonly IUsuarioCoreSSORepositorio _repositoryCoreSSO;

            public AutenticarUsuarioCommandHandler(IAplicacaoContext context, IAutenticacaoService autenticacaoService, IUsuarioRepository repository, IUsuarioCoreSSORepositorio repositoryCoreSSO)
            {
                _context = context;
                _autenticacaoService = autenticacaoService;
                _repositoryCoreSSO = repositoryCoreSSO;
                _repository = repository;
            }

            public async Task<RespostaApi> Handle(AutenticarUsuarioCommand request, CancellationToken cancellationToken)
            {
                var validator = new AutenticarUsuarioUseCaseValidatior();
                ValidationResult validacao = validator.Validate(request);
                var usuarioRetorno = _repository.ObterPorCpf(request.Cpf).Result;
                if (!validacao.IsValid)
                    return RespostaApi.Falha(validacao.Errors);



                //selecionar alunos do responsável buscando apenas pelo cpf
                var usuarioAlunos = await _autenticacaoService.SelecionarAlunosResponsavel(request.Cpf);
                if (usuarioAlunos == null || !usuarioAlunos.Any())
                {
                    validacao.Errors.Add(new ValidationFailure("Usuário", "Este CPF não está relacionado como responsável de um aluno ativo na rede municipal."));
                    ExcluiUsuarioSeExistir(request, usuarioRetorno);
                    return RespostaApi.Falha(validacao.Errors);
                }

                if (!usuarioAlunos.Any(w => w.DataNascimento == request.DataNascimento))
                {
                    validacao.Errors.Add(new ValidationFailure("Usuário", "Data de Nascimento inválida."));
                    ExcluiUsuarioSeExistir(request, usuarioRetorno);
                    return RespostaApi.Falha(validacao.Errors);
                }

                var usuario = usuarioAlunos.FirstOrDefault(w => w.DataNascimento == request.DataNascimento);
                if (usuario.TipoSigilo == (int)AlunoTipoSigilo.Restricao)
                {
                    validacao.Errors.Add(new ValidationFailure("Usuário", "Usuário não cadastrado, qualquer dúvida procure a unidade escolar."));
                    return RespostaApi.Falha(validacao.Errors);
                }


                //necessário implementar unit of work para transacionar essas operações

                //verificar se usuário está cadastrado no coreSSO
                var retornoUsuarioCoreSSO = await _repositoryCoreSSO.Selecionar(request.Cpf);
                string senhaCriptografada = Criptografia.CriptografarSenhaTripleDES(request.Senha);
                var grupos = await _repositoryCoreSSO.SelecionarGrupos();
                //se não estiver cadastra um novo
                if (!retornoUsuarioCoreSSO.Any())
                {
                    try
                    {
                        
                        await _repositoryCoreSSO.Criar(new Comum.Modelos.Entrada.UsuarioCoreSSO { Cpf = request.Cpf, Nome = usuario.Nome, Senha = senhaCriptografada, Grupos = grupos });
                    }
                    catch
                    {
                        validacao.Errors.Add(new ValidationFailure("Usuário", "Erro ao tentar cadastrar o usuário no CoreSSO"));
                        return RespostaApi.Falha(validacao.Errors);
                    }
                }//caso contrário verificar se o usuário está incluído em todos os grupos
                else
                {
                    var gruposNaoIncluidos = grupos.Where(w =>  !retornoUsuarioCoreSSO.Select(x => x.GrupoId).Contains(w));
                    if (gruposNaoIncluidos.Any())
                        _repositoryCoreSSO.IncluirUsuarioNosGrupos(retornoUsuarioCoreSSO.FirstOrDefault().UsuId, gruposNaoIncluidos);
                }

                CriaUsuarioEhSeJaExistirAtualizaUltimoLogin(request, usuarioRetorno, usuario);

                return MapearResposta(usuario);
            }


            private void CriaUsuarioEhSeJaExistirAtualizaUltimoLogin(AutenticarUsuarioCommand request, Dominio.Entidades.Usuario usuarioRetorno, RetornoUsuarioEol usuario)
            {
                usuario.Cpf = request.Cpf;

                if (usuarioRetorno != null)
                {
                    _repository.AtualizaUltimoLoginUsuario(request.Cpf);
                }

                else
                {
                    _repository.Criar(MapearDominioUsuario(usuario));
                }
            }

            private void ExcluiUsuarioSeExistir(AutenticarUsuarioCommand request, Dominio.Entidades.Usuario usuarioRetorno)
            {
                if (usuarioRetorno != null)
                    _repository.ExcluirUsuario(request.Cpf);
            }

            private Dominio.Entidades.Usuario MapearDominioUsuario(RetornoUsuarioEol usuarioEol)
            {
                var usuario = new Dominio.Entidades.Usuario
                {
                    Cpf = usuarioEol.Cpf,
                    Nome = usuarioEol.Nome,
                    Email = usuarioEol.Email,
                    Excluido = false,
                    UltimoLogin = DateTime.Now
                };

                return usuario;

            }

            private RespostaApi MapearResposta(RetornoUsuarioEol usuarioEol)
            {
                RespostaAutenticar usuario = new RespostaAutenticar
                {
                    Cpf = usuarioEol.Cpf,
                    Email = usuarioEol.Email,
                    Id = usuarioEol.Id,
                    Nome = usuarioEol.Nome,
                    Token = ""
                };
                return RespostaApi.Sucesso(usuario);
            }
        }
    }
}
