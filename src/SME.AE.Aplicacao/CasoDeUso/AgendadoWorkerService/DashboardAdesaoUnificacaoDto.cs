namespace SME.AE.Aplicacao.CasoDeUso
{
    public class DashboardAdesaoUnificacaoDto
    {
        public long CPF { get; set; }
        public string UeCodigo { get; set; }
        public string DreCodigo { get; set; }
        public long TurmaCodigo { get; set; }

        public int PrimeiroAcessoIncompleto { get; set; }
        public int UsuarioValido { get; set; }
        public int UsuarioCpfInvalido { get; set; }
        public int UsuarioSemAppInstalado { get; set; }
    }
}