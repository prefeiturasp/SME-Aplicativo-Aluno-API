using SME.AE.Aplicacao.Consultas;
using SME.AE.Aplicacao.Consultas.ObterBimestres;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Consultas.Aluno
{
    public class ObterBimestresLiberacaoBoletimQueryTeste
    {
        [Fact]
        public void Construtor_FactoryNula_DeveLancarArgumentNullException()
        {
            var excecao = Assert.Throws<ArgumentNullException>(() => new ObterBimestresLiberacaoBoletimQueryHandler(null));

            Assert.Equal("httpClientFactory", excecao.ParamName);
        }

        [Fact]
        public async Task Handle_DeveRetornarTodosOsBimestresConfigurados()
        {
            var factory = new Moq.Mock<System.Net.Http.IHttpClientFactory>();
            var handler = new ObterBimestresLiberacaoBoletimQueryHandler(factory.Object);

            var resultado = await handler.Handle(new ObterBimestresLiberacaoBoletimQuery("T1"), CancellationToken.None);

            Assert.Equal(new[] { 1, 2, 3, 4, 0 }, resultado);
            factory.Verify(x => x.CreateClient(Moq.It.IsAny<string>()), Moq.Times.Never);
        }
    }
}