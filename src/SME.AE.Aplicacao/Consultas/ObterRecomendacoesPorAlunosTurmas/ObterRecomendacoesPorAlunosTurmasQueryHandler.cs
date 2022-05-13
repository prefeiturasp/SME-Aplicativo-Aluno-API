using MediatR;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Comum.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.ObterRecomendacoesPorAlunosTurmas
{
    public class ObterRecomendacoesPorAlunosTurmasQueryHandler : IRequestHandler<ObterRecomendacoesPorAlunosTurmasQuery, IEnumerable<RecomendacaoConselhoClasseAluno>>
    {
        private readonly IAlunoRepositorio alunoRepositorio;
        public ObterRecomendacoesPorAlunosTurmasQueryHandler(IAlunoRepositorio alunoRepositorio)
        {
            this.alunoRepositorio = alunoRepositorio ?? throw new ArgumentNullException(nameof(alunoRepositorio));
        }
        public async Task<IEnumerable<RecomendacaoConselhoClasseAluno>> Handle(ObterRecomendacoesPorAlunosTurmasQuery request, CancellationToken cancellationToken)
        {
            var recomendacoes = await alunoRepositorio.ObterRecomendacoesPorAlunoTurma(request.CodigoAluno, request.CodigoTurma, request.AnoLetivo, request.Modalidade, request.Semestre);
            if (recomendacoes == null && recomendacoes.Any())
            {
                var recomendacoesGeral = await alunoRepositorio.ObterRecomendacoesGeral();
                foreach (var recomendacao in recomendacoes)
                {
                    recomendacao.RecomendacoesAluno = recomendacao?.RecomendacoesAluno ?? MontaTextUlLis(recomendacoesGeral.Where(a => a.Tipo == ConselhoClasseRecomendacaoTipo.Aluno).Select(b => b.Recomendacao));
                    recomendacao.RecomendacoesFamilia = recomendacao?.RecomendacoesFamilia ?? MontaTextUlLis(recomendacoesGeral.Where(a => a.Tipo == ConselhoClasseRecomendacaoTipo.Familia).Select(b => b.Recomendacao));
                }
            }
            await FormatarRecomendacoes(recomendacoes);

            return recomendacoes;
        }


        private async Task FormatarRecomendacoes(IEnumerable<RecomendacaoConselhoClasseAluno> recomendacoesConselho)
        {
            foreach (var recomendacao in recomendacoesConselho)
            {
                var recomendacoesDoAluno = await alunoRepositorio.ObterRecomendacoesAlunoFamiliaPorAlunoETurma(recomendacao.AlunoCodigo, recomendacao.TurmaCodigo);

                var concatenaRecomendacaoAluno = new StringBuilder();
                foreach (var aluno in recomendacoesDoAluno.Where(r => r.Tipo == (int)ConselhoClasseRecomendacaoTipo.Aluno).ToList())
                    concatenaRecomendacaoAluno.AppendLine("- " + aluno.Recomendacao);

                concatenaRecomendacaoAluno.AppendLine("<br/>");
                concatenaRecomendacaoAluno.AppendLine(UtilHtml.FormatarHtmlParaTexto(recomendacao.RecomendacoesAluno));

                var concatenaRecomendacaoFamilia = new StringBuilder();
                foreach (var aluno in recomendacoesDoAluno.Where(r => r.Tipo == (int)ConselhoClasseRecomendacaoTipo.Familia).ToList())
                    concatenaRecomendacaoFamilia.AppendLine("- " + aluno.Recomendacao);

                concatenaRecomendacaoFamilia.AppendLine("<br/>");
                concatenaRecomendacaoFamilia.AppendLine(UtilHtml.FormatarHtmlParaTexto(recomendacao.RecomendacoesFamilia));


                recomendacao.AnotacoesPedagogicas = UtilHtml.FormatarHtmlParaTexto(recomendacao.AnotacoesPedagogicas);
                recomendacao.RecomendacoesAluno = concatenaRecomendacaoAluno.ToString();
                recomendacao.RecomendacoesFamilia = concatenaRecomendacaoFamilia.ToString();
            }

        }

        private string MontaTextUlLis(IEnumerable<string> textos)
        {
            var str = new StringBuilder();

            foreach (var item in textos)
            {
                str.AppendFormat(item);
            }

            return str.ToString().Trim();
        }
    }
}
