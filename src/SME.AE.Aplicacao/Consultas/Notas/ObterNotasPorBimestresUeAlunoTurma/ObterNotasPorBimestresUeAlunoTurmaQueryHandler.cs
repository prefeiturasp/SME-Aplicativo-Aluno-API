using MediatR;
using Newtonsoft.Json;
using Sentry;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.NotasDoAluno;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class ObterNotasPorBimestresUeAlunoTurmaQueryHandler : IRequestHandler<ObterNotasPorBimestresUeAlunoTurmaQuery, IEnumerable<NotaConceitoBimestreComponenteDto>>
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly INotaAlunoCorRepositorio _notaAlunoCorRepositorio;

        public ObterNotasPorBimestresUeAlunoTurmaQueryHandler(IHttpClientFactory httpClientFactory, INotaAlunoCorRepositorio notaAlunoCorRepositorio)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _notaAlunoCorRepositorio = notaAlunoCorRepositorio ?? throw new System.ArgumentNullException(nameof(notaAlunoCorRepositorio));
        }

        public async Task<IEnumerable<NotaConceitoBimestreComponenteDto>> Handle(ObterNotasPorBimestresUeAlunoTurmaQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<NotaConceitoBimestreComponenteDto> notasConceitos;

            var httpClient = httpClientFactory.CreateClient("servicoApiSgp");
            var resposta = await httpClient.GetAsync($"v1/avaliacoes/notas/ues/{request.UeCodigo}/turmas/{request.TurmaCodigo}/alunos/{request.AlunoCodigo}?bimestres={string.Join("&bimestres=", request.Bimestres)}");
            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                notasConceitos = JsonConvert.DeserializeObject<IEnumerable<NotaConceitoBimestreComponenteDto>>(json);

                notasConceitos = await AtualizarCores(notasConceitos);
            }
            else
            {                
                throw new Exception($"Não foi possível localizar as notas do aluno : {request.AlunoCodigo} da turma {request.TurmaCodigo}.");
            }

            return notasConceitos;
        }

        private async Task<IEnumerable<NotaConceitoBimestreComponenteDto>> AtualizarCores(IEnumerable<NotaConceitoBimestreComponenteDto> notasConceitos)
        {
            var notaAlunoCores = await _notaAlunoCorRepositorio.ObterAsync();

            foreach (var notaAluno in notasConceitos)
            {
                notaAluno.CorDaNota = decimal.TryParse(notaAluno.NotaConceito, out var notaEmValor)
                    ? DefinirCorDaNotaPorValor(notaEmValor, notaAlunoCores)
                    : DefinirCorDaNotaPorConceito(notaAluno.NotaConceito, notaAlunoCores);
            }

            return notasConceitos;
        }

        private string DefinirCorDaNotaPorValor(decimal nota, IEnumerable<NotaAlunoCor> notaAlunoCores)
        {
            string cor = null;
            switch (nota)
            {
                case decimal n when (n < 5.00m):
                    cor = notaAlunoCores.FirstOrDefault(x => x.Nota == NotaAlunoCor.NotaAbaixo5)?.Cor;
                    break;

                case decimal n when (n <= 6.99m && n >= 5.00m):
                    cor = notaAlunoCores.FirstOrDefault(x => x.Nota == NotaAlunoCor.NotaEntre7e5)?.Cor;
                    break;

                case decimal n when (n >= 7.00m):
                    cor = notaAlunoCores.FirstOrDefault(x => x.Nota == NotaAlunoCor.NotaAcimaDe7)?.Cor;
                    break;
            }

            return cor ?? NotaAlunoCor.CorPadrao;
        }

        private string DefinirCorDaNotaPorConceito(string conceito, IEnumerable<NotaAlunoCor> notaAlunoCores)
        {
            var notaAlunoCor = notaAlunoCores.FirstOrDefault(x => x.Nota.ToUpper() == conceito.ToUpper());
            return notaAlunoCor?.Cor ?? NotaAlunoCor.CorPadrao;
        }
    }
}
