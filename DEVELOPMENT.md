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

1.变动 ✅已完成
1.1 变动可撤回某员工的最新的一条变动记录 ✅已完成
(涉及到两种情况,一种是变动时间还没到,另一种是变动时间已经到了,两种情况的撤回逻辑不同)
1.2 到达变动时间才做修改员工信息表操作 ✅已完成
(当前是保存变更信息的时候就修改了员工信息表,我想的是程序设置一个定时任务,在每天凌晨的时候检查变动数据,最好是使用数据库的时间进行查询,防止用户修改本地时间)
1.3 变动增加入职选项,入职的逻辑与平调一致 ✅已完成
2.修改 ✅已完成
2.1 修改界面新增两个单选按钮(试用和应届 对应的字段则是 ZX_config_yg 表中的 shiyong 和 yjbys) ✅已完成
如果勾选按钮则对应的字段值为1 不勾选则为0 
3.新增 ✅已完成
3.1 增加入职按钮,填写员工基本信息完成信息保存(基本信息就按照修改员工信息的界面来设计,不同的是所有的字段都需要用户填写,所有字段都默认为空) ✅已完成
不需要填写但是默认写入的字段:
字段名 初始值 备注
zaizhi 1 在职
zaigang 1 在岗
fanpin 0 返聘
jiediao 0 借调
kaoqin 1 是否读取考勤信息
xinchou 1 是否计算薪酬信息
xuhao 序号读取当前最大序号+1即可
4.状态栏 ✅已完成
以下界面中的表格查询数据后需要显示状态栏,状态栏显示查询到的数据条数.
UserControl_EmpMaint
UserControl_EmpQuery
UserControl_EmpRsQuery
UserControl_PfmcQuery
UserControl_EmpCgQuery
UserControl_SalaryQuery
UserControl_SalaryStatistics
5.表格表头字段点击可以排序
需要排序的字段,有些字段需按照指定序列排列
6.表格行支持拖拽移动 ✅已完成
UserControl_EmpMaint页面的表格默认排序是根据员工的序号升序排列,拖拽移动后需要更新员工的序号,并且需要保存到数据库中.
7.以下界面的表格不再展示层级信息 ✅已完成
UserControl_EmpQuery
UserControl_EmpRsQuery
7.1 UserControl_EmpMaint 界面表格的层级信息根据调用方的CanChange值来决定是否显示
如果CanChange为false,则不显示层级信息
如果CanChange为true,则显示层级信息
7.2 WpfEmpMaint.xaml 的层级信息不再展示
8.调整列宽 ✅已完成
当前默认列宽是100,有些列需要调整:
姓名,民族列宽改为 64
序号,性别,学位,层级,属相,年龄 列宽改为 56
UserControl_PfmcQuery 页面的表格中,后面跟的年份列宽设置为 80
9.调整变动项目列表顺序 ✅已完成
晋升,平调,入职,变更,入职,离职,降职
10.人员信息和工资汇总查询供部门主任使用 ✅已完成
UserControl_EmpQuery和UserControl_SalaryStatistics
接收主程序的部门信息,如果主程序没有传递部门信息,则显示所有部门
具体实现可以参考UserControl_EmpMaint接收变动权限的逻辑
11.不显示层级 ✅已完成
员工信息查询页 UserControl_EmpQuery
员工离职信息页 UserControl_EmpRsQuery
12.年终奖 上一年 下一年 4.30




## 潜在问题记录

## 问题修复记录
