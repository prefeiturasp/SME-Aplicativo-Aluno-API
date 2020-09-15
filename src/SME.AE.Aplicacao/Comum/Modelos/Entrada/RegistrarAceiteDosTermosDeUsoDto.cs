using System;

namespace SME.AE.Aplicacao.Comum.Modelos.Entrada
{
    public class RegistrarAceiteDosTermosDeUsoDto
    {
        public long TermoDeUsoId { get; set; }
        public string Usuario { get; set; }
        public string Device { get; set; }
        public string Ip { get; set; }
        public double Versao { get; set; }
    }
}
