using Npgsql;
using SME.AE.Aplicacao.Comum.Interfaces.Contextos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SME.AE.Infra.Persistencia
{
    public class AplicacaoDapperContext : IAplicacaoDapperContext
    {
        private readonly NpgsqlConnection conexao;

        public AplicacaoDapperContext(string connectionString)
        {
            conexao = new NpgsqlConnection(connectionString);
        }

        public IDbConnection Conexao
        {
            get
            {
                if (conexao.State != ConnectionState.Open)
                    Open();
                return Conexao;
            }
        }

        public string ConnectionString { get { return Conexao.ConnectionString; } set { Conexao.ConnectionString = value; } }

        public int ConnectionTimeout => Conexao.ConnectionTimeout;

        public string Database => Conexao.Database;

        public ConnectionState State => Conexao.State;

        public void Open()
        {
            if (conexao.State != ConnectionState.Open)
                conexao.Open();
        }

        public IDbTransaction BeginTransaction()
        {
            return Conexao.BeginTransaction();
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return Conexao.BeginTransaction(il);
        }

        public void ChangeDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            Conexao.Close();
        }

        public IDbCommand CreateCommand()
        {
            return Conexao.CreateCommand();
        }

        public void Dispose() => Conexao.Close();
    }
}
