using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
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
        private readonly IMediator mediator;
        private readonly IUsuarioRepository usuarioRepository;
        private readonly INotificacaoRepositorio notificacaoRepository;

        public ObterDadosLeituraAlunosQueryHandler(IDadosLeituraRepositorio dadosLeituraRepositorio,
                                                    IUsuarioRepository usuarioRepository,
                                                    INotificacaoRepositorio notificacaoRepository,
                                                    IMediator mediator)
        {
            this.dadosLeituraRepositorio = dadosLeituraRepositorio ?? throw new System.ArgumentNullException(nameof(dadosLeituraRepositorio));
            this.usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
            this.notificacaoRepository = notificacaoRepository ?? throw new ArgumentNullException(nameof(notificacaoRepository));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<DadosLeituraAlunosComunicado>> Handle(ObterDadosLeituraAlunosQuery request, CancellationToken cancellationToken)
        {
            var alunosTurma = await mediator.Send(new ObterAlunosPorTurmaQuery(request.CodigoTurma));

            var notificacaoAlunos = await notificacaoRepository.ObterNotificacoesAlunoPorId(request.NotificaoId);

            if (!alunosTurma.Any())
                throw new NegocioException("Não foi possível localizar alunos para a turma informada");
            string codigosAlunos = "";

            if (notificacaoAlunos != null && notificacaoAlunos.Any())
                codigosAlunos = string.Join(',', notificacaoAlunos.Select(at => at.CodigoAluno).ToArray());
            else
                codigosAlunos = string.Join(',', alunosTurma.Select(at => at.CodigoEOLAluno).ToArray());

            var dadosLeituraComunicados = await dadosLeituraRepositorio.ObterDadosLeituraAlunos(request.NotificaoId, codigosAlunos);

            if (notificacaoAlunos != null && notificacaoAlunos.Any())
            {
                List<long> IDS = notificacaoAlunos.Select(aluno => aluno.CodigoAluno).ToList();
                var dadosLeituraAlunos = alunosTurma.Where(a => IDS.Contains(a.CodigoEOLAluno)).Select(aluno =>
                {
                    var dataleitura = dadosLeituraComunicados
                    .Where(dados => dados.CodigoAluno == aluno.CodigoEOLAluno)
                    .Select(dados => dados.DataLeitura)
                    .FirstOrDefault();

                    var cpf = aluno.Cpf.ToString("00000000000");

                    var usuario = usuarioRepository.ObterPorCpf(cpf).Result;
                    var possueApp = usuario != null;
                    var usuarioEol = (mediator.Send(new ObterDadosResponsavelResumidoQuery(cpf))).Result;
                    var telefone = possueApp ? usuarioEol.NumeroCelular : "";
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
            else
            {
                var dadosLeituraAlunos = alunosTurma
                    .Select(aluno =>
                    {
                        var dataleitura = dadosLeituraComunicados
                            .Where(dados => dados.CodigoAluno == aluno.CodigoEOLAluno)
                            .Select(dados => dados.DataLeitura)
                            .FirstOrDefault();

                        var cpf = aluno.Cpf.ToString("00000000000");

                        var usuario = usuarioRepository.ObterPorCpf(cpf).Result;
                        var possueApp = usuario != null;
                        var usuarioEol = (mediator.Send(new ObterDadosResponsavelResumidoQuery(cpf))).Result;
                        var telefone = possueApp ? usuarioEol.NumeroCelular : "";
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
