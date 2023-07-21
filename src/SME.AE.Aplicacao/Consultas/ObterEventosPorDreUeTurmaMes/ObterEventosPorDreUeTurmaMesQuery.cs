using MediatR;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Consultas
{
    public class ObterEventosPorDreUeTurmaMesQuery : IRequest<IEnumerable<EventoDto>>
    {
        public ObterEventosPorDreUeTurmaMesQuery(string dre_id, string ue_id, string turma_id, int modalidadeCalendario, DateTime mesAno)
        {
            Dre_id = dre_id;
            Ue_id = ue_id;
            Turma_id = turma_id;
            ModalidadeCalendario = modalidadeCalendario;
            MesAno = mesAno;
        }

        public string Dre_id { get; set; }
        public string Ue_id { get; set; }
        public string Turma_id { get; set; }
        public int ModalidadeCalendario { get; set; }
        public DateTime MesAno { get; set; }


    }
}
