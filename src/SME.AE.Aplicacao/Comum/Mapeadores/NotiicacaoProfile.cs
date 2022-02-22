using AutoMapper;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Dominio.Entidades;

namespace SME.AE.Aplicacao.Comum.Mapeadores
{
    public class NotiicacaoProfile : Profile
    {
        public NotiicacaoProfile()
        {
            CreateMap<NotificacaoSgpDto, Notificacao>();
            CreateMap<Notificacao, NotificacaoSgpDto>();
        }
    }
}
