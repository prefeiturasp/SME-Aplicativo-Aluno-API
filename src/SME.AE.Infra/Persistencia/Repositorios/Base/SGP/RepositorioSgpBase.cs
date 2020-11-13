﻿using Npgsql;
using SME.AE.Aplicacao.Comum.Config;

namespace SME.AE.Infra.Persistencia.Repositorios.Base.SGP
{
    // TO DO: Colocar os demais reposiórios para herdar da classe base
    public abstract class RepositorioSgpBase
    {
        protected NpgsqlConnection CriaConexao() => new NpgsqlConnection(ConnectionStrings.ConexaoSgp);
    }
}