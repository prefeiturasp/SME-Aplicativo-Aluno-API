CREATE TABLE public.Notificacao(
   Id bigint not null,
   Titulo varchar(50) not null,
   Mensagem varchar(2048),
   Grupo varchar(128),
   DataEnvio timestamp not null,
   DataExpiracao timestamp not null,
   CriadoEm timestamp not null default current_timestamp,
   CriadoPor varchar(128) not null,
   AlteradoEm timestamp,
   AlteradoPor varchar(128),
   constraint Pk_Notificacao_Id primary key(Id)
);

ALTER TABLE public.Usuario ADD COLUMN Email varchar(128);
