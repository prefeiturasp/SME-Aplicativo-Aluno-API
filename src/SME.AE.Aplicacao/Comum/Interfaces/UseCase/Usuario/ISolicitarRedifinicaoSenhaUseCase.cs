﻿using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Usuario;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.UseCase.Usuario
{
    public interface ISolicitarRedifinicaoSenhaUseCase
    {
        Task<RespostaApi> Executar(GerarTokenDto gerarTokenDto);
    }
}
