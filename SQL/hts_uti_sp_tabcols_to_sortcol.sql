if exists (select * from sysobjects where id = object_id(N'[dbo].[hts_uti_sp_tabcols_to_sortcol]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
 drop procedure [dbo].[hts_uti_sp_tabcols_to_sortcol]
GO

CREATE procedure [dbo].[hts_uti_sp_tabcols_to_sortcol] 
      	@sql_order_by varchar(200) OUT
as    
-------------------------------------------------------------------------------  
-- Procedura  : hts_uti_sp_tabcols_to_sortcol
-- Baza danych: MSSQL   
-- Argumenty  :    
-- Zwraca     :   
-- Stworzyl   : mac 
-- Opis       : Na postawie roboczej tabeli kolumn tworzy string order_by do sortowania rekordów resulseta
--%%ver_begin%%-----------------------------------------------------------------  
-- Historia zmian wersji   
-- 2015-12-27 stworzenie procedury
--%%ver_end%%-------------------------------------------------------------------
begin    
declare
@col varchar(100),
@order_type char(4)

	set @sql_order_by = ''
	--kolumny do posortowania resultseta
	DECLARE col_cur CURSOR FAST_FORWARD FOR  select tcol, upper(order_type) from #tab_cols where order_number is not null order by order_number
	OPEN col_cur;
	FETCH NEXT FROM col_cur INTO @col, @order_type
	WHILE @@FETCH_STATUS = 0
		BEGIN
			if len(@sql_order_by) = 0
				set @sql_order_by = @col
			else 
				set @sql_order_by = @sql_order_by + ', '+ @col
			if @order_type = 'DESC'
				set @sql_order_by = @sql_order_by + ' DESC'
			FETCH NEXT FROM col_cur INTO @col, @order_type
		END
	CLOSE col_cur;
	DEALLOCATE col_cur;
	
end 
GO

