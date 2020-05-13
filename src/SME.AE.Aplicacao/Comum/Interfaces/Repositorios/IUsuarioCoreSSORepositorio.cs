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
        Task Criar(UsuarioCoreSSO usuario);
        Task<IEnumerable<RetornoUsuarioCoreSSO>> Selecionar(string cpf);
        Task<List<Guid>> SelecionarGrupos();
        void IncluirUsuarioNosGrupos(Guid usuId, IEnumerable<Guid> gruposNaoIncluidos);
        Task AlterarStatusUsuario(Guid id, StatusUsuarioCoreSSO status);
    }
}
