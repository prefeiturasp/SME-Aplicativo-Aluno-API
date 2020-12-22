﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comum.Modelos.Resposta
{
    public class DadosLeituraAlunosComunicado
    {
        public short NumeroChamada { get; set; }
        public string NomeAluno { get; set; }
        public string NomeResponsavel { get; set; }
        public string TelefoneResponsavel { get; set; }
        public bool PossueApp { get; set; }
        public bool LeuComunicado { get; set; }
        public DateTime? DataLeitura { get; set; }
    }
}