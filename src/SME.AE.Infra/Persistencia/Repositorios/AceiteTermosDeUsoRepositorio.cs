using Dapper;
using Npgsql;
using Sentry;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Comum;
using SME.AE.Dominio.Entidades;
using System;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class AceiteTermosDeUsoRepositorio : BaseRepositorio<AceiteTermosDeUso>, IAceiteTermosDeUsoRepositorio
    {
        private readonly VariaveisGlobaisOptions variaveisGlobaisOptions;

        public AceiteTermosDeUsoRepositorio(VariaveisGlobaisOptions variaveisGlobaisOptions) : base(variaveisGlobaisOptions.AEConnection)
        {
            this.variaveisGlobaisOptions = variaveisGlobaisOptions ?? throw new ArgumentNullException(nameof(variaveisGlobaisOptions));
        }

        public async Task<bool> RegistrarAceite(AceiteTermosDeUso aceiteTermosDeUso)
        {
            try
            {
                await using var conn = new NpgsqlConnection(variaveisGlobaisOptions.AEConnection);
                conn.Open();
                var dataAtual = DateTime.Now;
                aceiteTermosDeUso.InserirAuditoria();
                var retorno = await conn.ExecuteAsync(
                    @"INSERT INTO public.aceite_termos_de_uso 
                    (termos_de_uso_id,
                     cpf_usuario,
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
                           @CpfUsuario,
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
                        aceiteTermosDeUso.CpfUsuario,
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

        public async Task<bool> ValidarAceiteDoTermoDeUsoPorUsuarioEVersao(string cpfUsuario, double versao)
        {
            try
            {
                using var conexao = InstanciarConexao();
                conexao.Open();
                var aceiteExiste = await conexao.QueryFirstOrDefaultAsync<int>($"SELECT count(id) FROM public.aceite_termos_de_uso WHERE cpf_usuario = @cpfUsuario AND versao = @versao", new { cpfUsuario, versao });
                conexao.Close();
                return aceiteExiste >= 1 ? true : false;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return false;
            }

        }
    }
}