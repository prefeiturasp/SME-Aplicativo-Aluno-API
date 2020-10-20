using SME.AE.Aplicacao.Comum.Modelos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IEventoRepositorio
    {
        Task<DateTime?> ObterUltimaAlteracao();
        Task Salvar(EventoDto evento);
        Task Remover(EventoDto evento);
        Task<IEnumerable<EventoDto>> ObterPorDreUeTurmaMes(string dre_id, string ue_id, string turma_id, int modalidadeCalendario, DateTime mesAno);
        Task<IEnumerable<EventoDto>> ObterPorDreUeTurmaDataEspecifica(string dre_id, string ue_id, string turma_id, int modalidadeCalendario, DateTime mesAno);
    }
}
