﻿using MediatR;
using Sentry;
using SME.AE.Aplicacao.Comandos.Usuario.SalvarUsuario;
using SME.AE.Aplicacao.Comandos.Usuario.ValidarAlunoInativoRestrito;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Servicos;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.Usuario;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Usuario;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using SME.AE.Aplicacao.Consultas.ObterUsuarioCoreSSO;
using SME.AE.Aplicacao.Servicos;
using SME.AE.Comum.Excecoes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso
{
    public class SolicitarRedifinicaoSenhaUseCase : ISolicitarRedifinicaoSenhaUseCase
    {
        private readonly IMediator mediator;
        private readonly IEmailServico emailServico;

        public SolicitarRedifinicaoSenhaUseCase(IMediator mediator, IEmailServico emailServico)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.emailServico = emailServico ?? throw new ArgumentNullException(nameof(emailServico));
        }

        public async Task<RespostaApi> Executar(GerarTokenDto gerarTokenDto)
        {
            var usuario = await mediator.Send(new ObterUsuarioQuery(gerarTokenDto.CPF));

            if (usuario == null)
                throw new NegocioException("Este CPF não existe na base do Escola Aqui. Você deve realizar o login utilizando a senha padrão.");

            if (string.IsNullOrWhiteSpace(usuario.Email))
                throw new NegocioException("Usuário não possui e-mail cadastrado");

            var usuarioCoreSSO = await mediator.Send(new ObterUsuarioCoreSSOQuery(usuario.Cpf));

            if(usuarioCoreSSO == null)
                throw new NegocioException("Este CPF não existe na base do Escola Aqui. Você deve realizar o login utilizando a senha padrão.");

            usuarioCoreSSO.Cpf = usuario.Cpf;

            await mediator.Send(new ValidarAlunoInativoRestritoCommand(usuarioCoreSSO));

            usuario.InicarRedefinicaoSenha();

            await EnvioEmail(usuario);

            await mediator.Send(new SalvarUsuarioCommand(usuario));

            return RespostaApi.Sucesso(usuario.Email);
        }

        private async Task EnvioEmail(Dominio.Entidades.Usuario usuario)
        {
            try
            { 
                string caminho = $"{Directory.GetCurrentDirectory()}/wwwroot/ModelosEmail/RecuperacaoSenha.html";
                var textoArquivo = await File.ReadAllTextAsync(caminho);
                var urlFrontEnd = VariaveisAmbiente.UrlArquivosEstaticos;
                var textoEmail = textoArquivo
                    .Replace("#NOME", usuario.Nome)
                    .Replace("#CODIGO", usuario.Token)
                    .Replace("#URL_BASE#", urlFrontEnd)
                    .Replace("#VALIDADE", usuario.ValidadeToken?.ToString("dd/MM/yyyy HH:mm"));

                emailServico.Enviar(usuario.Nome, usuario.Email, "Redefinição de Senha Escola Aqui", textoEmail);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw new NegocioException("Não foi possivel realizar o envio de email, por favor contate o suporte");
            }
        }
    }
}
