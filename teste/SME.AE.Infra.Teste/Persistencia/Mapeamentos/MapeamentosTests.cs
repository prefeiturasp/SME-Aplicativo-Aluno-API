using NUnit.Framework;
using SME.AE.Dominio.Entidades;
using SME.AE.Dominio.Entidades.Externas;
using SME.AE.Infra.Persistencia.Mapeamentos;
using SME.AE.Infra.Persistencia.Mapeamentos.Externos;
using System.Collections.Generic;
using System.Linq;

namespace SME.AE.Infra.Teste.Persistencia.Mapeamentos
{
    [TestFixture]
    public class MapeamentosTests
    {
        [Test]
        public void UsuarioMapDeveMapearTabelaChaveEColunasDeAutenticacao()
        {
            var mapa = new UsuarioMap();

            Assert.Multiple(() =>
            {
                Assert.That(mapa.TableName, Is.EqualTo("usuario"));
                Assert.That(ObterColuna(mapa, nameof(Usuario.Cpf)).ColumnName, Is.EqualTo("cpf"));
                Assert.That(ObterColuna(mapa, nameof(Usuario.Token)).ColumnName, Is.EqualTo("token_redefinicao"));
                Assert.That(ObterColuna(mapa, nameof(Usuario.ValidadeToken)).ColumnName, Is.EqualTo("validade_token"));
            });
        }

        [Test]
        public void AceiteTermosMapDeveMapearTabelaEHash()
        {
            var mapa = new AceiteTermosDeUsoMap();

            Assert.Multiple(() =>
            {
                Assert.That(mapa.TableName, Is.EqualTo("aceite_termos_de_uso"));
                Assert.That(ObterColuna(mapa, nameof(AceiteTermosDeUso.Hash)).ColumnName, Is.EqualTo("hash"));
                Assert.That(ObterColuna(mapa, nameof(AceiteTermosDeUso.Excluido)).ColumnName, Is.EqualTo("excluido"));
            });
        }

        [Test]
        public void PessoaCoreSsoMapDeveMapearTabelaEIdentificadorExterno()
        {
            var mapa = new PessoaCoreSSOMap();

            Assert.Multiple(() =>
            {
                Assert.That(mapa.TableName, Is.Null);
                Assert.That(ObterColuna(mapa, nameof(PessoaCoreSSO.Id)).ColumnName, Is.EqualTo("pes_id"));
                Assert.That(ObterColuna(mapa, nameof(PessoaCoreSSO.Nome)).ColumnName, Is.EqualTo("pes_nome"));
            });
        }

        private static Dapper.FluentMap.Mapping.IPropertyMap ObterColuna(Dapper.FluentMap.Mapping.IEntityMap mapa, string propriedade)
        {
            return mapa.PropertyMaps.Single(item => item.PropertyInfo.Name == propriedade);
        }
    }
}
