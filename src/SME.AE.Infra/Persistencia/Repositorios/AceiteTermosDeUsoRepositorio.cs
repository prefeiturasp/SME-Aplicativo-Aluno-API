using Dapper;
using Sentry;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Interfaces.Servicos;
using SME.AE.Comum;
using SME.AE.Dominio.Entidades;
using System;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class AceiteTermosDeUsoRepositorio : BaseRepositorio<AceiteTermosDeUso>, IAceiteTermosDeUsoRepositorio
    {
        private readonly IServicoTelemetria servicoTelemetria;

        public AceiteTermosDeUsoRepositorio(VariaveisGlobaisOptions variaveisGlobaisOptions,
            IServicoTelemetria servicoTelemetria) : base(variaveisGlobaisOptions.AEConnection)
        {
            this.servicoTelemetria = servicoTelemetria;
        }

        public async Task<bool> RegistrarAceite(AceiteTermosDeUso aceiteTermosDeUso)
        {
            try
            {
                await using var conn = InstanciarConexao();
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

                var consulta = $"SELECT count(id) FROM public.aceite_termos_de_uso WHERE cpf_usuario = @cpfUsuario AND versao = @versao";
                var parametros = new { cpfUsuario, versao };

                var aceiteExiste = await servicoTelemetria.RegistrarComRetornoAsync<int>(async () => await SqlMapper.QueryFirstOrDefaultAsync<int>(conexao,
                    consulta, parametros), "query", "Query AE", consulta, parametros.ToString()); 

                conexao.Close();
                return aceiteExiste >= 1;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return false;
            }
        }
    }
}