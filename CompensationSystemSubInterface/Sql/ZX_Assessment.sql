CREATE TABLE [dbo].[ZX_Assessment] (
  [id] int  IDENTITY(1,1) NOT NULL,
  [ygid] int  NULL,
  [year] int  NULL,
  [Assessment] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [updateBy] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [updateTime] datetime  NULL,
  CONSTRAINT [PK_ZX_Assessment] PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO

ALTER TABLE [dbo].[ZX_Assessment] SET (LOCK_ESCALATION = TABLE)
GO

EXEC sp_addextendedproperty
'MS_Description', N'绩效考核ID',
'SCHEMA', N'dbo',
'TABLE', N'ZX_Assessment',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'员工ID',
'SCHEMA', N'dbo',
'TABLE', N'ZX_Assessment',
'COLUMN', N'ygid'
GO

EXEC sp_addextendedproperty
'MS_Description', N'年度',
'SCHEMA', N'dbo',
'TABLE', N'ZX_Assessment',
'COLUMN', N'year'
GO

EXEC sp_addextendedproperty
'MS_Description', N'考核结果',
'SCHEMA', N'dbo',
'TABLE', N'ZX_Assessment',
'COLUMN', N'Assessment'
GO

EXEC sp_addextendedproperty
'MS_Description', N'修改人',
'SCHEMA', N'dbo',
'TABLE', N'ZX_Assessment',
'COLUMN', N'updateBy'
GO

EXEC sp_addextendedproperty
'MS_Description', N'修改时间',
'SCHEMA', N'dbo',
'TABLE', N'ZX_Assessment',
'COLUMN', N'updateTime'