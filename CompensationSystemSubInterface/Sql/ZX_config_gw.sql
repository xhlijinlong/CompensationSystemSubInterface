CREATE TABLE [dbo].[ZX_config_gw] (
  [id] int  IDENTITY(1,1) NOT NULL,
  [gwname] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [MinLevelId] int  NULL,
  [zjid] int  NULL,
  [lingdao] bit  NULL,
  [bumenfzr] bit  NULL,
  [zhongceng] bit  NULL,
  [zerenxm] float(53)  NULL,
  [zerenxm_xd] float(53)  NULL,
  [IsEnabled] bit DEFAULT 1 NULL,
  [DeleteType] bit DEFAULT 0 NULL,
  [DisplayOrder] int  NULL,
  CONSTRAINT [PK_ZX_config_gw_1] PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO

ALTER TABLE [dbo].[ZX_config_gw] SET (LOCK_ESCALATION = TABLE)
GO

EXEC sp_addextendedproperty
'MS_Description', N'职务ID',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_gw',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'职务名称',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_gw',
'COLUMN', N'gwname'
GO

EXEC sp_addextendedproperty
'MS_Description', N'是否可用(1可用0不可用)',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_gw',
'COLUMN', N'IsEnabled'
GO

EXEC sp_addextendedproperty
'MS_Description', N'是否删除(1删除0未删除)',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_gw',
'COLUMN', N'DeleteType'
GO

EXEC sp_addextendedproperty
'MS_Description', N'展示顺序',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_gw',
'COLUMN', N'DisplayOrder'