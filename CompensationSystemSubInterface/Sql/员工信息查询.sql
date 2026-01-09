SELECT
	yg.id AS '员工ID',
	yg.xingming AS '姓名',
	yg.yuangongbh AS '编号',
	yg.bmid AS '部门ID',
	yg.xlid AS '序列ID',
	yg.gwid AS '职务ID',
	yg.cjid AS '层级ID' 
FROM
	ZX_config_yg yg 
WHERE
	yg.zaizhi = 1 
ORDER BY
	yg.xuhao