using FluentValidation.Results;
using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.Aluno
{
    public class DadosAlunoCommand : IRequest<RespostaApi>
    {
        public DadosAlunoCommand(string cpf)
        {
            Cpf = cpf;

        }

        public string Cpf { get; set; }

        public class DadosAlunoComandoHandler : IRequestHandler<DadosAlunoCommand, RespostaApi>
        {
            private readonly IAlunoRepositorio _repository;

            public DadosAlunoComandoHandler(IAlunoRepositorio repository)
            {
                _repository = repository;
            }
            public async Task<RespostaApi> Handle
             (DadosAlunoCommand request, CancellationToken cancellationToken)
            {
                var validator = new DadosAlunoUseCaseValidation();
                ValidationResult validacao = validator.Validate(request);
                
                if (!validacao.IsValid)
                    return RespostaApi.Falha(validacao.Errors);

                var resultado = await _repository.ObterDadosAlunos(request.Cpf);

                if (resultado == null || !resultado.Any())
                {
                    validacao.Errors.Add(new ValidationFailure("Usuário", "Este CPF não está relacionado como responsável de um aluno ativo na rede municipal."));
                    return RespostaApi.Falha(validacao.Errors);
                }
                
                var tipoEscola =
                    resultado
                    .GroupBy(g => g.Grupo )
                    .Select(s => new ListaEscola
                    {
                        Grupo = s.Key,
                        Alunos = resultado
                                .Where(w => w.Grupo == s.Key)
                                .Select(a => new SME.AE.Dominio.Entidades.Aluno
                                {
                                    CodigoEol = a.CodigoEol,
                                    Nome = a.Nome,
                                    NomeSocial = a.NomeSocial,
                                    DataNascimento = a.DataNascimento.Date,
                                    CodigoTipoEscola = a.CodigoTipoEscola,
                                    DescricaoTipoEscola = a.DescricaoTipoEscola,
                                    Escola = a.Escola,
                                    SiglaDre = a.SiglaDre,
                                    Turma = a.Turma,
                                    SituacaoMatricula = a.SituacaoMatricula,
                                    DataSituacaoMatricula = a.DataSituacaoMatricula
                                })
                    });

                return RespostaApi.Sucesso(tipoEscola);
            }

        }
    }
}
