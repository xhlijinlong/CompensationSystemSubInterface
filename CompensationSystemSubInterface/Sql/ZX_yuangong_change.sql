CREATE TABLE [dbo].[ZX_yuangong_change] (
  [id] int  IDENTITY(1,1) NOT NULL,
  [ygid] int  NULL,
  [changeType] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [changeTime] datetime  NULL,
  [oldxlid] int  NULL,
  [oldxl] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [oldbmid] int  NULL,
  [oldbm] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [oldzwid] int  NULL,
  [oldzw] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [oldcjid] int  NULL,
  [oldcj] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [newxlid] int  NULL,
  [newxl] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [newbmid] int  NULL,
  [newbm] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [newzwid] int  NULL,
  [newzw] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [newcjid] int  NULL,
  [newcj] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [wageStart] datetime  NULL,
  [wageEnd] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  CONSTRAINT [PK_ZX_yuangong_change] PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO

ALTER TABLE [dbo].[ZX_yuangong_change] SET (LOCK_ESCALATION = TABLE)