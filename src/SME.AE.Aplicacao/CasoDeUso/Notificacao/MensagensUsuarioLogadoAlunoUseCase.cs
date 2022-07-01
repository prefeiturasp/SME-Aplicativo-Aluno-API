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

            var resposta = await mediator.Send(new DadosAlunoCommand(cpf));

            if (resposta.Data == null)
                throw new NegocioException("Não foi possivel obter os alunos por escola");

            var listaEscolas = (IEnumerable<ListaEscola>)resposta.Data;

            if (listaEscolas == null || !listaEscolas.Any())
                throw new NegocioException($"Não encontrado usuário com o codigo {codigoAluno}");

            var mensagensUsuarioLogadoAlunoQuery = new MensagensUsuarioLogadoAlunoQuery();

            foreach (var item in listaEscolas)
            {
                item.Alunos.ToList().ForEach(a =>
                {
                    mensagensUsuarioLogadoAlunoQuery.Parametros.Add(new ParametrosMensagensUsuarioLogado()
                    {
                        CodigoAluno = a.CodigoEol.ToString(),
                        CodigoDRE = a.CodigoDre,
                        CodigoTurma = a.CodigoTurma.ToString(),
                        CodigoUE = a.CodigoEscola,
                        CodigoUsuario = usuario.Id,
                        DataUltimaConsulta = dataUltimaConsulta,
                        ModalidadesId = item.ModalidadeCodigo.ToString(),
                        SerieResumida = a.SerieResumida
                    });
                });
            }

            return await mediator.Send(mensagensUsuarioLogadoAlunoQuery);
        }
    }
}
