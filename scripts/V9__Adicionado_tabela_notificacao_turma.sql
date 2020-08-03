 create table if not exists notificacao_turma (
id bigint not null generated always as identity,
codigo_eol_turma bigint not null,
notificacao_id bigint not null,
criadoem timestamp,
  CONSTRAINT fk_notificaoId_turma
      FOREIGN KEY(notificacao_id) 
	  REFERENCES notificacao(id));	