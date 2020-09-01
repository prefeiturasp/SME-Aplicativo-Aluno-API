using MediatR;
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
using SME.AE.Comum.Excecoes;
using System;
using System.IO;
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
            Dominio.Entidades.Usuario usuario = await ObterUsuario(gerarTokenDto);

            if (usuario == null)
                throw new NegocioException("Este CPF não está relacionado como responsável de um aluno ativo na rede municipal.");

            if (string.IsNullOrWhiteSpace(usuario.Email))
                throw new NegocioException("Usuário não possui e-mail cadastrado");

            var usuarioCoreSSO = await ObterUsuarioCoreSSO(gerarTokenDto);

            await mediator.Send(new ValidarAlunoInativoRestritoCommand(usuarioCoreSSO));

            usuario.InicarRedefinicaoSenha();

            await EnvioEmail(usuario);

            await mediator.Send(new SalvarUsuarioCommand(usuario));

            return RespostaApi.Sucesso(usuario.Email);
        }

        private async Task<Dominio.Entidades.Usuario> ObterUsuario(GerarTokenDto gerarTokenDto)
        {
            try
            {
                var usuario = await mediator.Send(new ObterUsuarioQuery(gerarTokenDto.CPF));

                //if (usuario == null)
                //    throw new NegocioException("Este CPF não existe na base do Escola Aqui. Você deve realizar o login utilizando a senha padrão.");

                return usuario;
            }
            catch (Exception)
            {
                throw new NegocioException("Este CPF não existe na base do Escola Aqui. Você deve realizar o login utilizando a senha padrão.");                
            }
        }

        private async Task<RetornoUsuarioCoreSSO> ObterUsuarioCoreSSO(GerarTokenDto gerarTokenDto)
        {
            var usuarioCoreSSO = await mediator.Send(new ObterUsuarioCoreSSOQuery(gerarTokenDto.CPF));

            if (usuarioCoreSSO == null)
                throw new NegocioException("Este CPF não existe na base do Escola Aqui. Você deve realizar o login utilizando a senha padrão.");

            usuarioCoreSSO.Cpf = gerarTokenDto.CPF;

            return usuarioCoreSSO;
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

                await emailServico.Enviar(usuario.Nome, usuario.Email, "Redefinição de Senha Escola Aqui", textoEmail);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw new NegocioException("Não foi possivel realizar o envio de email, por favor contate o suporte");
            }
        }
    }
}
