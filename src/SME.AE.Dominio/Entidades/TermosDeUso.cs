namespace SME.AE.Dominio.Entidades
{
    public class TermosDeUso : EntidadeBase
    {
        public string DescricaoTermosDeUso { get; private set; }

        public string DescricaoPoliticaPrivacidade { get; private set; }

        public double Versao { get; private set; }

        public TermosDeUso()
        {

        }

        public TermosDeUso(string descricaoTermosDeUso, string descricaoPoliticaPrivacidade, double versao)
        {
            DescricaoTermosDeUso = descricaoTermosDeUso;
            DescricaoPoliticaPrivacidade = descricaoPoliticaPrivacidade;
            Versao = versao;
        }

        public void AtualizaVersao(double versao)
        {
            Versao = versao;
        }
    }
}
