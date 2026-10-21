using Moq;
using SME.AE.Aplicacao.Comandos.Usuario.MarcarMensagemLida;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Comandos
{
    public class UsuarioNotificacaoCommandTeste : BaseTeste
    {
        private readonly UsuarioNotificacaoCommand.UsuarioMensagemCommandHandler handler;
        private readonly Mock<IUsuarioNotificacaoRepositorio> repositorio;

        public UsuarioNotificacaoCommandTeste()
        {
            repositorio = new Mock<IUsuarioNotificacaoRepositorio>();
            handler = new UsuarioNotificacaoCommand.UsuarioMensagemCommandHandler(repositorio.Object);
        }

        [Fact(DisplayName = "Deve criar o registro de leitura quando ainda não existir")]
        public async Task Deve_Criar_Registro_De_Leitura_Quando_Nao_Existir()
        {
            var usuarioNotificacao = CriarUsuarioNotificacao(mensagemVisualizada: true);
            MockarRegistroExistente(null);
            repositorio.Setup(x => x.Criar(It.IsAny<Dominio.Entidades.UsuarioNotificacao>())).ReturnsAsync(true);

            await handler.Handle(new UsuarioNotificacaoCommand(usuarioNotificacao), new CancellationToken());

            repositorio.Verify(x => x.ObterPorNotificacaoIdEhUsuarioCpf(usuarioNotificacao.NotificacaoId,
                                                                         usuarioNotificacao.UsuarioCpf,
                                                                         usuarioNotificacao.DreCodigoEol,
                                                                         usuarioNotificacao.UeCodigoEol,
                                                                         usuarioNotificacao.CodigoEolAluno), Times.Once);
            repositorio.Verify(x => x.Criar(usuarioNotificacao), Times.Once);
            repositorio.Verify(x => x.Atualizar(It.IsAny<Dominio.Entidades.UsuarioNotificacao>()), Times.Never);
        }

        [Theory(DisplayName = "Deve atualizar o status de visualização e a auditoria do registro de leitura existente")]
        [InlineData(false, true)]
        [InlineData(true, false)]
        public async Task Deve_Atualizar_Registro_De_Leitura_Existente(bool statusAtual, bool novoStatus)
        {
            var registroExistente = CriarUsuarioNotificacao(mensagemVisualizada: statusAtual);
            MockarRegistroExistente(registroExistente);
            repositorio.Setup(x => x.Atualizar(It.IsAny<Dominio.Entidades.UsuarioNotificacao>())).ReturnsAsync(true);

            await handler.Handle(new UsuarioNotificacaoCommand(CriarUsuarioNotificacao(mensagemVisualizada: novoStatus)), new CancellationToken());

            Assert.Equal(novoStatus, registroExistente.MensagemVisualizada);
            Assert.Equal("Sistema", registroExistente.AlteradoPor);
            Assert.NotNull(registroExistente.AlteradoEm);
            repositorio.Verify(x => x.Atualizar(registroExistente), Times.Once);
            repositorio.Verify(x => x.Criar(It.IsAny<Dominio.Entidades.UsuarioNotificacao>()), Times.Never);
        }

        [Theory(DisplayName = "Deve retornar o resultado do repositório")]
        [InlineData(false, true)]
        [InlineData(false, false)]
        [InlineData(true, true)]
        [InlineData(true, false)]
        public async Task Deve_Retornar_Resultado_Do_Repositorio(bool registroExiste, bool resultadoRepositorio)
        {
            MockarRegistroExistente(registroExiste ? CriarUsuarioNotificacao(mensagemVisualizada: false) : null);
            repositorio.Setup(x => x.Criar(It.IsAny<Dominio.Entidades.UsuarioNotificacao>())).ReturnsAsync(resultadoRepositorio);
            repositorio.Setup(x => x.Atualizar(It.IsAny<Dominio.Entidades.UsuarioNotificacao>())).ReturnsAsync(resultadoRepositorio);

            var resultado = await handler.Handle(new UsuarioNotificacaoCommand(CriarUsuarioNotificacao(mensagemVisualizada: true)), new CancellationToken());

            Assert.Equal(resultadoRepositorio, resultado);
        }

        private void MockarRegistroExistente(Dominio.Entidades.UsuarioNotificacao registroExistente)
        {
            repositorio.Setup(x => x.ObterPorNotificacaoIdEhUsuarioCpf(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<long>()))
                       .ReturnsAsync(registroExistente);
        }

        private static Dominio.Entidades.UsuarioNotificacao CriarUsuarioNotificacao(bool mensagemVisualizada)
        {
            return new Dominio.Entidades.UsuarioNotificacao
            {
                UsuarioCpf = "12345678900",
                UsuarioId = 1,
                NotificacaoId = 10,
                DreCodigoEol = 108100,
                UeCodigoEol = "019493",
                CodigoEolAluno = 1234567,
                MensagemVisualizada = mensagemVisualizada,
                CriadoEm = DateTime.Now.AddDays(-1)
            };
        }
    }
}
