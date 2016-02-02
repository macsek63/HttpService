declare
@idoc int,
@param_xml xml,
@ret int,
@ret_mess nvarchar(1000)

set @param_xml =
				'<param_proc dok_id="28348">'+
				'  <columns>' +
				'    <column name="dwz_poz_id"/>' +
				'    <column name="nrpoz" order_number="1" order_type="desc"/>' +
				'    <column name="wytwor_nazwa"/>' +
				'    <column name="wytwor_idm"/>' +
				'    <column name="ilosc"/>' +
				'    <column name="jm_idn"/>' +
				'    <column name="cena"/>' +
				'    <column name="przel_jm"/>' +
				'  </columns>' +
				'</param_proc>'

exec hts_ot_sp_select_dwz_poz @param_xml, @ret OUT, @ret_mess OUT


