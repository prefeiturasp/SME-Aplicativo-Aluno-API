namespace SME.AE.Comum
{
    public abstract class PoliticaPolly
    {
        public static string SGP => "RetryPolicySGP";
        public static string PublicaFila => "RetryPolicyFilasRabbit";
    }
}
