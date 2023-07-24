using MediatR;
using Microsoft.Practices.ObjectBuilder2;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using SME.AE.Comum.Excecoes;
using System;
using System.Collections.Generic;
using System.Linq;
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
            private readonly IAlunoRepositorio alunoRepositorio;
            private readonly IMediator mediator;

            public DadosAlunoComandoHandler(IAlunoRepositorio alunoRepositorio, IMediator mediator)
            {
                this.alunoRepositorio = alunoRepositorio ?? throw new ArgumentNullException(nameof(alunoRepositorio));
                this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            }
            public async Task<RespostaApi> Handle(DadosAlunoCommand request, CancellationToken cancellationToken)
            {
                var dadosDosAlunos = await mediator.Send(new ObterDadosAlunosQuery(request.Cpf, null, null, null));
                
                if (dadosDosAlunos == null || !dadosDosAlunos.Any())
                    throw new NegocioException("Este CPF não está relacionado como responsável de um aluno ativo na rede municipal.");

                var turmasCodigo = dadosDosAlunos.Select(a => a.CodigoTurma.ToString())
                                                 .Distinct()
                                                 .ToArray();

                var turmasModalidade = await mediator.Send(new ObterTurmasModalidadesPorCodigosQuery(turmasCodigo));

                dadosDosAlunos.ForEach(dadoDoAluno =>
                {
                    var modalidadeDaTurma = turmasModalidade.FirstOrDefault(a => a.TurmaCodigo == dadoDoAluno.CodigoTurma);
                    dadoDoAluno.ModalidadeCodigo = modalidadeDaTurma.ModalidadeCodigo;
                    dadoDoAluno.ModalidadeDescricao = modalidadeDaTurma.ModalidadeDescricao;
                });

                var tipoEscola =
                    dadosDosAlunos
                    .GroupBy(g => new { g.ModalidadeCodigo, g.ModalidadeDescricao })
                    .Select(s => new ListaEscola
                    {
                        Modalidade = s.Key.ModalidadeDescricao,
                        ModalidadeCodigo = s.Key.ModalidadeCodigo,
                        Alunos = dadosDosAlunos
                                .Where(w => w.ModalidadeCodigo == s.Key.ModalidadeCodigo)
                                .Select(a => new Dominio.Entidades.Aluno
                                {
                                    CodigoEol = a.CodigoEol,
                                    Nome = a.Nome,
                                    NomeResponsavel = a.TipoResponsavel == TipoResponsavelEnum.Proprio_Aluno &&
                                                        !string.IsNullOrWhiteSpace(a.NomeSocial) ?
                                                        a.NomeSocial.Trim() :
                                                        a.NomeResponsavel.Trim(),
                                    CpfResponsavel = a.CpfResponsavel,
                                    NomeSocial = a.NomeSocial,
                                    DataNascimento = a.DataNascimento.Date,
                                    CodigoTipoEscola = a.CodigoTipoEscola,
                                    CodigoEscola = a.CodigoEscola,
                                    DescricaoTipoEscola = a.DescricaoTipoEscola,
                                    Escola = a.Escola,
                                    CodigoDre = a.CodigoDre,
                                    SiglaDre = a.SiglaDre,
                                    CodigoTurma = a.CodigoTurma,
                                    Turma = a.Turma,
                                    SituacaoMatricula = a.SituacaoMatricula,
                                    DataSituacaoMatricula = a.DataSituacaoMatricula,
                                    SerieResumida = a.SerieResumida
                                })
                    });

                return RespostaApi.Sucesso(tipoEscola);
            }
        }
    }
}
