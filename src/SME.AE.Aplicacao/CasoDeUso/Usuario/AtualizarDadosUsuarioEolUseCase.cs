using MediatR;
using Sentry;
using SME.AE.Aplicacao.Comum.Interfaces;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using SME.AE.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class AtualizarDadosUsuarioEolUseCase : IAtualizarDadosUsuarioEolUseCase
    {
        private readonly IMediator mediator;

        public AtualizarDadosUsuarioEolUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Executar(MensagemRabbit mensagemRabbit)
        {
            var dto = mensagemRabbit.ObterObjetoMensagem<AtualizarDadosUsuarioDto>();

            if (dto == null)
            {
                SentrySdk.CaptureMessage($"Não foi possível realizar a atualização dos dados do responsável do aluno no eol", Sentry.Protocol.SentryLevel.Error);
            }

            var usuarioApp = await mediator.Send(new ObterUsuarioQuery(dto.Id));

            if (usuarioApp == null)
                return false;

            var usuariosEol = await mediator.Send(new ObterDadosReponsavelPorCpfQuery(usuarioApp.Cpf));

            if (usuariosEol == null || !usuariosEol.Any())
                return false;

            MapearAlteracoes(usuariosEol, dto);

            foreach (var usuarioEol in usuariosEol)
            {
                await mediator.Send(new AtualizarDadosResponsavelEolCommand(long.Parse(usuarioEol.CPF), usuarioEol.Email, dto.DataNascimentoResponsavel, usuarioEol.NomeMae, usuarioEol.NumeroCelular, usuarioEol.DDDCelular));
            }

            return true;
        }

        private void MapearAlteracoes(IEnumerable<ResponsavelAlunoDetalhadoEolDto> lstUsuarioEol, AtualizarDadosUsuarioDto dto)
        {
            foreach (var usuarioEol in lstUsuarioEol)
            {
                usuarioEol.NomeMae = dto.NomeMae;
                usuarioEol.Email = dto.Email;
                usuarioEol.NumeroCelular = dto.CelularResponsavel;
                usuarioEol.DDDCelular = dto.DDD;
            }
        }
    }
}
