using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao
{
    public static class CustomValidators
    {
        public static IRuleBuilderOptions<T, string> ValidarCpf<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Must(m => CpfEhValido(m) == true).WithMessage("O CPF informado não é valido");
        }

        public static IRuleBuilderOptions<T, DateTime> DataNascimentoEhValida<T>(this IRuleBuilder<T, DateTime> ruleBuilder)
        {
            return ruleBuilder.Must(m => NascimentoEhValida(m) == true);
        }

        public static IRuleBuilderOptions<T, string> ValidarSenha<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var rule = ruleBuilder.MinimumLength(8).WithMessage("A senha deve conter no Minimo 8 digitos");

            rule = rule.MaximumLength(12).WithMessage("A senha deve conter no Maximo 12 digitos");

            rule = rule.Must(x => !x.Contains(" ")).WithMessage("A senha não pode conter espaços em branco");

            rule = rule.Matches(@"(?=.*?[A-Z])(?=.*?[a-z])(?=((?=.*[!@#$\-%&/\\\[\]|*()_=+])|(?=.*?[0-9]+)))").WithMessage("Sua nova senha deve conter letras maiúsculas, minúsculas, números e símbolos. Por favor digite outra senha");

            return rule;
        }

        private static bool NascimentoEhValida(DateTime data) => data > new DateTime(1900, 1, 1) && data < DateTime.Now;

        private static bool CpfEhValido(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCpf;
            string digito;
            int soma;
            int resto;
            cpf = cpf.Trim();
            cpf = cpf.Replace(".", "").Replace("-", "");

            if (cpf.Length != 11)
                return false;

            tempCpf = cpf.Substring(0, 9);
            soma = 0;
            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
            resto = soma % 11;

            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = resto.ToString();
            tempCpf += digito;

            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito += resto.ToString();
            return cpf.EndsWith(digito);
        }
    }
}
