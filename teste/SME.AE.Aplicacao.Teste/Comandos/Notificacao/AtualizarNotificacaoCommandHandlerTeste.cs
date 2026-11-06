using Moq;
using SME.AE.Aplicacao.Comandos.Notificacao.Atualizar;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Comandos.Notificacao
{
    public class AtualizarNotificacaoCommandHandlerTeste
    {
        private readonly Mock<INotificacaoRepositorio> repositorioMock;
        private readonly AtualizarNotificacaoCommandHandler handler;

        public AtualizarNotificacaoCommandHandlerTeste()
        {
            repositorioMock = new Mock<INotificacaoRepositorio>();
            handler = new AtualizarNotificacaoCommandHandler(repositorioMock.Object);
        }

        [Fact(DisplayName = "Deve lançar ArgumentNullException quando repositório for nulo")]
        public void DeveLancarArgumentNullException_QuandoRepositorioNulo()
        {
            Assert.Throws<ArgumentNullException>(() => new AtualizarNotificacaoCommandHandler(null));
        }

        [Fact(DisplayName = "Deve atualizar notificação no repositório com EnviadoPushNotification true e retornar resposta")]
        public async Task DeveAtualizarNotificacao_ComSucesso()
        {
            // Arrange
            var dataExpiracao = DateTime.Now.AddDays(5);
            var command = new AtualizarNotificacaoCommand(
                12345,
                "Título Atualizado",
                "Mensagem Atualizada",
                dataExpiracao,
                "RF123456"
            );

            repositorioMock
                .Setup(r => r.Atualizar(It.IsAny<AtualizarNotificacaoDto>()))
                .Returns(Task.CompletedTask);

            // Act
            var resposta = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(resposta);
            Assert.Equal(command.Id, resposta.Id);
            Assert.Equal(command.Titulo, resposta.Titulo);
            Assert.Equal(command.Mensagem, resposta.Mensagem);
            Assert.Equal(command.DataExpiracao, resposta.DataExpiracao);

            repositorioMock.Verify(r => r.Atualizar(It.Is<AtualizarNotificacaoDto>(dto =>
                dto.Id == command.Id &&
                dto.Titulo == command.Titulo &&
                dto.Mensagem == command.Mensagem &&
                dto.DataExpiracao == command.DataExpiracao &&
                dto.AlteradoPor == command.AlteradoPor &&
                dto.EnviadoPushNotification == true &&
                dto.AlteradoEm != default
            )), Times.Once);
        }
    }
}