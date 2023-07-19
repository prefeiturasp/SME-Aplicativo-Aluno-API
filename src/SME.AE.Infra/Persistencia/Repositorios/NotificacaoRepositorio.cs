using Dapper;
using Dommel;
using Sentry;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Interfaces.Servicos;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Aplicacao.Comum.Modelos.NotificacaoPorUsuario;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Comum;
using SME.AE.Dominio.Entidades;
using SME.AE.Infra.Persistencia.Consultas;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class NotificacaoRepositorio : BaseRepositorio<Notificacao>, INotificacaoRepositorio
    {
        private readonly VariaveisGlobaisOptions variaveisGlobaisOptions;
        private readonly IServicoTelemetria servicoTelemetria;

        public NotificacaoRepositorio(VariaveisGlobaisOptions variaveisGlobaisOptions,
            IServicoTelemetria servicoTelemetria) : base(variaveisGlobaisOptions.AEConnection)
        {
            this.variaveisGlobaisOptions = variaveisGlobaisOptions ?? throw new ArgumentNullException(nameof(variaveisGlobaisOptions));
            this.servicoTelemetria = servicoTelemetria;
        }

        public async Task<IEnumerable<NotificacaoPorUsuario>> ObterPorGrupoUsuario(string grupo, string cpf)
        {
            var query = NotificacaoConsultas.ObterPorUsuarioLogado
                  + " WHERE (DATE(DataExpiracao) >= @dataAtual OR DataExpiracao IS NULL) " +
                    " AND (DATE(DataEnvio) <= @dataAtual)  and not n.excluido ";

            var dataAtual = DateTime.Now.Date;
            var parametros = new { grupo, cpf, dataAtual };

            using var conn = InstanciarConexao();

            conn.Open();

            IEnumerable<NotificacaoPorUsuario> list = await servicoTelemetria.RegistrarComRetornoAsync<NotificacaoPorUsuario>(async () => 
                await SqlMapper.QueryAsync<NotificacaoPorUsuario>(conn, query, parametros), "query", "Query AE", query, parametros.ToString());

            conn.Close();

            return list;
        }

        // TODO Refatorar para montar a query aqui ao inves de receber por parametro
        public async Task<IDictionary<string, object>> ObterGruposDoResponsavel(string cpf, string grupos, string nomeGrupos)
        {
            IDictionary<string, object> list = null;

            try
            {
                await using var conn = new SqlConnection(variaveisGlobaisOptions.EolConnection);
                conn.Open();
                var query = $"select {nomeGrupos} from(select {grupos + NotificacaoConsultas.GruposDoResponsavel}) grupos";
                var resultado = await conn.QueryAsync(query, new { cpf });

                if (resultado.Any())
                    list = resultado.First() as IDictionary<string, object>;

                conn.Close();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return null;
            }

            return list;
        }

        public async Task<IEnumerable<string>> ObterResponsaveisPorGrupo(string where)
        {
            await using var conn = new SqlConnection(variaveisGlobaisOptions.EolConnection);
            {
                conn.Open();
                var query = $"{NotificacaoConsultas.ResponsaveisPorGrupo}{where}";
                var resultado = await conn.QueryAsync<string>(query);
                conn.Close();
                if (resultado.Any())
                    return resultado;
            }

            return null;
        }

        public async Task Criar(Notificacao notificacao)
        {
            await using var conn = InstanciarConexao();
            conn.Open();
            notificacao.InserirAuditoria();
            notificacao.InserirCategoria();
            await conn.InsertAsync(notificacao);
            conn.Close();
        }

        public async Task InserirNotificacaoAluno(NotificacaoAluno notificacaoAluno)
        {
            await using var conn = InstanciarConexao();
            conn.Open();
            notificacaoAluno.InserirAuditoria();
            await conn.InsertAsync(notificacaoAluno);
            conn.Close();
        }

        public async Task InserirNotificacaoTurma(NotificacaoTurma notificacaoTurma)
        {
            await using var conn = InstanciarConexao();
            conn.Open();
            notificacaoTurma.InserirAuditoria();
            await conn.InsertAsync(notificacaoTurma);
            conn.Close();
        }

        public async Task<IEnumerable<NotificacaoTurma>> ObterTurmasPorNotificacao(long id)
        {
            var consulta = @"select nt.id, nt.notificacao_id, nt.codigo_eol_turma, nt.criadoem from notificacao_turma nt 
                            where notificacao_id = @id and not nt.excluido ";

            var parametros = new { id };

            using var conexao = InstanciarConexao();

            conexao.Open();

            IEnumerable<NotificacaoTurma> retorno = await servicoTelemetria.RegistrarComRetornoAsync<NotificacaoTurma>(async () =>
                await SqlMapper.QueryAsync<NotificacaoTurma>(conexao, consulta, parametros), "query", "Query AE", consulta, parametros.ToString());

            conexao.Close();

            return retorno == null || !retorno.Any() ? default : retorno;
        }

        public async Task<IEnumerable<NotificacaoResposta>> ListarNotificacoes(string modalidades, string tiposEscolas, string codigoUe, string codigoDre, string codigoTurma, string codigoAluno, long usuarioId, string serieResumida, DateTime? ultimaAtualizacao = null)
        {
            using var conexao = InstanciarConexao();

            var consulta = MontarQueryListagemCompleta(serieResumida, codigoAluno);
            var parametros = new { modalidades, tiposEscolas, codigoUe, codigoDre, codigoTurma = long.Parse(codigoTurma), codigoAluno = long.Parse(codigoAluno), usuarioId, serieResumida };

            //TODO: BOTAR A DATA DE FILTRO de ULTIMA ATUALIZACAO AQUI!
            conexao.Open();
            var retorno = await servicoTelemetria.RegistrarComRetornoAsync<NotificacaoResposta>(async () =>
                await SqlMapper.QueryAsync<NotificacaoResposta>(conexao, consulta, parametros), "query", "Query AE", consulta, parametros.ToString());

            conexao.Close();

            return retorno;
        }

        public async Task<IEnumerable<NotificacaoSgpDto>> ListarNotificacoesNaoEnviadas()
        {
            using var conexao = InstanciarConexao();

            var consulta = MontarQueryListagemCompletaNaoEnviadoPushNotification();

            var retorno = await servicoTelemetria.RegistrarComRetornoAsync<NotificacaoSgpDto>(async () =>
                await SqlMapper.QueryAsync<NotificacaoSgpDto>(conexao, consulta), "query", "Query AE", consulta);

            conexao.Close();

            return retorno;
        }

        public async Task<NotificacaoResposta> NotificacaoPorId(long Id)
        {
            using var conexao = InstanciarConexao();

            var consulta = QueryPorId();
            var parametros = new { Id };

            var retorno = await servicoTelemetria.RegistrarComRetornoAsync<NotificacaoResposta>(async () =>
                await SqlMapper.QueryFirstOrDefaultAsync<NotificacaoResposta>(conexao, consulta, parametros), "query", "Query AE", consulta, parametros.ToString());

            conexao.Close();

            return retorno;
        }

        public async Task Atualizar(AtualizarNotificacaoDto notificacao)
        {
            try
            {
                await using var conn = InstanciarConexao();
                conn.Open();
                await conn.ExecuteAsync(
                    @"UPDATE notificacao set mensagem=@Mensagem, 
                                                 titulo=@Titulo, 
                                                 dataExpiracao=@DataExpiracao, 
                                                 alteradoEm=@AlteradoEm, 
                                                 alteradoPor=@AlteradoPor,
                                                 enviadopushnotification=@EnviadoPushNotification
                               WHERE id=@Id",
                    notificacao);
                conn.Close();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }
        }

        public async Task<bool> Remover(Notificacao notificacao)
        {
            bool resultado = false;
            var queryExcluirNotificacao = new StringBuilder();
            try
            {
                await using var conn = InstanciarConexao();
                conn.Open();

                if(notificacao.TipoComunicado == Dominio.Comum.Enumeradores.TipoComunicado.ALUNO)
                    queryExcluirNotificacao.AppendLine("DELETE FROM notificacao_aluno WHERE notificacao_id = @ID;");

                if (notificacao.TipoComunicado== Dominio.Comum.Enumeradores.TipoComunicado.TURMA)
                    queryExcluirNotificacao.AppendLine("DELETE FROM notificacao_turma WHERE notificacao_id = @ID;");

                queryExcluirNotificacao.AppendLine("DELETE FROM notificacao where id = @ID;");

                await conn.ExecuteAsync(queryExcluirNotificacao.ToString(), new { ID = notificacao.Id });

                conn.Close();

            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return resultado;
            }

            return resultado;
        }

        private string QueryPorId()
        {
            return $@"select {CamposConsultaNotificacao("n")}
                    from notificacao n
                    where n.id = @Id and not n.excluido ";
        }

        private string MontarQueryListagemCompleta(string serieResumida, string codigoAluno)
        {
            var whereSerieResumida = string.IsNullOrWhiteSpace(serieResumida) ?
                "" : " and (n.seriesresumidas isnull or n.seriesresumidas = '' or (string_to_array(n.seriesresumidas, ',') && string_to_array(@serieResumida, ',')))";

            return
                $@"drop table if exists tmp_lista_notificacoes;
                   create temporary table tmp_lista_notificacoes as
                   select *
                   	from notificacao n		
                   where ((n.tipocomunicado = {(int)TipoComunicado.SME} or
                   	      (n.tipocomunicado = {(int)TipoComunicado.DRE} and n.dre_codigoeol = @codigoDre) or
                   	      (n.tipocomunicado = {(int)TipoComunicado.UEMOD} and n.ue_codigoeol = @codigoUe{whereSerieResumida}) or
                   	      (n.tipocomunicado in ({(int)TipoComunicado.SME_ANO}, {(int)TipoComunicado.DRE_ANO}){whereSerieResumida}) and
                   	      String_to_array(n.modalidades, ',') && String_to_array(@gruposId, ',') or
                   	      (n.tipocomunicado = {(int)TipoComunicado.UE} and n.dre_codigoeol = @codigoDre)))
                   
                   union
                   
                   select n.*
                   	from notificacao n 
                   		inner join notificacao_turma nt 
                   			on n.id = nt.notificacao_id 
                   where n.tipocomunicado = {(int)TipoComunicado.TURMA} and
                   	nt.codigo_eol_turma = @codigoTurma
                   	
                   union
                   
                   select n.*
                   	from notificacao n 
                   		inner join notificacao_aluno na 
                   			on n.id = na.notificacao_id
                   where n.tipocomunicado = {(int)TipoComunicado.ALUNO} and
                   	na.codigo_eol_aluno = @codigoAluno;
                   
                   select tmp.id,
                          tmp.mensagem,
                          tmp.titulo,
                          String_to_array(tmp.modalidades, ',') gruposid,
                          tmp.dataenvio,
                          tmp.dataexpiracao,
                          tmp.criadoem,
                          tmp.criadopor,
                          tmp.alteradoem,
                          tmp.alteradopor,
                          tmp.tipocomunicado,
                          tmp.seriesresumidas,
                          tmp.ano_letivo,
                          tmp.categorianotificacao,
                          tmp.enviadopushnotification,
                          tmp.dre_codigoeol codigodre,
                          tmp.ue_codigoeol codigoue,
                          tmp.ano_letivo anoletivo,
                          unl.mensagemvisualizada 
                   	from tmp_lista_notificacoes tmp
                   		left join usuario_notificacao_leitura unl 
                   			on tmp.id = unl.notificacao_id and
                   			   unl.usuario_id = @usuarioId and
                   			   unl.codigo_eol_aluno = @codigoAluno
                   where (unl.mensagemexcluida is null or (unl.mensagemexcluida is not null and not unl.mensagemexcluida)) and
                   	     (tmp.dataexpiracao is null or tmp.dataexpiracao::date >= current_date) and
                   	     tmp.dataenvio::date <= current_date and
                   	     tmp.enviadopushnotification;";
                $@"
                    select {CamposConsultaNotificacao("notificacao", true)}
                      notificacao.ano_letivo AnoLetivo,
                      unl.mensagemvisualizada from(
                      {QueryComunicadosSME()}
                      union
                      {QueryComunicadosAutomaticos()}
                      union
                      {QueryComunicadosDRE()}
                      union
                      {QueryComunicadosSME_ANO()}
                      {whereSerieResumida}
                      union
                      {QueryComunicadosDRE_ANO()}
                      {whereSerieResumida}
                      union
                      {QueryComunicadosUE()}
                      union
                      {QueryComunicadosUEMOD()}
                      {whereSerieResumida}
                      union
                      {QueryComunicadosTurmas()}
                      union
                      {QueryComunicadosAlunos()}
                      )as notificacao
                      left join usuario_notificacao_leitura unl on 
                      unl.notificacao_id = notificacao.id 
                      and unl.usuario_id = @usuarioId
                      and unl.codigo_eol_aluno = @codigoAluno
                      where (unl.mensagemexcluida isnull or unl.mensagemexcluida = false) and
                      	(notificacao.dataexpiracao isnull or notificacao.dataexpiracao >= current_date) and 
                        date_trunc('day', notificacao.dataenvio) <= current_date and
                        notificacao.enviadopushnotification
                ";
        }

        private string MontarQueryListagemCompletaNaoEnviadoPushNotification()
        {
            return
                $@"
                    select {CamposConsultaNotificacao("n")},
					  array(select codigo_eol_aluno::varchar from notificacao_aluno na where na.notificacao_id = n.id) alunos,
					  array(select codigo_eol_turma::varchar from notificacao_turma nt where nt.notificacao_id = n.id) turmas
                      from notificacao n
                      where 
                          date_trunc('day', n.dataenvio) <= current_date
                      and date_trunc('day', n.dataexpiracao) >= current_date
                      and (not n.enviadopushnotification) and not n.excluido 
                ";
        }

        private string QueryComunicadosSME()
        {
            return $@"select {CamposConsultaNotificacao("n")}
                        from notificacao n 
                        where n.tipocomunicado = {(int)TipoComunicado.SME} and not n.excluido 
                        and string_to_array(n.modalidades,',') 
                        && string_to_array(@modalidades,',') and string_to_array(n.tipos_escolas,',') 
                        && string_to_array(@tiposEscolas,',')";
        }

        private string QueryComunicadosAutomaticos()
        {
            return $@"select {CamposConsultaNotificacao("n")}
                        from notificacao n 
                        inner join notificacao_aluno na 
                         on na.notificacao_id = n.id
                        where n.tipocomunicado = {(int)TipoComunicado.MENSAGEM_AUTOMATICA} and not n.excluido  and na.codigo_eol_aluno =@codigoAluno";
        }

        private string QueryComunicadosSME_ANO()
        {
            return $@"select {CamposConsultaNotificacao("n")}
                        from notificacao n 
                        where n.tipocomunicado = {(int)TipoComunicado.SME_ANO} and not n.excluido 
                        and string_to_array(n.modalidades,',') && string_to_array(@modalidades,',') and string_to_array(n.tipos_escolas,',') 
                        && string_to_array(@tiposEscolas,',')";
        }

        private string QueryComunicadosDRE()
        {
            return $@"select {CamposConsultaNotificacao("n")}
                    from notificacao n 
                    where n.tipocomunicado = {(int)TipoComunicado.DRE} and not n.excluido 
                    and string_to_array(n.modalidades,',') && string_to_array(@modalidades,',') and string_to_array(n.tipos_escolas,',') 
                        && string_to_array(@tiposEscolas,',') and n.dre_codigoeol = @codigoDre";
        }

        private string QueryComunicadosDRE_ANO()
        {
            return $@"select {CamposConsultaNotificacao("n")}
                    from notificacao n 
                    where n.tipocomunicado = {(int)TipoComunicado.DRE_ANO} and not n.excluido 
                    and string_to_array(n.modalidades,',') && string_to_array(@modalidades,',') and string_to_array(n.tipos_escolas,',') 
                        && string_to_array(@tiposEscolas,',') and n.dre_codigoeol = @codigoDre";
        }

        private string QueryComunicadosUE()
        {
            return $@"select {CamposConsultaNotificacao("n")}
                      from notificacao n
                      where n.tipocomunicado = {(int)TipoComunicado.UE} and not n.excluido 
                      and n.ue_codigoeol = @codigoUe";
        }

        private string QueryComunicadosUEMOD()
        {
            return $@"select {CamposConsultaNotificacao("n")}
                    from notificacao n
                    where n.tipocomunicado = {(int)TipoComunicado.UEMOD} and not excluido 
                    and n.ue_codigoeol = @codigoUe 
                    and string_to_array(n.modalidades,',') && string_to_array(@modalidades,',') and string_to_array(n.tipos_escolas,',') && string_to_array(@tiposEscolas,',')";
        }

        private string QueryComunicadosTurmas()
        {
            return $@"select {CamposConsultaNotificacao("n")}
                    from notificacao n
                    inner join notificacao_turma nt on nt.notificacao_id = n.id
                    where n.tipocomunicado = {(int)TipoComunicado.TURMA} and not n.excluido 
                    and nt.codigo_eol_turma = @codigoTurma";
        }

        private string QueryComunicadosAlunos()
        {
            return $@"select {CamposConsultaNotificacao("n")}
                    from notificacao n
                    inner join notificacao_aluno na 
                    on na.notificacao_id = n.id
                    where n.tipocomunicado = {(int)TipoComunicado.ALUNO} and not n.excluido 
                    and na.codigo_eol_aluno = @codigoAluno";
        }

        private string CamposConsultaNotificacao(string abreviacao, bool camposGeral = false)
        {
            return $@"{abreviacao}.Id,
                    {abreviacao}.Mensagem,
                    {abreviacao}.Titulo,
                    {(camposGeral ? $"string_to_array({abreviacao}.modalidades,',') as GruposId" : $"{abreviacao}.modalidades")},
                    {abreviacao}.DataEnvio,
                    {abreviacao}.DataExpiracao,
                    {abreviacao}.CriadoEm,
                    {abreviacao}.CriadoPor,
                    {abreviacao}.AlteradoEm,
                    {abreviacao}.AlteradoPor,
                    {abreviacao}.TipoComunicado,
                    {abreviacao}.SeriesResumidas,
                    {abreviacao}.ano_letivo,
                    {abreviacao}.CategoriaNotificacao,
                    {abreviacao}.enviadopushnotification,
                    {abreviacao}.dre_codigoeol {(camposGeral ? "as CodigoDre" : "")},
                    {abreviacao}.ue_codigoeol {(camposGeral ? "as CodigoUe," : "")}";
        }

        public async Task<IEnumerable<NotificacaoAlunoResposta>> ObterNotificacoesAlunoPorId(long notificacaoId)
        {
            try
            {
                using var conexao = InstanciarConexao();

                var consulta = $@"select 
                                  codigo_eol_aluno as codigoAluno,
                                  n.id as notificacaoId
                                  from notificacao n
                                  inner join notificacao_aluno na 
                                  on na.notificacao_id = n.id
                                  where n.tipocomunicado = {(int)TipoComunicado.ALUNO} and not n.excluido 
                                    and n.id = @notificacaoId";

                var parametros = new { notificacaoId };

                var retorno = await servicoTelemetria.RegistrarComRetornoAsync<NotificacaoAlunoResposta>(async () =>
                    await SqlMapper.QueryAsync<NotificacaoAlunoResposta>(conexao, consulta, parametros), "query", "Query AE", consulta, parametros.ToString());

                conexao.Close();

                return retorno;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}