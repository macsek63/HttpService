--tabela  procedur dostêpnych dla serwisu http
IF OBJECT_ID('hts_dostepne_procedury', 'U') IS NULL
begin
	print 'nowa tabela: [hts_dostepne_procedury]'
	
	CREATE TABLE [dbo].[hts_dostepne_procedury](
		[dost_proc_id] [numeric](10, 0) IDENTITY(1,1) NOT NULL,
		[nazwa_proc] [nvarchar](100) NOT NULL
	) ON [PRIMARY]
end
GO


-- indeks [dost_proc_id_ix] 

if not  exists (select 1 from sysindexes where id = object_id('hts_dostepne_procedury') and name = 'dost_proc_id_ix' )
begin
	print 'nowy indeks: dost_proc_id_ix on hts_dostepne_procedury ( dost_proc_id ASC )'

	create index dost_proc_id_ix on hts_dostepne_procedury ( dost_proc_id ASC )
end        
GO

-- indeks [nazwa_proc_ix] 

if not  exists (select 1 from sysindexes where id = object_id('hts_dostepne_procedury') and name = 'nazwa_proc_ix' )
begin
	print 'nowy indeks: nazwa_proc_ix on hts_dostepne_procedury ( nazwa_proc ASC )'

	create unique index nazwa_proc_ix on hts_dostepne_procedury ( nazwa_proc ASC )
end        
GO

-------------------------------------------------
--tabela u¿ytkowników serwisu http
IF OBJECT_ID('hts_user', 'U') IS NULL
begin
	print 'nowa tabela: [hts_user]'
	
	CREATE TABLE [dbo].[hts_user](
		[user_id] [numeric](10, 0) IDENTITY(1,1) NOT NULL,	
		[user_idn] [nvarchar](100) NOT NULL,		--idn uzytkownika serwisu http
		[haslo] [nvarchar](100) NULL,				--haslo uzytkownika serwisu http (jeœli null tzn., ¿e konto zosta³o za³o¿one ale user jeszcze nie okreœli³ has³a)
		[uzytk_id] [numeric](10, 0) NOT NULL		--id uzytkownika ERP (uzytkownik ERP powi¹zany z uzytkownikiem serwisu http)
	) ON [PRIMARY]
end
GO


-- indeks [user_id_ix] 

if not  exists (select 1 from sysindexes where id = object_id('hts_user') and name = 'user_id_ix' )
begin
	print 'nowy indeks: user_id_ix on hts_user ( user_id ASC )'

	create index user_id_ix on hts_user ( user_id ASC )
end        
GO

-- indeks [user_idn_ix] 

if not  exists (select 1 from sysindexes where id = object_id('hts_user') and name = 'user_idn_ix' )
begin
	print 'nowy indeks: user_idn_ix on hts_user ( user_idn ASC )'

	create unique index user_idn_ix on hts_user ( user_idn ASC )
end        
GO

-------------------------------------------------
--lista procedur dostêpnych dla serwisu http
--insert into hts_dostepne_procedury (nazwa_proc) values ('hts_ot_sp_select_dwz_poz')
-------------------------------------------------

