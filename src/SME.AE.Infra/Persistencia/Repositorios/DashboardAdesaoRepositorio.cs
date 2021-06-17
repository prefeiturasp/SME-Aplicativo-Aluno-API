using Dapper;
using Npgsql;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Comum;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class DashboardAdesaoRepositorio : IDashboardAdesaoRepositorio
    {
        private readonly VariaveisGlobaisOptions variaveisGlobaisOptions;

        public DashboardAdesaoRepositorio(VariaveisGlobaisOptions variaveisGlobaisOptions)
        {
            this.variaveisGlobaisOptions = variaveisGlobaisOptions ?? throw new System.ArgumentNullException(nameof(variaveisGlobaisOptions));
        }
        private NpgsqlConnection CriaConexao() => new NpgsqlConnection(variaveisGlobaisOptions.AEConnection);

        public async Task IncluiOuAtualizaPorDreUeTurmaEmBatch(IEnumerable<DashboardAdesaoDto> listaAdesao)
        {
            var sqlUpdate = @"
				UPDATE 
					dashboard_adesao 
				set
					dre_nome=@dre_nome, 
					ue_nome=@ue_nome,
					usuarios_primeiro_acesso_incompleto=@usuarios_primeiro_acesso_incompleto, 
					usuarios_validos=@usuarios_validos, 
					usuarios_cpf_invalidos=@usuarios_cpf_invalidos, 
					usuarios_sem_app_instalado=@usuarios_sem_app_instalado
				where 
					dre_codigo=@dre_codigo and
					ue_codigo=@ue_codigo and
					codigo_turma=@codigo_turma 
            ";

            var sqlInsert = @"
				INSERT INTO dashboard_adesao 
					(dre_codigo, dre_nome, ue_codigo, ue_nome, codigo_turma, usuarios_primeiro_acesso_incompleto, usuarios_validos, usuarios_cpf_invalidos, usuarios_sem_app_instalado) 
				VALUES
					(@dre_codigo, @dre_nome, @ue_codigo, @ue_nome, @codigo_turma, @usuarios_primeiro_acesso_incompleto, @usuarios_validos, @usuarios_cpf_invalidos, @usuarios_sem_app_instalado) 
            ";

            foreach (var adesao in listaAdesao)
            {
                var conn = CriaConexao();
                try
                {
                    conn.Open();
                    var alterado = (conn.Execute(sqlUpdate, adesao)) > 0;
                    if (!alterado)
                        conn.Execute(sqlInsert, adesao);
                    conn.Close();
                }
                finally
                {
                    conn.Dispose();
                }
            }
            await Task.CompletedTask;
        }
    }
}
