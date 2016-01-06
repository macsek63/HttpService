if exists (select * from sysobjects where id = object_id(N'[dbo].[hts_uti_sp_tabcols_to_selectcols]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
 drop procedure [dbo].[hts_uti_sp_tabcols_to_selectcols]
GO

CREATE procedure [dbo].[hts_uti_sp_tabcols_to_selectcols] 
	@ret_type char(4),			--format resultseta 
								--'':zwyk³y select, 
								--'json' , 'xml' formaty dla grida dhtmlx
	@sql_order_by varchar(200),
    @sql_selectcols varchar(2000) OUT
as    
-------------------------------------------------------------------------------  
-- Procedura  : hts_uti_sp_tabcols_to_selectcols
-- Baza danych: MSSQL   
-- Argumenty  :    
-- Zwraca     :   
-- Stworzyl   : mac 
-- Opis       : Na podstawie roboczej tabeli tworzy string selecta z kolumnami resulseta
--				Na pocz¹tku dodaje kolumnê row z numerem wiersza (wymagane przez grida dhtmlx)
--%%ver_begin%%-----------------------------------------------------------------  
-- Historia zmian wersji   
-- 2015-12-27 stworzenie procedury
--%%ver_end%%-------------------------------------------------------------------
begin    
declare
@col varchar(100),
@tcol varchar(100),
@sql_cols varchar(2000),
@order_type char(4)

	set @sql_cols = ''

	if @ret_type = ''
		set @sql_selectcols = 'select (row_number() over ('+@sql_order_by+')) as row '
	if @ret_type = 'json'
		set @sql_selectcols = 'select ''{ id:''+cast((row_number() over ('+@sql_order_by+')) as varchar(200))+'', data: [''+'
	if @ret_type = 'xml'
		set @sql_selectcols = 'select ''<row id="''+cast((row_number() over ('+@sql_order_by+')) as varchar(200))+''">''+'

	set @sql_cols = ''
	DECLARE col_cur CURSOR FAST_FORWARD FOR  select col, tcol from #tab_cols 
	OPEN col_cur;
	FETCH NEXT FROM col_cur INTO @col, @tcol
	WHILE @@FETCH_STATUS = 0
		BEGIN
			set @tcol = 'rtrim(cast(' + @tcol + ' as varchar(200)))'
			if @ret_type = ''
				set @sql_cols = @sql_cols + ', '+ @tcol + ' as ' + @col
			if @ret_type = 'json'
				begin
					if len(@sql_cols) > 0
						set @sql_cols = @sql_cols + ''',''+'
					set @sql_cols = @sql_cols + '''"''+' + @tcol + '+''"''+'
				end
			if @ret_type = 'xml'
				set @sql_cols = @sql_cols + '''<cell>''+' + @tcol + '+''</cell>''+'

			FETCH NEXT FROM col_cur INTO @col, @tcol
		END
	CLOSE col_cur;
	DEALLOCATE col_cur;

	if @ret_type = 'json'
		set @sql_cols = @sql_cols + ''']}'''
	if @ret_type = 'xml'
		set @sql_cols = @sql_cols + '''</row>'''

	set @sql_selectcols = @sql_selectcols + @sql_cols
	
end 
GO

