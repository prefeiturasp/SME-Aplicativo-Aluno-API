using MediatR;

namespace SME.AE.Aplicacao.Comandos.Usuario.ReiniciarSenha
{
    public class ReiniciarSenhaCommand : IRequest
    {
        public long Id { get; set; }
        public bool PrimeiroAcesso { get; set; }
    }
}
