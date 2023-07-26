namespace SME.AE.Dominio.Options
{
    public class TelemetriaOptions
    {
        public const string Secao = "Telemetria";
        public bool ApplicationInsights { get; set; }
        public bool Apm { get; set; }
    }
}
