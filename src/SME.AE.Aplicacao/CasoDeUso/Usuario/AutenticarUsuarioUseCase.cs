using System.Threading.Tasks;
using MediatR;
using SME.AE.Aplicacao.Comandos.Token.Criar;

namespace SME.AE.Aplicacao.CasoDeUso.Usuario
{
    public class AutenticarUsuarioUseCase
    {
        public static async Task<string> Executar(IMediator mediator, string cpf, string senha)
        {
            // 1. Verificar se o usuario existe na base do EOL (responsavel legal)
            // 2. Obter os alunos relacionados ao responsavel legal
            // 3. Verificar se a senha bate com a data de nascimento de um dos alunos
            // 4. Gerar token
            string token = await mediator.Send(new CriarTokenCommand(cpf));
            return token;
        }
    }
}