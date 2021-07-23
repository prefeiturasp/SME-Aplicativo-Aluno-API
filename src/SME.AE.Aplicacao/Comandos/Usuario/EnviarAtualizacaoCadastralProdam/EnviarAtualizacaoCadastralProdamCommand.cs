using MediatR;

namespace SME.AE.Aplicacao
{
    public class EnviarAtualizacaoCadastralProdamCommand : IRequest<bool>
    {
        public EnviarAtualizacaoCadastralProdamCommand(ResponsavelAlunoDetalhadoEolDto responsavelDto)
        {
            ResponsavelDto = responsavelDto;
        }

        public ResponsavelAlunoDetalhadoEolDto ResponsavelDto { get; set; }
    }
}
