using MediatR;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System;

namespace SME.AE.Aplicacao.Comandos.Notificacao.Atualizar
{
    public class AtualizarNotificacaoCommand : IRequest<AtualizacaoNotificacaoResposta>
    {
        public long Id { get; set; }

        public string Titulo { get; set; }

        public string Mensagem { get; set; }

        public DateTime DataExpiracao { get; set; }

        public string AlteradoPor { get; set; }

        public AtualizarNotificacaoCommand(long id, string titulo, string mensagem, DateTime dataExpiracao, string alteradoPor)
        {
            Id = id;
            Titulo = titulo;
            Mensagem = mensagem;
            DataExpiracao = dataExpiracao;
            AlteradoPor = alteradoPor;
        }
    }
}