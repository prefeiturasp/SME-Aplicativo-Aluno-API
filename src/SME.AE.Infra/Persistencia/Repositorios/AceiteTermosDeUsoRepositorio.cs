using Dapper;
using Npgsql;
using Sentry;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Dominio.Entidades;
using System;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class AceiteTermosDeUsoRepositorio : BaseRepositorio<AceiteTermosDeUso>, IAceiteTermosDeUsoRepositorio
    {
        public AceiteTermosDeUsoRepositorio() : base(ConnectionStrings.Conexao)
        {
        }

        public async Task<bool> RegistrarAceite(AceiteTermosDeUso aceiteTermosDeUso)
        {
         try
            {
                await using var conn = new NpgsqlConnection(ConnectionStrings.Conexao);
                conn.Open();
                var dataAtual = DateTime.Now;
                aceiteTermosDeUso.InserirAuditoria();
                var retorno = await conn.ExecuteAsync(
                    @"INSERT INTO public.aceite_termos_de_uso 
                    (termos_de_uso_id,
                     usuario,
                     device,
                     ip,
                     versao,
                     data_hora_aceite,
                     hash,
                     criado_em, 
                     criado_por, 
                     alterado_em,
                     alterado_por
                    )
                    VALUES(@TermosDeUsoId,
                           @Usuario,
                           @Device,
                           @Ip,
                           @Versao,
                           @DataAtual,
                           @Hash,
                           @CriadoEm,
                           @CriadoPor,
                           @AlteradoEm,                           
                           @AlteradoPor
                        );",
                    new
                    {
                        aceiteTermosDeUso.TermosDeUsoId,
                        aceiteTermosDeUso.Usuario,
                        aceiteTermosDeUso.Device,
                        aceiteTermosDeUso.Ip,
                        aceiteTermosDeUso.Versao,
                        dataAtual,
                        aceiteTermosDeUso.Hash,
                        aceiteTermosDeUso.CriadoEm,
                        aceiteTermosDeUso.CriadoPor,
                        aceiteTermosDeUso.AlteradoEm,
                        aceiteTermosDeUso.AlteradoPor
                    });
                conn.Close();
                return true;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureMessage(ex.Message);
                throw ex;
            }
        }

        public async Task<bool> ValidarAceiteDoTermoDeUsoPorUsuarioEVersao(string usuario, double versao)
        {
            try
            {
                using var conexao = InstanciarConexao();
                conexao.Open();
                var aceiteExiste = await conexao.QueryFirstOrDefaultAsync<int>($"SELECT count(id) FROM public.aceite_termos_de_uso WHERE usuario = @usuario AND versao = @versao", new { usuario, versao });
                conexao.Close();
                return aceiteExiste == 1 ? true : false;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return false;
            }

        }
    }
}