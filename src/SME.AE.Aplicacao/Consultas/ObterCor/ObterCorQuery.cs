using MediatR;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.FrequenciasDoAluno;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Consultas.ObterUltimaAtualizacaoPorProcesso
{
    public class ObterCorQuery : IRequest<string>
    {
        public ObterCorQuery(IEnumerable<ParametroEscolaAqui> parametros, double frequencia, IEnumerable<FrequenciaAlunoCor> frequenciaAlunoCores, IEnumerable<FrequenciaAlunoFaixa> frequenciaAlunoFaixas, ModalidadeDeEnsino modalidade)
        {
            Parametros = parametros;
            Frequencia = frequencia;
            FrequenciaAlunoCores = frequenciaAlunoCores;
            FrequenciaAlunoFaixas = frequenciaAlunoFaixas;
            Modalidade = modalidade;
        }

        public ModalidadeDeEnsino Modalidade { get; set; }
        public IEnumerable<ParametroEscolaAqui> Parametros { get; set; }
        public double Frequencia { get; set; }
        public IEnumerable<FrequenciaAlunoCor> FrequenciaAlunoCores { get; set; }
        public IEnumerable<FrequenciaAlunoFaixa> FrequenciaAlunoFaixas { get; set; }

        
    }
}
