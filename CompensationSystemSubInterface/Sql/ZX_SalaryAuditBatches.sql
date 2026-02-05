CREATE TABLE [dbo].[ZX_SalaryAuditBatches] (
  [BatchId] int  IDENTITY(1,1) NOT NULL,
  [SalaryMonth] datetime  NULL,
  [InitiatorId] int  NULL,
  [InitiateTime] datetime  NULL,
  [PreliminaryAudit] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [PreliminaryAuditId] int  NULL,
  [PreliminaryAuditTime] datetime  NULL,
  [FinalAudit] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [FinalAuditId] int  NULL,
  [FinalAuditTime] datetime  NULL,
  [OverallStatus] int  NULL,
  [ObjId] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [Wfrule] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [Zwfnum] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  CONSTRAINT [PK_ZX_SalaryAuditBatches] PRIMARY KEY CLUSTERED ([BatchId])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO

ALTER TABLE [dbo].[ZX_SalaryAuditBatches] SET (LOCK_ESCALATION = TABLE)
GO

EXEC sp_addextendedproperty
'MS_Description', N'审批ID',
'SCHEMA', N'dbo',
'TABLE', N'ZX_SalaryAuditBatches',
'COLUMN', N'BatchId'
GO

EXEC sp_addextendedproperty
'MS_Description', N'薪资月份',
'SCHEMA', N'dbo',
'TABLE', N'ZX_SalaryAuditBatches',
'COLUMN', N'SalaryMonth'
GO

EXEC sp_addextendedproperty
'MS_Description', N'发起人ID',
'SCHEMA', N'dbo',
'TABLE', N'ZX_SalaryAuditBatches',
'COLUMN', N'InitiatorId'
GO

EXEC sp_addextendedproperty
'MS_Description', N'发起时间',
'SCHEMA', N'dbo',
'TABLE', N'ZX_SalaryAuditBatches',
'COLUMN', N'InitiateTime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'初审状态',
'SCHEMA', N'dbo',
'TABLE', N'ZX_SalaryAuditBatches',
'COLUMN', N'PreliminaryAudit'
GO

EXEC sp_addextendedproperty
'MS_Description', N'初审人ID',
'SCHEMA', N'dbo',
'TABLE', N'ZX_SalaryAuditBatches',
'COLUMN', N'PreliminaryAuditId'
GO

EXEC sp_addextendedproperty
'MS_Description', N'初审时间',
'SCHEMA', N'dbo',
'TABLE', N'ZX_SalaryAuditBatches',
'COLUMN', N'PreliminaryAuditTime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'终审状态',
'SCHEMA', N'dbo',
'TABLE', N'ZX_SalaryAuditBatches',
'COLUMN', N'FinalAudit'
GO

EXEC sp_addextendedproperty
'MS_Description', N'终审人ID',
'SCHEMA', N'dbo',
'TABLE', N'ZX_SalaryAuditBatches',
'COLUMN', N'FinalAuditId'
GO

EXEC sp_addextendedproperty
'MS_Description', N'终审时间',
'SCHEMA', N'dbo',
'TABLE', N'ZX_SalaryAuditBatches',
'COLUMN', N'FinalAuditTime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'整体状态(0已生成,1已提交,2初审通过,3终审通过,4驳回,5已发布)',
'SCHEMA', N'dbo',
'TABLE', N'ZX_SalaryAuditBatches',
'COLUMN', N'OverallStatus'