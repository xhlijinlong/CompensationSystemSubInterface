using CompensationSystemSubInterface.Models;
using CompensationSystemSubInterface.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CompensationSystemSubInterface {
    /// <summary>
    /// 新员工入职窗口
    /// 用于添加新员工的基本信息
    /// </summary>
    public partial class WpfEmpAdd : Window {
        /// <summary>
        /// 员工服务类实例，用于数据库操作
        /// </summary>
        private EmpService _service = new EmpService();

        /// <summary>
        /// 构造函数：初始化新员工入职窗口
        /// </summary>
        public WpfEmpAdd() {
            InitializeComponent();
            this.Loaded += WpfEmpAdd_Loaded;
        }

        /// <summary>
        /// 窗口加载完成事件处理
        /// 初始化所有下拉框数据源
        /// </summary>
        private void WpfEmpAdd_Loaded(object sender, RoutedEventArgs e) {
            LoadDropdowns();
        }

        /// <summary>
        /// 加载下拉框数据源
        /// 包括性别、部门、序列、职务、层级的下拉框数据
        /// </summary>
        private void LoadDropdowns() {
            try {
                // 1. 性别下拉框
                cbGender.ItemsSource = new string[] { "男", "女" };

                // 2. 部门下拉框 (头部和关联)
                var deptList = _service.GetComboList("ZX_config_bm");
                cbDeptHeader.ItemsSource = deptList;
                cbDeptHeader.DisplayMemberPath = "Name";
                cbDeptHeader.SelectedValuePath = "Id";

                // 3. 序列下拉框
                cbSequence.ItemsSource = _service.GetComboList("ZX_config_xl");
                cbSequence.DisplayMemberPath = "Name";
                cbSequence.SelectedValuePath = "Id";
                cbSequence.SelectionChanged += CbSequence_SelectionChanged;

                // 4. 职务下拉框
                cbJob.ItemsSource = _service.GetComboList("ZX_config_gw");
                cbJob.DisplayMemberPath = "Name";
                cbJob.SelectedValuePath = "Id";

                // 5. 层级下拉框
                cbLevel.ItemsSource = _service.GetComboList("ZX_config_cj");
                cbLevel.DisplayMemberPath = "Name";
                cbLevel.SelectedValuePath = "Id";

            } catch (Exception ex) {
                MessageBox.Show("加载下拉框数据失败: " + ex.Message);
            }
        }

        /// <summary>
        /// 序列下拉框选择变更事件
        /// 根据选择的序列联动刷新部门列表
        /// </summary>
        private void CbSequence_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (cbSequence.SelectedValue != null) {
                int seqId = Convert.ToInt32(cbSequence.SelectedValue);
                var deptList = _service.GetDeptListBySeq(seqId);
                cbDeptHeader.ItemsSource = deptList;

                // 如果只有一个部门,自动选中
                if (deptList.Count == 1) {
                    cbDeptHeader.SelectedIndex = 0;
                } else {
                    cbDeptHeader.SelectedIndex = -1;
                }
            }
        }

        /// <summary>
        /// 保存按钮点击事件
        /// 收集界面数据并保存到数据库
        /// </summary>
        private void btnSave_Click(object sender, RoutedEventArgs e) {
            try {
                // 1. 校验必填字段
                if (string.IsNullOrWhiteSpace(txtEmpNo.Text)) {
                    MessageBox.Show("请输入员工编号");
                    txtEmpNo.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtNameHeader.Text)) {
                    MessageBox.Show("请输入员工姓名");
                    txtNameHeader.Focus();
                    return;
                }

                if (cbGender.SelectedValue == null) {
                    MessageBox.Show("请选择性别");
                    cbGender.Focus();
                    return;
                }

                if (cbDeptHeader.SelectedValue == null) {
                    MessageBox.Show("请选择部门");
                    cbDeptHeader.Focus();
                    return;
                }

                if (cbSequence.SelectedValue == null) {
                    MessageBox.Show("请选择序列");
                    cbSequence.Focus();
                    return;
                }

                if (cbJob.SelectedValue == null) {
                    MessageBox.Show("请选择职务");
                    cbJob.Focus();
                    return;
                }

                if (cbLevel.SelectedValue == null) {
                    MessageBox.Show("请选择层级");
                    cbLevel.Focus();
                    return;
                }

                // 2. 收集界面数据到实体对象
                EmpDetail empToSave = new EmpDetail {
                    EmployeeNo = txtEmpNo.Text.Trim(),
                    Name = txtNameHeader.Text.Trim(),

                    // 基础身份
                    Gender = cbGender.Text,
                    Nation = txtNation.Text,
                    Politic = txtPolitic.Text,
                    Zodiac = txtZodiac.Text,
                    Age = int.TryParse(txtAge.Text, out int age) ? age : 0,
                    Birthday = dpBirthday.SelectedDate,

                    // 敏感信息
                    IdCard = txtIdCard.Text,
                    BankCard = txtBankCard.Text,

                    // 联系信息
                    Phone = txtPhone.Text,

                    // 组织信息
                    DeptId = Convert.ToInt32(cbDeptHeader.SelectedValue),
                    SeqId = Convert.ToInt32(cbSequence.SelectedValue),
                    JobId = Convert.ToInt32(cbJob.SelectedValue),
                    LevelId = Convert.ToInt32(cbLevel.SelectedValue),

                    // 工作时间
                    WorkStart = dpWorkStart.SelectedDate,
                    JoinDate = dpJoinDate.SelectedDate,
                    PostDate = dpPostDate.SelectedDate,

                    // 学历技能
                    Education = txtEducation.Text,
                    Degree = txtDegree.Text,
                    TechSpecialty = txtTech.Text,
                    TitleLevel = txtTitleLevel.Text,
                    TitleDate = dpTitleDate.SelectedDate,
                    Skill = txtSkill.Text,
                    SkillDate = dpSkillDate.SelectedDate,

                    // 状态标记
                    IsProbation = cbProbation.IsChecked == true,
                    IsFreshGraduate = cbFreshGraduate.IsChecked == true
                };

                // 3. 调用 Service 保存
                _service.InsertEmployee(empToSave);

                MessageBox.Show("入职信息保存成功！");
                this.DialogResult = true;
                this.Close();

            } catch (Exception ex) {
                MessageBox.Show("保存失败: " + ex.Message);
            }
        }

        /// <summary>
        /// 取消按钮点击事件
        /// 关闭窗口，不保存任何数据
        /// </summary>
        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}
