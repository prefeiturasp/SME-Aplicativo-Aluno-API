using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Teste
{
    public class BaseTeste
    {
        protected readonly Mock<IMediator> mediator;

        public BaseTeste()
        {
            mediator = new Mock<IMediator>();
        }
                
        protected void MediatorSetup<T>(object retorno = default)
        {
            var m = mediator.Setup(a => a.Send(It.IsAny<T>(), It.IsAny<CancellationToken>()));

            if (retorno != default)
                m.ReturnsAsync(retorno);

        }
    }
}
