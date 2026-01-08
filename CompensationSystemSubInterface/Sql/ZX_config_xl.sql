CREATE TABLE [dbo].[ZX_config_xl] (
  [id] int  IDENTITY(1,1) NOT NULL,
  [xlname] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [gongzilb] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [beizhu] varchar(50) COLLATE Chinese_PRC_CI_AS DEFAULT 0 NULL,
  [IsEnabled] bit DEFAULT 1 NULL,
  [DeleteType] bit DEFAULT 0 NULL,
  CONSTRAINT [PK_ZX_config] PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO

ALTER TABLE [dbo].[ZX_config_xl] SET (LOCK_ESCALATION = TABLE)
GO

EXEC sp_addextendedproperty
'MS_Description', N'序列ID',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_xl',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'序列名称',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_xl',
'COLUMN', N'xlname'
GO

EXEC sp_addextendedproperty
'MS_Description', N'是否可用(1可用0不可用)',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_xl',
'COLUMN', N'IsEnabled'
GO

EXEC sp_addextendedproperty
'MS_Description', N'是否删除(1删除0未删除)',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_xl',
'COLUMN', N'DeleteType'