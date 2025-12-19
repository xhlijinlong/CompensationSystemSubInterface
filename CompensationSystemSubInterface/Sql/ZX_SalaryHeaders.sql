CREATE TABLE [dbo].[ZX_SalaryHeaders] (
  [SalaryId] int  IDENTITY(1,1) NOT NULL,
  [EmployeeId] int  NULL,
  [SalaryMonth] datetime  NULL,
  [BatchId] int  NULL,
  [SequenceId] int  NULL,
  [DepartmentId] int  NULL,
  [PositionId] int  NULL,
  [LevelId] int  NULL,
  [CreateUserId] int  NULL,
  [CreateTime] datetime  NULL,
  [DisplayOrder] int  NULL,
  CONSTRAINT [PK_ZX_SalaryHeaders] PRIMARY KEY CLUSTERED ([SalaryId])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO

ALTER TABLE [dbo].[ZX_SalaryHeaders] SET (LOCK_ESCALATION = TABLE)
GO

EXEC sp_addextendedproperty
'MS_Description', N'员工ID',
'SCHEMA', N'dbo',
'TABLE', N'ZX_SalaryHeaders',
'COLUMN', N'EmployeeId'
GO

EXEC sp_addextendedproperty
'MS_Description', N'薪资月份',
'SCHEMA', N'dbo',
'TABLE', N'ZX_SalaryHeaders',
'COLUMN', N'SalaryMonth'
GO

EXEC sp_addextendedproperty
'MS_Description', N'审核ID',
'SCHEMA', N'dbo',
'TABLE', N'ZX_SalaryHeaders',
'COLUMN', N'BatchId'
GO

EXEC sp_addextendedproperty
'MS_Description', N'序列ID',
'SCHEMA', N'dbo',
'TABLE', N'ZX_SalaryHeaders',
'COLUMN', N'SequenceId'
GO

EXEC sp_addextendedproperty
'MS_Description', N'部门ID',
'SCHEMA', N'dbo',
'TABLE', N'ZX_SalaryHeaders',
'COLUMN', N'DepartmentId'
GO

EXEC sp_addextendedproperty
'MS_Description', N'职务ID',
'SCHEMA', N'dbo',
'TABLE', N'ZX_SalaryHeaders',
'COLUMN', N'PositionId'
GO

EXEC sp_addextendedproperty
'MS_Description', N'层级ID',
'SCHEMA', N'dbo',
'TABLE', N'ZX_SalaryHeaders',
'COLUMN', N'LevelId'
GO

EXEC sp_addextendedproperty
'MS_Description', N'创建人ID',
'SCHEMA', N'dbo',
'TABLE', N'ZX_SalaryHeaders',
'COLUMN', N'CreateUserId'
GO

EXEC sp_addextendedproperty
'MS_Description', N'创建时间',
'SCHEMA', N'dbo',
'TABLE', N'ZX_SalaryHeaders',
'COLUMN', N'CreateTime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'显示顺序',
'SCHEMA', N'dbo',
'TABLE', N'ZX_SalaryHeaders',
'COLUMN', N'DisplayOrder'