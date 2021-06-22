using FluentValidation;
using System;

namespace SME.AE.Aplicacao
{
    public class AtualizarDadosUsuarioDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime DataNascimentoResponsavel { get; set; }
        public string NomeMae { get; set; }
        public string Celular { get; set; }
        public string DDD
        {
            get => string.IsNullOrEmpty(CelularNormalizado) ? "" : CelularNormalizado.Substring(0, 2);
        }
        public string CelularResponsavel
        {
            get => string.IsNullOrEmpty(CelularNormalizado) ? "" : CelularNormalizado[2..];
        }

        private string CelularNormalizado { get => string.IsNullOrEmpty(Celular) ? "" : Celular.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", ""); }

        public string TextoParaVerificarPersistencia()
        {
            return $"{NomeMae} {Email}";
        }

    }

    public class AtualizarDadosUsuarioValidator : AbstractValidator<AtualizarDadosUsuarioDto>
    {
        public AtualizarDadosUsuarioValidator()
        {
            RuleFor(x => x.DataNascimentoResponsavel).NotEmpty().WithMessage("A Data de Nascimento é obrigátoria").DataNascimentoEhValida();
            RuleFor(x => x.Email).NotEmpty().WithMessage("Deve ser informado o Email");
            RuleFor(x => x.NomeMae).NotEmpty().WithMessage("Deve ser informado o Nome da Mãe ");
            RuleFor(x => x.Celular).NotEmpty().WithMessage("Deve ser informado o Celular");
        }
    }
}
