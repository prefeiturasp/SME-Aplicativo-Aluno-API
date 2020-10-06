﻿using SME.AE.Aplicacao.Comum.Enumeradores;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comum.Modelos
{
    public class EventoDto
    {
        public string evento_id { get; set; }
        public string nome { get; set; }
        public string descricao { get; set; }
        public DateTime data_inicio { get; set; }
        public DateTime data_fim { get; set; }
        public string dre_id { get; set; }
        public string ue_id { get; set; }
        public int tipo_evento { get; set; }
        public string turma_id { get; set; }
        public int ano_letivo { get; set; }
        public int modalidade { get; set; }
        public DateTime ultima_alteracao_sgp { get; set; }
        public bool excluido { get; set; }
    }
}
