namespace SME.AE.Aplicacao.Comum.Modelos.Resposta
{
    public class TotaisAdesaoResultado
    {
        public string NomeCompletoDre { get; set; }
        public string NomeCompletoUe { get; set; }
        public int Codigoturma { get; set; }
        public long TotalUsuariosComCpfInvalidos { get; set; }
        public long TotalUsuariosPrimeiroAcessoIncompleto { get; set; }
        public long TotalUsuariosSemAppInstalado { get; set; }
        public long TotalUsuariosValidos { get; set; }
    }
}
