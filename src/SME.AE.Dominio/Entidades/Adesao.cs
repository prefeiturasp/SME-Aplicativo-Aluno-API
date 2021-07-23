namespace SME.AE.Dominio.Entidades
{
    public class Adesao : EntidadeBase
    {
        public string CodigoDre { get; private set; }
        public string NomeDre { get; private set; }
        public string CodigoUe { get; private set; }
        public string NomeUe { get; private set; }
        public long CodigoTurma { get; private set; }
        public long UsuariosPrimeiroAcessoIncompleto { get; private set; }
        public long UsuariosValidos { get; private set; }
        public long UsuariosCpfInvalidos { get; private set; }
        public long UsuariosSemAppInstalado { get; private set; }

        public Adesao()
        {

        }

        public Adesao(string codigoDre, string nomeDre, string codigoUe, string nomeUe, long codigoTurma, long usuariosPrimeiroAcessoIncompleto, long usuariosValidos, long usuariosCpfInvalidos, long usuariosSemAppInstalado)
        {
            CodigoDre = codigoDre;
            NomeDre = nomeDre;
            CodigoUe = codigoUe;
            NomeUe = nomeUe;
            CodigoTurma = codigoTurma;
            UsuariosPrimeiroAcessoIncompleto = usuariosPrimeiroAcessoIncompleto;
            UsuariosValidos = usuariosValidos;
            UsuariosCpfInvalidos = usuariosCpfInvalidos;
            UsuariosSemAppInstalado = usuariosSemAppInstalado;
        }        
    }
}
