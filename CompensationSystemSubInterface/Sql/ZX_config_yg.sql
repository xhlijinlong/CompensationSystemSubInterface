CREATE TABLE [dbo].[ZX_config_yg] (
  [id] int  NOT NULL,
  [yonghuming] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [yuangongbh] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [xingming] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [xingbie] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [minzu] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [zhengzhimm] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [xueli] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [xuewei] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [shenfenzheng] varchar(500) COLLATE Chinese_PRC_CI_AS  NULL,
  [chushengrq] datetime  NULL,
  [nianling] int  NULL,
  [shuxing] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [gongzuosj] datetime  NULL,
  [rusisj] datetime  NULL,
  [lizhisj] datetime  NULL,
  [gangweisj] datetime  NULL,
  [bmid] int  NULL,
  [xlid] int  NULL,
  [gwid] int  NULL,
  [dygwid] int  NULL,
  [zxdygwid] int  NULL,
  [gwzjid] int  NULL,
  [cjid] int  NULL,
  [xinjigz] float(53)  NULL,
  [renyuanlb] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [zhuanyejn] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [jinengsj] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [zhuanyejs] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [zhichengdj] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [zhichengsj] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [zaizhi] bit  NULL,
  [zaigang] bit  NULL,
  [fanpin] bit  NULL,
  [jiediao] bit  NULL,
  [shiyong] bit  NULL,
  [hunyinzk] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [qishisfzrq] datetime  NULL,
  [jieshusfzrq] datetime  NULL,
  [hujidz] varchar(500) COLLATE Chinese_PRC_CI_AS  NULL,
  [xianzhuzhi] varchar(500) COLLATE Chinese_PRC_CI_AS  NULL,
  [lianxidh] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [gongzikh] varchar(500) COLLATE Chinese_PRC_CI_AS  NULL,
  [gongzuonx] int  NULL,
  [nianjiats] float(53)  NULL,
  [yanglaoyg] float(53) DEFAULT 0 NULL,
  [yanglaoqy] float(53) DEFAULT 0 NULL,
  [shiyeyg] float(53) DEFAULT 0 NULL,
  [shiyeqy] float(53) DEFAULT 0 NULL,
  [yiliaoyg] float(53) DEFAULT 0 NULL,
  [yiliaoqy] float(53) DEFAULT 0 NULL,
  [gongshangqy] float(53) DEFAULT 0 NULL,
  [shengyuqy] float(53) DEFAULT 0 NULL,
  [zhufangyg] float(53) DEFAULT 0 NULL,
  [zhufangqy] float(53) DEFAULT 0 NULL,
  [qiyeyg] float(53) DEFAULT 0 NULL,
  [qiyeqy] float(53) DEFAULT 0 NULL,
  [rtxuser] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [kaoqin] bit  NULL,
  [xinchou] bit  NULL,
  [sczid] int  NULL,
  [xuhao] int  NULL,
  [shanchu] bit  NULL,
  [password] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [gzdrb_grbx] bit  NULL,
  [yjbys] bit  NULL,
  [juzhensj] datetime  NULL,
  [isStock] bit  NULL,
  [stockRole] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [caigou] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [kuguan] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [ygqr] bit  NULL,
  [isWaterMark] bit  NULL,
  [wmLastLoginTime] datetime  NULL,
  [wmUseCount] int  NULL,
  [iswmAdmin] bit  NULL,
  [sapSystemId] varchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [newChange] bit  NULL,
  CONSTRAINT [PK_ZX_config_yg] PRIMARY KEY CLUSTERED ([id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
)  
ON [PRIMARY]
GO

ALTER TABLE [dbo].[ZX_config_yg] SET (LOCK_ESCALATION = TABLE)
GO

EXEC sp_addextendedproperty
'MS_Description', N'域用户名',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'yonghuming'
GO

EXEC sp_addextendedproperty
'MS_Description', N'员工编号',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'yuangongbh'
GO

EXEC sp_addextendedproperty
'MS_Description', N'姓名',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'xingming'
GO

EXEC sp_addextendedproperty
'MS_Description', N'性别',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'xingbie'
GO

EXEC sp_addextendedproperty
'MS_Description', N'民族',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'minzu'
GO

EXEC sp_addextendedproperty
'MS_Description', N'政治面貌',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'zhengzhimm'
GO

EXEC sp_addextendedproperty
'MS_Description', N'学历',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'xueli'
GO

EXEC sp_addextendedproperty
'MS_Description', N'学位',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'xuewei'
GO

EXEC sp_addextendedproperty
'MS_Description', N'身份证号',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'shenfenzheng'
GO

EXEC sp_addextendedproperty
'MS_Description', N'出生日期',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'chushengrq'
GO

EXEC sp_addextendedproperty
'MS_Description', N'年龄',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'nianling'
GO

EXEC sp_addextendedproperty
'MS_Description', N'属相',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'shuxing'
GO

EXEC sp_addextendedproperty
'MS_Description', N'参加工作时间',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'gongzuosj'
GO

EXEC sp_addextendedproperty
'MS_Description', N'入本单位时间',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'rusisj'
GO

EXEC sp_addextendedproperty
'MS_Description', N'现岗位时间',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'gangweisj'
GO

EXEC sp_addextendedproperty
'MS_Description', N'所在部门ID',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'bmid'
GO

EXEC sp_addextendedproperty
'MS_Description', N'序列ID',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'xlid'
GO

EXEC sp_addextendedproperty
'MS_Description', N'岗位ID',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'gwid'
GO

EXEC sp_addextendedproperty
'MS_Description', N'待遇岗位ID',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'dygwid'
GO

EXEC sp_addextendedproperty
'MS_Description', N'层级ID',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'cjid'
GO

EXEC sp_addextendedproperty
'MS_Description', N'薪级工资',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'xinjigz'
GO

EXEC sp_addextendedproperty
'MS_Description', N'人员类别',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'renyuanlb'
GO

EXEC sp_addextendedproperty
'MS_Description', N'专业技能',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'zhuanyejn'
GO

EXEC sp_addextendedproperty
'MS_Description', N'专业技术职务任职资格',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'zhuanyejs'
GO

EXEC sp_addextendedproperty
'MS_Description', N'职称等级',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'zhichengdj'
GO

EXEC sp_addextendedproperty
'MS_Description', N'职称取得时间',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'zhichengsj'
GO

EXEC sp_addextendedproperty
'MS_Description', N'在职',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'zaizhi'
GO

EXEC sp_addextendedproperty
'MS_Description', N'在岗',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'zaigang'
GO

EXEC sp_addextendedproperty
'MS_Description', N'返聘',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'fanpin'
GO

EXEC sp_addextendedproperty
'MS_Description', N'借调',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'jiediao'
GO

EXEC sp_addextendedproperty
'MS_Description', N'试用',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'shiyong'
GO

EXEC sp_addextendedproperty
'MS_Description', N'婚姻状况',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'hunyinzk'
GO

EXEC sp_addextendedproperty
'MS_Description', N'身份证起始日期',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'qishisfzrq'
GO

EXEC sp_addextendedproperty
'MS_Description', N'身份证结束日期',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'jieshusfzrq'
GO

EXEC sp_addextendedproperty
'MS_Description', N'户籍地址（身份证地址）',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'hujidz'
GO

EXEC sp_addextendedproperty
'MS_Description', N'现住址',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'xianzhuzhi'
GO

EXEC sp_addextendedproperty
'MS_Description', N'联系电话',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'lianxidh'
GO

EXEC sp_addextendedproperty
'MS_Description', N'工资卡号',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'gongzikh'
GO

EXEC sp_addextendedproperty
'MS_Description', N'工作年限',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'gongzuonx'
GO

EXEC sp_addextendedproperty
'MS_Description', N'年假天数',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'nianjiats'
GO

EXEC sp_addextendedproperty
'MS_Description', N'生产组ID',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'sczid'
GO

EXEC sp_addextendedproperty
'MS_Description', N'工资导入表是否显示个人保险',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'gzdrb_grbx'
GO

EXEC sp_addextendedproperty
'MS_Description', N'应届毕业生标记',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'yjbys'
GO

EXEC sp_addextendedproperty
'MS_Description', N'库存权限',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'isStock'
GO

EXEC sp_addextendedproperty
'MS_Description', N'采购标记',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'stockRole'
GO

EXEC sp_addextendedproperty
'MS_Description', N'员工确认信息',
'SCHEMA', N'dbo',
'TABLE', N'ZX_config_yg',
'COLUMN', N'ygqr'