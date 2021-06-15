namespace SME.AE.Infra
{
    public static class RotasRabbitEol
    {
        public const string AtualizarDadosUsuarioEolSync = "eol.usuario.atualizar.dados.local.sync";
        public const string AtualizarDadosUsuarioEolTratar = "eol.usuario.atualizar.dados.local.tratar";

        public const string AtualizarDadosUsuarioProdamSync = "eol.usuario.atualizar.dados.prodam.sync";
        public const string AtualizarDadosUsuarioProdamTratar = "eol.usuario.atualizar.dados.prodam.tratar";

        public const string RotaRabbitDeadletterSync = "eol.rabbit.deadletter.sync";
        public const string RotaRabbitDeadletterTratar = "eol.rabbit.deadletter.tratar";
    }
}
