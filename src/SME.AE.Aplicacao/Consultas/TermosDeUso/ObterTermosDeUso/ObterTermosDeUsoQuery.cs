using MediatR;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;

namespace SME.AE.Aplicacao.Consultas.TermosDeUso
{
    public class ObterTermosDeUsoQuery : IRequest<RetornoTermosDeUsoDto>
    { }
}
