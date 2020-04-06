using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SME.AE.Aplicacao.Comum.Modelos;

namespace SME.AE.Aplicacao.Comum.Interfaces.Servicos
{
    public interface IAutenticacaoService
    {
        Task<string> ObterNomeUsuarioAsync(string userId);

        Task<(RespostaApi resposta, string id)> CriarUsuarioAsync(string cpf, string senha);

        Task<IEnumerable<RetornoUsuarioEol>> SelecionarAlunosResponsavel(string cpf);
    }
}
