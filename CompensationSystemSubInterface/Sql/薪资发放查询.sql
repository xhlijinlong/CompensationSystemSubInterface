SELECT
	h.SalaryId as '薪资发放ID',
	h.EmployeeId as '员工ID',
	h.SalaryMonth as '薪资月份'
FROM
	ZX_SalaryHeaders h 
ORDER BY
h.DisplayOrder ASC