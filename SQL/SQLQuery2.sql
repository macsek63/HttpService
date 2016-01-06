declare
@idoc int,
@param_xml xml

set @param_xml =
				'<param dok_id="28348" ret_type="json">'+
				'  <columns>' +
				'    <column name="dwz_poz_id"/>' +
				'    <column name="nrpoz" order_number="1" order_type="asc"/>' +
				'    <column name="wytwor_nazwa"/>' +
				'    <column name="wytwor_idm"/>' +
				'    <column name="ilosc"/>' +
				'    <column name="jm_idn"/>' +
				'    <column name="cena"/>' +
				'    <column name="przel_jm"/>' +
				'  </columns>' +
				'</param>'

exec hts_ot_sp_get_poz_dok_wz @param_xml 


