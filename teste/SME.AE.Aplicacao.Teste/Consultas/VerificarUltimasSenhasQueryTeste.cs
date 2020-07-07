using FluentValidation.TestHelper;
using Moq;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios.Externos;
using SME.AE.Aplicacao.Consultas.VerificarSenha;
using SME.AE.Dominio.Entidades.Externas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Consultas
{
    public class VerificarUltimasSenhasQueryTeste : BaseTeste
    {
        public readonly VerificarUltimasSenhasQueryHandler verificarUltimasSenhasQueryHandler;
        private readonly Mock<IUsuarioSenhaHistoricoCoreSSORepositorio> usuarioSenhaHistoricoCoreSSORepositorio;

        public VerificarUltimasSenhasQueryTeste() 
        {
            usuarioSenhaHistoricoCoreSSORepositorio = new Mock<IUsuarioSenhaHistoricoCoreSSORepositorio>();

            verificarUltimasSenhasQueryHandler = new VerificarUltimasSenhasQueryHandler(usuarioSenhaHistoricoCoreSSORepositorio.Object);
        }

        [Theory(DisplayName = "Deve verificar ultimas senhas")]
        [InlineData("527f1d1c-ec1e-4efc-92ab-8c33534f3fb4", "e0f09c61c07c60adcbdc44fa0efda8d4")]
        [InlineData("1f8d6746-02eb-4540-a5e1-40d862c60d4d", "e0f09c61c07c60adcbdc44fa0efda8d4")]
        [InlineData("8754cc51-2534-49bd-af50-5eaf7bc54239", "e0f09c61c07c60adcbdc44fa0efda8d4")]
        [InlineData("c1614f9c-8d11-49b5-8503-30da6b2fbc58", "e0f09c61c07c60adcbdc44fa0efda8d4")]
        [InlineData("918bfb72-6a4f-4603-8cb3-320d633cc634", "e0f09c61c07c60adcbdc44fa0efda8d4")]
        [InlineData("a413548c-c9ab-45ee-9dd7-2aab398390b1", "e0f09c61c07c60adcbdc44fa0efda8d4")]
        [InlineData("e3660563-a80a-4947-b65c-82639e936c63", "e0f09c61c07c60adcbdc44fa0efda8d4")]
        [InlineData("85b27e5a-f24d-4f24-add0-084dd8e0bbbc", "e0f09c61c07c60adcbdc44fa0efda8d4")]
        [InlineData("3006af29-3ddd-44fd-8409-627568dd7605", "e0f09c61c07c60adcbdc44fa0efda8d4")]
        [InlineData("ac86a0ae-5121-4101-a26e-ff4695a753f0", "e0f09c61c07c60adcbdc44fa0efda8d4")]
        public async Task Deve_Verificar_Ultimas_Senhas(string usuarioIdCoreS, string senhaCriptografada)
        {
            MockarRepositorio();

            var result = Guid.TryParse(usuarioIdCoreS, out Guid usuarioIdCore);

            var query = new VerificarUltimasSenhasQuery(result ? usuarioIdCore : Guid.Empty, senhaCriptografada);

            await verificarUltimasSenhasQueryHandler.Handle(query, new CancellationToken());
        }

        [Theory(DisplayName = "Deve verificar query")]
        [InlineData("527f1d1c-ec1e-4efc-92ab-8c33534f3fb4", "e0f09c61c07c60adcbdc44fa0efda8d4", true)]
        [InlineData("", "e0f09c61c07c60adcbdc44fa0efda8d4", false)]
        [InlineData("8754cc51-2534-49bd-af50-5eaf7bc54239", "", false)]
        [InlineData("", "", false)]
        public void Deve_Validar_Query(string usuarioIdCoreS, string senhaCriptografada, bool resultadoEsperado)
        {
            var result = Guid.TryParse(usuarioIdCoreS, out Guid usuarioIdCore);

            var validator = new VerificarUltimasSenhasQueryValidator();

            var query = new VerificarUltimasSenhasQuery(result ? usuarioIdCore : Guid.Empty, senhaCriptografada);

            var validacao = validator.TestValidate(query);

            Assert.Equal(resultadoEsperado, validacao.IsValid);
        }

        private void MockarRepositorio()
        {
            usuarioSenhaHistoricoCoreSSORepositorio.Setup(x => x.AdicionarSenhaHistorico(It.IsAny<UsuarioSenhaHistoricoCoreSSO>()));
        }        
    }
}
