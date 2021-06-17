﻿using FluentValidation;
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