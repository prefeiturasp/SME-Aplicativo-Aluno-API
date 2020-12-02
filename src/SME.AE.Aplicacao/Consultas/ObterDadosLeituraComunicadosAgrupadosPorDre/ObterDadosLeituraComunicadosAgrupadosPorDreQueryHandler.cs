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
        private readonly IDreSgpRepositorio dreSgpRepositorio;

        public ObterDadosLeituraComunicadosAgrupadosPorDreQueryHandler(IDadosLeituraRepositorio dadosLeituraRepositorio, 
                                                                       IUsuarioNotificacaoRepositorio usuarioNotificacaoLeituraRepositorio,
                                                                       IDreSgpRepositorio dreSgpRepositorio)
        {
            this.dadosLeituraRepositorio = dadosLeituraRepositorio ?? throw new System.ArgumentNullException(nameof(dadosLeituraRepositorio));
            this.usuarioNotificacaoLeituraRepositorio = usuarioNotificacaoLeituraRepositorio ?? throw new System.ArgumentNullException(nameof(usuarioNotificacaoLeituraRepositorio));
            this.dreSgpRepositorio = dreSgpRepositorio ?? throw new System.ArgumentNullException(nameof(dreSgpRepositorio));
        }

        public async Task<IEnumerable<DadosLeituraComunicadosResultado>> Handle(ObterDadosLeituraComunicadosAgrupadosPorDreQuery request, CancellationToken cancellationToken)
        {
            var dadosLeituraComunicadosAgrupadosPorDre = await dadosLeituraRepositorio.ObterDadosLeituraComunicadosPorDre(request.NotificaoId);

            if (dadosLeituraComunicadosAgrupadosPorDre == null || !dadosLeituraComunicadosAgrupadosPorDre.Any())
                throw new Exception("Não foram encontrados dados de leitura de comunicados");

            var retornoDadosLeituraComunicadosResultado = new List<DadosLeituraComunicadosResultado>();
            
            foreach (var dadosLeituraDre in dadosLeituraComunicadosAgrupadosPorDre)
            {
                var dadosLeituraComunicadosResultado = new DadosLeituraComunicadosResultado();
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

            var nomeAbreviadoDre = await ObterNomeAvreviadoDrePorCodigo(dadosLeituraComunicados);

            dadosLeituraComunicadosResultado.NomeAbreviadoDre = nomeAbreviadoDre.NomeAbreviado;
            dadosLeituraComunicadosResultado.ReceberamENaoVisualizaram = (dadosLeituraComunicados.QuantidadeAlunosComApp - totalNotificacoesLeituraPorAluno);
            dadosLeituraComunicadosResultado.NaoReceberamComunicado = dadosLeituraComunicados.QuantidadeAlunosSemApp;
            dadosLeituraComunicadosResultado.VisualizaramComunicado = totalNotificacoesLeituraPorAluno;
        }

        private async Task ObterTotaisDeLeituraPorResponsavel(ObterDadosLeituraComunicadosAgrupadosPorDreQuery request, DadosConsolidacaoNotificacaoResultado dadosLeituraComunicados, DadosLeituraComunicadosResultado dadosLeituraComunicadosResultado)
        {
            var totalNotificacoesLeituraPorResponsavel = await usuarioNotificacaoLeituraRepositorio.ObterTotalNotificacoesLeituraPorResponsavel(request.NotificaoId, long.Parse(dadosLeituraComunicados.DreCodigo));
            
            var nomeAbreviadoDre = await ObterNomeAvreviadoDrePorCodigo(dadosLeituraComunicados);
            dadosLeituraComunicadosResultado.NomeAbreviadoDre = nomeAbreviadoDre.NomeAbreviado;
            dadosLeituraComunicadosResultado.ReceberamENaoVisualizaram = (dadosLeituraComunicados.QuantidadeResponsaveisComApp - totalNotificacoesLeituraPorResponsavel);
            dadosLeituraComunicadosResultado.NaoReceberamComunicado = dadosLeituraComunicados.QuantidadeResponsaveisSemApp;
            dadosLeituraComunicadosResultado.VisualizaramComunicado = totalNotificacoesLeituraPorResponsavel;
        }
        private async Task<DreResposta> ObterNomeAvreviadoDrePorCodigo(DadosConsolidacaoNotificacaoResultado dadosLeituraComunicados)
        {
            var nomeAbreviadoDre = await dreSgpRepositorio.ObterNomeAbreviadoDrePorCodigo(dadosLeituraComunicados.DreCodigo);
            if (nomeAbreviadoDre == null)
                throw new Exception("Não foi possível encontrar a DRE!");
            return nomeAbreviadoDre;
        }
    }
}
