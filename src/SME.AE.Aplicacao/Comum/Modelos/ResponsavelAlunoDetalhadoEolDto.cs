namespace SME.AE.Aplicacao
{
    public class ResponsavelAlunoDetalhadoEolDto
    {
        public string Usuario { get => "webResp"; }
        public string Senha { get => "resp"; }
        public string CodigoAluno { get; set; }
        public string TipoPessoa { get; set; }
        public string Nome { get; set; }
        public string NumeroRG { get; set; }
        public string DigitoRG { get; set; }
        public string UfRG { get; set; }
        public string CPF { get; set; }
        public string CPFConfere { get; set; }
        public string DDDCelular { get; set; }
        public string NumeroCelular { get; set; }
        public string TipoTurnoCelular { get; set; }
        public string DDDTelefoneFixo { get; set; }
        public string NumeroTelefoneFixo { get; set; }
        public string TipoTurnoTelefoneFixo { get; set; }
        public string DDDTelefoneComercial { get; set; }
        public string NumeroTelefoneComercial { get; set; }
        public string TipoTurnoTelefoneComercial { get; set; }
        public string AutorizaEnvioSMS { get; set; }
        public string Email { get; set; }
        public string NomeMae { get; set; }
        public string DataNascimentoMae { get; set; }
    }
}
