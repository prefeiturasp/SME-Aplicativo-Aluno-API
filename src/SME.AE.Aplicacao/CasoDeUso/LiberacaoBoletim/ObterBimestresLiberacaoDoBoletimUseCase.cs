﻿using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces;
using SME.AE.Aplicacao.Consultas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso
{
    public class ObterBimestresLiberacaoDoBoletimUseCase : IObterBimestresLiberacaoBoletimAlunoUseCase
    {
        private readonly IMediator mediator;

        public ObterBimestresLiberacaoDoBoletimUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<int[]> Executar(string turmaCodigo)
        {
            return await mediator.Send(new ObterBimestresLiberacaoBoletimQuery(turmaCodigo));
        }
    }
}

