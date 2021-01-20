using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.Notificacao.Atualizar
{
    public class AtualizarNotificacaoCommandHandler : IRequestHandler<AtualizarNotificacaoCommand, AtualizacaoNotificacaoResposta>
    {
        private readonly INotificacaoRepository _repository;

        public AtualizarNotificacaoCommandHandler(INotificacaoRepository repository)
        {
            this._repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<AtualizacaoNotificacaoResposta> Handle(AtualizarNotificacaoCommand request, CancellationToken cancellationToken)
        {
            var atualizarNotificacaoDto = MontaObjetoAtualizacoNotificacao(request);
            await _repository.Atualizar(atualizarNotificacaoDto);
            return MontaObjetoDeAlteracaoNotificacaoResposta(request);
        }

        private static AtualizacaoNotificacaoResposta MontaObjetoDeAlteracaoNotificacaoResposta(AtualizarNotificacaoCommand request)
        {
            var atualizacaoNotificacaoResposa = new AtualizacaoNotificacaoResposta
            {
                Id = request.Id,
                Titulo = request.Titulo,
                Mensagem = request.Mensagem,
                DataExpiracao = request.DataExpiracao
            };
            return atualizacaoNotificacaoResposa;
        }

        private static AtualizarNotificacaoDto MontaObjetoAtualizacoNotificacao(AtualizarNotificacaoCommand request)
        {
            var notificacaoDTO = new AtualizarNotificacaoDto
            {
                Id = request.Id,
                Titulo = request.Titulo,
                Mensagem = request.Mensagem,
                DataExpiracao = request.DataExpiracao,
                AlteradoEm = DateTime.Now,
                AlteradoPor = request.AlteradoPor
            };
            return notificacaoDTO;
        }
    }
}