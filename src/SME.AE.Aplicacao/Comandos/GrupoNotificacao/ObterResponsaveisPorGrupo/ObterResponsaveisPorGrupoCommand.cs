using MediatR;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Practices.ObjectBuilder2;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.GrupoNotificacao.ObterResponsaveisPorGrupo
{
    public class ObterResponsaveisPorGrupoCommand : IRequest<IEnumerable<string>>
    {
        public string IdsGrupo { get; set; }

        public ObterResponsaveisPorGrupoCommand(string idsGrupo)
        {
            IdsGrupo = idsGrupo;
        }

        public class ObterResponsaveisPorGrupoCommandHandler : IRequestHandler<ObterResponsaveisPorGrupoCommand, IEnumerable<string>>
        {
            private readonly INotificacaoRepository _repository;

            private readonly IGrupoComunicadoRepository _grupoComunicadoRepository;

            private readonly IUsuarioRepository _usuarioRepository;

            public ObterResponsaveisPorGrupoCommandHandler(
                INotificacaoRepository repository, IGrupoComunicadoRepository grupo,
               IUsuarioRepository usuario
            )
            {
                _repository = repository;
                _grupoComunicadoRepository = grupo;
                _usuarioRepository = usuario;
            }

            public async Task<IEnumerable<string>> Handle(
                ObterResponsaveisPorGrupoCommand request, CancellationToken cancellationToken)
            {
                string filtroTipoCiclo = "";
                string filtroEtapaEnsinoId = "";
                string filtroTipoEscola = "";

                var grupos = await _grupoComunicadoRepository.ObterPorIds(request.IdsGrupo);

                var tipoCiclo = grupos.Select(x => x.TipoCicloId).Distinct().Aggregate((i, j) => i + "," + j);
                var etapaEnsinoId = grupos.Select(x => x.EtapaEnsinoId).Distinct().Aggregate((i, j) => i + "," + j);
                var tipoEscola = grupos.Select(x => x.TipoEscolaId).Distinct().Aggregate((i, j) => i + "," + j);

                if (tipoCiclo != null)
                    filtroTipoCiclo = $" and se.cd_ciclo_ensino in ({tipoCiclo})";

                if (etapaEnsinoId != null)
                    filtroEtapaEnsinoId = $" and se.cd_etapa_ensino in ({etapaEnsinoId})";

                if (tipoEscola != null)
                    filtroTipoEscola = $" and esc.tp_escola in ({tipoEscola})";

                var where = filtroTipoCiclo + filtroEtapaEnsinoId + filtroTipoEscola;

                var responsaveisEol = await _repository.ObterResponsaveisPorGrupo(where);
                responsaveisEol.Distinct();

                var usuariosAPP = await _usuarioRepository.ObterTodos();

                return responsaveisEol.Where(x => usuariosAPP.Contains(x)).ToList();
            }
        }
    }
}
