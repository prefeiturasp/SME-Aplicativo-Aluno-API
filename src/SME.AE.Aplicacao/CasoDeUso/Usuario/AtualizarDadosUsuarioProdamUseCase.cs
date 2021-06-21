using MediatR;
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
            var atualizarDadosUsuario = mensagemRabbit.ObterObjetoMensagem<AtualizarDadosUsuarioDto>();

            var usuarioApp = await mediator.Send(new ObterUsuarioQuery(atualizarDadosUsuario.Id));

            if (usuarioApp == null)
                return false;

            var usuariosEol = await mediator.Send(new ObterDadosReponsavelPorCpfQuery(usuarioApp.Cpf));

            if (usuariosEol == null || !usuariosEol.Any())
                return false;

            MapearAlteracoes(usuariosEol, atualizarDadosUsuario);

            foreach (var usuarioEol in usuariosEol)
            {
                await mediator.Send(new EnviarAtualizacaoCadastralProdamCommand(usuarioEol));
            }

            return true;
        }

        private void MapearAlteracoes(IEnumerable<ResponsavelAlunoDetalhadoEolDto> lstUsuarioEol, AtualizarDadosUsuarioDto dto)
        {
            var ddd = dto.Celular.Substring(0, 2);
            var celular = dto.Celular[2..];

            foreach (var usuarioEol in lstUsuarioEol)
            {
                usuarioEol.DataNascimentoMae = dto.DataNascimentoResponsavel.ToString("yyyyMMdd");
                usuarioEol.NomeMae = dto.NomeMae;
                usuarioEol.Email = dto.Email;
                usuarioEol.NumeroCelular = celular;
                usuarioEol.DDDCelular = ddd;
            }
        }
    }
}
