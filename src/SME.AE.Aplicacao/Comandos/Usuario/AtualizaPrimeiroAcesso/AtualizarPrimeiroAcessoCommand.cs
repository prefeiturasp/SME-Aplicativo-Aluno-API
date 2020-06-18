using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comandos.Usuario.AtualizaPrimeiroAcesso
{
    public class AtualizarPrimeiroAcessoCommand : IRequest
    {
        public long Id { get; set; }
        public bool PrimeiroAcesso { get; set; }
    }
}
