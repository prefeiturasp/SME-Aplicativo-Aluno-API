using MediatR;
using Moq;
using SME.AE.Aplicacao.CasoDeUso.UsuarioNotificacaoMensagemLida;
using SME.AE.Aplicacao.Comandos.Aluno;
using SME.AE.Aplicacao.Comandos.Usuario.MarcarMensagemLida;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Consultas.Notificacao.ObterNotificacaoPorid;
using SME.AE.Comum.Excecoes;
using SME.AE.Dominio.Comum.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.CasosDeUso.UsuarioNotificacao
{
    public class MarcarMensagemLidaUseCaseTeste : BaseTeste
    {
        private const string CpfUsuario = "12345678900";
        private const long IdNotificacao = 10;
        private const int ModalidadeFundamental = 5;
        private const int ModalidadeMedio = 6;
        private const string DreNotificacao = "108100";
        private const string OutraDre = "108200";
        private const string UeNotificacao = "019493";
        private const string OutraUe = "019500";
        private const int TurmaNotificacao = 100;
        private const int OutraTurma = 200;
        private const string SerieNotificacao = "1";
        private const string OutraSerie = "2";

        private readonly IMarcarMensagemLidaUseCase marcarMensagemLidaUseCase;
        private readonly List<Dominio.Entidades.UsuarioNotificacao> leiturasGravadas;

        public MarcarMensagemLidaUseCaseTeste()
        {
            marcarMensagemLidaUseCase = new MarcarMensagemLidaUseCase();
            leiturasGravadas = new List<Dominio.Entidades.UsuarioNotificacao>();

            mediator.Setup(m => m.Send(It.IsAny<UsuarioNotificacaoCommand>(), It.IsAny<CancellationToken>()))
                    .Callback<IRequest<bool>, CancellationToken>((request, token) =>
                    {
                        if (request is UsuarioNotificacaoCommand command)
                            leiturasGravadas.Add(command.UsuarioNotificacao);
                    })
                    .ReturnsAsync(true);
        }

        [Fact(DisplayName = "Deve marcar leitura SME para os alunos das escolas na modalidade da notificação")]
        public async Task Deve_Marcar_Leitura_SME_Para_Alunos_Da_Modalidade()
        {
            MockarAlunosDoResponsavel(
                CriarEscola(ModalidadeFundamental, CriarAluno(1), CriarAluno(2)),
                CriarEscola(ModalidadeMedio, CriarAluno(3)));
            MockarNotificacao(CriarNotificacao(TipoComunicado.SME, modalidades: new[] { ModalidadeFundamental.ToString() }));

            await marcarMensagemLidaUseCase.Executar(mediator.Object, GerarUsuarioNotificacaoDto(), CpfUsuario);

            ValidarAlunosComLeituraGravada(1, 2);
        }

        [Fact(DisplayName = "Deve marcar leitura SME_ANO filtrando modalidade e série")]
        public async Task Deve_Marcar_Leitura_SME_ANO_Filtrando_Modalidade_E_Serie()
        {
            MockarAlunosDoResponsavel(
                CriarEscola(ModalidadeFundamental, CriarAluno(1), CriarAluno(2, serieResumida: OutraSerie)),
                CriarEscola(ModalidadeMedio, CriarAluno(3)));
            MockarNotificacao(CriarNotificacao(TipoComunicado.SME_ANO, modalidades: new[] { ModalidadeFundamental.ToString() }, seriesResumidas: $"{SerieNotificacao},3"));

            await marcarMensagemLidaUseCase.Executar(mediator.Object, GerarUsuarioNotificacaoDto(), CpfUsuario);

            ValidarAlunosComLeituraGravada(1);
        }

        [Fact(DisplayName = "Deve marcar leitura SME_ANO para todas as séries quando a notificação não tem série")]
        public async Task Deve_Marcar_Leitura_SME_ANO_Para_Todas_As_Series_Quando_Notificacao_Sem_Serie()
        {
            MockarAlunosDoResponsavel(
                CriarEscola(ModalidadeFundamental, CriarAluno(1), CriarAluno(2, serieResumida: OutraSerie)),
                CriarEscola(ModalidadeMedio, CriarAluno(3)));
            MockarNotificacao(CriarNotificacao(TipoComunicado.SME_ANO, modalidades: new[] { ModalidadeFundamental.ToString() }));

            await marcarMensagemLidaUseCase.Executar(mediator.Object, GerarUsuarioNotificacaoDto(), CpfUsuario);

            ValidarAlunosComLeituraGravada(1, 2);
        }

        [Theory(DisplayName = "Deve marcar leitura DRE para os alunos da DRE da notificação em todas as modalidades")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Deve_Marcar_Leitura_DRE_Para_Alunos_Da_Dre(bool modalidadesNulas)
        {
            MockarAlunosDoResponsavel(
                CriarEscola(ModalidadeFundamental, CriarAluno(1), CriarAluno(2, codigoDre: OutraDre)),
                CriarEscola(ModalidadeMedio, CriarAluno(3)),
                CriarEscola(ModalidadeMedio, CriarAluno(4, codigoDre: OutraDre)));
            var notificacao = CriarNotificacao(TipoComunicado.DRE, codigoDre: DreNotificacao);
            notificacao.ModalidadesId = modalidadesNulas ? null : new string[] { };
            MockarNotificacao(notificacao);

            await marcarMensagemLidaUseCase.Executar(mediator.Object, GerarUsuarioNotificacaoDto(), CpfUsuario);

            ValidarAlunosComLeituraGravada(1, 3);
        }

        [Fact(DisplayName = "Deve marcar leitura DRE ignorando escolas fora da modalidade da notificação")]
        public async Task Deve_Marcar_Leitura_DRE_Ignorando_Escolas_Fora_Da_Modalidade()
        {
            MockarAlunosDoResponsavel(
                CriarEscola(ModalidadeFundamental, CriarAluno(1), CriarAluno(2, codigoDre: OutraDre)),
                CriarEscola(ModalidadeMedio, CriarAluno(3)));
            MockarNotificacao(CriarNotificacao(TipoComunicado.DRE, modalidades: new[] { ModalidadeFundamental.ToString() }, codigoDre: DreNotificacao));

            await marcarMensagemLidaUseCase.Executar(mediator.Object, GerarUsuarioNotificacaoDto(), CpfUsuario);

            ValidarAlunosComLeituraGravada(1);
        }

        [Fact(DisplayName = "Deve marcar leitura DRE_ANO filtrando DRE, modalidade e série")]
        public async Task Deve_Marcar_Leitura_DRE_ANO_Filtrando_Dre_Modalidade_E_Serie()
        {
            MockarAlunosDoResponsavel(
                CriarEscola(ModalidadeFundamental,
                    CriarAluno(1),
                    CriarAluno(2, codigoDre: OutraDre),
                    CriarAluno(3, serieResumida: OutraSerie)),
                CriarEscola(ModalidadeMedio, CriarAluno(4)));
            MockarNotificacao(CriarNotificacao(TipoComunicado.DRE_ANO, modalidades: new[] { ModalidadeFundamental.ToString() }, codigoDre: DreNotificacao, seriesResumidas: SerieNotificacao));

            await marcarMensagemLidaUseCase.Executar(mediator.Object, GerarUsuarioNotificacaoDto(), CpfUsuario);

            ValidarAlunosComLeituraGravada(1);
        }

        [Fact(DisplayName = "Deve marcar leitura DRE_ANO para todas as modalidades e séries quando a notificação não as restringe")]
        public async Task Deve_Marcar_Leitura_DRE_ANO_Sem_Filtro_De_Modalidade_E_Serie()
        {
            MockarAlunosDoResponsavel(
                CriarEscola(ModalidadeFundamental,
                    CriarAluno(1),
                    CriarAluno(2, codigoDre: OutraDre),
                    CriarAluno(3, serieResumida: OutraSerie)),
                CriarEscola(ModalidadeMedio, CriarAluno(4)));
            MockarNotificacao(CriarNotificacao(TipoComunicado.DRE_ANO, codigoDre: DreNotificacao));

            await marcarMensagemLidaUseCase.Executar(mediator.Object, GerarUsuarioNotificacaoDto(), CpfUsuario);

            ValidarAlunosComLeituraGravada(1, 3, 4);
        }

        [Fact(DisplayName = "Deve marcar leitura UE para os alunos da escola da notificação sem filtrar série")]
        public async Task Deve_Marcar_Leitura_UE_Para_Alunos_Da_Escola_Sem_Filtrar_Serie()
        {
            MockarAlunosDoResponsavel(
                CriarEscola(ModalidadeFundamental, CriarAluno(1), CriarAluno(2, codigoEscola: OutraUe)),
                CriarEscola(ModalidadeMedio, CriarAluno(3, serieResumida: OutraSerie)));
            MockarNotificacao(CriarNotificacao(TipoComunicado.UE, codigoDre: DreNotificacao, codigoUe: UeNotificacao, seriesResumidas: SerieNotificacao));

            await marcarMensagemLidaUseCase.Executar(mediator.Object, GerarUsuarioNotificacaoDto(), CpfUsuario);

            ValidarAlunosComLeituraGravada(1, 3);
        }

        [Fact(DisplayName = "Deve marcar leitura UEMOD filtrando UE, modalidade e série")]
        public async Task Deve_Marcar_Leitura_UEMOD_Filtrando_Ue_Modalidade_E_Serie()
        {
            MockarAlunosDoResponsavel(
                CriarEscola(ModalidadeFundamental,
                    CriarAluno(1),
                    CriarAluno(2, codigoEscola: OutraUe),
                    CriarAluno(3, serieResumida: OutraSerie)),
                CriarEscola(ModalidadeMedio, CriarAluno(4)));
            MockarNotificacao(CriarNotificacao(TipoComunicado.UEMOD, modalidades: new[] { ModalidadeFundamental.ToString() }, codigoDre: DreNotificacao, codigoUe: UeNotificacao, seriesResumidas: SerieNotificacao));

            await marcarMensagemLidaUseCase.Executar(mediator.Object, GerarUsuarioNotificacaoDto(), CpfUsuario);

            ValidarAlunosComLeituraGravada(1);
        }

        [Fact(DisplayName = "Deve marcar leitura UEMOD para todas as modalidades e séries quando a notificação não as restringe")]
        public async Task Deve_Marcar_Leitura_UEMOD_Sem_Filtro_De_Modalidade_E_Serie()
        {
            MockarAlunosDoResponsavel(
                CriarEscola(ModalidadeFundamental,
                    CriarAluno(1),
                    CriarAluno(2, codigoEscola: OutraUe),
                    CriarAluno(3, serieResumida: OutraSerie)),
                CriarEscola(ModalidadeMedio, CriarAluno(4)));
            MockarNotificacao(CriarNotificacao(TipoComunicado.UEMOD, codigoDre: DreNotificacao, codigoUe: UeNotificacao));

            await marcarMensagemLidaUseCase.Executar(mediator.Object, GerarUsuarioNotificacaoDto(), CpfUsuario);

            ValidarAlunosComLeituraGravada(1, 3, 4);
        }

        [Fact(DisplayName = "Deve marcar leitura TURMA para os alunos das turmas da notificação")]
        public async Task Deve_Marcar_Leitura_TURMA_Para_Alunos_Das_Turmas()
        {
            MockarAlunosDoResponsavel(
                CriarEscola(ModalidadeFundamental, CriarAluno(1), CriarAluno(2, codigoTurma: OutraTurma)),
                CriarEscola(ModalidadeMedio, CriarAluno(3, codigoTurma: 300)));
            MockarNotificacao(CriarNotificacao(TipoComunicado.TURMA, codigoDre: DreNotificacao, codigoUe: UeNotificacao, turmas: new long[] { TurmaNotificacao, 300 }));

            await marcarMensagemLidaUseCase.Executar(mediator.Object, GerarUsuarioNotificacaoDto(), CpfUsuario);

            ValidarAlunosComLeituraGravada(1, 3);
        }

        [Theory(DisplayName = "Deve marcar leitura somente do aluno informado")]
        [InlineData(TipoComunicado.ALUNO)]
        [InlineData(TipoComunicado.MENSAGEM_AUTOMATICA)]
        public async Task Deve_Marcar_Leitura_Somente_Do_Aluno_Informado(TipoComunicado tipoComunicado)
        {
            MockarAlunosDoResponsavel(
                CriarEscola(ModalidadeFundamental, CriarAluno(1), CriarAluno(2)),
                CriarEscola(ModalidadeMedio, CriarAluno(3)));
            MockarNotificacao(CriarNotificacao(tipoComunicado, codigoDre: DreNotificacao, codigoUe: UeNotificacao));

            await marcarMensagemLidaUseCase.Executar(mediator.Object, GerarUsuarioNotificacaoDto(codigoAlunoEol: 2), CpfUsuario);

            ValidarAlunosComLeituraGravada(2);
        }

        [Theory(DisplayName = "Deve marcar leitura uma única vez quando o aluno aparece em mais de uma escola")]
        [InlineData(TipoComunicado.ALUNO)]
        [InlineData(TipoComunicado.MENSAGEM_AUTOMATICA)]
        public async Task Deve_Marcar_Leitura_Uma_Unica_Vez_Quando_Aluno_Em_Mais_De_Uma_Escola(TipoComunicado tipoComunicado)
        {
            MockarAlunosDoResponsavel(
                CriarEscola(ModalidadeFundamental, CriarAluno(1), CriarAluno(2)),
                CriarEscola(ModalidadeMedio, CriarAluno(2, codigoDre: OutraDre, codigoEscola: OutraUe)));
            MockarNotificacao(CriarNotificacao(tipoComunicado, codigoDre: DreNotificacao, codigoUe: UeNotificacao));

            await marcarMensagemLidaUseCase.Executar(mediator.Object, GerarUsuarioNotificacaoDto(codigoAlunoEol: 2), CpfUsuario);

            ValidarAlunosComLeituraGravada(2);
            var leitura = Assert.Single(leiturasGravadas);
            Assert.Contains((leitura.DreCodigoEol, leitura.UeCodigoEol), new[]
            {
                (long.Parse(DreNotificacao), UeNotificacao),
                (long.Parse(OutraDre), OutraUe)
            });
        }

        [Fact(DisplayName = "Deve montar o registro de leitura com os dados do aluno e do usuário")]
        public async Task Deve_Montar_Registro_De_Leitura_Com_Dados_Do_Aluno_E_Usuario()
        {
            var aluno = CriarAluno(1);
            var usuarioNotificacaoDto = GerarUsuarioNotificacaoDto(mensagemVisualizada: true);
            MockarAlunosDoResponsavel(CriarEscola(ModalidadeFundamental, aluno));
            MockarNotificacao(CriarNotificacao(TipoComunicado.UE, codigoDre: DreNotificacao, codigoUe: UeNotificacao));

            await marcarMensagemLidaUseCase.Executar(mediator.Object, usuarioNotificacaoDto, CpfUsuario);

            var leitura = Assert.Single(leiturasGravadas);
            Assert.Equal(CpfUsuario, leitura.UsuarioCpf);
            Assert.Equal(IdNotificacao, leitura.NotificacaoId);
            Assert.Equal(usuarioNotificacaoDto.UsuarioId, leitura.UsuarioId);
            Assert.Equal(long.Parse(aluno.CodigoDre), leitura.DreCodigoEol);
            Assert.Equal(aluno.CodigoEscola, leitura.UeCodigoEol);
            Assert.Equal((long)aluno.CodigoEol, leitura.CodigoEolAluno);
            Assert.True(leitura.MensagemVisualizada);
            Assert.False(leitura.MensagemExcluida);
            Assert.Equal("Sistema", leitura.CriadoPor);
            Assert.NotEqual(default, leitura.CriadoEm);
        }

        [Theory(DisplayName = "Deve retornar a notificação com o status de visualização informado")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Deve_Retornar_Notificacao_Com_Status_De_Visualizacao(bool mensagemVisualizada)
        {
            var notificacao = CriarNotificacao(TipoComunicado.UE, codigoDre: DreNotificacao, codigoUe: UeNotificacao);
            MockarAlunosDoResponsavel(CriarEscola(ModalidadeFundamental, CriarAluno(1)));
            MockarNotificacao(notificacao);

            var resultado = await marcarMensagemLidaUseCase.Executar(mediator.Object, GerarUsuarioNotificacaoDto(mensagemVisualizada: mensagemVisualizada), CpfUsuario);

            Assert.Same(notificacao, resultado);
            Assert.Equal(mensagemVisualizada, resultado.MensagemVisualizada);
            Assert.Equal(mensagemVisualizada, Assert.Single(leiturasGravadas).MensagemVisualizada);
        }

        [Fact(DisplayName = "Deve lançar erro quando não obtiver os alunos do responsável")]
        public async Task Deve_Lancar_Erro_Quando_Nao_Obtiver_Alunos_Do_Responsavel()
        {
            mediator.Setup(m => m.Send(It.IsAny<DadosAlunoCommand>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(RespostaApi.Falha("Erro ao obter os alunos"));

            var excecao = await Assert.ThrowsAsync<NegocioException>(async () => await marcarMensagemLidaUseCase.Executar(mediator.Object, GerarUsuarioNotificacaoDto(), CpfUsuario));

            Assert.Equal("Não foi possivel obter os alunos por escola", excecao.Message);
            mediator.Verify(m => m.Send(It.IsAny<ObterNotificacaoPorIdQuery>(), It.IsAny<CancellationToken>()), Times.Never);
            mediator.Verify(m => m.Send(It.IsAny<UsuarioNotificacaoCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact(DisplayName = "Deve lançar erro quando a notificação não existir")]
        public async Task Deve_Lancar_Erro_Quando_Notificacao_Nao_Existir()
        {
            MockarAlunosDoResponsavel(CriarEscola(ModalidadeFundamental, CriarAluno(1)));
            MockarNotificacao(null);

            var excecao = await Assert.ThrowsAsync<NegocioException>(async () => await marcarMensagemLidaUseCase.Executar(mediator.Object, GerarUsuarioNotificacaoDto(), CpfUsuario));

            Assert.Equal("Não foi possivel localizar a notificação.", excecao.Message);
            mediator.Verify(m => m.Send(It.IsAny<UsuarioNotificacaoCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact(DisplayName = "Deve lançar erro quando o tipo de comunicado não for reconhecido")]
        public async Task Deve_Lancar_Erro_Quando_Tipo_De_Comunicado_Nao_Reconhecido()
        {
            MockarAlunosDoResponsavel(CriarEscola(ModalidadeFundamental, CriarAluno(1)));
            MockarNotificacao(CriarNotificacao((TipoComunicado)99));

            var excecao = await Assert.ThrowsAsync<NegocioException>(async () => await marcarMensagemLidaUseCase.Executar(mediator.Object, GerarUsuarioNotificacaoDto(), CpfUsuario));

            Assert.Equal("Não identificado o Tipo desta notificação", excecao.Message);
            mediator.Verify(m => m.Send(It.IsAny<UsuarioNotificacaoCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory(DisplayName = "Deve lançar erro quando nenhum aluno do responsável se encaixar na notificação")]
        [InlineData(TipoComunicado.SME, "Nenhum aluno deste usuário está matriculado no grupo desta Notificação")]
        [InlineData(TipoComunicado.SME_ANO, "Nenhum aluno deste usuário está matriculado no grupo/ano desta Notificação")]
        [InlineData(TipoComunicado.DRE, "Nenhum aluno deste usuário está matriculado na DRE desta Notificação")]
        [InlineData(TipoComunicado.DRE_ANO, "Nenhum aluno deste usuário está matriculado na DRE/Ano desta Notificação")]
        [InlineData(TipoComunicado.UE, "Nenhum Aluno está matriculado na escola da notificação")]
        [InlineData(TipoComunicado.UEMOD, "Nenhum Aluno está matriculado na modalidade/ano da escola da notificação")]
        [InlineData(TipoComunicado.TURMA, "Nenhum Aluno está matriculado na turma notificada")]
        [InlineData(TipoComunicado.ALUNO, "Não encontrado aluno com o codigo 999")]
        [InlineData(TipoComunicado.MENSAGEM_AUTOMATICA, "Não encontrado aluno com o codigo 999")]
        public async Task Deve_Lancar_Erro_Quando_Nenhum_Aluno_Se_Encaixar_Na_Notificacao(TipoComunicado tipoComunicado, string mensagemEsperada)
        {
            MockarAlunosDoResponsavel(CriarEscola(ModalidadeFundamental, CriarAluno(1)));
            MockarNotificacao(CriarNotificacao(tipoComunicado,
                                               modalidades: new[] { "99" },
                                               codigoDre: "999999",
                                               codigoUe: "999999",
                                               seriesResumidas: "9",
                                               turmas: new long[] { 999 }));

            var excecao = await Assert.ThrowsAsync<NegocioException>(async () => await marcarMensagemLidaUseCase.Executar(mediator.Object, GerarUsuarioNotificacaoDto(codigoAlunoEol: 999), CpfUsuario));

            Assert.Equal(mensagemEsperada, excecao.Message);
            mediator.Verify(m => m.Send(It.IsAny<UsuarioNotificacaoCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        private void ValidarAlunosComLeituraGravada(params long[] codigosAlunos)
        {
            Assert.Equal(codigosAlunos.OrderBy(codigo => codigo), leiturasGravadas.Select(leitura => leitura.CodigoEolAluno).OrderBy(codigo => codigo));
            mediator.Verify(m => m.Send(It.IsAny<UsuarioNotificacaoCommand>(), It.IsAny<CancellationToken>()), Times.Exactly(codigosAlunos.Length));
        }

        private void MockarAlunosDoResponsavel(params ListaEscola[] escolas)
        {
            mediator.Setup(m => m.Send(It.Is<DadosAlunoCommand>(c => c.Cpf == CpfUsuario), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(RespostaApi.Sucesso(escolas));
        }

        private void MockarNotificacao(NotificacaoResposta notificacao)
        {
            mediator.Setup(m => m.Send(It.Is<ObterNotificacaoPorIdQuery>(q => q.Id == IdNotificacao), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(notificacao);
        }

        private static UsuarioNotificacaoDto GerarUsuarioNotificacaoDto(long codigoAlunoEol = 0, bool mensagemVisualizada = true)
        {
            return new UsuarioNotificacaoDto
            {
                NotificacaoId = IdNotificacao,
                UsuarioId = 1,
                CodigoAlunoEol = codigoAlunoEol,
                MensagemVisualizada = mensagemVisualizada
            };
        }

        private static ListaEscola CriarEscola(int modalidadeCodigo, params Dominio.Entidades.Aluno[] alunos)
        {
            return new ListaEscola
            {
                ModalidadeCodigo = modalidadeCodigo,
                Modalidade = $"Modalidade {modalidadeCodigo}",
                Alunos = alunos
            };
        }

        private static Dominio.Entidades.Aluno CriarAluno(int codigoEol,
                                                          string codigoDre = DreNotificacao,
                                                          string codigoEscola = UeNotificacao,
                                                          int codigoTurma = TurmaNotificacao,
                                                          string serieResumida = SerieNotificacao)
        {
            return new Dominio.Entidades.Aluno
            {
                CodigoEol = codigoEol,
                Nome = $"Aluno {codigoEol}",
                CpfResponsavel = CpfUsuario,
                CodigoDre = codigoDre,
                CodigoEscola = codigoEscola,
                CodigoTurma = codigoTurma,
                SerieResumida = serieResumida
            };
        }

        private static NotificacaoResposta CriarNotificacao(TipoComunicado tipoComunicado,
                                                            string[] modalidades = null,
                                                            string codigoDre = null,
                                                            string codigoUe = null,
                                                            string seriesResumidas = null,
                                                            long[] turmas = null)
        {
            return new NotificacaoResposta
            {
                Id = IdNotificacao,
                Titulo = "Teste",
                Mensagem = "Teste Mensagem",
                TipoComunicado = tipoComunicado,
                DataEnvio = DateTime.Now,
                DataExpiracao = DateTime.Now.AddDays(7),
                ModalidadesId = modalidades ?? new string[] { },
                CodigoDre = codigoDre,
                CodigoUe = codigoUe,
                SeriesResumidas = seriesResumidas,
                Turmas = (turmas ?? new long[] { }).Select(codigoTurma => new Dominio.Entidades.NotificacaoTurma
                {
                    CodigoTurma = codigoTurma,
                    NotificacaoId = IdNotificacao
                }).ToList()
            };
        }
    }
}
