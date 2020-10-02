﻿using SME.AE.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IParametrosEscolaAquiRepositorio
    {
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
