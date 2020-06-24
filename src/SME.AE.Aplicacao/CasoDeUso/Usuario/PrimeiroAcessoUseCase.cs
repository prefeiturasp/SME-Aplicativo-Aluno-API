using MediatR;
using SME.AE.Aplicacao.Comandos.CoreSSO.AssociarGrupoUsuario;
using SME.AE.Aplicacao.Comandos.CoreSSO.Usuario;
using SME.AE.Aplicacao.Comandos.Usuario.AtualizaPrimeiroAcesso;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Aplicacao.Comum.Modelos.Usuario;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using SME.AE.Aplicacao.Consultas.ObterUsuarioCoreSSO;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso.Usuario
{
    public class PrimeiroAcessoUseCase : ICriarUsuarioPrimeiroAcessoUseCase
    {
        public async Task<RespostaApi> Executar(IMediator mediator, NovaSenhaDto novaSenhaDto)
        {
            var usuario = await mediator.Send(new ObterUsuarioQuery() { Id = novaSenhaDto.Id });

            var usuarioCoreSSO = await mediator.Send(new ObterUsuarioCoreSSOQuery(usuario.Cpf));

            await CriarUsuarioOuAssociarGrupo(mediator, novaSenhaDto, usuario, usuarioCoreSSO);

            var atualizarPrimeiroAcesso = MapearAtualizarPrimeiroAcessoCommand(usuario);

            await mediator.Send(atualizarPrimeiroAcesso);

            return new RespostaApi
            {
                Ok = true
            };
        }

        private async Task CriarUsuarioOuAssociarGrupo(IMediator mediator, NovaSenhaDto novaSenhaDto, Dominio.Entidades.Usuario usuario, RetornoUsuarioCoreSSO usuarioCoreSSO)
        {
            if (usuarioCoreSSO != null)
            {
                await mediator.Send(new AssociarGrupoUsuarioCommand(usuarioCoreSSO));
                return;
            }

            var comandoCriaUsuario = MapearCriarUsuarioCoreSSOCommand(novaSenhaDto, usuario);

            await mediator.Send(comandoCriaUsuario);
        }

        private AtualizarPrimeiroAcessoCommand MapearAtualizarPrimeiroAcessoCommand(Dominio.Entidades.Usuario usuario)
        {
            return new AtualizarPrimeiroAcessoCommand
            {
                Id = usuario.Id,
                PrimeiroAcesso = false
            };
        }

        private CriarUsuarioCoreSSOCommand MapearCriarUsuarioCoreSSOCommand(NovaSenhaDto novaSenhaDto, Dominio.Entidades.Usuario usuario)
        {
            return new CriarUsuarioCoreSSOCommand()
            {
                Usuario = new UsuarioCoreSSO
                {
                    Cpf = usuario.Cpf,
                    Nome = usuario.Nome,
                    Senha = novaSenhaDto.NovaSenha,
                }
            };
        }
    }
}
