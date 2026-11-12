using NUnit.Framework;
using SME.AE.Dominio.Comum.Enumeradores;
using SME.AE.Dominio.Entidades;
using SME.AE.Dominio.Entidades.Externas;
using System;

namespace SME.AE.Dominio.Teste.Entidades.Externas
{
    [TestFixture]
    public class EntidadesExternasTests
    {
        [Test]
        public void PessoaCoreSsoDeveArmazenarDadosDaPessoa()
        {
            var criado = DateTime.UtcNow.AddDays(-1);
            var alterado = DateTime.UtcNow;
            var pessoa = new PessoaCoreSSO
            {
                Id = Guid.NewGuid(),
                Nome = "Pessoa",
                Situacao = 1,
                DataCriacao = criado,
                DataAlteracao = alterado,
                Integridade = 2,
                NomeSocial = "Nome Social"
            };

            Assert.Multiple(() =>
            {
                Assert.That(pessoa.Id, Is.Not.EqualTo(Guid.Empty));
                Assert.That(pessoa.Nome, Is.EqualTo("Pessoa"));
                Assert.That(pessoa.Situacao, Is.EqualTo(1));
                Assert.That(pessoa.DataCriacao, Is.EqualTo(criado));
                Assert.That(pessoa.DataAlteracao, Is.EqualTo(alterado));
                Assert.That(pessoa.Integridade, Is.EqualTo(2));
                Assert.That(pessoa.NomeSocial, Is.EqualTo("Nome Social"));
                Assert.That(pessoa, Is.InstanceOf<EntidadeExterna>());
            });
        }

        [Test]
        public void UsuarioGrupoCoreSsoDeveArmazenarRelacionamentoEStatus()
        {
            var usuarioId = Guid.NewGuid();
            var grupoId = Guid.NewGuid();
            var grupo = new UsuarioGrupoCoreSSO
            {
                UsuarioId = usuarioId,
                GrupoId = grupoId,
                Situacao = 1
            };

            Assert.Multiple(() =>
            {
                Assert.That(grupo.UsuarioId, Is.EqualTo(usuarioId));
                Assert.That(grupo.GrupoId, Is.EqualTo(grupoId));
                Assert.That(grupo.Situacao, Is.EqualTo(1));
                Assert.That(grupo, Is.InstanceOf<EntidadeExterna>());
            });
        }

        [Test]
        public void UsuarioSenhaHistoricoCoreSsoDeveArmazenarDadosDaSenha()
        {
            var usuarioId = Guid.NewGuid();
            var senhaId = Guid.NewGuid();
            var data = DateTime.UtcNow;
            var historico = new UsuarioSenhaHistoricoCoreSSO
            {
                UsuarioId = usuarioId,
                SenhaId = senhaId,
                Senha = "hash",
                Criptografia = TipoCriptografia.SHA512,
                Data = data
            };

            Assert.Multiple(() =>
            {
                Assert.That(historico.UsuarioId, Is.EqualTo(usuarioId));
                Assert.That(historico.SenhaId, Is.EqualTo(senhaId));
                Assert.That(historico.Senha, Is.EqualTo("hash"));
                Assert.That(historico.Criptografia, Is.EqualTo(TipoCriptografia.SHA512));
                Assert.That(historico.Data, Is.EqualTo(data));
                Assert.That(historico, Is.InstanceOf<EntidadeExterna>());
            });
        }

        [Test]
        public void AlunoDeveArmazenarDadosAcademicosEResponsavel()
        {
            var nascimento = new DateTime(2010, 5, 10);
            var situacao = new DateTime(2025, 2, 1);
            var aluno = new Aluno
            {
                CodigoEol = 10,
                Nome = "Aluno",
                NomeSocial = "Nome Social",
                NomeResponsavel = "Responsável",
                CpfResponsavel = "12345678900",
                CodigoEscola = "E1",
                Escola = "Escola",
                CodigoTipoEscola = 2,
                DescricaoTipoEscola = "Municipal",
                CodigoDre = "D1",
                SiglaDre = "DRE",
                CodigoTurma = 3,
                Turma = "3A",
                SituacaoMatricula = "Ativa",
                DataNascimento = nascimento,
                DataSituacaoMatricula = situacao,
                SerieResumida = "3"
            };

            Assert.Multiple(() =>
            {
                Assert.That(aluno.CodigoEol, Is.EqualTo(10));
                Assert.That(aluno.NomeSocial, Is.EqualTo("Nome Social"));
                Assert.That(aluno.CpfResponsavel, Is.EqualTo("12345678900"));
                Assert.That(aluno.CodigoTipoEscola, Is.EqualTo(2));
                Assert.That(aluno.DataNascimento, Is.EqualTo(nascimento));
                Assert.That(aluno.DataSituacaoMatricula, Is.EqualTo(situacao));
                Assert.That(aluno.SerieResumida, Is.EqualTo("3"));
            });
        }
    }
}
