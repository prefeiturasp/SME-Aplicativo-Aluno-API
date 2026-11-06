using AutoMapper;
using MediatR;
using Moq;
using System.Threading;

namespace SME.AE.Aplicacao.Teste
{
    public class BaseTeste
    {
        protected readonly Mock<IMediator> mediator;
        protected readonly Mock<IMapper> mapper;

        public BaseTeste()
        {
            mediator = new Mock<IMediator>();
            mapper = new Mock<IMapper>();
        }

        protected void MediatorSetup<T>(object retorno = default)
        {
            var m = mediator.Setup(a => a.Send(It.IsAny<T>(), It.IsAny<CancellationToken>()));

            if (retorno != default)
                m.ReturnsAsync(retorno);

        }

        protected void MediatorSetup<TRequest, TResponse>(TResponse retorno)
        where TRequest : IRequest<TResponse>
            {
                mediator.Setup(a => a.Send(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(retorno);
            }
    }
}
