﻿using MediatR;
using Microsoft.EntityFrameworkCore.Internal;
using Rocket.Core.Plugins.NuGet;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class ObterDadosUsuarioPorCpfUseCase : IObterDadosUsuarioPorCpfUseCase
    {
        private readonly IMediator mediator;

        public ObterDadosUsuarioPorCpfUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }
        public async Task<UsuarioDadosDetalhesDto> Executar(string cpf)
        {
            var responsavel = await mediator.Send(new ObterDadosResponsavelResumidoQuery(cpf));
            return new UsuarioDadosDetalhesDto()
            {
                Celular = responsavel.ObterCelularComDDD(),
                Cpf = responsavel.Cpf,
                DataNascimento = responsavel.DataNascimento,
                Email = responsavel.Email,
                Nome = responsavel.Nome,
                NomeMae = responsavel.NomeMae,
                UltimaAtualizacao = responsavel.DataAtualizacao
            };
        }
    }
}
