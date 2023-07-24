using Dapper;
using SME.AE.Aplicacao;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Comum;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class ResponsavelEOLRepositorio : IResponsavelEOLRepositorio
    {
        private readonly VariaveisGlobaisOptions variaveisGlobaisOptions;

        public ResponsavelEOLRepositorio(VariaveisGlobaisOptions variaveisGlobaisOptions)
        {
            this.variaveisGlobaisOptions = variaveisGlobaisOptions ?? throw new ArgumentNullException(nameof(variaveisGlobaisOptions));
        }

        private SqlConnection CriaConexao() => new SqlConnection(variaveisGlobaisOptions.EolConnection);

        public async Task<int> AtualizarDadosResponsavel(string codigoAluno, long cpfResponsavel, string email, DateTime dataNascimentoResponsavel, string nomeMae, string dddCelular, string celular)
        {
            var query = @"update responsavel_aluno 
                             set email_responsavel = @email,
                                 dt_nascimento_mae_responsavel = @dataNascimentoResponsavel,
                                 nm_mae_responsavel = @NomeMae,
                                 cd_ddd_celular_responsavel = @dddCelular,
                                 nr_celular_responsavel = @celular,
                                 in_cpf_responsavel_confere = 'S',
                                 in_autoriza_envio_sms = 'S',
                                 cd_tipo_turno_celular = 1,
                                 dt_atualizacao_tabela = @dataAtualizacao
                           where cd_cpf_responsavel = @cpfResponsavel 
                             and cd_aluno = @codigoAluno";

            var parametros = new
            {
                codigoAluno,
                cpfResponsavel,
                email,
                dataNascimentoResponsavel,
                nomeMae,
                dddCelular,
                celular,
                dataAtualizacao = DateTime.Now
            };

            using var conn = CriaConexao();
            await conn.OpenAsync();
            var resultado = await conn.ExecuteAsync(query, parametros);
            await conn.CloseAsync();

            return resultado;
        }
    }
}
