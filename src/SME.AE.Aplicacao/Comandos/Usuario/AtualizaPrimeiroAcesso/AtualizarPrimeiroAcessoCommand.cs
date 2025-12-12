using MediatR;

namespace SME.AE.Aplicacao.Comandos.Usuario.AtualizaPrimeiroAcesso
{
    public class AtualizarPrimeiroAcessoCommand : IRequest<Unit>
    {
        public long Id { get; set; }
        public bool PrimeiroAcesso { get; set; }
    }
}
