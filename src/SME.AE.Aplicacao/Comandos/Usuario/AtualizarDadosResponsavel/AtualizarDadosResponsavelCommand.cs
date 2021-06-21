using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao
{
    public class AtualizarDadosResponsavelCommand : IRequest<bool>
    {
        public AtualizarDadosResponsavelCommand(int id, string email, DateTime dataNascimentoResponsavel, string nomeMae, string celular, string ddd)
        {
            Id = id;
            Email = email;
            DataNascimentoResponsavel = dataNascimentoResponsavel;
            NomeMae = nomeMae;
            Celular = celular;
            DDD = ddd;
        }

        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime DataNascimentoResponsavel { get; set; }
        public string NomeMae { get; set; }
        public string Celular { get; set; }
        public string DDD { get; set; }
    }
}
