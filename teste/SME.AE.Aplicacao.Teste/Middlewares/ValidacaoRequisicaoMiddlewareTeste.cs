using FluentValidation;
using MediatR;
using SME.AE.Aplicacao.Comum.Middlewares;
using SME.AE.Comum.Excecoes;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Middlewares
{
    public class ValidacaoRequisicaoMiddlewareTeste
    {
        private const string NomeValido = "Nome";
        private const int QuantidadeValida = 1;
        private const int QuantidadeNegativa = -1;
        private const string RespostaHandler = "Resposta do handler";
        private const string CampoNome = nameof(RequisicaoTeste.Nome);
        private const string CampoQuantidade = nameof(RequisicaoTeste.Quantidade);
        private const string MensagemNomeObrigatorio = "O nome é obrigatório";
        private const string MensagemQuantidadeObrigatoria = "A quantidade é obrigatória";
        private const string MensagemQuantidadePositiva = "A quantidade deve ser maior que zero";

        private readonly RequestHandlerDelegate<string> next;
        private int chamadasHandler;

        public class RequisicaoTeste : IRequest<string>
        {
            public string Nome { get; set; }
            public int Quantidade { get; set; }
        }

        public ValidacaoRequisicaoMiddlewareTeste()
        {
            next = () =>
            {
                chamadasHandler++;
                return Task.FromResult(RespostaHandler);
            };
        }

        [Fact(DisplayName = "Deve seguir para o handler quando a requisição for válida")]
        public async Task Deve_Seguir_Para_O_Handler_Quando_A_Requisicao_For_Valida()
        {
            var middleware = CriarMiddleware(CriarValidator());

            var resultado = await middleware.Handle(CriarRequisicao(NomeValido, QuantidadeValida), next, new CancellationToken());

            Assert.Equal(1, chamadasHandler);
            Assert.Equal(RespostaHandler, resultado);
        }

        [Fact(DisplayName = "Deve seguir para o handler quando a requisição não tiver validator")]
        public async Task Deve_Seguir_Para_O_Handler_Quando_A_Requisicao_Nao_Tiver_Validator()
        {
            var middleware = CriarMiddleware();

            var resultado = await middleware.Handle(CriarRequisicao(null, 0), next, new CancellationToken());

            Assert.Equal(1, chamadasHandler);
            Assert.Equal(RespostaHandler, resultado);
        }

        [Fact(DisplayName = "Deve lançar ValidacaoException sem chamar o handler quando a requisição for inválida")]
        public async Task Deve_Lancar_ValidacaoException_Sem_Chamar_O_Handler_Quando_A_Requisicao_For_Invalida()
        {
            var middleware = CriarMiddleware(CriarValidator());

            var excecao = await Assert.ThrowsAsync<ValidacaoException>(() => middleware.Handle(CriarRequisicao(null, QuantidadeValida), next, new CancellationToken()));

            var erro = Assert.Single(excecao.Errors);
            Assert.Equal(CampoNome, erro.Key);
            Assert.Equal(new[] { MensagemNomeObrigatorio }, erro.Value);
            Assert.Equal(0, chamadasHandler);
        }

        [Fact(DisplayName = "Deve retornar um erro para cada campo inválido da requisição")]
        public async Task Deve_Retornar_Um_Erro_Para_Cada_Campo_Invalido_Da_Requisicao()
        {
            var middleware = CriarMiddleware(CriarValidator());

            var excecao = await Assert.ThrowsAsync<ValidacaoException>(() => middleware.Handle(CriarRequisicao(null, QuantidadeNegativa), next, new CancellationToken()));

            Assert.Equal(2, excecao.Errors.Count);
            Assert.Equal(new[] { MensagemNomeObrigatorio }, excecao.Errors[CampoNome]);
            Assert.Equal(new[] { MensagemQuantidadePositiva }, excecao.Errors[CampoQuantidade]);
            Assert.Equal(0, chamadasHandler);
        }

        [Fact(DisplayName = "Deve agrupar no mesmo campo as mensagens de regras diferentes, na ordem das regras")]
        public async Task Deve_Agrupar_No_Mesmo_Campo_As_Mensagens_De_Regras_Diferentes_Na_Ordem_Das_Regras()
        {
            var middleware = CriarMiddleware(CriarValidator());

            var excecao = await Assert.ThrowsAsync<ValidacaoException>(() => middleware.Handle(CriarRequisicao(NomeValido, 0), next, new CancellationToken()));

            var erro = Assert.Single(excecao.Errors);
            Assert.Equal(CampoQuantidade, erro.Key);
            Assert.Equal(new[] { MensagemQuantidadeObrigatoria, MensagemQuantidadePositiva }, erro.Value);
            Assert.Equal(0, chamadasHandler);
        }

        private static ValidacaoRequisicaoMiddleware<RequisicaoTeste, string> CriarMiddleware(params IValidator<RequisicaoTeste>[] validators)
        {
            return new ValidacaoRequisicaoMiddleware<RequisicaoTeste, string>(validators);
        }

        private static InlineValidator<RequisicaoTeste> CriarValidator()
        {
            return new InlineValidator<RequisicaoTeste>
            {
                v => v.RuleFor(x => x.Nome).NotEmpty().WithMessage(MensagemNomeObrigatorio),
                v => v.RuleFor(x => x.Quantidade).NotEmpty().WithMessage(MensagemQuantidadeObrigatoria),
                v => v.RuleFor(x => x.Quantidade).GreaterThan(0).WithMessage(MensagemQuantidadePositiva)
            };
        }

        private static RequisicaoTeste CriarRequisicao(string nome, int quantidade)
        {
            return new RequisicaoTeste { Nome = nome, Quantidade = quantidade };
        }
    }
}
