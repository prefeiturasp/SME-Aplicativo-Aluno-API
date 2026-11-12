using NUnit.Framework;
using SME.AE.Comum.Excecoes;
using SME.AE.Dominio.Comum.Enumeradores;
using SME.AE.Dominio.Entidades;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SME.AE.Dominio.Teste.Entidades
{
    [TestFixture]
    public class EntidadesTests
    {
        [Test]
        public void EntidadeBaseDeveAtualizarEInserirAuditoria()
        {
            var entidade = new Adesao();

            entidade.InserirAuditoria();
            var criadoEm = entidade.CriadoEm;
            entidade.AtualizarAuditoria();

            Assert.Multiple(() =>
            {
                Assert.That(criadoEm, Is.Not.EqualTo(default(DateTime)));
                Assert.That(entidade.CriadoPor, Is.EqualTo("Sistema"));
                Assert.That(entidade.AlteradoEm, Is.Not.Null);
                Assert.That(entidade.AlteradoPor, Is.EqualTo("Sistema"));
            });
        }

        [Test]
        public void AutenticacaoDeveIniciarSemDadosQuandoNaoPossuiConstrutorDeCarga()
        {
            var autenticacao = new Autenticacao();

            Assert.Multiple(() =>
            {
                Assert.That(autenticacao.Cpf, Is.Null);
                Assert.That(autenticacao.Nome, Is.Null);
            });
        }

        [Test]
        public void AdesaoDeveInicializarDadosInformados()
        {
            var adesao = new Adesao("DRE", "Nome DRE", "UE", "Nome UE", 10, 1, 2, 3, 4);

            Assert.Multiple(() =>
            {
                Assert.That(adesao.CodigoDre, Is.EqualTo("DRE"));
                Assert.That(adesao.NomeDre, Is.EqualTo("Nome DRE"));
                Assert.That(adesao.CodigoUe, Is.EqualTo("UE"));
                Assert.That(adesao.NomeUe, Is.EqualTo("Nome UE"));
                Assert.That(adesao.CodigoTurma, Is.EqualTo(10));
                Assert.That(adesao.UsuariosPrimeiroAcessoIncompleto, Is.EqualTo(1));
                Assert.That(adesao.UsuariosValidos, Is.EqualTo(2));
                Assert.That(adesao.UsuariosCpfInvalidos, Is.EqualTo(3));
                Assert.That(adesao.UsuariosSemAppInstalado, Is.EqualTo(4));
            });
        }

        [Test]
        public void AceiteTermosDeUsoDeveGerarHashEPermitirExclusaoLogica()
        {
            const long termosId = 7;
            const string cpf = "123";
            const string device = "device";
            const string ip = "127.0.0.1";
            const double versao = 1.2;
            var aceite = new AceiteTermosDeUso(termosId, cpf, device, ip, versao);
            var esperado = MD5.HashData(Encoding.ASCII.GetBytes($"{termosId}{cpf}{device}{ip}{versao}"));

            aceite.MarcarComoExcluido();

            Assert.Multiple(() =>
            {
                Assert.That(aceite.TermosDeUsoId, Is.EqualTo(termosId));
                Assert.That(aceite.CpfUsuario, Is.EqualTo(cpf));
                Assert.That(aceite.Device, Is.EqualTo(device));
                Assert.That(aceite.Ip, Is.EqualTo(ip));
                Assert.That(aceite.Versao, Is.EqualTo(versao));
                Assert.That(aceite.DataHoraAceite, Is.Not.EqualTo(default(DateTime)));
                Assert.That(aceite.Hash, Is.EqualTo(esperado));
                Assert.That(aceite.Excluido, Is.True);
                Assert.That(aceite.AlteradoPor, Is.EqualTo("Sistema"));
            });
        }

        [Test]
        public void NotificacaoDeveDefinirCategoriaSmeQuandoNaoPossuiEscopo()
        {
            var notificacao = new Notificacao();

            notificacao.InserirCategoria();

            Assert.That(notificacao.CategoriaNotificacao, Is.EqualTo("SME"));
        }

        [Test]
        public void NotificacaoDeveDefinirCategoriaDreQuandoPossuiDreSemUe()
        {
            var notificacao = new Notificacao { CodigoDre = "DRE" };

            notificacao.InserirCategoria();

            Assert.That(notificacao.CategoriaNotificacao, Is.EqualTo("DRE"));
        }

        [Test]
        public void NotificacaoDeveDefinirCategoriaUeQuandoPossuiUe()
        {
            var notificacao = new Notificacao { CodigoUe = "UE" };

            notificacao.InserirCategoria();

            Assert.That(notificacao.CategoriaNotificacao, Is.EqualTo("UE"));
        }

        [Test]
        public void NotificacaoDeveArmazenarDadosDeEnvio()
        {
            var envio = DateTime.UtcNow;
            var expiracao = envio.AddDays(1);
            var notificacao = new Notificacao
            {
                Id = 1,
                DataEnvio = envio,
                DataExpiracao = expiracao,
                Alunos = new[] { "1" },
                Mensagem = "mensagem",
                Titulo = "titulo",
                AnoLetivo = 2025,
                SeriesResumidas = "1A",
                CodigoDre = "DRE",
                CodigoUe = "UE",
                Turmas = new[] { "T1" },
                TipoComunicado = TipoComunicado.ALUNO,
                EnviadoPushNotification = true,
                Modalidades = "modalidade",
                TiposEscolas = "escola"
            };

            Assert.Multiple(() =>
            {
                Assert.That(notificacao.Id, Is.EqualTo(1));
                Assert.That(notificacao.DataEnvio, Is.EqualTo(envio));
                Assert.That(notificacao.DataExpiracao, Is.EqualTo(expiracao));
                Assert.That(notificacao.Alunos.Single(), Is.EqualTo("1"));
                Assert.That(notificacao.Turmas.Single(), Is.EqualTo("T1"));
                Assert.That(notificacao.TipoComunicado, Is.EqualTo(TipoComunicado.ALUNO));
                Assert.That(notificacao.EnviadoPushNotification, Is.True);
            });
        }

        [Test]
        public void UsuarioDeveAtualizarLoginEFinalizarRedefinicao()
        {
            var usuario = new Usuario { Excluido = true };

            usuario.AtualizarLogin(true);
            usuario.IniciarRedefinicaoSenha();
            var token = usuario.Token;
            usuario.FinalizarRedefinicaoSenha();

            Assert.Multiple(() =>
            {
                Assert.That(usuario.UltimoLogin, Is.Not.EqualTo(default(DateTime)));
                Assert.That(usuario.Excluido, Is.False);
                Assert.That(usuario.PrimeiroAcesso, Is.True);
                Assert.That(token, Has.Length.EqualTo(8));
                Assert.That(token, Is.EqualTo(token.ToUpperInvariant()));
                Assert.That(usuario.RedefinirSenha, Is.False);
                Assert.That(usuario.Token, Is.Empty);
                Assert.That(usuario.ValidadeToken, Is.Null);
            });
        }

        [Test]
        public void UsuarioDeveValidarTokenAtual()
        {
            var usuario = new Usuario();
            usuario.IniciarRedefinicaoSenha();

            Assert.That(() => usuario.ValidarTokenRedefinicao(usuario.Token), Throws.Nothing);
        }

        [Test]
        public void UsuarioDeveAceitarTokenQuandoValidadeNaoFoiDefinida()
        {
            var usuario = new Usuario { Token = "VALIDO" };

            Assert.That(() => usuario.ValidarTokenRedefinicao("VALIDO"), Throws.Nothing);
        }

        [Test]
        public void UsuarioDeveRejeitarTokenVazioExpiradoEInvalido()
        {
            var usuario = new Usuario { ValidadeToken = DateTime.Now.AddMinutes(-1), Token = "VALIDO" };

            Assert.Multiple(() =>
            {
                Assert.That(() => usuario.ValidarTokenRedefinicao(null), Throws.TypeOf<NegocioException>());
                Assert.That(() => usuario.ValidarTokenRedefinicao("OUTRO"), Throws.TypeOf<NegocioException>());
                usuario.ValidadeToken = DateTime.Now.AddMinutes(1);
                Assert.That(() => usuario.ValidarTokenRedefinicao("OUTRO"), Throws.TypeOf<NegocioException>());
            });
        }

        [Test]
        public void TermosDeUsoDeveCriarEAtualizarVersao()
        {
            var termos = new TermosDeUso("termos", "politica", 1.0);

            termos.AtualizaVersao(2.0);

            Assert.Multiple(() =>
            {
                Assert.That(termos.DescricaoTermosDeUso, Is.EqualTo("termos"));
                Assert.That(termos.DescricaoPoliticaPrivacidade, Is.EqualTo("politica"));
                Assert.That(termos.Versao, Is.EqualTo(2.0));
            });
        }

        [Test]
        public void EntidadesComAuditoriaDevemArmazenarPropriedades()
        {
            var configuracao = new ConfiguracaoEmail
            {
                EmailRemetente = "from@example.com", NomeRemetente = "Remetente", Porta = 587,
                Senha = "senha", ServidorSmtp = "smtp", UsarTls = true, Usuario = "usuario"
            };
            var grupo = new GrupoComunicado
            {
                Nome = "grupo", TipoEscolaId = "escola", TipoCicloId = "ciclo", EtapaEnsinoId = "etapa",
                Excluido = true, CriadoRf = "criador", AlteradoRf = "editor"
            };
            var outro = new OutroServico
            {
                Titulo = "titulo", Descricao = "descricao", Categoria = "categoria", UrlSite = "url",
                Icone = "icone", Destaque = true, Ativo = true
            };
            var usuarioNotificacao = new UsuarioNotificacao
            {
                UsuarioId = 1, CodigoEolAluno = 2, NotificacaoId = 3, DreCodigoEol = 4,
                UeCodigoEol = "UE", UsuarioCpf = "CPF", MensagemVisualizada = true, MensagemExcluida = true
            };

            Assert.Multiple(() =>
            {
                Assert.That(configuracao.Porta, Is.EqualTo(587));
                Assert.That(configuracao.UsarTls, Is.True);
                Assert.That(grupo.Excluido, Is.True);
                Assert.That(outro.Ativo, Is.True);
                Assert.That(outro.Destaque, Is.True);
                Assert.That(usuarioNotificacao.MensagemVisualizada, Is.True);
                Assert.That(usuarioNotificacao.MensagemExcluida, Is.True);
            });
        }

        [Test]
        public void EntidadesDeNotificacaoDevemRegistrarDataDeCriacao()
        {
            var aluno = new NotificacaoAluno { Id = 1, CodigoAluno = 10, NotificacaoId = 20 };
            var turma = new NotificacaoTurma { Id = 2, CodigoTurma = 30, NotificacaoId = 40 };

            aluno.InserirAuditoria();
            turma.InserirAuditoria();

            Assert.Multiple(() =>
            {
                Assert.That(aluno.CriadoEm, Is.Not.EqualTo(default(DateTime)));
                Assert.That(turma.CriadoEm, Is.Not.EqualTo(default(DateTime)));
                Assert.That(aluno.CodigoAluno, Is.EqualTo(10));
                Assert.That(turma.CodigoTurma, Is.EqualTo(30));
            });
        }

        [Test]
        public void TermosDeUsoDevePermitirConstrucaoSemDados()
        {
            var termos = new TermosDeUso();

            Assert.Multiple(() =>
            {
                Assert.That(termos.DescricaoTermosDeUso, Is.Null);
                Assert.That(termos.DescricaoPoliticaPrivacidade, Is.Null);
                Assert.That(termos.Versao, Is.Zero);
            });
        }

        [Test]
        public void UsuarioDeveUsarPrimeiroAcessoFalsoQuandoNaoInformado()
        {
            var usuario = new Usuario { Cpf = "123" };

            usuario.AtualizarLogin();

            Assert.Multiple(() =>
            {
                Assert.That(usuario.Cpf, Is.EqualTo("123"));
                Assert.That(usuario.PrimeiroAcesso, Is.False);
                Assert.That(usuario.UltimoLogin, Is.Not.EqualTo(default(DateTime)));
            });
        }
    }
}
