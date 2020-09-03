using Dapper;
using Dommel;
using Npgsql;
using Sentry;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.NotificacaoPorUsuario;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Dominio.Entidades;
using SME.AE.Infra.Persistencia.Consultas;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class NotificacaoRepository : BaseRepositorio<Notificacao>, INotificacaoRepository
    {
        public NotificacaoRepository() : base(ConnectionStrings.Conexao)
        {

        }

        public async Task<IEnumerable<NotificacaoPorUsuario>> ObterPorGrupoUsuario(string grupo, string cpf)
        {
            IEnumerable<NotificacaoPorUsuario> list = null;

            var query = NotificacaoConsultas.ObterPorUsuarioLogado
                  //"WHERE UNL.usuario_cpf = @cpf" +
                  + " WHERE (DATE(DataExpiracao) >= @dataAtual OR DataExpiracao IS NULL) " +
                    " AND (DATE(DataEnvio) <= @dataAtual) ";


            await using var conn = new NpgsqlConnection(ConnectionStrings.Conexao);
            conn.Open();
            var dataAtual = DateTime.Now.Date;
            list = await conn.QueryAsync<NotificacaoPorUsuario>(
                query, new
                {
                    grupo,
                    cpf,
                    dataAtual
                });
            conn.Close();
            return list;
        }

        // TODO Refatorar para montar a query aqui ao inves de receber por parametro
        public async Task<IDictionary<string, object>> ObterGruposDoResponsavel(string cpf, string grupos, string nomeGrupos)
        {
            IDictionary<string, object> list = null;

            try
            {
                await using (var conn = new SqlConnection(ConnectionStrings.ConexaoEol))
                {
                    conn.Open();
                    var query = $"select {nomeGrupos} from(select {grupos + NotificacaoConsultas.GruposDoResponsavel}) grupos";
                    var resultado = await conn.QueryAsync(query, new { cpf });

                    if (resultado.Any())
                        list = resultado.First() as IDictionary<string, object>;

                    conn.Close();
                }
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
            await using (var conn = new SqlConnection(ConnectionStrings.ConexaoEol))
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
            await using var conn = new NpgsqlConnection(ConnectionStrings.Conexao);
            conn.Open();
            notificacao.InserirAuditoria();
            notificacao.InserirCategoria();
            await conn.InsertAsync(notificacao);
            conn.Close();
        }


        public async Task InserirNotificacaoAluno(NotificacaoAluno notificacaoAluno)
        {
            await using var conn = new NpgsqlConnection(ConnectionStrings.Conexao);
            conn.Open();
            notificacaoAluno.InserirAuditoria();
            await conn.InsertAsync(notificacaoAluno);
            conn.Close();
        }

        public async Task InserirNotificacaoTurma(NotificacaoTurma notificacaoTurma)
        {
            await using var conn = new NpgsqlConnection(ConnectionStrings.Conexao);
            conn.Open();
            notificacaoTurma.InserirAuditoria();
            await conn.InsertAsync(notificacaoTurma);
            conn.Close();
        }

        public async Task<IEnumerable<NotificacaoTurma>> ObterTurmasPorNotificacao(long id)
        {
            var consulta = @"select nt.id, nt.notificacao_id, nt.codigo_eol_turma, nt.criadoem from notificacao_turma nt 
                            where notificacao_id = @id";

            IEnumerable<NotificacaoTurma> retorno = default;

            using (var conexao = InstanciarConexao())
            {
                conexao.Open();

                retorno = await conexao.QueryAsync<NotificacaoTurma>(consulta, new { id });

                conexao.Close();
            }

            return retorno == null || !retorno.Any() ? default : retorno;  
        }

        public async Task<IEnumerable<NotificacaoResposta>> ListarNotificacoes(string gruposId, string codigoUe, string codigoDre, string codigoTurma, string codigoAluno, long usuarioId)
        {
            using (var conexao = InstanciarConexao())
            {
                var consulta = MontarQueryListagemCompleta();

                var retorno = await conexao.QueryAsync<NotificacaoResposta>(consulta, new { gruposId, codigoUe, codigoDre, codigoTurma = long.Parse(codigoTurma), codigoAluno = long.Parse(codigoAluno), usuarioId });

                conexao.Close();

                return retorno;
            }
        }

        public async Task<NotificacaoResposta> NotificacaoPorId(long Id)
        {
            using (var conexao = InstanciarConexao())
            {
                var consulta = QueryPorId();
                var retorno = await conexao.QueryFirstOrDefaultAsync<NotificacaoResposta>(consulta, new { Id });
                conexao.Close();
                return retorno;
            }
        }

        public async Task<Notificacao> Atualizar(Notificacao notificacao)
        {
            try
            {
                await using (var conn = new NpgsqlConnection(ConnectionStrings.Conexao))
                {
                    conn.Open();
                    await conn.ExecuteAsync(
                        @"UPDATE notificacao set mensagem=@Mensagem, titulo=@Titulo, grupo=@Grupo, 
                                    dataEnvio=@DataEnvio, dataExpiracao=@DataExpiracao, criadoEm=@CriadoEm, 
                                    criadoPor=@CriadoPor, alteradoEm=@AlteradoEm, alteradoPor=@AlteradoPor 
                               WHERE id=@Id",
                        notificacao);
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return null;
            }

            return notificacao;
        }

        public async Task<bool> Remover(Notificacao notificacao)
        {
            bool resultado = false;

            try
            {
                await using (var conn = new NpgsqlConnection(ConnectionStrings.Conexao))
                {
                    conn.Open();

                    var retorno = await conn.ExecuteAsync(
                        @"DELETE FROM notificacao where id = @ID", notificacao);
                    conn.Close();
                }
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
                    where n.id = @Id ";
        }

        private string MontarQueryListagemCompleta()
        {
            return $@"select {CamposConsultaNotificacao("notificacao", true)}
                      unl.mensagemvisualizada from(
                      {QueryComunicadosSME()}
                      union
                      {QueryComunicadosDRE()}
                      union
                      {QueryComunicadosUE()}
                      union
                      {QueryComunicadosUEMOD()}
                      union
                      {QueryComunicadosTurmas()}
                      union
                      {QueryComunicadosAlunos()}
                      )as notificacao
                      left join usuario_notificacao_leitura unl on 
                      unl.notificacao_id = notificacao.id 
                      and unl.usuario_id = @usuarioId
                      and unl.codigo_eol_aluno = @codigoAluno";
        }

        private string QueryComunicadosSME()
        {
            return $@"select {CamposConsultaNotificacao("n")}
                        from notificacao n 
                        where n.tipocomunicado = 1
                        and string_to_array(n.grupo,',') 
                        && string_to_array(@gruposId,',')";
        }

        private string QueryComunicadosDRE()
        {
            return $@"select {CamposConsultaNotificacao("n")}
                    from notificacao n 
                    where n.tipocomunicado = 2
                    and n.dre_codigoeol = @codigoDre";
        }

        private string QueryComunicadosUE()
        {
            return $@"select {CamposConsultaNotificacao("n")}
                      from notificacao n
                      where n.tipocomunicado = 3
                      and n.ue_codigoeol = @codigoUe";
        }

        private string QueryComunicadosUEMOD()
        {
            return $@"select {CamposConsultaNotificacao("n")}
                    from notificacao n
                    where n.tipocomunicado = 4
                    and n.ue_codigoeol = @codigoUe 
                    and string_to_array(n.grupo,',') && string_to_array(@gruposId,',')";
        }

        private string QueryComunicadosTurmas()
        {
            return $@"select {CamposConsultaNotificacao("n")}
                    from notificacao n
                    inner join notificacao_turma nt on nt.notificacao_id = n.id
                    where n.tipocomunicado = 5 
                    and nt.codigo_eol_turma = @codigoTurma";
        }

        private string QueryComunicadosAlunos()
        {
            return $@"select {CamposConsultaNotificacao("n")}
                    from notificacao n
                    inner join notificacao_aluno na 
                    on na.notificacao_id = n.id
                    where n.tipocomunicado = 6 
                    and na.codigo_eol_aluno = @codigoAluno";
        }

        private string CamposConsultaNotificacao(string abreviacao, bool camposGeral = false)
        {
            return $@"{abreviacao}.Id,
                    {abreviacao}.Mensagem,
                    {abreviacao}.Titulo,
                    {(camposGeral ? $"string_to_array({abreviacao}.Grupo,',') as GruposId" : $"{abreviacao}.grupo")},
                    {abreviacao}.DataEnvio,
                    {abreviacao}.DataExpiracao,
                    {abreviacao}.CriadoEm,
                    {abreviacao}.CriadoPor,
                    {abreviacao}.AlteradoEm,
                    {abreviacao}.AlteradoPor,
                    {abreviacao}.TipoComunicado,
                    {abreviacao}.CategoriaNotificacao,
                    {abreviacao}.dre_codigoeol {(camposGeral ? "as CodigoDre" : "")},
                    {abreviacao}.ue_codigoeol {(camposGeral ? "as CodigoUe," : "")}";
        }
    }
}