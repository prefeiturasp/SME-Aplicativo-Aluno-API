using Dapper;
using Newtonsoft.Json;
using Npgsql;
using Sentry;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class NotaAlunoSgpRepositorio : INotaAlunoSgpRepositorio
    {
        private NpgsqlConnection CriaConexao() => new NpgsqlConnection(ConnectionStrings.ConexaoSgp);

        public async Task<IEnumerable<NotaAlunoSgpDto>> ObterNotaAlunoSgp()
        {
            try
            {
                using var conexao = CriaConexao();
                conexao.Open();

                var notaAlunosSgp = await conexao
                    .QueryAsync<NotaAlunoSgpDto>(
                        @"
                        select distinct 
	                        t.ano_letivo AnoLetivo,
	                        aa.ue_id    CodigoUe,
	                        aa.turma_id CodigoTurma,
	                        pe.bimestre,
	                        nc.aluno_id CodigoAluno,
	                        cc.id CodigoComponenteCurricular,
	                        coalesce(cc.descricao_sgp, cc.descricao) ComponenteCurricular,
	                        coalesce (
		                        coalesce(ccn.nota::varchar, cv2.valor),
		                        coalesce(nc.nota::varchar, cv.valor)
	                        ) Nota
                        from 
	                        fechamento_turma ft
                        inner join periodo_escolar pe on pe.id = ft.periodo_escolar_id 
                        inner join turma t on t.id = ft.turma_id
                        inner join atividade_avaliativa aa on aa.turma_id = t.turma_id 
                        inner join notas_conceito nc on nc.atividade_avaliativa = aa.id 
                        left join componente_curricular cc on cc.id::varchar = nc.disciplina_id 
                        left join conceito_valores cv on cv.id = nc.conceito
                        left join conselho_classe_aluno cca on cca.aluno_codigo = nc.aluno_id 
                        left join conselho_classe_nota ccn on ccn.conselho_classe_aluno_id = cca.id 
                        left join conceito_valores cv2 on cv2.id = ccn.conceito_id 
                        left join conselho_classe cc2 on cc2.id = cca.conselho_classe_id 
                        where t.ano_letivo >= 2020	
                        ");
                conexao.Close();

                return notaAlunosSgp;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }
    }
}