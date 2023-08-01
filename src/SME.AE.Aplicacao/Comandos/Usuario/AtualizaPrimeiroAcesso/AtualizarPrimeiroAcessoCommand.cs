using MediatR;

namespace SME.AE.Aplicacao.Comandos.Usuario.AtualizaPrimeiroAcesso
{
    public class AtualizarPrimeiroAcessoCommand : IRequest
    {
        public long Id { get; set; }
        public bool PrimeiroAcesso { get; set; }
    }
}
