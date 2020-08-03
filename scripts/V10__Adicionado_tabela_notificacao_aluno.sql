 create table if not exists notificacao_aluno (
id bigint not null generated always as identity,
codigo_eol_aluno bigint not null,
notificacao_id bigint not null,
criadoem timestamp,
  CONSTRAINT fk_notificao_aluno_id
      FOREIGN KEY(notificacao_id) 
	  REFERENCES notificacao(id));
	 