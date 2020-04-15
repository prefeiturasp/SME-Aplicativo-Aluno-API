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

namespace SME.AE.Aplicacao.Comandos.Autenticacao.AutenticarUsuario
{
    public class AutenticarUsuarioCommand : IRequest<RespostaApi>
    {
        public AutenticarUsuarioCommand(string cpf, string dataNascimento)
        {
            Cpf = cpf;
            DataNascimento = DateTime.ParseExact(dataNascimento, "ddMMyyyy", CultureInfo.InvariantCulture);
        }

        public string Cpf { get; set; }
        public DateTime DataNascimento { get; set; }


        public class AutenticarUsuarioCommandHandler : IRequestHandler<AutenticarUsuarioCommand, RespostaApi>
        {
            private readonly IAplicacaoContext _context;
            private readonly IAutenticacaoService _autenticacaoService;

            public AutenticarUsuarioCommandHandler(IAplicacaoContext context, IAutenticacaoService autenticacaoService)
            {
                _context = context;
                _autenticacaoService = autenticacaoService;
            }

            public async Task<RespostaApi> Handle(AutenticarUsuarioCommand request, CancellationToken cancellationToken)
            {
                var validator = new AutenticarUsuarioUseCaseValidatior();
                ValidationResult validacao = validator.Validate(request);
                if (!validacao.IsValid)
                    return RespostaApi.Falha(validacao.Errors);

                //selecionar alunos do responsável buscando apenas pelo cpf
                var usuarioAlunos = await _autenticacaoService.SelecionarAlunosResponsavel(request.Cpf);
                if (usuarioAlunos == null || !usuarioAlunos.Any())
                {
                    validacao.Errors.Add(new ValidationFailure("Usuário", "Este CPF não está relacionado como responsável de um aluno ativo na rede municipal."));
                    return RespostaApi.Falha(validacao.Errors);
                }

                if (!usuarioAlunos.Any(w => w.DataNascimento == request.DataNascimento))
                {
                    validacao.Errors.Add(new ValidationFailure("Usuário", "Data de Nascimento inválida."));
                    return RespostaApi.Falha(validacao.Errors);
                }

                var usuario = usuarioAlunos.FirstOrDefault(w => w.DataNascimento == request.DataNascimento);
                if (usuario.TipoSigilo == (int)AlunoTipoSigilo.Restricao)
                {
                    validacao.Errors.Add(new ValidationFailure("Usuário", "Usuário não cadastrado, qualquer dúvida procure a unidade escolar."));
                    return RespostaApi.Falha(validacao.Errors);
                }

                return MapearResposta(usuario);
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
