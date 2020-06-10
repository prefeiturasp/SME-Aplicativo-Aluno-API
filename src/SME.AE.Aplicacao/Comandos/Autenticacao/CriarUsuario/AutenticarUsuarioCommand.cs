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

        }

        public string Cpf { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Senha { get; set; }

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
                bool primeiroAcesso = false;
                string email = "";
                string celular = "";

                var validator = new AutenticarUsuarioUseCaseValidatior();
                ValidationResult validacao = validator.Validate(request);
                if (!validacao.IsValid)
                    return RespostaApi.Falha(validacao.Errors);

                //verificar se o usuário está cadastrado no CoreSSO
                var usuarioCoreSSO = await _repositoryCoreSSO.Selecionar(request.Cpf);


                string senhaCriptografada = string.Empty;

                //verificar se as senhas são iguais
                if (usuarioCoreSSO.Any())
                {
                    if (!Criptografia.EqualsSenha(request.Senha, usuarioCoreSSO.FirstOrDefault().Senha, usuarioCoreSSO.FirstOrDefault().TipoCriptografia))
                    {
                        validacao.Errors.Add(new ValidationFailure("Usuário", "Usuário ou senha incorretos."));
                        return RespostaApi.Falha(validacao.Errors);
                    }
                }

                //se for primeiro acesso
                if (!usuarioCoreSSO.Any())
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
                var usuarioRetorno = _repository.ObterPorCpf(request.Cpf).Result;

                //selecionar alunos do responsável buscando apenas pelo cpf
                var usuarioAlunos = await _autenticacaoService.SelecionarAlunosResponsavel(request.Cpf);

                //caso nao tenha nenhum filho matriculado, retornar falha e inativá-lo no coresso
                if (usuarioAlunos == null || !usuarioAlunos.Any())
                {
                    validacao.Errors.Add(new ValidationFailure("Usuário", "Este CPF não está relacionado como responsável de um aluno ativo na rede municipal."));
                    ExcluiUsuarioSeExistir(request, usuarioRetorno);

                    if (usuarioCoreSSO.Any())
                        await _repositoryCoreSSO.AlterarStatusUsuario(usuarioCoreSSO.FirstOrDefault().UsuId, StatusUsuarioCoreSSO.Inativo);
                    return RespostaApi.Falha(validacao.Errors);
                }

                //se for primeiro acesso, a senha validar se a senha inputada é alguma data de nascimento de algum aluno do responsável
                if (primeiroAcesso)
                {
                    if (!usuarioAlunos.Any(w => w.DataNascimento == request.DataNascimento))
                    {
                        validacao.Errors.Add(new ValidationFailure("Usuário", "Data de Nascimento inválida."));
                        ExcluiUsuarioSeExistir(request, usuarioRetorno);
                        return RespostaApi.Falha(validacao.Errors);
                    }
                    if (usuarioAlunos.Any(w => w.DataNascimento == request.DataNascimento && w.TipoSigilo == (int)AlunoTipoSigilo.Restricao))
                    {
                        validacao.Errors.Add(new ValidationFailure("Usuário", "Usuário não cadastrado, qualquer dúvida procure a unidade escolar."));
                        return RespostaApi.Falha(validacao.Errors);
                    }
                }

                //verificar se o usuário tem e-mail e celular cadastrado
                if (usuarioAlunos.Any(w => !string.IsNullOrEmpty(w.Email)))
                    email = usuarioAlunos.FirstOrDefault(w => !string.IsNullOrEmpty(w.Email)).Email;
                if (usuarioAlunos.Any(w => !string.IsNullOrEmpty(w.Celular)))
                {
                    celular = usuarioAlunos.FirstOrDefault(w => !string.IsNullOrEmpty(w.Celular)).Celular;
                    if (usuarioAlunos.Any(w => !string.IsNullOrEmpty(w.DDD)))
                        celular = $"{usuarioAlunos.FirstOrDefault(w => !string.IsNullOrEmpty(w.DDD)).DDD}{celular}";
                }



                //necessário implementar unit of work para transacionar essas operações
                senhaCriptografada = Criptografia.CriptografarSenhaTripleDES(request.Senha);
                var grupos = await _repositoryCoreSSO.SelecionarGrupos();
                var usuario = usuarioAlunos.FirstOrDefault();
                //se usuário  não estiver cadastrado no CoreSSO
                if (!usuarioCoreSSO.Any())
                {
                    try
                    {
                        await _repositoryCoreSSO.Criar(new Comum.Modelos.Entrada.UsuarioCoreSSO
                        {
                            Cpf = request.Cpf,
                            Nome = usuario.Nome,
                            Senha = senhaCriptografada,
                            Grupos = grupos
                        });
                    }
                    catch
                    {
                        validacao.Errors.Add(new ValidationFailure("Usuário", "Erro ao tentar cadastrar o usuário no CoreSSO"));
                        return RespostaApi.Falha(validacao.Errors);
                    }
                }//caso contrário verificar se o usuário está incluído em todos os grupos
                else
                {
                    if (usuarioCoreSSO.FirstOrDefault().Status == (int)StatusUsuarioCoreSSO.Inativo)
                        await _repositoryCoreSSO.AlterarStatusUsuario(usuarioCoreSSO.FirstOrDefault().UsuId, StatusUsuarioCoreSSO.Ativo);
                    var gruposNaoIncluidos = grupos.Where(w => !usuarioCoreSSO.Select(x => x.GrupoId).Contains(w));
                    if (gruposNaoIncluidos.Any())
                        _repositoryCoreSSO.IncluirUsuarioNosGrupos(usuarioCoreSSO.FirstOrDefault().UsuId, gruposNaoIncluidos);
                    if (usuarioCoreSSO.FirstOrDefault().TipoCriptografia != TipoCriptografia.TripleDES)
                        await _repositoryCoreSSO.AtualizarCriptografiaUsuario(usuarioCoreSSO.FirstOrDefault().UsuId, request.Senha);
                }

                CriaUsuarioEhSeJaExistirAtualizaUltimoLogin(request, usuarioRetorno, usuario);

                usuarioRetorno.Email = usuarioRetorno.Email ?? email;
                usuarioRetorno.Celular = usuarioRetorno.Celular ?? celular;

                return MapearResposta(usuario, usuarioRetorno, primeiroAcesso);
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

            private RespostaApi MapearResposta(RetornoUsuarioEol usuarioEol, Dominio.Entidades.Usuario usuarioApp, bool primeiroAcesso)
            {
                RespostaAutenticar usuario = new RespostaAutenticar
                {
                    Cpf = usuarioEol.Cpf,
                    Email = usuarioApp.Email,
                    Id = usuarioApp.Id,
                    Nome = usuarioEol.Nome,
                    PrimeiroAcesso = primeiroAcesso,
                    Celular = usuarioApp.Celular,
                    Token = ""
                };
                return RespostaApi.Sucesso(usuario);
            }
        }
    }
}
