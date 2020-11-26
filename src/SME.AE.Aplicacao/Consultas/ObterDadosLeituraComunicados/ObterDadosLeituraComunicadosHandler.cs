using MediatR;
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
            var dadosLeituraComunicados =  await dadosLeituraRepositorio.ObterDadosLeituraComunicados(request.CodigoDre, request.CodigoUe, request.NotificaoId);

            if (dadosLeituraComunicados == null || !dadosLeituraComunicados.Any())
                throw new Exception("Não foram encontrados dados de leitura de comunicados");
            
            var resultado = new List<DadosLeituraComunicadosResultado>();
            var dadosLeituraComunicadosResultado = new DadosLeituraComunicadosResultado();
            
            // Por responsável
            if (request.ModoVisualizacao == 1)
            {
                try
                {
                    var totalNotificacoesLeituraPorResponsavel = await usuarioNotificacaoLeituraRepositorio.ObterTotalNotificacoesLeituraPorResponsavel(request.NotificaoId, long.Parse(request.CodigoDre), request.CodigoUe);

                    dadosLeituraComunicadosResultado.ReceberamENaoVisualizaram = (dadosLeituraComunicados.FirstOrDefault().QuantidadeResponsaveisComApp - totalNotificacoesLeituraPorResponsavel);
                    dadosLeituraComunicadosResultado.NaoReceberamComunicado = dadosLeituraComunicados.FirstOrDefault().QuantidadeResponsaveisComApp;
                    dadosLeituraComunicadosResultado.VisualizaramComunicado = totalNotificacoesLeituraPorResponsavel;

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            // Por aluno
            if (request.ModoVisualizacao == 2) {
                var totalNotificacoesLeituraPorAluno = await usuarioNotificacaoLeituraRepositorio.ObterTotalNotificacoesLeituraPorAluno(request.NotificaoId, long.Parse(request.CodigoDre), request.CodigoUe);

                dadosLeituraComunicadosResultado.ReceberamENaoVisualizaram = (dadosLeituraComunicados.FirstOrDefault().QuantidadeResponsaveisComApp - totalNotificacoesLeituraPorAluno);
                dadosLeituraComunicadosResultado.NaoReceberamComunicado = dadosLeituraComunicados.FirstOrDefault().QuantidadeResponsaveisComApp;
                dadosLeituraComunicadosResultado.VisualizaramComunicado = totalNotificacoesLeituraPorAluno;
            }

            resultado.Add(dadosLeituraComunicadosResultado);
            return resultado;
        }
    }
}
