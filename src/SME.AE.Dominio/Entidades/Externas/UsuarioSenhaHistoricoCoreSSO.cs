using SME.AE.Dominio.Comum.Enumeradores;
using System;

namespace SME.AE.Dominio.Entidades.Externas
{
    public class UsuarioSenhaHistoricoCoreSSO : EntidadeExterna
    {
        public Guid UsuarioId { get; set; }
        public Guid SenhaId { get; set; }
        public string Senha { get; set; }
        public TipoCriptografia Criptografia { get; set; }
        public DateTime Data { get; set; }
    }
}
