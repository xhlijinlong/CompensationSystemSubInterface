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