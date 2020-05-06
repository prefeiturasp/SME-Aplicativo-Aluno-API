using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Dominio.Entidades;

namespace SME.AE.Aplicacao.Comandos.Notificacao.ObterPorGrupo
{
    public class ObterNotificacaoPorGrupoCommand : IRequest<IEnumerable<NotificacaoResposta>>
    {
        public string Grupo { get; set; }

        public ObterNotificacaoPorGrupoCommand(string grupo)
        {
            Grupo = grupo;
        }
    }

    public class ObterNotificacaoPorGrupoCommandHandler : IRequestHandler<ObterNotificacaoPorGrupoCommand,
        IEnumerable<NotificacaoResposta>>
    {
        private readonly INotificacaoRepository _repository;
        private readonly IGrupoComunicadoRepository _grupoComunicadoRepository;
        public ObterNotificacaoPorGrupoCommandHandler(INotificacaoRepository repository, IGrupoComunicadoRepository grupoComunicadoRepository)
        {
            _repository = repository;
            _grupoComunicadoRepository = grupoComunicadoRepository;
        }

        public async Task<IEnumerable<NotificacaoResposta>> Handle
            (ObterNotificacaoPorGrupoCommand request, CancellationToken cancellationToken)
        {
            var grupos = await _grupoComunicadoRepository.ObterTodos();
            var grupo = await _repository.ObterPorGrupo(request.Grupo);
            return grupo.Select(x => new NotificacaoResposta
            {
                AlteradoEm = x.AlteradoEm,
                AlteradoPor = x.AlteradoPor,
                CriadoEm = x.CriadoEm,
                CriadoPor = x.CriadoPor,
                Id = x.Id,
                DataEnvio = x.DataEnvio,
                DataExpiracao = x.DataExpiracao,
                Mensagem = x.Mensagem,
                Titulo = x.Titulo,
                Grupos = SelecionarGrupos(x.Grupo, grupos)
            });
        }

        private IEnumerable<Grupo> SelecionarGrupos(string grupo, IEnumerable<GrupoComunicado> grupos)
        {
            var ids = grupo.Split(',').Select(x => Convert.ToInt64(x));
            return grupos.Where(w => ids.Contains(w.Id)).Select(s => new Grupo { Codigo = s.Id, Nome = s.Nome });
        }
    }
}