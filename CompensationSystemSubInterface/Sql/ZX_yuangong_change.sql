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
GO

EXEC sp_addextendedproperty
'MS_Description', N'员工变动ID',
'SCHEMA', N'dbo',
'TABLE', N'ZX_yuangong_change',
'COLUMN', N'id'
GO

EXEC sp_addextendedproperty
'MS_Description', N'员工ID',
'SCHEMA', N'dbo',
'TABLE', N'ZX_yuangong_change',
'COLUMN', N'ygid'
GO

EXEC sp_addextendedproperty
'MS_Description', N'变动类型(入职,晋升,降职,平调,变更,离职)',
'SCHEMA', N'dbo',
'TABLE', N'ZX_yuangong_change',
'COLUMN', N'changeType'
GO

EXEC sp_addextendedproperty
'MS_Description', N'变动时间',
'SCHEMA', N'dbo',
'TABLE', N'ZX_yuangong_change',
'COLUMN', N'changeTime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'原序列ID',
'SCHEMA', N'dbo',
'TABLE', N'ZX_yuangong_change',
'COLUMN', N'oldxlid'
GO

EXEC sp_addextendedproperty
'MS_Description', N'原序列名称',
'SCHEMA', N'dbo',
'TABLE', N'ZX_yuangong_change',
'COLUMN', N'oldxl'
GO

EXEC sp_addextendedproperty
'MS_Description', N'原部门ID',
'SCHEMA', N'dbo',
'TABLE', N'ZX_yuangong_change',
'COLUMN', N'oldbmid'
GO

EXEC sp_addextendedproperty
'MS_Description', N'原部门名称',
'SCHEMA', N'dbo',
'TABLE', N'ZX_yuangong_change',
'COLUMN', N'oldbm'
GO

EXEC sp_addextendedproperty
'MS_Description', N'原职务ID',
'SCHEMA', N'dbo',
'TABLE', N'ZX_yuangong_change',
'COLUMN', N'oldzwid'
GO

EXEC sp_addextendedproperty
'MS_Description', N'原职务名称',
'SCHEMA', N'dbo',
'TABLE', N'ZX_yuangong_change',
'COLUMN', N'oldzw'
GO

EXEC sp_addextendedproperty
'MS_Description', N'原层级ID',
'SCHEMA', N'dbo',
'TABLE', N'ZX_yuangong_change',
'COLUMN', N'oldcjid'
GO

EXEC sp_addextendedproperty
'MS_Description', N'原层级名称',
'SCHEMA', N'dbo',
'TABLE', N'ZX_yuangong_change',
'COLUMN', N'oldcj'
GO

EXEC sp_addextendedproperty
'MS_Description', N'现序列ID',
'SCHEMA', N'dbo',
'TABLE', N'ZX_yuangong_change',
'COLUMN', N'newxlid'
GO

EXEC sp_addextendedproperty
'MS_Description', N'现序列名称',
'SCHEMA', N'dbo',
'TABLE', N'ZX_yuangong_change',
'COLUMN', N'newxl'
GO

EXEC sp_addextendedproperty
'MS_Description', N'现部门ID',
'SCHEMA', N'dbo',
'TABLE', N'ZX_yuangong_change',
'COLUMN', N'newbmid'
GO

EXEC sp_addextendedproperty
'MS_Description', N'现部门名称',
'SCHEMA', N'dbo',
'TABLE', N'ZX_yuangong_change',
'COLUMN', N'newbm'
GO

EXEC sp_addextendedproperty
'MS_Description', N'现职务ID',
'SCHEMA', N'dbo',
'TABLE', N'ZX_yuangong_change',
'COLUMN', N'newzwid'
GO

EXEC sp_addextendedproperty
'MS_Description', N'现职务名称',
'SCHEMA', N'dbo',
'TABLE', N'ZX_yuangong_change',
'COLUMN', N'newzw'
GO

EXEC sp_addextendedproperty
'MS_Description', N'现层级ID',
'SCHEMA', N'dbo',
'TABLE', N'ZX_yuangong_change',
'COLUMN', N'newcjid'
GO

EXEC sp_addextendedproperty
'MS_Description', N'现层级名称',
'SCHEMA', N'dbo',
'TABLE', N'ZX_yuangong_change',
'COLUMN', N'newcj'
GO

EXEC sp_addextendedproperty
'MS_Description', N'薪资起始',
'SCHEMA', N'dbo',
'TABLE', N'ZX_yuangong_change',
'COLUMN', N'wageStart'
GO

EXEC sp_addextendedproperty
'MS_Description', N'薪资截止',
'SCHEMA', N'dbo',
'TABLE', N'ZX_yuangong_change',
'COLUMN', N'wageEnd'