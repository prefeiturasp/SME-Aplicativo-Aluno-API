using SME.AE.Aplicacao.Comum.Modelos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IConsolidarLeituraNotificacaoRepositorio
    {
        Task ExcluirConsolidacaoNotificacao(ConsolidacaoNotificacaoDto consolidacaoNotificacao);
        Task<IEnumerable<UsuarioAlunoNotificacaoApp>> ObterUsuariosAlunosNotificacoesApp();
        Task SalvarConsolidacaoNotificacao(ConsolidacaoNotificacaoDto consolidacaoNotificacao);
        Task SalvarConsolidacaoNotificacoesEmBatch(IEnumerable<ConsolidacaoNotificacaoDto> consolidacaoNotificacoes);
    }
}
