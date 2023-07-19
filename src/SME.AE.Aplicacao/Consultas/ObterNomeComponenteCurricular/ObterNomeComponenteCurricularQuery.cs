using MediatR;

namespace SME.AE.Aplicacao
{
    public class ObterNomeComponenteCurricularQuery : IRequest<string>
    {
        public ObterNomeComponenteCurricularQuery(long codigoComponenteCurricular)
        {
            CodigoComponenteCurricular = codigoComponenteCurricular;
        }

        public long CodigoComponenteCurricular { get; set; }
    }
}
