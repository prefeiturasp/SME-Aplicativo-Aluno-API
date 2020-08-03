using MediatR;
using Moq;
using System.Threading;

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
