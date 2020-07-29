using MediatR;
using Microsoft.Practices.ObjectBuilder2;
using SME.AE.Aplicacao.Comandos.GrupoNotificacao.ObterPorResponsavel;
using SME.AE.Aplicacao.Comandos.Notificacao.ObterPorGrupo;
using SME.AE.Dominio.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;
using System.Threading.Tasks;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Excecoes;
using SME.AE.Aplicacao.Comandos.Aluno;
using System.Linq;

namespace SME.AE.Aplicacao.CasoDeUso.Notificacao
{
    public class ObterNotificacaoDoUsuarioLogadoUseCase : IObterNotificacaoDoUsuarioLogadoUseCase
    {
        private readonly IMediator mediator;

        public ObterNotificacaoDoUsuarioLogadoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<NotificacaoResposta>> Executar(string usuario)
        {
            List<string> grupos = await mediator.Send(new ObterGrupoNotificacaoPorResponsavelCommand(usuario));


            RespostaApi resposta = await mediator.Send(new DadosAlunoCommand(usuario));

            if (resposta.Data == null)
                throw new NegocioException("Não foi possivel obter os alunos por escola");

            //var listaEscolas = (IEnumerable<ListaEscola>)resposta.Data;
            //var codigoALuno = new List<string>();
            //listaEscolas.ForEach(x => x.Alunos.Select( new List<string>() )


            return await mediator.Send(new ObterNotificacaoPorGrupoCommand(grupos.JoinStrings(","), usuario));
        }
    }
}
