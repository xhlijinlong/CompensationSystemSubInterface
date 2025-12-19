CREATE TABLE [dbo].[ZX_SalaryItems] (
  [ItemId] int  NOT NULL,
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
  CONSTRAINT [PK_ZX_SalaryItems] PRIMARY KEY CLUSTERED ([ItemId])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO

ALTER TABLE [dbo].[ZX_SalaryItems] SET (LOCK_ESCALATION = TABLE)
GO

EXEC sp_addextendedproperty
'MS_Description', N'是否是工会基数项目',
'SCHEMA', N'dbo',
'TABLE', N'ZX_SalaryItems',
'COLUMN', N'TradeUnionBase'