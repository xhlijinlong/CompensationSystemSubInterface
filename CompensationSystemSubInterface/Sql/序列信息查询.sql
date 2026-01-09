SELECT
	xl.id AS '序列ID',
	xl.xlname AS '序列名称' 
FROM
	ZX_config_xl xl 
WHERE
	xl.IsEnabled= 1 
	AND xl.DeleteType=0