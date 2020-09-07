using MediatR;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Consultas.ObterUsuario
{
    public class ObterDadosAlunosQuery : IRequest<IEnumerable<AlunoRespostaEol>>
    {

        public ObterDadosAlunosQuery(string cpf)
        {
            Cpf = cpf;
        }

        public string Cpf { get; set; }
    }
}
