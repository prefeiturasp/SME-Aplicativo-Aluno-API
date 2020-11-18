using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comum.Modelos
{
    public class ResponsavelEOLDto
    {
        public string CodigoDre { get; set; }
        public string Dre { get; set; }
        public string CodigoUe { get; set; }
        public string Ue { get; set; }
        public long CodigoTurma { get; set; }
        public long CpfResponsavel { get; set; }
    }
}
