using SME.AE.Aplicacao.Compartilhado;
using System;
using System.Data;
using System.Data.SqlClient;

namespace SME.AE.Infra.Persistencia
{
    public class EolDataContext : IDisposable
    {
        public SqlConnection Connection { get; set; }
        public EolDataContext()
        {
            Connection = new SqlConnection(ConnectionStrings.ConexaoEol);
            Connection.Open();
        }

       
        public void Dispose()
        {
            if (Connection.State != ConnectionState.Closed)
                Connection.Close();
        }
    }
}
