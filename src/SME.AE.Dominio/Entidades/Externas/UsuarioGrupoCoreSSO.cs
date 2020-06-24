using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Dominio.Entidades.Externas
{
    public class UsuarioGrupoCoreSSO : EntidadeExterna
    {
        public Guid UsuarioId { get; set; }
        public Guid GrupoId { get; set; }
        public int Situacao { get; set; }
    }
}
