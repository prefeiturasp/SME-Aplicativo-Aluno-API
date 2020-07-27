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

namespace SME.AE.Aplicacao.Consultas.Notificacao.ObterNotificacaoPorid
{
    public class ObterNotificacaoPorIdQueryHandler : IRequestHandler<ObterNotificacaoPorIdQuery, NotificacaoResposta>
    {
        private readonly INotificacaoRepository _repository;
        private readonly IGrupoComunicadoRepository _grupoComunicadoRepository;

        public ObterNotificacaoPorIdQueryHandler(INotificacaoRepository repository, IGrupoComunicadoRepository grupoComunicadoRepository)
        {
            this._repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this._grupoComunicadoRepository = grupoComunicadoRepository ?? throw new ArgumentNullException(nameof(grupoComunicadoRepository));
        }

        public async Task<NotificacaoResposta> Handle(ObterNotificacaoPorIdQuery request, CancellationToken cancellationToken)
        {
            var Notificacao = await _repository.ObterPorId(request.Id);
            var grupos = await _grupoComunicadoRepository.ObterTodos();


            var notificacaoResposta = new NotificacaoResposta()
            {
                AlteradoEm = Notificacao.AlteradoEm,
                AlteradoPor = Notificacao.AlteradoPor,
                CriadoEm = Notificacao.CriadoEm,
                CriadoPor = Notificacao.CriadoPor,
                Id = Notificacao.Id,
                CodigoDre = Notificacao.CodigoDre,
                CodigoUe = Notificacao.CodigoUe,

                DataEnvio = Notificacao.DataEnvio,
                DataExpiracao = Notificacao.DataExpiracao,
                Mensagem = Notificacao.Mensagem,
                Titulo = Notificacao.Titulo, 
                TipoComunicado = Notificacao.TipoComunicado,
                CategoriaNotificacao = Notificacao.CategoriaNotificacao,
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
