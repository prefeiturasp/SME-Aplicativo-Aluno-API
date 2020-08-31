namespace SME.AE.Aplicacao.Comum.Modelos.Entrada
{
    public class UsuarioDispositivoDto
    {
        public long UsuarioId { get; set; }
        public string DispositivoId { get; set; }

        public UsuarioDispositivoDto(long usuarioId, string dispositivoId)
        {
            UsuarioId = usuarioId;
            DispositivoId = dispositivoId;
        }
    }
}
