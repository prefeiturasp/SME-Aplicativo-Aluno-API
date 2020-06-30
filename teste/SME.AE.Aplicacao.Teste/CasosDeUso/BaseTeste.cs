using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Teste.CasosDeUso
{
    public class BaseTeste
    {
        protected readonly Mock<IMediator> mediator;

        public BaseTeste()
        {
            mediator = new Mock<IMediator>();
        }
    }
}
