using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public interface IObterDadosUsuarioPorCpfUseCase
    {
        Task<UsuarioDadosDetalhesDto> Executar(string cpf);
    }
}