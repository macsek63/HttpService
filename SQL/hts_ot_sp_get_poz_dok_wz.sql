if exists (select * from sysobjects where id = object_id(N'[dbo].[hts_ot_sp_get_poz_dok_wz]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
 drop procedure [dbo].[hts_ot_sp_get_poz_dok_wz]
GO

CREATE procedure [dbo].[hts_ot_sp_get_poz_dok_wz] 
      	@param_xml  xml
as    
-------------------------------------------------------------------------------  
-- Procedura  : hts_ot_sp_get_poz_dok_wz
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
@ret_type varchar(10),
@col varchar(100),
@tcol varchar(100),
@sql_selectcols varchar(2000),
@sql_order_by varchar(200),
@sql nvarchar(max)

	if rtrim(isnull(cast(@param_xml as varchar(max)),'')) = ''
		begin
			select 
				-1 as status,
				'Brak parametrów' as message,
				'Wymagana struktura parametrów:\n'+
				'<param dok_id="..">\n'+
				'  <columns>\n' +
				'    <column name=" .. " order_number=".." order_type=".."/>\n' +
				'  </columns>\n' +
				'</param>' as data
			return
		end

	--Create an internal representation of the XML document.
	EXEC sp_xml_preparedocument @idoc OUTPUT, @param_xml


  	SELECT   @dok_id = dok_id	FROM OPENXML (@idoc, '/param')	WITH (dok_id decimal(10,0))	--id dokumentu
  	SELECT   @ret_type = ret_type	FROM OPENXML (@idoc, '/param')	WITH (ret_type varchar(10))	--typ zwracanych danych: xml,json

	--w tabeli #tab_cols zapisujemy nazwy kolumn, które maj¹ byæ zwrócone
	SELECT 
		[name] as 'col',
		[name] as 'tcol',
		[order_number] 'order_number',
		[order_type] 'order_type'
	into #tab_cols
	FROM  OPENXML (@idoc, '/param/columns/column') 
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
	exec hts_uti_sp_tabcols_to_selectcols @ret_type, @sql_order_by, @sql_selectcols OUT

	set @sql = @sql_selectcols + 
		' from 
			dokument_wydania_zewn_poz p with (nolock)
			left outer join wytwor w with (nolock) on p.wytwor_id = w.wytwor_id
			left outer join partmag pm with (nolock) on p.partmag_id = pm.partmag_id
			left outer join jm with (nolock) on p.jm_id=jm.jm_id
			left outer join jm jmbaz with (nolock) on w.jmbazowa=jmbaz.jm_id
		where
			p.dokwydzew_id = ' + cast(@dok_id as varchar(200)) + @sql_order_by

	print @sql
	EXEC sp_executesql @sql
	
end 
GO

