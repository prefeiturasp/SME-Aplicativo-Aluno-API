using System.Threading.Tasks;
using SME.AE.Aplicacao.Comum.Modelos;

namespace SME.AE.Aplicacao.Comum.Interfaces
{
    public interface IAutenticacaoService
    {
        Task<string> ObterNomeUsuarioAsync(string userId);

        Task<(RespostaApi resposta, string id)> CriarUsuarioAsync(string cpf, string senha);
    }
}
