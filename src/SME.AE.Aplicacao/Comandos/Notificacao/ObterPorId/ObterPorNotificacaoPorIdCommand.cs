using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.Notificacao
{

    public class ObterPorNotificacaoPorIdCommand : IRequest<NotificacaoResposta>
    {
        public long Id { get; set; }

        public ObterPorNotificacaoPorIdCommand(long id)
        {
            Id = id;
        }
    }

    public class ObterPorNotificacaoPorIdCommandHandler : IRequestHandler<ObterPorNotificacaoPorIdCommand, NotificacaoResposta>
    {
        private readonly INotificacaoRepository _repository;
        private readonly IGrupoComunicadoRepository _grupoComunicadoRepository;

        public ObterPorNotificacaoPorIdCommandHandler(INotificacaoRepository repository, IGrupoComunicadoRepository grupoComunicadoRepository)
        {
            _repository = repository;
            _grupoComunicadoRepository = grupoComunicadoRepository;
        }


        public async Task<NotificacaoResposta> Handle(ObterPorNotificacaoPorIdCommand request, CancellationToken cancellationToken)
        {

            var Notificacao =  await _repository.ObterPorId(request.Id);
            var grupos = await _grupoComunicadoRepository.ObterTodos();


            var notificacaoResposta = new NotificacaoResposta()
            {
                AlteradoEm = Notificacao.AlteradoEm,
                AlteradoPor = Notificacao.AlteradoPor,
                CriadoEm = Notificacao.CriadoEm,
                CriadoPor = Notificacao.CriadoPor,
                Id = Notificacao.Id,
                DataEnvio = Notificacao.DataEnvio,
                DataExpiracao = Notificacao.DataExpiracao,
                Mensagem = Notificacao.Mensagem,
                Titulo = Notificacao.Titulo,
               // MensagemVisualizada = Notificacao.MensagemVisualizada,
                Grupos = SelecionarGrupos(Notificacao.Grupo, grupos)

            };
      
            return notificacaoResposta;
        }

        private IEnumerable<Grupo> SelecionarGrupos(string grupo, IEnumerable<GrupoComunicado> grupos)
        {
            var ids = grupo.Split(',').Select(x => Convert.ToInt64(x));
            return grupos.Where(w => ids.Contains(w.Id)).Select(s => new Grupo { Codigo = s.Id, Nome = s.Nome });
        }
    }

}
