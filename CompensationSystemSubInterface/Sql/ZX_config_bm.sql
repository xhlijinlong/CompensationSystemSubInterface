CREATE TABLE [dbo].[ZX_config_bm] (
  [id] int  IDENTITY(1,1) NOT NULL,
  [bmname] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [xlid] int  NULL,
  [fgryid] int  NULL,
  [fzrid] int  NULL,
  [IsEnabled] bit DEFAULT 1 NULL,
  [DeleteType] bit DEFAULT 0 NULL,
  [DisplayOrder] int  NULL,
  CONSTRAINT [PK_ZX_config_bm] PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO

ALTER TABLE [dbo].[ZX_config_bm] SET (LOCK_ESCALATION = TABLE)
GO

EXEC sp_addextendedproperty
'MS_Description', N'部门ID',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_bm',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'部门名称',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_bm',
'COLUMN', N'bmname'
GO

EXEC sp_addextendedproperty
'MS_Description', N'序列ID',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_bm',
'COLUMN', N'xlid'
GO

EXEC sp_addextendedproperty
'MS_Description', N'是否可用(1可用0不可用)',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_bm',
'COLUMN', N'IsEnabled'
GO

EXEC sp_addextendedproperty
'MS_Description', N'是否删除(1删除0未删除)',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_bm',
'COLUMN', N'DeleteType'
GO

EXEC sp_addextendedproperty
'MS_Description', N'展示顺序',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_bm',
'COLUMN', N'DisplayOrder'