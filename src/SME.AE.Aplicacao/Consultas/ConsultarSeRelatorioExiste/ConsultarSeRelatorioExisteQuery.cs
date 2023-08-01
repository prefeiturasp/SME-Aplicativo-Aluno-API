using MediatR;
using System;

namespace SME.AE.Aplicacao
{
    public class ConsultarSeRelatorioExisteQuery : IRequest<bool>
    {
        public Guid CodigoCorrelacao { get; set; }

        public ConsultarSeRelatorioExisteQuery(Guid codigoCorrelacao)
        {
            CodigoCorrelacao = codigoCorrelacao;
        }
    }
}
