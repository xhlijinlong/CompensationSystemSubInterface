CREATE TABLE [dbo].[ZX_SalaryItems] (
  [ItemId] int  IDENTITY(1,1) NOT NULL,
  [ItemCode] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [ItemName] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [TypeId] int  NULL,
  [IsSystem] bit  NULL,
  [IsEnabled] bit  NULL,
  [CreateTime] datetime  NULL,
  [DisplayOrder] int  NULL,
  [CreateOrder] int  NULL,
  [CreateType] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [IsEveryMonth] bit  NULL,
  [SapCode] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [TradeUnionBase] bit  NULL,
  [QueryType] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  CONSTRAINT [PK_ZX_SalaryItems] PRIMARY KEY CLUSTERED ([ItemId])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO

ALTER TABLE [dbo].[ZX_SalaryItems] SET (LOCK_ESCALATION = TABLE)
GO

EXEC sp_addextendedproperty
'MS_Description', N'薪资项目ID',
'SCHEMA', N'dbo',
'TABLE', N'ZX_SalaryItems',
'COLUMN', N'ItemId'
GO

EXEC sp_addextendedproperty
'MS_Description', N'项目名称',
'SCHEMA', N'dbo',
'TABLE', N'ZX_SalaryItems',
'COLUMN', N'ItemName'
GO

EXEC sp_addextendedproperty
'MS_Description', N'是否可用((1可用0不可用))',
'SCHEMA', N'dbo',
'TABLE', N'ZX_SalaryItems',
'COLUMN', N'IsEnabled'
GO

EXEC sp_addextendedproperty
'MS_Description', N'展示顺序',
'SCHEMA', N'dbo',
'TABLE', N'ZX_SalaryItems',
'COLUMN', N'DisplayOrder'
GO

EXEC sp_addextendedproperty
'MS_Description', N'是否每月都有(1每月有0偶尔有)',
'SCHEMA', N'dbo',
'TABLE', N'ZX_SalaryItems',
'COLUMN', N'IsEveryMonth'
GO

EXEC sp_addextendedproperty
'MS_Description', N'sap工资导入表代码',
'SCHEMA', N'dbo',
'TABLE', N'ZX_SalaryItems',
'COLUMN', N'SapCode'
GO

EXEC sp_addextendedproperty
'MS_Description', N'是否是工会会费基数项目',
'SCHEMA', N'dbo',
'TABLE', N'ZX_SalaryItems',
'COLUMN', N'TradeUnionBase'