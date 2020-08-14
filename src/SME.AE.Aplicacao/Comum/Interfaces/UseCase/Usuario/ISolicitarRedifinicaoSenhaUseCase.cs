using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Usuario;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.UseCase.Usuario
{
    public interface ISolicitarRedifinicaoSenhaUseCase
    {
        Task<RespostaApi> Executar(GerarTokenDto gerarTokenDto);
    }
}
