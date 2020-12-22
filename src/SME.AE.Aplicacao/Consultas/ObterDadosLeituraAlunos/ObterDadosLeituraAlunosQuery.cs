using MediatR;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Consultas.ObterDadosLeituraComunicados
{
    public class ObterDadosLeituraAlunosQuery : IRequest<IEnumerable<DadosLeituraAlunosComunicado>>
    {
        public ObterDadosLeituraAlunosQuery(long notificaoId, long codigoTurma)
        {
            NotificaoId = notificaoId;
            CodigoTurma = codigoTurma;
        }

        public long NotificaoId { get; set; }
        public long CodigoTurma { get; set; }
    }
}
