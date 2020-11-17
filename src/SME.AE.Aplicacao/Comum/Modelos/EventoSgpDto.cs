using SME.AE.Aplicacao.Comum.Enumeradores;
using System;

namespace SME.AE.Aplicacao.Comum.Modelos
{
    public class EventoSgpDto
    {
        public int id { get; set; }
        public string nome { get; set; }
        public string descricao { get; set; }
        public DateTime data_inicio { get; set; }
        public DateTime data_fim { get; set; }
        public string dre_id { get; set; }
        public string ue_id { get; set; }
        public int tipo_evento_id { get; set; }
        public string turma_id { get; set; }
        public int ano_letivo { get; set; }
        public int? modalidade_turma { get; set; }
        public int? modalidade_calendario { get; set; }
        public DateTime alterado_em { get; set; }
        public bool excluido { get; set; }
        public string componente_curricular { get; set; }
        public long? tipo_calendario_id { get; set; }
    }

    public static class EventoSgpDtoExtensions
    {
        public static string ObterEventoId(this EventoSgpDto e) => e.tipo_evento_id == (int)TipoEvento.Avaliacao ? $"A-{e.id}" : $"E-{e.id}";
    }
}
