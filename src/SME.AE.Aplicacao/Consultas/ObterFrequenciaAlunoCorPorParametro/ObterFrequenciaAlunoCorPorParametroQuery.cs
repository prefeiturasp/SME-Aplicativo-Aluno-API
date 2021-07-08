using MediatR;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.FrequenciasDoAluno;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Consultas.ObterUltimaAtualizacaoPorProcesso
{
    public class ObterFrequenciaAlunoCorPorParametroQuery : IRequest<IEnumerable<FrequenciaAlunoCor>>
    {
        public IEnumerable<ParametroEscolaAqui> Parametros { get; set; }

        public ObterFrequenciaAlunoCorPorParametroQuery(IEnumerable<ParametroEscolaAqui> parametros)
        {
            Parametros = parametros;
        }
    }
}
