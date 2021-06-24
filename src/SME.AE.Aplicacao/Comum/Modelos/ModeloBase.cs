using System;

namespace SME.AE.Aplicacao.Comum.Modelos
{
    public class ModeloBase
    {
        public long Id { get; set; }
        public DateTime CriadoEm { get; set; }
        public string CriadoPor { get; set; }
        public DateTime? AlteradoEm { get; set; }
        public string AlteradoPor { get; set; }
    }
}
