using MediatR;
using SME.AE.Dominio.Entidades;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Consultas.ObterUsuario
{
    public class ObterDadosAlunosQuery : IRequest<IEnumerable<Aluno>>
    {

        public ObterDadosAlunosQuery(string cpf)
        {
            Cpf = cpf;
        }

        public string Cpf { get; set; }
    }
}
