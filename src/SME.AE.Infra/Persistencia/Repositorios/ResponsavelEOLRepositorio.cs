using Dapper;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
	public class ResponsavelEOLRepositorio : IResponsavelEOLRepositorio {
		private SqlConnection CriaConexao() => new SqlConnection(ConnectionStrings.ConexaoEol);
		public async Task<IEnumerable<ResponsavelEOLDto>> ListarCpfResponsavelDaDreUeTurma()
		{
			var sql =
				@"
					select 
						distinct
						dre.cd_unidade_educacao CodigoDre,
						dre.nm_exibicao_unidade Dre,
						vue.cd_unidade_educacao CodigoUe,
						vue.nm_exibicao_unidade Ue,
						te.cd_turma_escola CodigoTurma,
						coalesce(ra.cd_cpf_responsavel, 0) CpfResponsavel
					from v_aluno_cotic(nolock) a
					inner join responsavel_aluno(nolock) ra on ra.cd_aluno = a.cd_aluno 
					inner join v_matricula_cotic(nolock) m on m.cd_aluno = a.cd_aluno 
					inner join matricula_turma_escola(nolock) mte on mte.cd_matricula = m.cd_matricula 
					inner join v_cadastro_unidade_educacao(nolock) vue on vue.cd_unidade_educacao = m.cd_escola
					inner join v_cadastro_unidade_educacao(nolock) dre on dre.cd_unidade_educacao = vue.cd_unidade_administrativa_referencia
					inner join turma_escola(nolock) te on te.cd_turma_escola = mte.cd_turma_escola
					where 
						mte.cd_situacao_aluno IN ( 1, 6, 10, 13 ) and
						ra.dt_fim IS NULL
				";

			using var conn = CriaConexao();
			await conn.OpenAsync();
			var responsaveisEOL = await conn.QueryAsync<ResponsavelEOLDto>(sql);
			await conn.CloseAsync();
			return responsaveisEOL;
		}

	}
}
