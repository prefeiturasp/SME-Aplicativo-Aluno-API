using MediatR;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.FrequenciasDoAluno;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Consultas.ObterUltimaAtualizacaoPorProcesso
{
    public class ObterFrequenciaAlunoFaixaPorParametroQuery : IRequest<IEnumerable<FrequenciaAlunoFaixa>>
    {
        public IEnumerable<ParametroEscolaAqui> Parametros { get; set; }

        public ObterFrequenciaAlunoFaixaPorParametroQuery(IEnumerable<ParametroEscolaAqui> parametros)
        {
            Parametros = parametros;
        }
    }
}
