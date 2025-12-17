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
    public partial class WpfEmpCg : Window {
        private int _empId;
        private EmpService _service = new EmpService();
        private EmployeeDetail _oldData;

        public WpfEmpCg(int empId) {
            InitializeComponent();
            _empId = empId;

            this.Loaded += WpfEmpCg_Loaded;
            cbChangeType.SelectionChanged += CbChangeType_SelectionChanged;
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += (s, e) => this.Close();
        }

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

        private void LoadOriginalData() {
            _oldData = _service.GetEmpDetailObj(_empId);
            if (_oldData == null) {
                MessageBox.Show("获取员工信息失败");
                Close();
                return;
            }

            // === 1. 回显原数据 (左侧灰色文本框) ===
            // 假设你的 XAML 中有 txtEmpNoHeader 等控件，这里简单略过头部赋值

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
                // 这里选择禁用，保持显示为“原值”状态，但在保存时特殊处理
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

        private void ToggleNewPostControls(bool isEnabled) {
            cbNewDept.IsEnabled = isEnabled;
            cbNewSeq.IsEnabled = isEnabled;
            cbNewJob.IsEnabled = isEnabled;
            cbNewLevel.IsEnabled = isEnabled;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e) {
            if (cbChangeType.SelectedItem == null) {
                MessageBox.Show("请选择变动项目");
                return;
            }

            string changeType = (cbChangeType.SelectedItem as ComboBoxItem).Content.ToString();
            DateTime changeTime = dpChangeTime.SelectedDate ?? DateTime.Now;

            // 校验
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
                // 非离职必须校验
                if (cbNewDept.SelectedValue == null) {
                    MessageBox.Show("请选择新部门！");
                    return;
                }
            }

            try {
                // 获取选中的ID，如果没选或者为空（as int? ?? 0），则使用 0
                // ★注意：如果用户没改，因为我们在 LoadOriginalData 里设置了默认值，
                // 所以这里 SelectedValue 就是原 ID，不需要再写额外的逻辑去判断 "IsChanged"
                int newBmId = (cbNewDept.SelectedValue as int?) ?? 0;
                int newXlId = (cbNewSeq.SelectedValue as int?) ?? 0;
                int newZwId = (cbNewJob.SelectedValue as int?) ?? 0;
                int newCjId = (cbNewLevel.SelectedValue as int?) ?? 0;

                // 如果是离职，虽然 UI 禁用了，但 SelectedValue 还在。
                // 数据库层面离职人员的 bmid/gwid 是否要更新？通常离职记录表存原部门，员工表状态改为离职。
                // 这里保持传入选中的值（即原值）。

                _service.ExecuteEmployeeChange(
                    _empId,
                    changeType,
                    changeTime,
                    newBmId,
                    newXlId,
                    newZwId,
                    newCjId,
                    dpWageStart.SelectedDate,
                    dpWageEnd.SelectedDate,
                    _oldData.DeptName,
                    _oldData.SeqName,
                    _oldData.JobName,
                    _oldData.LevelName
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
