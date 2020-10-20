using MediatR;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Consultas
{
    public class ObterEventosAlunoPorMesQuery: IRequest<IEnumerable<EventoRespostaDto>>
    {
        public string Cpf { get; set; }
        public long CodigoAluno { get; set; }
        public DateTime MesAno { get; set; }
    }
}
