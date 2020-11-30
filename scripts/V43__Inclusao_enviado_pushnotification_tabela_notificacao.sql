alter table if exists notificacao
add column if not exists enviadopushnotification bool NOT NULL DEFAULT true;

alter table if exists notificacao
alter column enviadopushnotification set DEFAULT false;