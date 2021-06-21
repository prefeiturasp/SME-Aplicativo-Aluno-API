using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;

namespace SME.AE.Aplicacao.Comandos.GrupoNotificacao.ObterPorResponsavel
{
    public class ObterGrupoNotificacaoPorResponsavelCommand : IRequest<List<string>>
    {
        public string Cpf { get; set; }

        public ObterGrupoNotificacaoPorResponsavelCommand(string cpf)
        {
            Cpf = cpf;
        }

    }

    public class ObterGrupoNotificacaoPorResponsavelCommandHandler : IRequestHandler<ObterGrupoNotificacaoPorResponsavelCommand, List<string>>
    {
        private readonly INotificacaoRepository _repository;

        private readonly IGrupoComunicadoRepository _grupoComunicadoRepository;
        
        public ObterGrupoNotificacaoPorResponsavelCommandHandler(
            INotificacaoRepository repository, IGrupoComunicadoRepository grupo
        )
        {
            _repository = repository;
            _grupoComunicadoRepository = grupo;
        }

        public async Task<List<string>> Handle(
            ObterGrupoNotificacaoPorResponsavelCommand request, CancellationToken cancellationToken)
        {
            var grupos = await _grupoComunicadoRepository.ObterTodos();

            var query = string.Join("", (from g in grupos 
                where g.TipoCicloId != null
                select $"case when (se.cd_ciclo_ensino in ({g.TipoCicloId}) and se.cd_etapa_ensino IN ({g.EtapaEnsinoId})) then 1 else 0 end as \"{g.Nome}\","));

            query = query.Substring(0, query.Count() - 1);

            var nomeGrupos = string.Join("", (from n in grupos
                              where n.TipoCicloId != null
                              select $"max({n.Nome}){n.Nome},"));

            nomeGrupos = nomeGrupos.Substring(0, nomeGrupos.Count() - 1);

            var gruposDoResponsavel = await _repository.ObterGruposDoResponsavel(request.Cpf, query, nomeGrupos);
            
            if(gruposDoResponsavel == null)
                return new List<string>();

            string ids = "";

            foreach(var gr in gruposDoResponsavel.AsEnumerable())
            {
                if (1.Equals(gr.Value))
                {
                    var id = (
                        from g in grupos 
                        where g.Nome.Equals(gr.Key) 
                        select g.Id.ToString()
                        ).First();
                    ids += $"{id},";
                }
            }

            if (ids.Length > 0) 
                ids = ids.Substring(0, ids.Count() - 1);

            return ids.Split(",").ToList();
        }
    }
}