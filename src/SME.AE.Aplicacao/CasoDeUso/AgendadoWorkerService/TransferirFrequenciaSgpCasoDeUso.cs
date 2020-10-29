using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso
{
    public class TransferirFrequenciaSgpCasoDeUso
    {
        private readonly IFrequenciaAlunoRepositorio frequenciaAlunoRepositorio;
        private readonly IFrequenciaAlunoSgpRepositorio frequenciaAlunoSgpRepositorio;

        public TransferirFrequenciaSgpCasoDeUso(IFrequenciaAlunoRepositorio frequenciaAlunoRepositorio, IFrequenciaAlunoSgpRepositorio frequenciaAlunoSgpRepositorio)
        {
            this.frequenciaAlunoRepositorio = frequenciaAlunoRepositorio ?? throw new ArgumentNullException(nameof(frequenciaAlunoRepositorio));
            this.frequenciaAlunoSgpRepositorio = frequenciaAlunoSgpRepositorio ?? throw new ArgumentNullException(nameof(frequenciaAlunoSgpRepositorio));
        }

        public async Task ExecutarAsync()
        {
            IEnumerable<FrequenciaAlunoSgpDto> frequenciaAlunosSgp = await frequenciaAlunoSgpRepositorio.ObterFrequenciaAlunoSgp();
            await frequenciaAlunoRepositorio.SalvarFrequenciaAlunosBatch(frequenciaAlunosSgp);
        }
    }
}
