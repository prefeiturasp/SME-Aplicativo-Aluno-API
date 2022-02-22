namespace SME.AE.Dominio.Entidades
{
    public class OutroServico : EntidadeBase
    {
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string Categoria { get; set; }
        public string UrlSite { get; set; }
        public string Icone { get; set; }
        public bool Destaque { get; set; }
        public bool Ativo { get; set; }
    }
}
