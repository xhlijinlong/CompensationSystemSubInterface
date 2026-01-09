SELECT
	d.DetailId AS '薪资发放明细ID',
	d.SalaryId AS '薪资发放ID',
	d.ItemId AS '薪资项目ID',
	d.Amount AS '发放金额' 
FROM
	ZX_SalaryDetails d