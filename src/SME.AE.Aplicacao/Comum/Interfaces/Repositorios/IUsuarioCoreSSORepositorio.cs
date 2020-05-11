using System.Collections.Generic;
using System.Threading.Tasks;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Dominio.Entidades;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IUsuarioCoreSSORepositorio
    {
        Task Criar(UsuarioCoreSSO usuario);
        Task<IEnumerable<RetornoUsuarioCoreSSO>> Selecionar(string cpf);
    }
}
