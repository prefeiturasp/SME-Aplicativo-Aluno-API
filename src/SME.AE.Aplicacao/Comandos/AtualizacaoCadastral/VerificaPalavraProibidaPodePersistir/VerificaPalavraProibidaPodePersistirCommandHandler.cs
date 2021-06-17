﻿using MediatR;
using Newtonsoft.Json;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class VerificaPalavraProibidaPodePersistirCommandHandler : IRequestHandler<VerificaPalavraProibidaPodePersistirCommand, bool>
    {
        private readonly IMediator mediator;
        private readonly ICacheRepositorio cacheRepositorio;

        public VerificaPalavraProibidaPodePersistirCommandHandler(IMediator mediator, ICacheRepositorio cacheRepositorio)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.cacheRepositorio = cacheRepositorio ?? throw new ArgumentNullException(nameof(cacheRepositorio));
        }

        public async Task<bool> Handle(VerificaPalavraProibidaPodePersistirCommand request, CancellationToken cancellationToken)
        {
            var palavrasBloqueadas = new string[] { };

            var chaveCache = $"palavras-bloqueadas";
            var cachePalavrasBloqueadas = cacheRepositorio.Obter(chaveCache);

            if(cachePalavrasBloqueadas == null)
            {
                palavrasBloqueadas = await mediator.Send(new ObterPalavrasProibidasQuery());

                if (palavrasBloqueadas == null || !palavrasBloqueadas.Any())
                    return true;

                await cacheRepositorio.SalvarAsync(chaveCache, palavrasBloqueadas);                
            }
            else
                palavrasBloqueadas = JsonConvert.DeserializeObject<string[]>(cachePalavrasBloqueadas);

            var palavrasTratadas = request.Nome.ToLower();

            var palavrasSeparadas = palavrasTratadas.Split(new char[] { '.', '@', ' ' }, StringSplitOptions.None);            

            var listaPalavrasBloqueadas = palavrasBloqueadas.Select(i => i.ToLower()).ToList();            

            var resultado = palavrasSeparadas.FirstOrDefault(t => listaPalavrasBloqueadas.Contains(t));

            return string.IsNullOrEmpty(resultado);
        }
    }
}
