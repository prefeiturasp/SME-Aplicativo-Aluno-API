using NUnit.Framework;
using SME.AE.Dominio.Comum;
using SME.AE.Dominio.Comum.Enumeradores;
using SME.AE.Dominio.Options;
using System;

namespace SME.AE.Dominio.Teste.Comum
{
    [TestFixture]
    public class ComumTests
    {
        [Test]
        public void EntidadeAuditavelDevePermitirArmazenarDadosDeAuditoria()
        {
            var criado = DateTime.UtcNow.AddMinutes(-1);
            var alterado = DateTime.UtcNow;
            var entidade = new EntidadeAuditavel
            {
                Id = 12,
                CreatedBy = "criador",
                Created = criado,
                LastModifiedBy = "editor",
                LastModified = alterado
            };

            Assert.Multiple(() =>
            {
                Assert.That(entidade.Id, Is.EqualTo(12));
                Assert.That(entidade.CreatedBy, Is.EqualTo("criador"));
                Assert.That(entidade.Created, Is.EqualTo(criado));
                Assert.That(entidade.LastModifiedBy, Is.EqualTo("editor"));
                Assert.That(entidade.LastModified, Is.EqualTo(alterado));
            });
        }

        [Test]
        public void TelemetriaOptionsDeveExporSecaoEPermitirConfigurarApm()
        {
            var options = new TelemetriaOptions { Apm = true };

            Assert.Multiple(() =>
            {
                Assert.That(TelemetriaOptions.Secao, Is.EqualTo("Telemetria"));
                Assert.That(options.Apm, Is.True);
            });
        }

        [Test]
        public void EnumeradorTipoComunicadoDevePreservarValoresDoContrato()
        {
            Assert.Multiple(() =>
            {
                Assert.That((int)TipoComunicado.SME, Is.EqualTo(1));
                Assert.That((int)TipoComunicado.DRE, Is.EqualTo(2));
                Assert.That((int)TipoComunicado.UE, Is.EqualTo(3));
                Assert.That((int)TipoComunicado.UEMOD, Is.EqualTo(4));
                Assert.That((int)TipoComunicado.TURMA, Is.EqualTo(5));
                Assert.That((int)TipoComunicado.ALUNO, Is.EqualTo(6));
                Assert.That((int)TipoComunicado.SME_ANO, Is.EqualTo(7));
                Assert.That((int)TipoComunicado.DRE_ANO, Is.EqualTo(8));
                Assert.That((int)TipoComunicado.MENSAGEM_AUTOMATICA, Is.EqualTo(9));
            });
        }

        [Test]
        public void EnumeradorTipoCriptografiaDevePreservarValoresDoContrato()
        {
            Assert.Multiple(() =>
            {
                Assert.That((int)TipoCriptografia.TripleDES, Is.EqualTo(1));
                Assert.That((int)TipoCriptografia.MD5, Is.EqualTo(2));
                Assert.That((int)TipoCriptografia.SHA512, Is.EqualTo(3));
            });
        }
    }
}
