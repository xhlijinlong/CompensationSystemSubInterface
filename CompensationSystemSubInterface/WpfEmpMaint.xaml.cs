using CompensationSystemSubInterface;
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
    /// 员工信息维护窗口
    /// 用于查看和修改员工的基本信息（不包括组织信息的变动）
    /// </summary>
    public partial class WpfEmpMaint : Window {
        /// <summary>
        /// 当前操作的员工ID
        /// </summary>
        private int _empId;
        
        /// <summary>
        /// 员工服务类实例，用于数据库操作
        /// </summary>
        private EmpService _service = new EmpService();
        
        /// <summary>
        /// 当前员工的详细信息对象（用于数据绑定和回显）
        /// </summary>
        private EmpDetail _currentEmp;

        /// <summary>
        /// 事件：请求打开员工变动窗口
        /// 当点击"变动"按钮时触发，通知外部调用者
        /// </summary>
        public event Action<int> OpenChangeWindowRequested;

        // 是否显示变动功能（由主程序控制）
        private bool _canChange = true;

        /// <summary>
        /// 获取或设置是否允许使用变动功能
        /// 设为false时隐藏变动按钮
        /// </summary>
        public bool CanChange {
            get => _canChange;
            set {
                _canChange = value;
                if (btnChange != null) btnChange.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 构造函数：初始化员工维护窗口
        /// </summary>
        /// <param name="empId">要维护的员工ID</param>
        public WpfEmpMaint(int empId) {
            InitializeComponent();
            _empId = empId;
            this.Loaded += WpfEmpMaint_Loaded;
        }

        /// <summary>
        /// 窗口加载完成事件处理
        /// 初始化下拉框数据源并加载员工详细信息
        /// </summary>
        private void WpfEmpMaint_Loaded(object sender, RoutedEventArgs e) {
            LoadDropdowns();
            LoadData();
        }

        /// <summary>
        /// 加载下拉框数据源
        /// 包括性别下拉框和组织信息下拉框（序列、职务、层级）
        /// </summary>
        private void LoadDropdowns() {
            // 1. 只有性别是下拉框，需要绑定数据源
            cbGender.ItemsSource = new string[] { "男", "女" };

            // 2. 组织信息 (保留下拉框用于 ID->Name 显示，虽然被锁定)
            try {
                // 如果数据库有问题，这里加个try-catch防止界面全崩
                cbSequence.ItemsSource = _service.GetComboList("ZX_config_xl");
                cbSequence.DisplayMemberPath = "Name"; cbSequence.SelectedValuePath = "Id";

                cbJob.ItemsSource = _service.GetComboList("ZX_config_gw");
                cbJob.DisplayMemberPath = "Name"; cbJob.SelectedValuePath = "Id";

                cbLevel.ItemsSource = _service.GetComboList("ZX_config_cj");
                cbLevel.DisplayMemberPath = "Name"; cbLevel.SelectedValuePath = "Id";
            } catch { /* 忽略下拉框加载错误，仅显示Text */ }
        }

        /// <summary>
        /// 加载员工详细数据并回显到界面控件
        /// 包括基础身份、组织职务、学历技能和联系信息等所有字段
        /// </summary>
        private void LoadData() {
            try {
                _currentEmp = _service.GetEmpDetailObj(_empId);
                if (_currentEmp == null) {
                    MessageBox.Show("未找到员工信息");
                    this.Close();
                    return;
                }

                // 头部
                txtEmpNo.Text = _currentEmp.EmployeeNo;
                txtNameHeader.Text = _currentEmp.Name;
                txtDeptHeader.Text = _currentEmp.DeptName;

                // === 1. 基础身份 ===
                cbGender.SelectedItem = _currentEmp.Gender; // 下拉框赋值

                txtNation.Text = _currentEmp.Nation;        // 文本框赋值
                txtPolitic.Text = _currentEmp.Politic;
                txtZodiac.Text = _currentEmp.Zodiac;
                txtAge.Text = _currentEmp.Age.ToString();
                dpBirthday.SelectedDate = _currentEmp.Birthday;
                txtIdCard.Text = _currentEmp.IdCard;

                // === 2. 组织职务 (锁定区域) ===
                cbSequence.SelectedValue = _currentEmp.SeqId;
                cbJob.SelectedValue = _currentEmp.JobId;
                cbLevel.SelectedValue = _currentEmp.LevelId;

                dpWorkStart.SelectedDate = _currentEmp.WorkStart;
                dpJoinDate.SelectedDate = _currentEmp.JoinDate;
                dpPostDate.SelectedDate = _currentEmp.PostDate;

                // 试用标记
                cbProbation.IsChecked = _currentEmp.IsProbation;

                // === 3. 学历技能 (文本框赋值) ===
                txtEducation.Text = _currentEmp.Education;
                txtDegree.Text = _currentEmp.Degree;
                txtTech.Text = _currentEmp.TechSpecialty;
                txtTitleLevel.Text = _currentEmp.TitleLevel;
                dpTitleDate.SelectedDate = _currentEmp.TitleDate;
                txtSkill.Text = _currentEmp.Skill;
                dpSkillDate.SelectedDate = _currentEmp.SkillDate;

                // 应届生标记
                cbFreshGraduate.IsChecked = _currentEmp.IsFreshGraduate;

                // === 4. 联系 ===
                txtPhone.Text = _currentEmp.Phone;
                txtBankCard.Text = _currentEmp.BankCard;

            } catch (Exception ex) {
                MessageBox.Show("加载数据失败: " + ex.Message);
            }
        }

        /// <summary>
        /// 保存按钮点击事件
        /// 收集界面数据并保存到数据库
        /// </summary>
        private void btnSave_Click(object sender, RoutedEventArgs e) {
            try {
                // 1. 收集界面数据到实体对象
                EmpDetail empToSave = new EmpDetail {
                    Id = _empId,
                    Name = txtNameHeader.Text,

                    // 基础身份
                    Gender = cbGender.Text,
                    Nation = txtNation.Text,
                    Politic = txtPolitic.Text,
                    Zodiac = txtZodiac.Text,
                    // 尝试解析年龄，防止空值报错
                    Age = int.TryParse(txtAge.Text, out int age) ? age : 0,
                    Birthday = dpBirthday.SelectedDate,

                    // 敏感信息
                    IdCard = txtIdCard.Text,
                    BankCard = txtBankCard.Text,

                    // 联系信息
                    Phone = txtPhone.Text,

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

                // 2. 调用 Service 保存
                _service.UpdateEmpBasicInfo(empToSave);

                MessageBox.Show("保存成功！");
                this.DialogResult = true;
                this.Close();

            } catch (Exception ex) {
                MessageBox.Show("保存失败: " + ex.Message);
            }
        }

        /// <summary>
        /// 取消按钮点击事件
        /// 关闭窗口，不保存任何修改
        /// </summary>
        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        /// <summary>
        /// 变动按钮点击事件
        /// 打开员工变动窗口，用于处理部门、职务等组织信息的变动
        /// 变动成功后会通知主界面刷新并关闭当前窗口
        /// </summary>
        private void btnChange_Click(object sender, RoutedEventArgs e) {
            // 打开变动窗口
            WpfEmpCg cgWindow = new WpfEmpCg(_empId);
            bool? result = cgWindow.ShowDialog();

            if (result == true) {
                // 变动成功，通知主界面刷新并关闭当前窗口
                this.DialogResult = true;
                this.Close();
            } else {
                // 变动取消，重新加载数据
                LoadData();
            }
        }

        /// <summary>
        /// 重置按钮点击事件
        /// 重新加载数据，放弃用户已修改但未保存的内容
        /// </summary>
        private void btnReset_Click(object sender, RoutedEventArgs e) {
            // 直接重新加载数据，覆盖用户已修改但未保存的内容
            LoadData();
        }
    }
}
