using MediatR;
using System;

namespace SME.AE.Aplicacao
{
    public class AtualizarDadosResponsavelEolCommand : IRequest<bool>
    {
        public AtualizarDadosResponsavelEolCommand(long cpf, string email, DateTime dataNascimentoResponsavel, string nomeMae, string celular, string ddd)
        {
            Cpf = cpf;
            Email = email;
            DataNascimentoResponsavel = dataNascimentoResponsavel;
            NomeMae = nomeMae;
            Celular = celular;
            DDD = ddd;
        }

        public long Cpf { get; set; }
        public string Email { get; set; }
        public DateTime DataNascimentoResponsavel { get; set; }
        public string NomeMae { get; set; }
        public string Celular { get; set; }
        public string DDD { get; set; }
    }
}
