using SME.AE.Aplicacao.Comum.Modelos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IParametrosEscolaAquiRepositorio
    {
        DateTime? ObterDateTime(string chave);

        int? ObterInt(string chave);

        long? ObterLong(string chave);

        string ObterString(string chave);

        Task<IEnumerable<ParametroEscolaAqui>> ObterParametros(IEnumerable<string> chaves);

        DateTime ObterDateTime(string chave, DateTime padrao);

        int ObterInt(string chave, int padrao);

        long ObterLong(string chave, long padrao);

        string ObterString(string chave, string padrao);

        bool TentaObterDateTime(string chave, out DateTime conteudo);

        bool TentaObterInt(string chave, out int conteudo);

        bool TentaObterLong(string chave, out long conteudo);

        bool TentaObterString(string chave, out string conteudo);

        void Salvar(string chave, string conteudo);

        void Salvar(string chave, int conteudo);

        void Salvar(string chave, long conteudo);

        void Salvar(string chave, DateTime conteudo);
    }
}