# 开发说明

## 项目背景

人教资源中心是人教社下属单位,可以看作是人教社的一个部门.中心具有独立结算能力,因此需要独立的薪酬结算系统.

## 项目结构

客户端使用 .NET Framework 4.6.2 + NPOI 2.5.6 框架, 主界面使用 WinForms 开发.

说明: 目前我在开发子界面, 主界面的开发工作由其他同事完成. 之后我的界面组件打包成 nupkg 文件, 供主界面引用. 我的界面组件的名称为 CompensationSystemSubInterface.

技术特点:

- 主体控件: WinForms UserControl
- 弹窗/筛选控件: WPF (通过 ElementHost 嵌入到 WinForms)
- 数据库: SQL Server
- Excel 导出: NPOI

用到的表结构sql语句在项目中的 sql 文件夹中.

## 业务数据规范

- 性别

  男

  女

- 学历类别

  博士研究生

  硕士研究生

  本科

  专科

  中专

  高中及以下

- 学位类别

  博士

  硕士

  学士

  无

- 政治面貌

  中共党员

  中共预备党员

  团员

  群众

- 民族

  XX 族

- 同义词

  员工编号 - 编号 - 员工号

  职务 - 岗位

  主任 - 中心主任 - 领导班子的主任职务

  副主任 - 中心副主任 - 领导班子的副主任职务

  部门主任 - 各个部门的主任职务

  部门副主任 - 各个部门的副主任职务

- 部门

  领导班子

  综合管理部

  资源事业部

  质量管控部

  制作部

  输出部

  技术部

- 职务

  主任

  副主任

  部门主任

  部门副主任

  高级主管

  主管

  助理

### 待开发需求

1. 员工信息管理中，员工表的列按照指定顺序查询展示
   涉及页面 UserControl_EmpMaint UserControl_EmpQuery UserControl_EmpRsQuery(离职日期保持最后一列位置不变)
   员工编号 部门 职务 姓名 性别 民族 政治面貌 学历 学位 出生日期 参加工作时间 入社时间 任现岗位时间 证件号码 专业技术 职称等级 取得时间 专业技能 技能时间 序列 层级 属相 年龄 联系电话 工资卡号

2. 还有2个工资统计中，把姓名和编号列的查询展示顺序对调一下
   涉及页面 UserControl_SalaryQuery UserControl_SalaryStatistics

3. 表格中的字体和页面窗体上的字体统一调整为 微软雅黑 12pt
   涉及页面 UserControl_EmpMaint UserControl_EmpQuery UserControl_EmpRsQuery UserControl_PfmcQuery UserControl_EmpCgQuery UserControl_SalaryQuery UserControl_SalaryStatistics

4. 表格列宽显示过窄的问题

## 潜在问题记录

## 问题修复记录
