using Moq;
using SME.AE.Aplicacao.Comandos.Notificacao.Atualizar;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Comandos
{
    public class AtualizarNotificacaoCommandTeste
    {
        private readonly Mock<INotificacaoRepositorio> repository;
        private readonly AtualizarNotificacaoCommandHandler handler;

        public AtualizarNotificacaoCommandTeste()
        {
            repository = new Mock<INotificacaoRepositorio>();
            handler = new AtualizarNotificacaoCommandHandler(repository.Object);
        }

        [Fact(DisplayName = "Deve lançar ArgumentNullException quando o repositório for nulo")]
        public void Deve_Lancar_ArgumentNullException_Quando_Repositorio_Nulo()
        {
            var excecao = Assert.Throws<ArgumentNullException>(() => new AtualizarNotificacaoCommandHandler(null));

            Assert.Equal("repository", excecao.ParamName);
        }

        [Theory(DisplayName = "Deve atualizar a notificação e retornar os dados atualizados")]
        [InlineData(1L, "Comunicado SME", "Mensagem de teste SME", "admin_sme")]
        [InlineData(25L, "Comunicado Escolar", "Reunião de pais e mestres", "rf123456")]
        public async Task Deve_Atualizar_Notificacao_Com_Sucesso(
            long id,
            string titulo,
            string mensagem,
            string alteradoPor)
        {
            // Arrange
            var dataExpiracao = DateTime.Now.AddDays(7);
            var antesDaExecucao = DateTime.Now.AddSeconds(-1);
            var command = new AtualizarNotificacaoCommand(id, titulo, mensagem, dataExpiracao, alteradoPor);

            AtualizarNotificacaoDto dtoCapturado = null;
            repository
                .Setup(x => x.Atualizar(It.IsAny<AtualizarNotificacaoDto>()))
                .Callback<AtualizarNotificacaoDto>(dto => dtoCapturado = dto)
                .Returns(Task.CompletedTask);

            // Act
            var resposta = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(resposta);
            Assert.Equal(id, resposta.Id);
            Assert.Equal(titulo, resposta.Titulo);
            Assert.Equal(mensagem, resposta.Mensagem);
            Assert.Equal(dataExpiracao, resposta.DataExpiracao);

            // Assert
            Assert.NotNull(dtoCapturado);
            Assert.Equal(id, dtoCapturado.Id);
            Assert.Equal(titulo, dtoCapturado.Titulo);
            Assert.Equal(mensagem, dtoCapturado.Mensagem);
            Assert.Equal(dataExpiracao, dtoCapturado.DataExpiracao);
            Assert.Equal(alteradoPor, dtoCapturado.AlteradoPor);
            Assert.True(dtoCapturado.EnviadoPushNotification);
            Assert.True(dtoCapturado.AlteradoEm >= antesDaExecucao);

            repository.Verify(x => x.Atualizar(It.IsAny<AtualizarNotificacaoDto>()), Times.Once);
        }
    }
}