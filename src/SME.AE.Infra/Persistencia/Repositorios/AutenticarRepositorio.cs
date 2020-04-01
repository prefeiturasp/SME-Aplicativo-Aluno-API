using System;
using System.Linq;
using Dapper;
using SME.AE.Aplicacao.Autenticacao.Queries;
using SME.AE.Infra.Persistencia.Queries;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class AutenticarRepositorio
    {
        private readonly EolDataContext _context;
        public AutenticarRepositorio(EolDataContext context)
        {
            _context = context;
        }


        public AutenticacaoQueryResult BuscarAlunoPorResponsavel(string cpfResponsavel, DateTime dataNascimentoAluno)
        {
            return
           _context
           .Connection
           .Query<AutenticacaoQueryResult>(
               QueriesAutenticar.ObterAlunosDoResponsavel,
               new
               {
                   cpfResponsavel,
                   dataNascimentoAluno
               })
               .FirstOrDefault();
        }
    }
}
