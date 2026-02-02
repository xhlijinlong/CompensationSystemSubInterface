using CompensationSystemSubInterface.Common;
using CompensationSystemSubInterface.Models;
using CompensationSystemSubInterface.Services;
using CompensationSystemSubInterface.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace CompensationSystemSubInterface {
    /// <summary>
    /// 员工变动查询用户控件，提供员工变动数据查询、筛选和导出功能
    /// </summary>
    public partial class UserControl_EmpCgQuery : UserControl {
        /// <summary>
        /// 员工变动服务实例
        /// </summary>
        private EmpService _service = new EmpService();

        /// <summary>
        /// 当前的查询筛选条件
        /// </summary>
        private EmpCondition _condition = new EmpCondition();

        /// <summary>
        /// 高级筛选条件窗体实例（WPF版本）
        /// </summary>
        private WpfEmpCondition _wpfCondition = null;

        // WPF 筛选树控件
        private WpfFilterPanel _treeSeq;
        private WpfFilterPanel _treeDept;
        private WpfFilterPanel _treePost;

        // 下拉弹窗
        private ToolStripDropDown _popupSeq;
        private ToolStripDropDown _popupDept;
        private ToolStripDropDown _popupPost;

        /// <summary>
        /// 初始化员工变动查询用户控件
        /// </summary>
        public UserControl_EmpCgQuery() {
            InitializeComponent();

            // 当控件销毁时关闭WPF弹窗
            this.HandleDestroyed += (s, e) => {
                _wpfCondition?.Close();
            };

            // 搜索框回车触发查询
            txtName.KeyDown += (s, e) => {
                if (e.KeyCode == Keys.Enter) {
                    PerformQuery();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            };
        }

        /// <summary>
        /// 用户控件加载事件处理，初始化日期下拉框并执行默认查询
        /// </summary>
        private void UserControl_EmpCgQuery_Load(object sender, EventArgs e) {
            if (this.DesignMode) return;

            InitDateCombos();
            InitFilterControls(); // 初始化筛选控件数据

            // 默认起止时间为今年1-12月
            int currentYear = DateTime.Now.Year;
            cbYear1.Text = currentYear.ToString();
            cbMonth1.Text = "01";
            cbYear2.Text = currentYear.ToString();
            cbMonth2.Text = "12";

            PerformQuery();
        }

        /// <summary>
        /// 初始化日期下拉框（年份和月份）
        /// </summary>
        private void InitDateCombos() {
            int curYear = DateTime.Now.Year;
            for (int i = 0; i < 5; i++) {
                cbYear1.Items.Add(curYear - i);
                cbYear2.Items.Add(curYear - i);
            }
            for (int i = 1; i <= 12; i++) {
                string m = i.ToString("00");
                cbMonth1.Items.Add(m);
                cbMonth2.Items.Add(m);
            }
        }

        /// <summary>
        /// 初始化筛选控件（使用 ToolStripDropDown + ElementHost）
        /// </summary>
        private void InitFilterControls() {
            // 常量定义弹窗尺寸
            int popWidth = 250, popHeight = 300;

            // 1. 序列
            _treeSeq = new WpfFilterPanel();
            _treeSeq.LoadSequences();
            _treeSeq.SelectionChanged += ids => {
                _condition.SequenceIds = ids;
                UpdateButtonText(btnSeq, "序列", _treeSeq);
                _treeDept?.LoadDepartments(ids.Count > 0 ? ids : null);
                _condition.DepartmentIds.Clear();
                UpdateButtonText(btnDept, "部门", _treeDept);
                RefreshConditionWindowEmployees();
            };
            _popupSeq = CreatePopup(_treeSeq, popWidth, popHeight);

            // 2. 部门
            _treeDept = new WpfFilterPanel();
            _treeDept.LoadDepartments(null);
            _treeDept.SelectionChanged += ids => {
                _condition.DepartmentIds = ids;
                UpdateButtonText(btnDept, "部门", _treeDept);
                RefreshConditionWindowEmployees();
            };
            _popupDept = CreatePopup(_treeDept, popWidth, popHeight);

            // 3. 职务
            _treePost = new WpfFilterPanel();
            _treePost.LoadPositions();
            _treePost.SelectionChanged += ids => {
                _condition.PositionIds = ids;
                UpdateButtonText(btnPost, "职务", _treePost);
                RefreshConditionWindowEmployees();
            };
            _popupPost = CreatePopup(_treePost, popWidth, popHeight);

            // 初始化按钮文本
            UpdateButtonText(btnSeq, "序列", _treeSeq);
            UpdateButtonText(btnDept, "部门", _treeDept);
            UpdateButtonText(btnPost, "职务", _treePost);
        }

        /// <summary>
        /// 当外部筛选条件变化时，同步更新条件设置窗体中的员工列表
        /// </summary>
        private void RefreshConditionWindowEmployees() {
            _wpfCondition?.RefreshFilterConditions(_condition);
        }

        /// <summary>
        /// 创建包含 WPF 控件的下拉弹窗
        /// </summary>
        private ToolStripDropDown CreatePopup(WpfFilterPanel treeContent, int width, int height) {
            ElementHost host = new ElementHost {
                AutoSize = false,
                Size = new System.Drawing.Size(width, height),
                Child = treeContent,
                Dock = DockStyle.Fill
            };

            ToolStripControlHost tsHost = new ToolStripControlHost(host);
            tsHost.Margin = Padding.Empty;
            tsHost.Padding = Padding.Empty;
            tsHost.AutoSize = false;
            tsHost.Size = new System.Drawing.Size(width, height);

            ToolStripDropDown popup = new ToolStripDropDown();
            popup.Margin = Padding.Empty;
            popup.Padding = Padding.Empty;
            popup.Items.Add(tsHost);
            return popup;
        }

        /// <summary>
        /// 更新按钮文本
        /// </summary>
        private void UpdateButtonText(Button btn, string name, WpfFilterPanel tree) {
            int count = tree.GetSelectedCount();
            bool isAll = tree.IsAllSelected();

            if (count == 0) btn.Text = name;
            else if (isAll) btn.Text = name;
            else btn.Text = $"{name}*";
        }

        /// <summary>
        /// 查询按钮点击事件处理
        /// </summary>
        private void btnQuery_Click(object sender, EventArgs e) {
            PerformQuery();
        }

        /// <summary>
        /// 执行员工变动查询并显示结果
        /// </summary>
        private void PerformQuery() {
            try {
                this.Cursor = Cursors.WaitCursor;

                if (string.IsNullOrEmpty(cbYear1.Text) || string.IsNullOrEmpty(cbMonth1.Text) ||
                    string.IsNullOrEmpty(cbYear2.Text) || string.IsNullOrEmpty(cbMonth2.Text)) {
                    MessageBox.Show("请选择起止时间！");
                    return;
                }

                DateTime start = new DateTime(int.Parse(cbYear1.Text), int.Parse(cbMonth1.Text), 1);
                DateTime endTemp = new DateTime(int.Parse(cbYear2.Text), int.Parse(cbMonth2.Text), 1);
                DateTime end = endTemp.AddMonths(1).AddDays(-1); // 月末

                if (start > end) {
                    MessageBox.Show("起始时间不能晚于结束时间！");
                    return;
                }

                DataTable dt = _service.GetEmpCgData(start, end, txtName.Text.Trim(), _condition);

                dgvSalary.DataSource = null;
                dgvSalary.Columns.Clear();
                dgvSalary.DataSource = dt;

                FormatGrid();

            } catch (Exception ex) {
                LogManager.Error("查询员工变动失败", ex);
                MessageBox.Show("查询出错: " + ex.Message);
            } finally {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 格式化 DataGridView 的显示效果，包括表头样式、列属性和冻结列
        /// </summary>
        private void FormatGrid() {
            // 设置整体字体为微软雅黑 12pt
            dgvSalary.Font = new Font("微软雅黑", 12F, FontStyle.Regular);

            // 统一表头样式
            dgvSalary.EnableHeadersVisualStyles = false;
            dgvSalary.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
            dgvSalary.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvSalary.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.White;
            dgvSalary.ColumnHeadersDefaultCellStyle.Font = new Font("微软雅黑", 12F, FontStyle.Bold);

            // 使用 DPI 缩放列宽
            int scaledWidth = DpiHelper.ScaleWidth(this, 100);

            // 设置列属性
            foreach (DataGridViewColumn col in dgvSalary.Columns) {
                // 隐藏不必要的列
                if (col.Name == "id" || col.Name == "ygid") {
                    col.Visible = false;
                    continue;
                }

                // 设置 DPI 缩放列宽
                col.Width = scaledWidth;

                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter; // 表头居中
                col.SortMode = DataGridViewColumnSortMode.NotSortable; // 禁用排序

                // 时间日期列格式化
                if (col.Name.Contains("时间") || col.Name.Contains("日期")) {
                    col.DefaultCellStyle.Format = "yyyy-MM-dd";
                }
            }

            // 特殊列宽度调整（日期时间列使用与联系电话相同的宽度）
            int phoneColumnWidth = DpiHelper.ScaleWidth(this, 120);
            if (dgvSalary.Columns["变动时间"] != null) dgvSalary.Columns["变动时间"].Width = phoneColumnWidth;
            if (dgvSalary.Columns["起薪时间"] != null) dgvSalary.Columns["起薪时间"].Width = phoneColumnWidth;

            // 设置前2列显示顺序：姓名, 编号
            if (dgvSalary.Columns["姓名"] != null) dgvSalary.Columns["姓名"].DisplayIndex = 0;
            if (dgvSalary.Columns["编号"] != null) dgvSalary.Columns["编号"].DisplayIndex = 1;

            // 冻结前2列（编号列）
            if (dgvSalary.Columns["编号"] != null) dgvSalary.Columns["编号"].Frozen = true;
        }

        /// <summary>
        /// 条件设置按钮点击事件处理，打开或激活员工筛选条件窗体
        /// </summary>
        private void btnCondition_Click(object sender, EventArgs e) {
            if (_wpfCondition == null) {
                _wpfCondition = new WpfEmpCondition(_condition.EmployeeIds, _condition.DepartmentIds);
                _wpfCondition.ApplySelect += (empIds) => {
                    _condition.EmployeeIds = empIds;
                    btnCondition.Text = _condition.HasFilter ? "条件设置*" : "条件设置";
                    PerformQuery();
                };

                _wpfCondition.Closed += (s, args) => {
                    _wpfCondition = null;
                };

                _wpfCondition.Show();
            } else {
                _wpfCondition.WindowState = System.Windows.WindowState.Normal;
                _wpfCondition.Activate();
            }
        }

        /// <summary>
        /// 导出按钮点击事件处理，将查询结果导出为 Excel 文件
        /// </summary>
        private void btnExport_Click(object sender, EventArgs e) {
            if (dgvSalary.Rows.Count == 0) {
                MessageBox.Show("没有数据可导出。", "提示");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel 文件 (*.xlsx)|*.xlsx";
            sfd.FileName = $"员工变动表_{cbYear1.Text}{cbMonth1.Text}-{cbYear2.Text}{cbMonth2.Text}.xlsx";

            if (sfd.ShowDialog() == DialogResult.OK) {
                try {
                    ExcelHelper.ExportToExcel(dgvSalary, sfd.FileName);
                    if (MessageBox.Show("导出成功！是否打开？", "成功", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                } catch (Exception ex) {
                    LogManager.Error("导出员工变动失败", ex);
                    MessageBox.Show("导出失败: " + ex.Message);
                }
            }
        }

        private void btnDept_Click(object sender, EventArgs e) {
            _popupDept?.Show(btnDept, 0, btnDept.Height);
        }

        private void btnSeq_Click(object sender, EventArgs e) {
            _popupSeq?.Show(btnSeq, 0, btnSeq.Height);
        }

        private void btnPost_Click(object sender, EventArgs e) {
            _popupPost?.Show(btnPost, 0, btnPost.Height);
        }

        private void btnWithdraw_Click(object sender, EventArgs e) {
            // 弹窗提示功能暂未实现
            MessageBox.Show("该功能暂未实现。", "提示");
        }
    }
}