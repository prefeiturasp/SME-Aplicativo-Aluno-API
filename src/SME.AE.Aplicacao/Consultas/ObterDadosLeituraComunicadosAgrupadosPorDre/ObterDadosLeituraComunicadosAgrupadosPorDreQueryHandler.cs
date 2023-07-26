using MediatR;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.Dre;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.ObterDadosLeituraComunicados
{
    public class ObterDadosLeituraComunicadosAgrupadosPorDreQueryHandler : IRequestHandler<ObterDadosLeituraComunicadosAgrupadosPorDreQuery, IEnumerable<DadosLeituraComunicadosResultado>>
    {
        private readonly IDadosLeituraRepositorio dadosLeituraRepositorio;
        private readonly IUsuarioNotificacaoRepositorio usuarioNotificacaoLeituraRepositorio;
        private readonly IMediator mediator;

        public ObterDadosLeituraComunicadosAgrupadosPorDreQueryHandler(IDadosLeituraRepositorio dadosLeituraRepositorio,
                                                                       IUsuarioNotificacaoRepositorio usuarioNotificacaoLeituraRepositorio,
                                                                       IMediator mediator)
        {
            this.dadosLeituraRepositorio = dadosLeituraRepositorio ?? throw new System.ArgumentNullException(nameof(dadosLeituraRepositorio));
            this.usuarioNotificacaoLeituraRepositorio = usuarioNotificacaoLeituraRepositorio ?? throw new System.ArgumentNullException(nameof(usuarioNotificacaoLeituraRepositorio));
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<DadosLeituraComunicadosResultado>> Handle(ObterDadosLeituraComunicadosAgrupadosPorDreQuery request, CancellationToken cancellationToken)
        {
            var dadosLeituraComunicadosAgrupadosPorDre = await dadosLeituraRepositorio.ObterDadosLeituraComunicadosPorDre(request.NotificaoId);

            if (dadosLeituraComunicadosAgrupadosPorDre == null || !dadosLeituraComunicadosAgrupadosPorDre.Any())
                throw new Exception("Não foram encontrados dados de leitura de comunicados");

            var retornoDadosLeituraComunicadosResultado = new List<DadosLeituraComunicadosResultado>();
            var dres = await mediator.Send(new ObterDresQuery());

            foreach (var dadosLeituraDre in dadosLeituraComunicadosAgrupadosPorDre)
            {
                var dadosLeituraComunicadosResultado = new DadosLeituraComunicadosResultado();
                dadosLeituraComunicadosResultado.NomeAbreviadoDre = dres?.Where(d => d.CodigoDre == dadosLeituraDre.DreCodigo).FirstOrDefault()?.Abreviacao;

                if (request.ModoVisualizacao == ModoVisualizacao.Responsavel)
                    await ObterTotaisDeLeituraPorResponsavel(request, dadosLeituraDre, dadosLeituraComunicadosResultado);

                if (request.ModoVisualizacao == ModoVisualizacao.Aluno)
                    await ObterTotaisDeLeituraPorAluno(request, dadosLeituraDre, dadosLeituraComunicadosResultado);

                retornoDadosLeituraComunicadosResultado.Add(dadosLeituraComunicadosResultado);
            }

            return retornoDadosLeituraComunicadosResultado;
        }

        private async Task ObterTotaisDeLeituraPorAluno(ObterDadosLeituraComunicadosAgrupadosPorDreQuery request, DadosConsolidacaoNotificacaoResultado dadosLeituraComunicados, DadosLeituraComunicadosResultado dadosLeituraComunicadosResultado)
        {
            var totalNotificacoesLeituraPorAluno = await usuarioNotificacaoLeituraRepositorio.ObterTotalNotificacoesLeituraPorAluno(request.NotificaoId, long.Parse(dadosLeituraComunicados.DreCodigo));
            dadosLeituraComunicadosResultado.ReceberamENaoVisualizaram = (dadosLeituraComunicados.QuantidadeAlunosComApp - totalNotificacoesLeituraPorAluno);
            dadosLeituraComunicadosResultado.NaoReceberamComunicado = dadosLeituraComunicados.QuantidadeAlunosSemApp;
            dadosLeituraComunicadosResultado.VisualizaramComunicado = totalNotificacoesLeituraPorAluno;
        }

        private async Task ObterTotaisDeLeituraPorResponsavel(ObterDadosLeituraComunicadosAgrupadosPorDreQuery request, DadosConsolidacaoNotificacaoResultado dadosLeituraComunicados, DadosLeituraComunicadosResultado dadosLeituraComunicadosResultado)
        {
            var totalNotificacoesLeituraPorResponsavel = await usuarioNotificacaoLeituraRepositorio.ObterTotalNotificacoesLeituraPorResponsavel(request.NotificaoId, long.Parse(dadosLeituraComunicados.DreCodigo));
            dadosLeituraComunicadosResultado.ReceberamENaoVisualizaram = (dadosLeituraComunicados.QuantidadeResponsaveisComApp - totalNotificacoesLeituraPorResponsavel);
            dadosLeituraComunicadosResultado.NaoReceberamComunicado = dadosLeituraComunicados.QuantidadeResponsaveisSemApp;
            dadosLeituraComunicadosResultado.VisualizaramComunicado = totalNotificacoesLeituraPorResponsavel;
        }
    }
}
