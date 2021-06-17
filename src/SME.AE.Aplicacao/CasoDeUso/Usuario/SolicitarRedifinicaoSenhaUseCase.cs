using MediatR;
using Sentry;
using SME.AE.Aplicacao.Comandos.Usuario.SalvarUsuario;
using SME.AE.Aplicacao.Comandos.Usuario.ValidarAlunoInativoRestrito;
using SME.AE.Aplicacao.Comum.Interfaces.Servicos;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.Usuario;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Usuario;
using SME.AE.Aplicacao.Consultas;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using SME.AE.Aplicacao.Consultas.ObterUsuarioCoreSSO;
using SME.AE.Comum;
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
        private readonly VariaveisGlobaisOptions variaveisGlobaisOptions;

        public SolicitarRedifinicaoSenhaUseCase(IMediator mediator, IEmailServico emailServico, VariaveisGlobaisOptions variaveisGlobaisOptions)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.emailServico = emailServico ?? throw new ArgumentNullException(nameof(emailServico));
            this.variaveisGlobaisOptions = variaveisGlobaisOptions ?? throw new ArgumentNullException(nameof(variaveisGlobaisOptions));
        }

        public async Task<RespostaApi> Executar(GerarTokenDto gerarTokenDto)
        {

            var usuario = await ObterUsuario(gerarTokenDto);

            var usuarioCoreSSO = await ObterUsuarioCoreSSO(gerarTokenDto);

            var usuarioEol = await mediator.Send(new ObterDadosResumidosReponsavelPorCpfQuery(usuario.Cpf));

            await mediator.Send(new ValidarAlunoInativoRestritoCommand(usuarioCoreSSO));

            usuario.IniciarRedefinicaoSenha();

            if (string.IsNullOrEmpty(usuarioEol.Email))
                throw new NegocioException("Usuário não possui e-mail cadastrado");

            await EnvioEmail(usuarioEol, usuario);

            await mediator.Send(new SalvarUsuarioCommand(usuario));

            return RespostaApi.Sucesso(usuarioEol.Email);
        }

        private async Task<Dominio.Entidades.Usuario> ObterUsuario(GerarTokenDto gerarTokenDto)
        {
            try
            {
                var usuario = await mediator.Send(new ObterUsuarioQuery(gerarTokenDto.CPF));
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

        private async Task EnvioEmail(ResponsavelAlunoEolResumidoDto usuarioEol, Dominio.Entidades.Usuario usuario)
        {
            try
            {
                string caminho = $"{Directory.GetCurrentDirectory()}/wwwroot/ModelosEmail/RecuperacaoSenha.html";
                var textoArquivo = await File.ReadAllTextAsync(caminho);
                var urlFrontEnd = variaveisGlobaisOptions.UrlArquivosEstaticos;
                var textoEmail = textoArquivo
                    .Replace("#NOME", usuarioEol.Nome)
                    .Replace("#CODIGO", usuario.Token)
                    .Replace("#URL_BASE#", urlFrontEnd)
                    .Replace("#VALIDADE", usuario.ValidadeToken?.ToString("dd/MM/yyyy HH:mm"));

                await emailServico.Enviar(usuarioEol.Nome, usuarioEol.Email, "Redefinição de Senha Escola Aqui", textoEmail);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw new NegocioException("Não foi possivel realizar o envio de email, por favor contate o suporte");
            }
        }
    }
}
