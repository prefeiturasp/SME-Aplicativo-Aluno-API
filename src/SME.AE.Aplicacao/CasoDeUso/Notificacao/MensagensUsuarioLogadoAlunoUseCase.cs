using MediatR;
using SME.AE.Aplicacao.Comandos.Aluno;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Consultas.Notificacao.ListarNotificacaoAluno;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using SME.AE.Comum.Excecoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso.Notificacao
{
    public class MensagensUsuarioLogadoAlunoUseCase : IMensagensUsuarioLogadoAlunoUseCase
    {
        private readonly IMediator mediator;

        public MensagensUsuarioLogadoAlunoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<NotificacaoResposta>> Executar(string cpf, long codigoAluno, DateTime dataUltimaConsulta)
        {
            var usuario = await mediator.Send(new ObterUsuarioQuery(cpf));

            if (usuario == null)
                throw new NegocioException($"Não encontrado usuário com o CPF {cpf}");

            RespostaApi resposta = await mediator.Send(new DadosAlunoCommand(cpf));

            if (resposta.Data == null)
                throw new NegocioException("Não foi possivel obter os alunos por escola");

            var listaEscolas = (IEnumerable<ListaEscola>)resposta.Data;

            var aluno = listaEscolas.FirstOrDefault(
                x => x.Alunos.Any(z => z.CodigoEol == codigoAluno)).Alunos.FirstOrDefault(x => x.CodigoEol == codigoAluno);

            if (aluno == null)
                throw new NegocioException($"Não encontrado usuário com o codigo {codigoAluno}");

            var grupos = listaEscolas.Where(x => x.Alunos.Any(z => z.CodigoEol == aluno.CodigoEol)).Select(x => x.CodigoGrupo);

            return await mediator.Send(new MensagensUsuarioLogadoAlunoQuery
            {
                CodigoAluno = aluno.CodigoEol.ToString(),
                CodigoDRE = aluno.CodigoDre,
                CodigoTurma = aluno.CodigoTurma.ToString(),
                CodigoUE = aluno.CodigoEscola,
                CodigoUsuario  = usuario.Id,
                GruposId = string.Join(',', grupos),
                DataUltimaConsulta = dataUltimaConsulta,
                SerieResumida = aluno.SerieResumida
            });
        }
    }
}
