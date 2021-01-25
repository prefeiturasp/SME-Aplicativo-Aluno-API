using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Comum.Excecoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.ObterDadosLeituraComunicados
{
    public class ObterDadosLeituraAlunosQueryHandler : IRequestHandler<ObterDadosLeituraAlunosQuery, IEnumerable<DadosLeituraAlunosComunicado>>
    {
        private readonly IDadosLeituraRepositorio dadosLeituraRepositorio;
        private readonly IAlunoRepositorio alunoRepositorio;
        private readonly IUsuarioRepository usuarioRepository;

        public ObterDadosLeituraAlunosQueryHandler(IDadosLeituraRepositorio dadosLeituraRepositorio, IAlunoRepositorio alunoRepositorio, IUsuarioRepository usuarioRepository)
        {
            this.dadosLeituraRepositorio = dadosLeituraRepositorio ?? throw new System.ArgumentNullException(nameof(dadosLeituraRepositorio));
            this.alunoRepositorio = alunoRepositorio ?? throw new ArgumentNullException(nameof(alunoRepositorio));
            this.usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        }

        public async Task<IEnumerable<DadosLeituraAlunosComunicado>> Handle(ObterDadosLeituraAlunosQuery request, CancellationToken cancellationToken)
        {
            var alunosTurma = await alunoRepositorio.ObterAlunosTurma(request.CodigoTurma);

            if (!alunosTurma.Any())
                throw new NegocioException("Não foi possível localizar alunos para a turma informada");

            var codigosAlunos = string.Join(',',
                alunosTurma
                .Select(at => at.CodigoEOLAluno)
                .ToArray());

            var dadosLeituraComunicados = await dadosLeituraRepositorio
                    .ObterDadosLeituraAlunos(request.NotificaoId, codigosAlunos);

            var dadosLeituraAlunos = alunosTurma
                .Select(aluno =>
                {
                    var dataleitura = dadosLeituraComunicados
                        .Where(dados => dados.CodigoAluno == aluno.CodigoEOLAluno)
                        .Select(dados => dados.DataLeitura)
                        .FirstOrDefault();

                    var usuario = usuarioRepository.ObterPorCpf(aluno.Cpf.ToString("00000000000")).Result;
                    var possueApp = usuario != null;
                    var telefone = possueApp ? usuario.Celular : "";
                    if (string.IsNullOrWhiteSpace(telefone))
                        if (!string.IsNullOrEmpty(aluno.DDDCelular) && !string.IsNullOrEmpty(aluno.Celular))
                        {
                            telefone = $"{aluno.DDDCelular.Trim()}{aluno.Celular.Trim()}";
                        }
                    return new DadosLeituraAlunosComunicado
                    {
                        CodigoAluno = aluno.CodigoEOLAluno,
                        LeuComunicado = dataleitura.HasValue,
                        DataLeitura = dataleitura,
                        NomeAluno = $"{aluno.NomeAluno.Trim()} ({aluno.CodigoEOLAluno})",
                        NomeResponsavel = $"{aluno.NomeResponsavel.Trim()} ({TipoFiliacao(aluno.TipoResponsavel)})",
                        NumeroChamada = short.Parse(aluno.NumeroChamada ?? "0"),
                        PossueApp = possueApp,
                        TelefoneResponsavel = telefone,
                        SituacaoAluno = aluno.SituacaoAluno,
                        DataSituacaoAluno = aluno.DataSituacaoAluno
                    };
                });

            return dadosLeituraAlunos;
        }

        string TipoFiliacao(int tipoResponsavel)
        {
            switch (tipoResponsavel)
            {
                case 1: return "Filiação 1";
                case 2: return "Filiação 2";
                case 3: return "Responsável Legal";
                case 4: return "Próprio estudante";
            }
            return "";
        }
    }
}
