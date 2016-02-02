if exists (select * from sysobjects where id = object_id(N'[dbo].[hts_uti_sp_tabcols_to_selectcols]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
 drop procedure [dbo].[hts_uti_sp_tabcols_to_selectcols]
GO

CREATE procedure [dbo].[hts_uti_sp_tabcols_to_selectcols] 
    @sql_cols varchar(2000) OUT
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
@tcol varchar(100)

	set @sql_cols = ''
	DECLARE col_cur CURSOR FAST_FORWARD FOR  select col, tcol from #tab_cols 
	OPEN col_cur;
	FETCH NEXT FROM col_cur INTO @col, @tcol
	WHILE @@FETCH_STATUS = 0
		BEGIN
			if len(@sql_cols) = 0
				set @sql_cols = @sql_cols + @tcol + ' as ' + @col
			else
				set @sql_cols = @sql_cols + ',' + @tcol + ' as ' + @col
			FETCH NEXT FROM col_cur INTO @col, @tcol
		END
	CLOSE col_cur;
	DEALLOCATE col_cur;
	
end 
GO

