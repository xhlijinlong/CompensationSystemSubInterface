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