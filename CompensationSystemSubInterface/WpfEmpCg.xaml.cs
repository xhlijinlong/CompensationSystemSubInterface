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
using System.Xml.Linq;

namespace CompensationSystemSubInterface {
    /// <summary>
    /// 员工变动窗口
    /// 用于处理员工的组织信息变动（部门、序列、职务、层级）和离职操作
    /// </summary>
    public partial class WpfEmpCg : Window {
        /// <summary>
        /// 当前操作的员工ID
        /// </summary>
        private int _empId;

        /// <summary>
        /// 员工服务类实例，用于数据库操作
        /// </summary>
        private EmpService _service = new EmpService();

        /// <summary>
        /// 员工原始数据（变动前的组织信息）
        /// 用于显示原部门、原序列、原职务、原层级等信息
        /// </summary>
        private EmployeeDetail _oldData;

        /// <summary>
        /// 构造函数：初始化员工变动窗口
        /// </summary>
        /// <param name="empId">要办理变动的员工ID</param>
        public WpfEmpCg(int empId) {
            InitializeComponent();
            _empId = empId;

            this.Loaded += WpfEmpCg_Loaded;
            cbChangeType.SelectionChanged += CbChangeType_SelectionChanged;
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += (s, e) => this.Close();
        }

        /// <summary>
        /// 窗口加载完成事件处理
        /// 初始化下拉框数据源并加载员工原始信息
        /// </summary>
        private void WpfEmpCg_Loaded(object sender, RoutedEventArgs e) {
            try {
                // 1. 先加载下拉框数据源
                LoadDropdowns();
                // 2. 再加载原数据，并设置默认选中项
                LoadOriginalData();
            } catch (Exception ex) {
                MessageBox.Show("界面加载出错: " + ex.Message);
            }
        }

        /// <summary>
        /// 加载所有下拉框的数据源
        /// 包括部门、序列、职务、层级的下拉框数据
        /// </summary>
        private void LoadDropdowns() {
            // === 部门 ===
            var deptList = _service.GetComboList("ZX_config_bm");
            cbNewDept.ItemsSource = deptList;
            cbNewDept.DisplayMemberPath = "Name"; // 显示名称
            cbNewDept.SelectedValuePath = "Id";   // 选中取值

            // === 序列 ===
            var seqList = _service.GetComboList("ZX_config_xl");
            cbNewSeq.ItemsSource = seqList;
            cbNewSeq.DisplayMemberPath = "Name";
            cbNewSeq.SelectedValuePath = "Id";

            // === 职务 ===
            var jobList = _service.GetComboList("ZX_config_gw");
            cbNewJob.ItemsSource = jobList;
            cbNewJob.DisplayMemberPath = "Name";
            cbNewJob.SelectedValuePath = "Id";

            // === 层级 ===
            var levelList = _service.GetComboList("ZX_config_cj");
            cbNewLevel.ItemsSource = levelList;
            cbNewLevel.DisplayMemberPath = "Name";
            cbNewLevel.SelectedValuePath = "Id";
        }

        /// <summary>
        /// 加载员工原始数据并回显到界面
        /// 左侧显示原组织信息（只读），右侧新组织信息默认选中原值
        /// </summary>
        private void LoadOriginalData() {
            _oldData = _service.GetEmpDetailObj(_empId);
            if (_oldData == null) {
                MessageBox.Show("获取员工信息失败");
                Close();
                return;
            }

            // === 1. 回显原数据 (左侧灰色文本框) ===
            // 假设你的 XAML 中有 txtEmpNoHeader 等控件，这里简单略过头部赋值
            txtEmpNo.Text = _oldData.EmployeeNo;  // 填充员工号
            txtName.Text = _oldData.Name;    // 填充姓名

            txtOldDept.Text = _oldData.DeptName;
            txtOldSeq.Text = _oldData.SeqName;
            txtOldJob.Text = _oldData.JobName;
            txtOldLevel.Text = _oldData.LevelName;

            // === 2. 设置新数据默认值 (右侧下拉框) ===
            // ★关键点：让新数据默认选中原数据★
            // 只要 _oldData.DeptId 类型是 int，且下拉框数据源里有这个 Id，就会自动选中
            if (_oldData.DeptId.HasValue) cbNewDept.SelectedValue = _oldData.DeptId.Value;
            if (_oldData.SeqId.HasValue) cbNewSeq.SelectedValue = _oldData.SeqId.Value;
            if (_oldData.JobId.HasValue) cbNewJob.SelectedValue = _oldData.JobId.Value;
            if (_oldData.LevelId.HasValue) cbNewLevel.SelectedValue = _oldData.LevelId.Value;

            // === 3. 时间设置 ===
            dpChangeTime.SelectedDate = DateTime.Now;
            dpWageStart.SelectedDate = DateTime.Now;
        }

        /// <summary>
        /// 变动类型下拉框选择改变事件
        /// 根据选择的变动类型（离职/非离职）切换界面控件的启用状态和显示逻辑
        /// </summary>
        private void CbChangeType_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (!(cbChangeType.SelectedItem is ComboBoxItem item)) return;
            string type = item.Content?.ToString();

            if (type == "离职") {
                // === 离职逻辑 ===
                dpWageEnd.IsEnabled = true;
                dpWageEnd.Background = Brushes.White;

                dpWageStart.IsEnabled = false;
                dpWageStart.SelectedDate = null;

                // 离职时，新部门等通常禁用，或清空，或保留原值均可
                // 这里选择禁用，保持显示为"原值"状态，但在保存时特殊处理
                ToggleNewPostControls(false);
            } else {
                // === 非离职逻辑 ===
                dpWageEnd.IsEnabled = false;
                dpWageEnd.SelectedDate = null;
                dpWageEnd.Background = new SolidColorBrush(Color.FromRgb(249, 249, 249));

                dpWageStart.IsEnabled = true;
                if (dpWageStart.SelectedDate == null) dpWageStart.SelectedDate = DateTime.Now;

                ToggleNewPostControls(true);
            }
        }

        /// <summary>
        /// 切换新岗位信息控件的启用状态
        /// 用于控制部门、序列、职务、层级下拉框的可用性
        /// </summary>
        /// <param name="isEnabled">是否启用控件（true=启用，false=禁用）</param>
        private void ToggleNewPostControls(bool isEnabled) {
            cbNewDept.IsEnabled = isEnabled;
            cbNewSeq.IsEnabled = isEnabled;
            cbNewJob.IsEnabled = isEnabled;
            cbNewLevel.IsEnabled = isEnabled;
        }

        /// <summary>
        /// 保存按钮点击事件
        /// 执行员工变动操作，包括数据校验、收集界面数据并调用服务层保存
        /// 使用事务确保变动记录和员工表的数据一致性
        /// </summary>
        private void BtnSave_Click(object sender, RoutedEventArgs e) {
            if (cbChangeType.SelectedItem == null) {
                MessageBox.Show("请选择变动项目");
                return;
            }

            string changeType = (cbChangeType.SelectedItem as ComboBoxItem).Content.ToString();
            DateTime changeTime = dpChangeTime.SelectedDate ?? DateTime.Now;

            // 校验逻辑 (保持不变)
            if (changeType == "离职") {
                if (dpWageEnd.SelectedDate == null) {
                    MessageBox.Show("办理离职时，【薪止时间】不能为空！");
                    return;
                }
            } else {
                if (dpWageStart.SelectedDate == null) {
                    MessageBox.Show("办理岗位变动时，【起薪时间】不能为空！");
                    return;
                }
                if (cbNewDept.SelectedValue == null) {
                    MessageBox.Show("请选择新部门！");
                    return;
                }
            }

            try {
                // 1. 获取新 ID
                int newBmId = (cbNewDept.SelectedValue as int?) ?? 0;
                int newXlId = (cbNewSeq.SelectedValue as int?) ?? 0;
                int newZwId = (cbNewJob.SelectedValue as int?) ?? 0;
                int newCjId = (cbNewLevel.SelectedValue as int?) ?? 0;

                // 2. 获取新名称
                string newBmName = (cbNewDept.SelectedItem as ComboItem)?.Name ?? "";
                string newXlName = (cbNewSeq.SelectedItem as ComboItem)?.Name ?? "";
                string newZwName = (cbNewJob.SelectedItem as ComboItem)?.Name ?? "";
                string newCjName = (cbNewLevel.SelectedItem as ComboItem)?.Name ?? "";

                // 3. 调用 Service (传入 原ID 和 原名称)
                _service.ExecuteEmployeeChange(
                    _empId,
                    changeType,
                    changeTime,

                    // --- 传入原 ID (来自 _oldData) ---
                    _oldData.DeptId, _oldData.DeptName,
                    _oldData.SeqId, _oldData.SeqName,
                    _oldData.JobId, _oldData.JobName,
                    _oldData.LevelId, _oldData.LevelName,
                    // -----------------------------

                    newBmId, newBmName,
                    newXlId, newXlName,
                    newZwId, newZwName,
                    newCjId, newCjName,

                    dpWageStart.SelectedDate,
                    dpWageEnd.SelectedDate
                );

                MessageBox.Show("变动办理成功！");
                this.DialogResult = true;
                this.Close();
            } catch (Exception ex) {
                MessageBox.Show("办理失败: " + ex.Message);
            }
        }
    }
}
