using Dapper;
using Sentry;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.UnidadeEscolar;
using SME.AE.Comum;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class UnidadeEscolarRepositorio : IUnidadeEscolarRepositorio
    {
        private readonly VariaveisGlobaisOptions variaveisGlobaisOptions;

        public UnidadeEscolarRepositorio(VariaveisGlobaisOptions variaveisGlobaisOptions)
        {
            this.variaveisGlobaisOptions = variaveisGlobaisOptions ?? throw new ArgumentNullException(nameof(variaveisGlobaisOptions));
        }
        private SqlConnection CriaConexao() => new SqlConnection(variaveisGlobaisOptions.EolConnection);

        public async Task<UnidadeEscolarResposta> ObterDadosUnidadeEscolarPorCodigoUe(string codigoUe)
        {
            try
            {
                using var conexao = CriaConexao();
                conexao.Open();

                var query = @"SELECT distinct
                               concat(ue.cd_unidade_educacao, '-', ue.nm_exibicao_unidade) nomeCompletoUe,
                               tp.dc_tp_logradouro tipoLogradouro,
                               RTRIM(LTRIM(ue.nm_logradouro)) logradouro,
                               RTRIM(LTRIM(ue.cd_nr_endereco)) numero,
                               ue.nm_bairro bairro,
                               ue.cd_cep cep,
                               mun.nm_municipio municipio,
                               mun.sg_uf uf,
                               (
                                  select top 1
                                     dcu.dc_dispositivo as email 
                                  from
                                     v_cadastro_unidade_educacao cue 
                                     inner join
                                        dispositivo_comunicacao_unidade dcu 
                                        on cue.cd_unidade_educacao = dcu.cd_unidade_educacao 
                                     inner join
                                        tipo_dispositivo_comunicacao tdc 
                                        on dcu.tp_dispositivo_comunicacao = tdc.tp_dispositivo_comunicacao 
                                  where
                                     cue.cd_unidade_educacao = @codigoUe 
                                     and tdc.tp_dispositivo_comunicacao = 9 
                               )
                               email,
                               (
                                  select top 1
                                     dcu.dc_dispositivo as telefone  
                                  from
                                     v_cadastro_unidade_educacao cue 
                                     inner join
                                        dispositivo_comunicacao_unidade dcu 
                                        on cue.cd_unidade_educacao = dcu.cd_unidade_educacao 
                                     inner join
                                        tipo_dispositivo_comunicacao tdc 
                                        on dcu.tp_dispositivo_comunicacao = tdc.tp_dispositivo_comunicacao 
                                  where
                                     cue.cd_unidade_educacao = @codigoUe 
                                     and tdc.tp_dispositivo_comunicacao = 1 
                               )
                               telefone 
                            from
                               v_cadastro_unidade_educacao ue 
                               inner join
                                  municipio mun 
                                  on mun.cd_municipio = ue.cd_municipio 
                               inner join
                                  tipo_logradouro tp 
                                  on tp.tp_logradouro = ue.tp_logradouro 
                            where
                               cd_unidade_educacao = @codigoUe ";

                var parametros = new { codigoUe };
                var dadosUnidadeEscolar = await conexao.QuerySingleAsync<UnidadeEscolarResposta>(query, parametros);
                conexao.Close();

                return dadosUnidadeEscolar;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }
    }
}