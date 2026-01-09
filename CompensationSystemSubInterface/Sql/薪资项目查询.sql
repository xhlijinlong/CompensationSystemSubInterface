SELECT
	si.ItemId AS '薪资项目ID',
	si.ItemName AS '项目名称',
	si.IsEveryMonth AS '是否每月都有' 
FROM
	ZX_SalaryItems si 
WHERE
	si.IsEnabled = 1 
ORDER BY
	si.DisplayOrder ASC