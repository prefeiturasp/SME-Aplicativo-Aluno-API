using MediatR;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.NotasDoAluno;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Consultas.ObterUltimaAtualizacaoPorProcesso
{
    public class ObterNotaAlunoCoresQuery : IRequest<IEnumerable<NotaAlunoCor>>
    {
        public ObterNotaAlunoCoresQuery()
        {
        }
    }
}
