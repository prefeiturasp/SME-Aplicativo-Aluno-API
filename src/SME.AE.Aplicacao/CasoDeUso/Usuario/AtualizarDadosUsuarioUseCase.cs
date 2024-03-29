﻿using MediatR;
using SME.AE.Aplicacao.Comandos.Usuario.SalvarUsuario;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Consultas;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using SME.AE.Comum;
using SME.AE.Dominio.Entidades;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class AtualizarDadosUsuarioUseCase : IAtualizarDadosUsuarioUseCase
    {
        private readonly IMediator mediator;

        public AtualizarDadosUsuarioUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<RespostaApi> Executar(AtualizarDadosUsuarioDto usuarioDto)
        {
            var usuarioApp = await mediator.Send(new ObterUsuarioQuery(usuarioDto.Id));

            var usuarioEol = await mediator.Send(new ObterDadosResponsavelResumidoQuery(usuarioApp.Cpf));
            
            if (usuarioEol == null)
                return RespostaApi.Falha("Usuário não encontrado!");

            await AtualizaUsuario(usuarioApp, usuarioDto);

            return MapearResposta(usuarioApp);
        }

        private async Task AtualizaUsuario(Usuario usuarioApp, AtualizarDadosUsuarioDto usuarioDto)
        {
            usuarioApp.AtualizarAuditoria();
            await mediator.Send(new SalvarUsuarioCommand(usuarioApp));

            var correlacaoCodigo = Guid.NewGuid();

            await mediator.Send(new PublicarFilaAeCommand(RotasRabbitAe.RotaAtualizacaoCadastralEol, usuarioDto, correlacaoCodigo));
            await mediator.Send(new PublicarFilaAeCommand(RotasRabbitAe.RotaAtualizacaoCadastralProdam, usuarioDto, correlacaoCodigo));
        }
        private RespostaApi MapearResposta(Usuario usuarioApp)
        {
            var usuario = new RespostaAutenticar
            {
                Cpf = usuarioApp.Cpf,
                Id = usuarioApp.Id,
                UltimaAtualizacao = usuarioApp.AlteradoEm
            };

            return RespostaApi.Sucesso(usuario);
        }
    }
}
