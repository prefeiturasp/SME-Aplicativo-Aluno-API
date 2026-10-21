using FluentValidation.TestHelper;
using System;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Modelos
{
    public class AtualizarDadosUsuarioDtoTeste : BaseTeste
    {
        private const string MensagemDataNascimentoObrigatoria = "A Data de Nascimento é obrigátoria";
        private const string MensagemEmailObrigatorio = "Deve ser informado o Email";
        private const string MensagemNomeMaeObrigatorio = "Deve ser informado o Nome da Mãe ";
        private const string MensagemCelularObrigatorio = "Deve ser informado o Celular";

        private readonly AtualizarDadosUsuarioValidator validator;

        public AtualizarDadosUsuarioDtoTeste()
        {
            validator = new AtualizarDadosUsuarioValidator();
        }

        [Theory(DisplayName = "Deve separar DDD e número do celular removendo a máscara")]
        [InlineData("(11) 98765-4321", "11", "987654321")]
        [InlineData("11987654321", "11", "987654321")]
        [InlineData("11 98765-4321", "11", "987654321")]
        [InlineData("(11)3456-7890", "11", "34567890")]
        public void Deve_Separar_DDD_E_Numero_Do_Celular_Removendo_A_Mascara(string celular, string dddEsperado, string numeroEsperado)
        {
            var dto = CriarDto(celular: celular);

            Assert.Equal(dddEsperado, dto.DDD);
            Assert.Equal(numeroEsperado, dto.CelularResponsavel);
        }

        [Theory(DisplayName = "Deve retornar DDD e celular vazios quando o celular não for informado")]
        [InlineData(null)]
        [InlineData("")]
        public void Deve_Retornar_DDD_E_Celular_Vazios_Quando_O_Celular_Nao_For_Informado(string celular)
        {
            var dto = CriarDto(celular: celular);

            Assert.Equal(string.Empty, dto.DDD);
            Assert.Equal(string.Empty, dto.CelularResponsavel);
        }

        [Fact(DisplayName = "Deve aceitar dados válidos")]
        public void Deve_Aceitar_Dados_Validos()
        {
            var resultado = validator.TestValidate(CriarDto());

            resultado.ShouldNotHaveAnyValidationErrors();
        }

        [Theory(DisplayName = "Deve exigir o e-mail")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Deve_Exigir_O_Email(string email)
        {
            var resultado = validator.TestValidate(CriarDto(email: email));

            resultado.ShouldHaveValidationErrorFor(x => x.Email).WithErrorMessage(MensagemEmailObrigatorio);
        }

        [Theory(DisplayName = "Deve exigir o nome da mãe")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Deve_Exigir_O_Nome_Da_Mae(string nomeMae)
        {
            var resultado = validator.TestValidate(CriarDto(nomeMae: nomeMae));

            resultado.ShouldHaveValidationErrorFor(x => x.NomeMae).WithErrorMessage(MensagemNomeMaeObrigatorio);
        }

        [Theory(DisplayName = "Deve exigir o celular")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Deve_Exigir_O_Celular(string celular)
        {
            var resultado = validator.TestValidate(CriarDto(celular: celular));

            resultado.ShouldHaveValidationErrorFor(x => x.Celular).WithErrorMessage(MensagemCelularObrigatorio);
        }

        [Fact(DisplayName = "Deve exigir a data de nascimento")]
        public void Deve_Exigir_A_Data_De_Nascimento()
        {
            var resultado = validator.TestValidate(CriarDto(dataNascimento: default(DateTime)));

            resultado.ShouldHaveValidationErrorFor(x => x.DataNascimentoResponsavel).WithErrorMessage(MensagemDataNascimentoObrigatoria);
        }

        [Theory(DisplayName = "Deve rejeitar data de nascimento até 01/01/1900")]
        [InlineData(1900, 1, 1)]
        [InlineData(1899, 12, 31)]
        public void Deve_Rejeitar_Data_De_Nascimento_Ate_01_01_1900(int ano, int mes, int dia)
        {
            var resultado = validator.TestValidate(CriarDto(dataNascimento: new DateTime(ano, mes, dia)));

            resultado.ShouldHaveValidationErrorFor(x => x.DataNascimentoResponsavel);
        }

        [Fact(DisplayName = "Deve rejeitar data de nascimento futura")]
        public void Deve_Rejeitar_Data_De_Nascimento_Futura()
        {
            var resultado = validator.TestValidate(CriarDto(dataNascimento: DateTime.Now.AddDays(1)));

            resultado.ShouldHaveValidationErrorFor(x => x.DataNascimentoResponsavel);
        }

        [Fact(DisplayName = "Deve aceitar data de nascimento a partir de 02/01/1900")]
        public void Deve_Aceitar_Data_De_Nascimento_A_Partir_De_02_01_1900()
        {
            var resultado = validator.TestValidate(CriarDto(dataNascimento: new DateTime(1900, 1, 2)));

            resultado.ShouldNotHaveValidationErrorFor(x => x.DataNascimentoResponsavel);
        }

        private static AtualizarDadosUsuarioDto CriarDto(string email = "responsavel@teste.com",
                                                         string nomeMae = "Maria da Silva",
                                                         string celular = "(11) 98765-4321",
                                                         DateTime? dataNascimento = null)
        {
            return new AtualizarDadosUsuarioDto
            {
                Id = 1,
                Email = email,
                NomeMae = nomeMae,
                Celular = celular,
                DataNascimentoResponsavel = dataNascimento ?? new DateTime(1985, 5, 10)
            };
        }
    }
}
