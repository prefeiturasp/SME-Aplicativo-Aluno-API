using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class AtualizarDadosResponsavelEolCommandHandler : IRequestHandler<AtualizarDadosResponsavelEolCommand, bool>
    {
        private readonly IResponsavelEOLRepositorio responsavelEOLRepositorio;

        public AtualizarDadosResponsavelEolCommandHandler(IResponsavelEOLRepositorio responsavelEOLRepositorio)
        {
            this.responsavelEOLRepositorio = responsavelEOLRepositorio ?? throw new ArgumentNullException(nameof(responsavelEOLRepositorio));
        }

        public async Task<bool> Handle(AtualizarDadosResponsavelEolCommand request, CancellationToken cancellationToken)
        {
            var retorno = await responsavelEOLRepositorio.AtualizarDadosResponsavel(request.CodigoAluno,
                                                                                    request.Cpf,
                                                                                    request.Email,
                                                                                    request.DataNascimentoResponsavel,
                                                                                    request.NomeMae,
                                                                                    request.DDD,
                                                                                    request.Celular);
            return retorno != 0;
        }
    }
}
