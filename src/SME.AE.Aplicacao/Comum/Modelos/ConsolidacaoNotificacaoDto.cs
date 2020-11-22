using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comum.Modelos
{
    public class ConsolidacaoNotificacaoDto
    {
        public short AnoLetivo { get; set; } 
        public long NotificacaoId { get; set; }
        public string DreCodigo { get; set; }
        public string UeCodigo { get; set; }
        public long QuantidadeResponsaveisSemApp { get; set; }
        public long QuantidadeAlunosSemApp { get; set; }
        public long QuantidadeResponsaveisSemLer { get; set; }
        public long QuantidadeAlunosSemLer { get; set; }
    }
}
