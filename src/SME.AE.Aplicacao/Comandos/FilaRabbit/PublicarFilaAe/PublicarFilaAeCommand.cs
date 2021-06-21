using MediatR;
using System;

namespace SME.AE.Aplicacao
{
    public class PublicarFilaAeCommand : IRequest<bool>
    {
        public PublicarFilaAeCommand(string rota, object mensagem, Guid codigoCorrelacao)
        {
            Mensagem = mensagem;
            CodigoCorrelacao = codigoCorrelacao;
            Rota = rota;
        }

        public string Rota { get; set; }
        public object Mensagem { get; set; }
        public Guid CodigoCorrelacao { get; set; }
    }
}
