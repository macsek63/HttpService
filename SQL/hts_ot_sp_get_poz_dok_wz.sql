if exists (select * from sysobjects where id = object_id(N'[dbo].[hts_ot_sp_select_dwz_poz]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
 drop procedure [dbo].[hts_ot_sp_select_dwz_poz]
GO

CREATE procedure [dbo].[hts_ot_sp_select_dwz_poz] 
      	@param_xml  xml,
		@ret int OUT,
		@ret_mess nvarchar(1000) OUT
as    
-------------------------------------------------------------------------------  
-- Procedura  : hts_ot_sp_select_dwz_poz
-- Baza danych: MSSQL   
-- Argumenty  :    
-- Zwraca     :   
-- Stworzyl   : mac 
-- Opis       : Procedura zwraca dane pozycji dokumentu WZ
--%%ver_begin%%-----------------------------------------------------------------  
-- Historia zmian wersji   
-- 2015-12-23 stworzenie procedury
--%%ver_end%%-------------------------------------------------------------------
begin    
declare
@idoc int,
@dok_id  decimal(10,0),
@ret_data_format varchar(10),
@col varchar(100),
@tcol varchar(100),
@sql_selectcols varchar(2000),
@sql_order_by varchar(2000),
@sql_from varchar(2000),
@sql_where varchar(2000),
@sql nvarchar(max),
@info_param nvarchar(1000)
--,
--@info_ok nvarchar(1000)

	set @ret = 0
	set @ret_mess = ''

	set @info_param = 
				'<param_proc dok_id="..">\n'+
				'  <columns>\n' +
				'    <column name=".." order_number=".." order_type=".."/>\n' +
				'  </columns>\n' +
				'</param_proc>'
	--print @info_param
	--set @info_ok = '<info><status>0</status><message></message></info>' 
				
	if rtrim(isnull(cast(@param_xml as varchar(max)),'')) = ''
		begin
			set @ret = -1
			set @ret_mess = 'Brak parametrów\nWymagana struktura parametrów:\n' + @info_param
			--select 
			--	'<info>' +
			--	'  <status>-1</status>' +
			--	'  <message>Brak parametrów\nWymagana struktura parametrów:\n' + @info_param +
			--	'  </message>' +
			--	'</info>' 
			--	as info
			return
		end

	--Create an internal representation of the XML document.
	EXEC sp_xml_preparedocument @idoc OUTPUT, @param_xml


  	SELECT   @dok_id = dok_id	FROM OPENXML (@idoc, '/param_proc')	WITH (dok_id decimal(10,0))	--id dokumentu

	--w tabeli #tab_cols zapisujemy nazwy kolumn, które maj¹ byæ zwrócone
	SELECT 
		[name] as 'col',
		[name] as 'tcol',
		[order_number] 'order_number',
		[order_type] 'order_type'
	into #tab_cols
	FROM  OPENXML (@idoc, '/param_proc/columns/column') 
	WITH 
	(
		[name] varchar(200),
		[order_number] int,
		[order_type] char(4)
	)

	--dodajemy kolumny, które maj¹ byæ zwrócone
	DECLARE col_cur CURSOR FAST_FORWARD FOR  select col	from #tab_cols
	OPEN col_cur;
	FETCH NEXT FROM col_cur INTO @col
	WHILE @@FETCH_STATUS = 0
		BEGIN
			--nazwy kolumn uzupe³niamy o prefiks tabeli
			if @col = 'wytwor_idm' set @tcol = 'w.wytwor_idm'
			else
			if @col = 'wytwor_nazwa' set @tcol = 'w.nazwa'
			else
			if @col = 'jm_idn' set @tcol = 'jm.jm_idn'
			else
			if @col = 'jmbaz_id' set @tcol = 'jmbaz.jm_id'
			else
			if @col = 'partmag_idn'	set @tcol = 'pm.partmag_idn'
			else
			if @col = 'przel_jm'set @tcol = 'dbo.ot_uf_wyt_przelicz_jm (p.wytwor_id, 1, jmbaz.jm_id, p.jm_id, 0)'
			else
				set @tcol = 'p.' + @col

			update #tab_cols set tcol = @tcol where col = @col

			FETCH NEXT FROM col_cur INTO @col
		END
	CLOSE col_cur;
	DEALLOCATE col_cur;

	exec hts_uti_sp_tabcols_to_sortcol @sql_order_by OUT
	exec hts_uti_sp_tabcols_to_selectcols @sql_selectcols OUT
	set @sql_from = ' from	dokument_wydania_zewn_poz p with (nolock)
							left outer join wytwor w with (nolock) on p.wytwor_id = w.wytwor_id
							left outer join partmag pm with (nolock) on p.partmag_id = pm.partmag_id
							left outer join jm with (nolock) on p.jm_id=jm.jm_id
							left outer join jm jmbaz with (nolock) on w.jmbazowa=jmbaz.jm_id'
	set @sql_where = ' where p.dokwydzew_id = ' + cast(@dok_id as varchar(200)) 

	--set @sql =	'select top 1 ''' + @info_ok + '''	as info, ' + @sql_selectcols + @sql_from + ',#tab_cols '+
	--			'union ' +
	--			'select	null as info, ' + @sql_selectcols + @sql_from + @sql_where + 
	--			' order by info desc, ' + @sql_order_by

	set @sql =	'select	' + @sql_selectcols + @sql_from + @sql_where + 
				' order by ' + @sql_order_by

	--print @sql
	EXEC sp_executesql @sql
	
end 
GO

