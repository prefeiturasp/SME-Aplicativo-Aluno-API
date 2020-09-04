using SME.AE.Aplicacao.Comum.Modelos;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.UseCase
{
    public interface IAutenticarUsuarioUseCase
    {
        Task<RespostaApi> Executar(string cpf, string senha, string dispositivoId);
    }
}
