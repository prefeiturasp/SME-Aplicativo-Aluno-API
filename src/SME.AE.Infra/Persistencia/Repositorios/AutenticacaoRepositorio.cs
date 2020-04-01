using System;
using System.Linq;
using Dapper;
using SME.AE.Aplicacao.Autenticacao.Queries;
using SME.AE.Infra.Persistencia.Consultas;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class AutenticacaoRepositorio
    {
        private readonly EolContextoDatabase _context;
        public AutenticacaoRepositorio(EolContextoDatabase context)
        {
            _context = context;
        }

        public AutenticacaoQueryResult BuscarAlunoPorResponsavel(string cpf, DateTime dataNascimentoAluno)
        {
            return
           _context
           .Connection
           .Query<AutenticacaoQueryResult>(
               AutenticacaoConsultas.ObterAlunosDoResponsavel,
               new
               {
                   cpfResponsavel = cpf,
                   dataNascimentoAluno
               })
               .FirstOrDefault();
        }
    }
}
