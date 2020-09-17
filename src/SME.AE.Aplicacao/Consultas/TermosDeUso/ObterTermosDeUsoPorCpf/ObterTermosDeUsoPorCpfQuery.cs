using MediatR;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;

namespace SME.AE.Aplicacao.Consultas.TermosDeUso
{
    public class ObterTermosDeUsoPorCpfQuery : IRequest<RetornoTermosDeUsoDto>
    {
        public string CPF { get; set; }

        public ObterTermosDeUsoPorCpfQuery(string cPF)
        {
            CPF = cPF;
        }
    }
}
