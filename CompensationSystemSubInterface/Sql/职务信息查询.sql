SELECT
	gw.id AS '职务ID',
	gw.gwname AS '职务名称' 
FROM
	ZX_config_gw gw 
WHERE
	gw.IsEnabled= 1 
	AND gw.DeleteType = 0 
ORDER BY
	gw.DisplayOrder ASC