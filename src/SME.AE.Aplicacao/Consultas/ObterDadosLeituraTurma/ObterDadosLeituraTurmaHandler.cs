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
    public class ObterDadosLeituraTurmaQueryHandler : IRequestHandler<ObterDadosLeituraTurmaQuery, IEnumerable<DadosLeituraComunicadosPorModalidadeTurmaResultado>>
    {
        private readonly IDadosLeituraRepositorio dadosLeituraRepositorio;

        public ObterDadosLeituraTurmaQueryHandler(IDadosLeituraRepositorio dadosLeituraRepositorio)
        {
            this.dadosLeituraRepositorio = dadosLeituraRepositorio ?? throw new System.ArgumentNullException(nameof(dadosLeituraRepositorio));
        }

        public async Task<IEnumerable<DadosLeituraComunicadosPorModalidadeTurmaResultado>> Handle(ObterDadosLeituraTurmaQuery request, CancellationToken cancellationToken)
        {
            var dadosLeituraComunicados =  await dadosLeituraRepositorio
                    .ObterDadosLeituraTurma(request.CodigoDre, request.CodigoUe, request.NotificaoId, request.Modalidade, request.ModoVisualizacao == ModoVisualizacao.Responsavel);

            return dadosLeituraComunicados;
        }

    }
}
