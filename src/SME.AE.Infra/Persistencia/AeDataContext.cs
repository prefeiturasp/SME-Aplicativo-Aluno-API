using Npgsql;
using SME.AE.Infra.Interfaces;
using System;
using Microsoft.Extensions.Configuration;
using SME.AE.Aplicacao.Compartilhado;
using System.Data;

namespace SME.AE.Infra.Persistencia
{
    public class AeDataContext : IcontextoAplicacao
    {
        private readonly NpgsqlConnection _conexao;
        public AeDataContext(IConfiguration configuration, IcontextoAplicacao contextoAplicacao)
        {
            _conexao = new NpgsqlConnection(ConnectionStrings.Conexao);
        }

        public IDbConnection Conexao
        {
            get
            {
                if (_conexao.State != ConnectionState.Open)
                    Open();
                return _conexao;
            }
        }

        public string ConnectionString { get { return Conexao.ConnectionString; } set { Conexao.ConnectionString = value; } }

        public int ConnectionTimeout => Conexao.ConnectionTimeout;

        public string Database => Conexao.Database;

        public ConnectionState State => Conexao.State;
      
        public IDbTransaction BeginTransaction()
        {
            return Conexao.BeginTransaction();
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return Conexao.BeginTransaction(il);
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

        public void Open()
        {
            if (_conexao.State != ConnectionState.Open)
                _conexao.Open();
        }
    }
}
