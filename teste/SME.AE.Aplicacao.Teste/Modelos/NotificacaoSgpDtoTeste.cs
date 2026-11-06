using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Modelos;
using System.Linq;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Modelos
{
    public class NotificacaoSgpDtoTeste
    {
        [Fact(DisplayName = "Deve definir categoria como UE quando TipoComunicado for MENSAGEM_AUTOMATICA")]
        public void DeveInserirCategoriaUE_QuandoMensagemAutomatica()
        {
            var dto = new NotificacaoSgpDto
            {
                TipoComunicado = TipoComunicado.MENSAGEM_AUTOMATICA,
                CodigoDre = null,
                CodigoUe = null
            };

            dto.InserirCategoria();

            Assert.Equal("UE", dto.CategoriaNotificacao);
        }

        [Theory(DisplayName = "Deve definir categoria corretamente conforme preenchimento de CodigoDre e CodigoUe")]
        [InlineData("", "", "SME")]
        [InlineData(null, null, "SME")]
        [InlineData("   ", "   ", "SME")]
        [InlineData("108500", "", "DRE")]
        [InlineData("108500", null, "DRE")]
        [InlineData("108500", "099887", "UE")]
        [InlineData("", "099887", "UE")]
        public void DeveInserirCategoria_ConformeDreEUe(string codigoDre, string codigoUe, string categoriaEsperada)
        {
            var dto = new NotificacaoSgpDto
            {
                TipoComunicado = TipoComunicado.SME,
                CodigoDre = codigoDre,
                CodigoUe = codigoUe
            };

            dto.InserirCategoria();

            Assert.Equal(categoriaEsperada, dto.CategoriaNotificacao);
        }

        [Fact(DisplayName = "Deve converter strings separadas por vírgula em coleções corretamente")]
        public void DeveConverterPropriedadesEmEnumerables()
        {
            var dto = new NotificacaoSgpDto
            {
                SeriesResumidas = "1,2,3",
                Modalidades = "5,6",
                TiposEscolas = "10,12"
            };

            Assert.Equal(new[] { "1", "2", "3" }, dto.ObterSeriesResumidas().ToArray());
            Assert.Equal(new[] { 5, 6 }, dto.ObterModalidades().ToArray());
            Assert.Equal(new[] { 10, 12 }, dto.ObterTiposEscolas().ToArray());
        }
    }
}