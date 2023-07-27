insert into parametroescolaaqui
select 'TransferirNotaSgpAnosConsiderados', extract(year from current_date)
where not exists (select 1
				     from parametroescolaaqui
				  where chave = 'TransferirNotaSgpAnosConsiderados');	
				  
insert into parametroescolaaqui
select 'TransferirFrequenciaSgpAnosConsiderados', extract(year from current_date)
where not exists (select 1
				     from parametroescolaaqui
				  where chave = 'TransferirFrequenciaSgpAnosConsiderados');
