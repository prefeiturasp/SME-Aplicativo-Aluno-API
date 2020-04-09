using System.Threading.Tasks;
using MediatR;
using SME.AE.Aplicacao.Comandos.Autenticacao.AutenticarUsuario;
using SME.AE.Aplicacao.Comandos.Token.Criar;
using SME.AE.Aplicacao.Comum.Modelos;

namespace SME.AE.Aplicacao.CasoDeUso.Usuario
{
    public class AutenticarUsuarioUseCase
    {
        public static async Task<RespostaApi> Executar(IMediator mediator, string cpf, string senha)
        {
            // 1. Verificar se o usuario existe na base do EOL (responsavel legal)
            // 2. Obter os alunos relacionados ao responsavel legal
            // 3. Verificar se a senha bate com a data de nascimento de um dos alunos
            // 4. Gerar token
            var resposta = await mediator.Send(new AutenticarUsuarioCommand(cpf, senha));
            string token = await mediator.Send(new CriarTokenCommand(cpf));
            return resposta;
        }
    }
}