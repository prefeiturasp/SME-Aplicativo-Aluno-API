namespace SME.AE.Aplicacao.Comum.Modelos.Usuario
{
    public class ReiniciarSenhaDto
    {
        public string Token { get; set; }
        public string Senha { get; set; }
        public string DispositivoId { get; set; }
    }
}
