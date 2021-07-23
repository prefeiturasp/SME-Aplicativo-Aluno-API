using MediatR;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.ObterDadosLeituraComunicados
{
    public class ObterDadosLeituraComunicadosQueryHandler : IRequestHandler<ObterDadosLeituraComunicadosQuery, IEnumerable<DadosLeituraComunicadosResultado>>
    {
        private readonly IDadosLeituraRepositorio dadosLeituraRepositorio;
        private readonly IUsuarioNotificacaoRepositorio usuarioNotificacaoLeituraRepositorio;

        public ObterDadosLeituraComunicadosQueryHandler(IDadosLeituraRepositorio dadosLeituraRepositorio, IUsuarioNotificacaoRepositorio usuarioNotificacaoLeituraRepositorio)
        {
            this.dadosLeituraRepositorio = dadosLeituraRepositorio ?? throw new System.ArgumentNullException(nameof(dadosLeituraRepositorio));
            this.usuarioNotificacaoLeituraRepositorio = usuarioNotificacaoLeituraRepositorio ?? throw new System.ArgumentNullException(nameof(usuarioNotificacaoLeituraRepositorio));
        }

        public async Task<IEnumerable<DadosLeituraComunicadosResultado>> Handle(ObterDadosLeituraComunicadosQuery request, CancellationToken cancellationToken)
        {
            var dadosLeituraComunicados = await dadosLeituraRepositorio.ObterDadosLeituraComunicados(request.CodigoDre, request.CodigoUe, request.NotificaoId, request.Modalidade);

            if (dadosLeituraComunicados == null || !dadosLeituraComunicados.Any())
                throw new Exception("Não foram encontrados dados de leitura de comunicados");

            var retornoDadosLeituraComunicadosResultado = new List<DadosLeituraComunicadosResultado>();
            var dadosLeituraComunicadosResultado = new DadosLeituraComunicadosResultado();
            
            if (request.ModoVisualizacao == ModoVisualizacao.Responsavel)
                await ObterTotaisDeLeituraPorResponsavel(request, dadosLeituraComunicados, dadosLeituraComunicadosResultado);

            if (request.ModoVisualizacao == ModoVisualizacao.Aluno)
                await ObterTotaisDeLeituraPorAluno(request, dadosLeituraComunicados, dadosLeituraComunicadosResultado);

            retornoDadosLeituraComunicadosResultado.Add(dadosLeituraComunicadosResultado);
            return retornoDadosLeituraComunicadosResultado;
        }

        private async Task ObterTotaisDeLeituraPorAluno(ObterDadosLeituraComunicadosQuery request, IEnumerable<DadosConsolidacaoNotificacaoResultado> dadosLeituraComunicados, DadosLeituraComunicadosResultado dadosLeituraComunicadosResultado)
        {
            var codigoDre = long.Parse(dadosLeituraComunicados.FirstOrDefault().DreCodigo == "" ? "0" : dadosLeituraComunicados.FirstOrDefault().DreCodigo);
            var totalNotificacoesLeituraPorAluno = await usuarioNotificacaoLeituraRepositorio.ObterTotalNotificacoesLeituraPorAluno(request.NotificaoId, codigoDre);

            if (dadosLeituraComunicados.Count() == 1)
            {
                dadosLeituraComunicadosResultado.ReceberamENaoVisualizaram = (dadosLeituraComunicados.FirstOrDefault().QuantidadeAlunosComApp - totalNotificacoesLeituraPorAluno);
                dadosLeituraComunicadosResultado.NaoReceberamComunicado = dadosLeituraComunicados.FirstOrDefault().QuantidadeAlunosSemApp;
                dadosLeituraComunicadosResultado.VisualizaramComunicado = totalNotificacoesLeituraPorAluno;                
            }

            if (dadosLeituraComunicados.Count() > 1)
            {
                dadosLeituraComunicadosResultado.ReceberamENaoVisualizaram = (dadosLeituraComunicados.Select(x => x.QuantidadeAlunosComApp).Sum() - totalNotificacoesLeituraPorAluno);
                dadosLeituraComunicadosResultado.NaoReceberamComunicado = dadosLeituraComunicados.Select(x => x.QuantidadeAlunosSemApp).Sum();
                dadosLeituraComunicadosResultado.VisualizaramComunicado = totalNotificacoesLeituraPorAluno;
            }
        }

        private async Task ObterTotaisDeLeituraPorResponsavel(ObterDadosLeituraComunicadosQuery request, IEnumerable<DadosConsolidacaoNotificacaoResultado> dadosLeituraComunicados, DadosLeituraComunicadosResultado dadosLeituraComunicadosResultado)
        {
            var codigoDre = long.Parse(dadosLeituraComunicados.FirstOrDefault().DreCodigo == "" ? "0" : dadosLeituraComunicados.FirstOrDefault().DreCodigo);
            var totalNotificacoesLeituraPorResponsavel = await usuarioNotificacaoLeituraRepositorio.ObterTotalNotificacoesLeituraPorResponsavel(request.NotificaoId, codigoDre);
            if (dadosLeituraComunicados.Count() == 1)
            {
                dadosLeituraComunicadosResultado.ReceberamENaoVisualizaram = (dadosLeituraComunicados.FirstOrDefault().QuantidadeResponsaveisComApp - totalNotificacoesLeituraPorResponsavel);
                dadosLeituraComunicadosResultado.NaoReceberamComunicado = dadosLeituraComunicados.FirstOrDefault().QuantidadeResponsaveisSemApp;
                dadosLeituraComunicadosResultado.VisualizaramComunicado = totalNotificacoesLeituraPorResponsavel;
            }

            if (dadosLeituraComunicados.Count() > 1)
            {
                dadosLeituraComunicadosResultado.ReceberamENaoVisualizaram = (dadosLeituraComunicados.Select(x => x.QuantidadeResponsaveisComApp).Sum() - totalNotificacoesLeituraPorResponsavel);
                dadosLeituraComunicadosResultado.NaoReceberamComunicado = dadosLeituraComunicados.Select(x => x.QuantidadeResponsaveisSemApp).Sum();
                dadosLeituraComunicadosResultado.VisualizaramComunicado = totalNotificacoesLeituraPorResponsavel;
            }
        }
    }
}
