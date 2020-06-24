using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Dominio.Entidades;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IUsuarioCoreSSORepositorio
    {
        Task<Guid> Criar(UsuarioCoreSSO usuario);
        Task<RetornoUsuarioCoreSSO> ObterPorId(Guid id);
        Task<RetornoUsuarioCoreSSO> ObterPorCPF(string cpf);
        Task<List<Guid>> SelecionarGrupos();
        Task IncluirUsuarioNosGrupos(Guid usuId, IEnumerable<Guid> gruposNaoIncluidos);
        Task AlterarStatusUsuario(Guid id, StatusUsuarioCoreSSO status);
        Task AtualizarCriptografiaUsuario(Guid usuId, string senha);
    }
}
