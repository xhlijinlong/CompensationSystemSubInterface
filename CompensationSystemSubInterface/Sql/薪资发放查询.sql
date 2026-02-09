SELECT
	h.SalaryId AS '薪资发放ID',
	h.EmployeeId AS '员工ID',
	h.SalaryMonth AS '薪资月份' 
FROM
	ZX_SalaryHeaders h
	JOIN ZX_SalaryAuditBatches ab ON h.BatchId = ab.BatchId 
WHERE
	ab.OverallStatus IN ( 3, 5 ) 
ORDER BY
	h.DisplayOrder ASC