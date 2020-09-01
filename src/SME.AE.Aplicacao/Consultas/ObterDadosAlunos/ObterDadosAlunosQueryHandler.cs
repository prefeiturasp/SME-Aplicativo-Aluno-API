using MediatR;
using Microsoft.EntityFrameworkCore.Internal;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Comum.Excecoes;
using SME.AE.Dominio.Entidades;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.ObterUsuario
{
    public class ObterDadosAlunosQueryHandler : IRequestHandler<ObterDadosAlunosQuery, IEnumerable<Aluno>>
    {
        private readonly IAlunoRepositorio _alunoRepositorio;

        public ObterDadosAlunosQueryHandler(IAlunoRepositorio alunoRepositorio)
        {
            _alunoRepositorio = alunoRepositorio ?? throw new System.ArgumentNullException(nameof(alunoRepositorio));
        }

        public async Task<IEnumerable<Aluno>> Handle(ObterDadosAlunosQuery request, CancellationToken cancellationToken)
        {
            var alunos = await _alunoRepositorio.ObterDadosAlunos(request.Cpf);
            if (alunos == null || !alunos.Any())
                throw new NegocioException("Este CPF não está relacionado como responsável de um aluno ativo na rede municipal.");
            
            return (IEnumerable<Aluno>)alunos;
        }
    }
}
