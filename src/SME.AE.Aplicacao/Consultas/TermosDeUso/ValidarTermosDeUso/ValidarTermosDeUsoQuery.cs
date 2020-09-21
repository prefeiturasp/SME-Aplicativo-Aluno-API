using MediatR;

namespace SME.AE.Aplicacao.Consultas.TermosDeUso
{
    public class ValidarTermosDeUsoQuery : IRequest<bool>
    {
        public long TermoDeUsoId { get; set; }

        public ValidarTermosDeUsoQuery(long termoDeUsoId)
        {
            TermoDeUsoId = termoDeUsoId;
        }
    }
}
