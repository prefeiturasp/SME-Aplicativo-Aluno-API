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
    public class ObterDadosLeituraModalidadeQueryHandler : IRequestHandler<ObterDadosLeituraModalidadeQuery, IEnumerable<DadosLeituraComunicadosPorModalidadeTurmaResultado>>
    {
        private readonly IDadosLeituraRepositorio dadosLeituraRepositorio;

        public ObterDadosLeituraModalidadeQueryHandler(IDadosLeituraRepositorio dadosLeituraRepositorio)
        {
            this.dadosLeituraRepositorio = dadosLeituraRepositorio ?? throw new System.ArgumentNullException(nameof(dadosLeituraRepositorio));
        }

        public async Task<IEnumerable<DadosLeituraComunicadosPorModalidadeTurmaResultado>> Handle(ObterDadosLeituraModalidadeQuery request, CancellationToken cancellationToken)
        {
            var dadosLeituraComunicados = await dadosLeituraRepositorio
                    .ObterDadosLeituraModalidade(request.CodigoDre, request.CodigoUe, request.NotificaoId, request.ModoVisualizacao == ModoVisualizacao.Responsavel);
            
            return ObterNomeModalidade(dadosLeituraComunicados);
        }

        private static List<DadosLeituraComunicadosPorModalidadeTurmaResultado> ObterNomeModalidade(IEnumerable<DadosLeituraComunicadosPorModalidadeTurmaResultado> dadosLeituraComunicados)
        {
            var resultado = new List<DadosLeituraComunicadosPorModalidadeTurmaResultado>();
            foreach (var item in dadosLeituraComunicados)
            {
                item.Modalidade = ObterModalidade(item.ModalidadeCodigo);
                resultado.Add(item);
            }
            return resultado;
        }

        private static string ObterModalidade(string modalidade)
        {
            switch (modalidade)
            {
                case "1":
                    modalidade = ModalidadeDeEnsino.Infantil.ToString();
                    break;
                case "5":
                    modalidade = ModalidadeDeEnsino.Fundamental.ToString();
                    break;
                case "6":
                    modalidade = ModalidadeDeEnsino.Medio.ToString();
                    break;
                case "3":
                    modalidade = ModalidadeDeEnsino.EJA.ToString();
                    break;
                default:
                    break;
            }
            return modalidade;
        }

    }
}
