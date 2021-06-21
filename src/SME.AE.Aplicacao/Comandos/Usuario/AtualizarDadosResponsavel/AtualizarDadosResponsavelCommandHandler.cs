using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class AtualizarDadosResponsavelCommandHandler : IRequestHandler<AtualizarDadosResponsavelCommand, bool>
    {
        private readonly IResponsavelEOLRepositorio responsavelEOLRepositorio;

        public AtualizarDadosResponsavelCommandHandler(IResponsavelEOLRepositorio responsavelEOLRepositorio)
        {
            this.responsavelEOLRepositorio = responsavelEOLRepositorio ?? throw new ArgumentNullException(nameof(responsavelEOLRepositorio));
        }

        public async Task<bool> Handle(AtualizarDadosResponsavelCommand request, CancellationToken cancellationToken)
        {
            var retorno = await responsavelEOLRepositorio.AtualizarDadosResponsavel(request.Id, 
                                                                                    request.Email, 
                                                                                    request.DataNascimentoResponsavel, 
                                                                                    request.NomeMae,
                                                                                    request.DDD,
                                                                                    request.Celular);
            return retorno != 0;
        }
    }
}
