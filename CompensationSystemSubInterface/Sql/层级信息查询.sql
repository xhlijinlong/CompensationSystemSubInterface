SELECT
	cj.id AS '层级ID',
	cj.cjname AS '层级名称' 
FROM
	ZX_config_cj cj 
WHERE
	cj.DeleteType= 0