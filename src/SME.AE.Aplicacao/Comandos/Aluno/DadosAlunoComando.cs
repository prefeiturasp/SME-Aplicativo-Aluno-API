using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.Aluno
{
    public class DadosAlunoCommand : IRequest<IEnumerable<Dominio.Entidades.Aluno>>
    {
        public DadosAlunoCommand(string cpf)
        {
            Cpf = cpf;

        }

        public string Cpf { get; set; }

        public class DadosAlunoComandoHandler : IRequestHandler<DadosAlunoCommand,  IEnumerable<Dominio.Entidades.Aluno>>
        {
            private readonly IAlunoRepositorio _repository;

            public DadosAlunoComandoHandler(IAlunoRepositorio repository)
            {
                _repository = repository;
            }
            public async Task<IEnumerable<Dominio.Entidades.Aluno>> Handle
             (DadosAlunoCommand request, CancellationToken cancellationToken)
            {
                var resultado = await _repository.ObterDadosAlunos(request.Cpf);
                return resultado;
            }

        }
    }
}
