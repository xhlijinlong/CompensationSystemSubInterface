CREATE TABLE [dbo].[ZX_SalaryDetails] (
  [DetailId] int  IDENTITY(1,1) NOT NULL,
  [SalaryId] int  NULL,
  [ItemId] int  NULL,
  [Amount] decimal(18,2)  NULL,
  CONSTRAINT [PK_ZX_SalaryDetails] PRIMARY KEY CLUSTERED ([DetailId])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO

ALTER TABLE [dbo].[ZX_SalaryDetails] SET (LOCK_ESCALATION = TABLE)
GO

EXEC sp_addextendedproperty
'MS_Description', N'薪资发放明细ID',
'SCHEMA', N'dbo',
'TABLE', N'ZX_SalaryDetails',
'COLUMN', N'DetailId'
GO

EXEC sp_addextendedproperty
'MS_Description', N'薪资发放ID',
'SCHEMA', N'dbo',
'TABLE', N'ZX_SalaryDetails',
'COLUMN', N'SalaryId'
GO

EXEC sp_addextendedproperty
'MS_Description', N'薪资项目ID',
'SCHEMA', N'dbo',
'TABLE', N'ZX_SalaryDetails',
'COLUMN', N'ItemId'
GO

EXEC sp_addextendedproperty
'MS_Description', N'发放金额',
'SCHEMA', N'dbo',
'TABLE', N'ZX_SalaryDetails',
'COLUMN', N'Amount'