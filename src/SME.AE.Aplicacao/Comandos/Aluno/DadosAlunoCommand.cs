using FluentValidation.Results;
using MediatR;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Comum.Excecoes;
using SME.AE.Dominio.Entidades;
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
            private readonly IGrupoComunicadoRepository _repositorioGrupoComunicado;

            public DadosAlunoComandoHandler(IAlunoRepositorio repository, IGrupoComunicadoRepository repositorioGrupoComunicado)
            {
                _repository = repository;
                _repositorioGrupoComunicado = repositorioGrupoComunicado;
            }
            public async Task<RespostaApi> Handle
             (DadosAlunoCommand request, CancellationToken cancellationToken)
            {
                var grupos = await _repositorioGrupoComunicado.ObterTodos();
                var resultado = await _repository.ObterDadosAlunos(request.Cpf);

                if (resultado == null || !resultado.Any())
                    throw new NegocioException("Este CPF não está relacionado como responsável de um aluno ativo na rede municipal.");

                resultado.ForEach(x => { var g = SelecionarGrupos(x.CodigoTipoEscola, x.CodigoEtapaEnsino, x.CodigoCicloEnsino, grupos); x.Grupo = g.gupo; x.CodigoGrupo = g.codigo; });

                var tipoEscola =
                    resultado
                    .GroupBy(g => new { g.Grupo, g.CodigoGrupo })
                    .Select(s => new ListaEscola
                    {
                        Grupo = s.Key.Grupo,
                        CodigoGrupo = s.Key.CodigoGrupo,
                        Alunos = resultado
                                .Where(w => w.CodigoGrupo == s.Key.CodigoGrupo)
                                .Select(a => new Dominio.Entidades.Aluno
                                {
                                    CodigoEol = a.CodigoEol,
                                    Nome = a.Nome,
                                    NomeResponsavel = a.TipoResponsavel == TipoResponsavelEnum.Proprio_Aluno &&
                                                        !string.IsNullOrWhiteSpace(a.NomeSocial) ?
                                                        a.NomeSocial :
                                                        a.NomeResponsavel,
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

            private (string gupo, long codigo) SelecionarGrupos(int? codigoTipoEscola, int codigoEtapaEnsino, int codigoCicloEnsino, IEnumerable<GrupoComunicado> grupos)
            {
                return grupos
                .Where(x => (x.TipoEscolaId != null
                    && x.TipoEscolaId.Split(',').Contains(codigoTipoEscola.Value.ToString()))
                || (x.TipoEscolaId == null
                    && x.TipoCicloId.Split(',').Contains(codigoCicloEnsino.ToString())
                    && x.EtapaEnsinoId.Split(',').Contains(codigoEtapaEnsino.ToString())))
                .Select(s => (s.Nome, s.Id))
                    .FirstOrDefault();
            }
        }
    }
}
