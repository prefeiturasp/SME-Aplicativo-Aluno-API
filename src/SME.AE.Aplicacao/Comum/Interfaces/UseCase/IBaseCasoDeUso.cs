using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces
{
    public interface IBaseCasoDeUso
    {
        public Task ExecutarAsync();
        public Task ExecutarAsync(int anoLetivo, long ueId);
    }
}
