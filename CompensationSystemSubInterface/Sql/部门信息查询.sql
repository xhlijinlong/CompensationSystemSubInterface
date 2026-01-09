SELECT
	bm.id AS '部门ID',
	bm.bmname AS '部门名称',
	bm.xlid AS '序列ID' 
FROM
	ZX_config_bm bm 
WHERE
	bm.IsEnabled= 1 
	AND bm.DeleteType= 0 
ORDER BY
	bm.DisplayOrder ASC