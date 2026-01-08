CREATE TABLE [dbo].[ZX_config_cj] (
  [id] int  IDENTITY(1,1) NOT NULL,
  [cjname] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [cjgw] varchar(500) COLLATE Chinese_PRC_CI_AS  NULL,
  [DeleteType] bit DEFAULT 0 NULL,
  CONSTRAINT [PK_ZX_config_cj] PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO

ALTER TABLE [dbo].[ZX_config_cj] SET (LOCK_ESCALATION = TABLE)
GO

EXEC sp_addextendedproperty
'MS_Description', N'层级ID',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_cj',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'层级名称',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_cj',
'COLUMN', N'cjname'
GO

EXEC sp_addextendedproperty
'MS_Description', N'是否删除(1删除0未删除)',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_cj',
'COLUMN', N'DeleteType'