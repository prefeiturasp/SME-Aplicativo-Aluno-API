using FluentValidation.Results;
using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.Autenticacao.CriarUsuario
{
    public class CriarUsuarioCommand : IRequest<RespostaAutenticar>
    {
        public CriarUsuarioCommand(string cpf, DateTime dataNascimento)
        {
            Cpf = cpf;
            DataNascimento = dataNascimento;
        }

        public string Cpf { get; set; }
        public DateTime DataNascimento { get; set; }


        public class CriarUsuarioCommandCommandHandler : IRequestHandler<CriarUsuarioCommand, RespostaApi>
        {
            private readonly IAplicacaoContext _context;
            private readonly IAutenticacaoService _autenticacaoService;

            public CriarUsuarioCommandCommandHandler(IAplicacaoContext context, IAutenticacaoService autenticacaoService)
            {
                _context = context;
                _autenticacaoService = autenticacaoService;
            }

            public async Task<RespostaApi> Handle(CriarUsuarioCommand request, CancellationToken cancellationToken)
            {
                var validator = new CriarUsuarioUseCaseValidatior();
                ValidationResult validacao = validator.Validate(request);
                if (!validacao.IsValid)
                    return RespostaApi.Falha(validacao.Errors);
                //validar usuário
                var usuario = await _autenticacaoService.ValidarUsuarioEol(request.Cpf, request.DataNascimento);
                return RespostaApi.Sucesso(usuario);
            }
        }
    }
}
