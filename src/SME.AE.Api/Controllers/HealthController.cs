using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Aplicacao.Comum.Config;

namespace SME.AE.Api.Controllers
{
    public class HealthController : ApiController
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<Object> check()
        {
            bool isEolConnOk = false;

            try
            {
                using var conexao = new SqlConnection(ConnectionStrings.ConexaoEol);
                conexao.Open();
                isEolConnOk = true;
                conexao.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            return new 
            {
                IsEolConnOk = isEolConnOk
            };
        } 
    }
}