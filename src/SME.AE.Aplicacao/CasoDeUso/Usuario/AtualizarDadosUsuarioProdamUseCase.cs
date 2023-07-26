using MediatR;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using SME.AE.Comum;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class AtualizarDadosUsuarioProdamUseCase : IAtualizarDadosUsuarioProdamUseCase
    {
        private readonly IMediator mediator;

        public AtualizarDadosUsuarioProdamUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Executar(MensagemRabbit mensagemRabbit)
        {
            var filtroDadosUsuario = mensagemRabbit.ObterObjetoMensagem<AtualizarDadosUsuarioDto>();

            var usuarioApp = await mediator.Send(new ObterUsuarioQuery(filtroDadosUsuario.Id));

            if (usuarioApp == null)
                return false;

            var usuariosEol = await mediator.Send(new ObterDadosResponsavelQuery(usuarioApp.Cpf));

            if (usuariosEol == null || !usuariosEol.Any())
                return false;

            MapearAlteracoes(usuariosEol, filtroDadosUsuario);

            foreach (var usuarioEol in usuariosEol)
            {
                await mediator.Send(new EnviarAtualizacaoCadastralProdamCommand(ResponsavelAlunoDetalhadoEolDto.Converter(usuarioEol)));
            }

            return true;
        }

        private void MapearAlteracoes(IEnumerable<DadosResponsavelAluno> lstUsuarioEol, AtualizarDadosUsuarioDto dto)
        {
            foreach (var usuarioEol in lstUsuarioEol)
            {
                usuarioEol.DataNascimentoMae = dto.DataNascimentoResponsavel;
                usuarioEol.NomeMae = dto.NomeMae;
                usuarioEol.Email = dto.Email;
                usuarioEol.NumeroCelular = dto.CelularResponsavel;
                usuarioEol.DDDCelular = dto.DDD;
                usuarioEol.Nome = usuarioEol.Nome.Trim();

                if (string.IsNullOrEmpty(usuarioEol.TipoTurnoTelefoneFixo))
                {
                    if (!string.IsNullOrEmpty(usuarioEol.NumeroTelefoneFixo) &&
                        !string.IsNullOrEmpty(usuarioEol.DDDTelefoneFixo))
                        usuarioEol.TipoTurnoTelefoneFixo = "1";
                    else
                        usuarioEol.TipoTurnoTelefoneFixo = "";
                }
            }
        }
    }
}
