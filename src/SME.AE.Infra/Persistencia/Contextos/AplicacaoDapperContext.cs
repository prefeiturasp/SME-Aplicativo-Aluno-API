using SME.AE.Aplicacao.Comum.Interfaces.Contextos;
using System;
using System.Data;

namespace SME.AE.Infra.Persistencia
{
    public class AplicacaoDapperContext<T> : IAplicacaoDapperContext<T> where T : IDbConnection
    {
        private readonly T conexao;

        public AplicacaoDapperContext(T connection)
        {
            conexao = connection;
        }

        public T Conexao
        {
            get
            {
                Open();
                return conexao;
            }
        }

        public string ConnectionString 
        { 
            get => Conexao.ConnectionString;  
            set => Conexao.ConnectionString = value;
        }

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

        public void Dispose() => Conexao?.Dispose();
    }
}
