using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.NotasDoAluno;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.UseCase
{
    public interface IObterNotasAlunoUseCase
    {
        Task<NotaAlunoPorBimestreResposta> Executar(NotaAlunoDto notaAlunoDto);
    }
}