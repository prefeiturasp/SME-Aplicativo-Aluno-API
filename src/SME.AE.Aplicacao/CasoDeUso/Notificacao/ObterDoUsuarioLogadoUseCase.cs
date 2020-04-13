using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;

namespace SME.AE.Aplicacao.CasoDeUso.Notificacao
{
    public class ObterDoUsuarioLogadoUseCase
    {
        public static async Task<IEnumerable<Dominio.Entidades.Notificacao>> Executar(IMediator mediator, string usuario)
        {
            
            return new List<Dominio.Entidades.Notificacao>();
        }
    }
}