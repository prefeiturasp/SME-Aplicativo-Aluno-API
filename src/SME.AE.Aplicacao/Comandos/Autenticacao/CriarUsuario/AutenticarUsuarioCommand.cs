using FluentValidation.Results;
using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.Autenticacao.AutenticarUsuario
{
    public class AutenticarUsuarioCommand : IRequest<RespostaApi>
    {
        public AutenticarUsuarioCommand(string cpf, DateTime dataNascimento)
        {
            Cpf = cpf;
            DataNascimento = dataNascimento;
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
                //validar usuário se usuário existe
                var usuario = await _autenticacaoService.SelecionarResponsavel(request.Cpf, request.DataNascimento);
                if (usuario == null)
                {
                    validacao.Errors.Add(new ValidationFailure("Usuário", "Esse CPF ou Data de Nascimento são inválidos"));
                    return RespostaApi.Falha(validacao.Errors);
                }

                //usuario = new RespostaAutenticar({ Cpf = "12345678900", Email = "jose@silva.com", Id = 1, Nome = "José da Silva Pereira", Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c" });
                return RespostaApi.Sucesso(usuario);
            }
        }
    }
}
