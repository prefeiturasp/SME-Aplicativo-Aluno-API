using System;

namespace SME.AE.Aplicacao
{
    public class ComponenteCurricularDto : IEquatable<ComponenteCurricularDto>
    {
        public ComponenteCurricularDto(long codigo, string descricao)
        {
            Codigo = codigo;
            Descricao = descricao;
        }

        public long Codigo { get; set; }
        public string Descricao { get; set; }

        public bool Equals(ComponenteCurricularDto other)
        {
            return this.Codigo == other.Codigo;
        }
        public override int GetHashCode()
        {
            return this.Codigo.GetHashCode();
        }
    }
}
