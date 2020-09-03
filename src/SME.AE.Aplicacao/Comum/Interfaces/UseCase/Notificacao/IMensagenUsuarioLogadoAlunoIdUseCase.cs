using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.UseCase
{
    public interface IMensagenUsuarioLogadoAlunoIdUseCase
    {
        Task<NotificacaoResposta> Executar(long id);
    }
}
