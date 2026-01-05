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
    /// 薪资查询用户控件，提供薪资数据查询、筛选和导出功能
    /// </summary>
    public partial class UserControl_SalaryQuery : UserControl {
        /// <summary>
        /// 薪资服务实例
        /// </summary>
        private SalaryService _service = new SalaryService();

        /// <summary>
        /// 当前的查询筛选条件
        /// </summary>
        private SalaryQueryCondition _condition = new SalaryQueryCondition();

        /// <summary>
        /// 高级筛选条件窗体实例（WPF版本）
        /// </summary>
        private WpfSalaryCondition _wpfCondition = null;

        // WPF 筛选树控件
        private WpfFilterPanel _treeSeq;
        private WpfFilterPanel _treeDept;
        private WpfFilterPanel _treePost;

        // 下拉弹窗
        private ToolStripDropDown _popupSeq;
        private ToolStripDropDown _popupDept;
        private ToolStripDropDown _popupPost;

        /// <summary>
        /// 初始化薪资查询用户控件
        /// </summary>
        public UserControl_SalaryQuery() {
            InitializeComponent();
            dgvSalary.RowPrePaint += dgvSalary_RowPrePaint; // 绑定行绘制前事件
        }

        /// <summary>
        /// 用户控件加载事件处理，初始化默认日期和执行自动查询
        /// </summary>
        private void UserControl_SalaryQuery_Load(object sender, EventArgs e) {
            if (this.DesignMode) return;

            InitDateCombos();
            InitFilterControls(); // 初始化筛选控件数据

            // 获取数据库中最近的发薪月份
            DateTime? latest = _service.GetLatestSalaryMonth();
            if (latest.HasValue) {
                int y = latest.Value.Year;
                int m = latest.Value.Month;
                // 默认起止时间都是最近那个月
                cbYear1.Text = y.ToString();
                cbMonth1.Text = m.ToString("00");
                cbYear2.Text = y.ToString();
                cbMonth2.Text = m.ToString("00");
            } else {
                // 如果没数据，默认显示当前年月
                cbYear1.Text = DateTime.Now.Year.ToString();
                cbMonth1.Text = DateTime.Now.Month.ToString("00");
                cbYear2.Text = DateTime.Now.Year.ToString();
                cbMonth2.Text = DateTime.Now.Month.ToString("00");
            }

            // 自动查询
            PerformQuery();
        }

        /// <summary>
        /// 初始化日期下拉框（年份和月份）
        /// </summary>
        private void InitDateCombos() {
            int curYear = DateTime.Now.Year;
            // 填充年份 (前后5年)
            for (int i = 0; i < 5; i++) {
                cbYear1.Items.Add(curYear - i);
                cbYear2.Items.Add(curYear - i);
            }
            // 填充月份 (01-12)
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
            // 1. 初始化序列树
            _treeSeq = new WpfFilterPanel();
            _treeSeq.LoadSequences();
            _treeSeq.SelectionChanged += ids => {
                _condition.SequenceIds = ids;
                UpdateButtonText(btnSeq, "序列", _treeSeq);
                // 级联更新部门
                _treeDept.LoadDepartments(ids);
                UpdateButtonText(btnDept, "部门", _treeDept);
                _condition.DepartmentIds = _treeDept.GetSelectedIds();
                // 同步更新条件设置窗体中的员工列表
                RefreshConditionWindowEmployees();
            };
            _popupSeq = CreatePopup(_treeSeq, 200, 300);
            
            // 2. 初始化部门树
            _treeDept = new WpfFilterPanel();
            _treeDept.LoadDepartments(null);
            _treeDept.SelectionChanged += ids => {
                _condition.DepartmentIds = ids;
                UpdateButtonText(btnDept, "部门", _treeDept);
                // 同步更新条件设置窗体中的员工列表
                RefreshConditionWindowEmployees();
            };
            _popupDept = CreatePopup(_treeDept, 250, 300);

            // 3. 初始化岗位树
            _treePost = new WpfFilterPanel();
            _treePost.LoadPositions();
            _treePost.SelectionChanged += ids => {
                _condition.PositionIds = ids;
                UpdateButtonText(btnPost, "岗位", _treePost);
                // 同步更新条件设置窗体中的员工列表
                RefreshConditionWindowEmployees();
            };
            _popupPost = CreatePopup(_treePost, 200, 300);

            // 初始化按钮文本
            UpdateButtonText(btnSeq, "序列", _treeSeq);
            UpdateButtonText(btnDept, "部门", _treeDept);
            UpdateButtonText(btnPost, "岗位", _treePost);
        }

        /// <summary>
        /// 当外部筛选条件变化时，同步更新条件设置窗体中的员工列表
        /// </summary>
        private void RefreshConditionWindowEmployees() {
            _wpfCondition?.RefreshFilterConditions(
                _condition.SequenceIds,
                _condition.DepartmentIds,
                _condition.PositionIds
            );
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
        /// 执行薪资查询并显示结果
        /// </summary>
        private void PerformQuery() {
            try {
                this.Cursor = Cursors.WaitCursor;

                // 1. 获取时间范围
                if (string.IsNullOrEmpty(cbYear1.Text) || string.IsNullOrEmpty(cbMonth1.Text) ||
                    string.IsNullOrEmpty(cbYear2.Text) || string.IsNullOrEmpty(cbMonth2.Text)) {
                    MessageBox.Show("请选择完整的起止时间！");
                    return;
                }

                DateTime start = new DateTime(int.Parse(cbYear1.Text), int.Parse(cbMonth1.Text), 1);
                // 结束时间是 结束月的最后一天
                DateTime endTemp = new DateTime(int.Parse(cbYear2.Text), int.Parse(cbMonth2.Text), 1);
                DateTime end = endTemp.AddMonths(1).AddDays(-1);

                if (start > end) {
                    MessageBox.Show("起始时间不能晚于结束时间！");
                    return;
                }

                // 2. 调用 Service
                DataTable allItems = _service.GetSalaryItems();
                DataTable rawData = _service.GetRawSalaryData(start, end, txtName.Text.Trim(), _condition);
                DataTable report = _service.BuildReportData(rawData, allItems, _condition);

                // 3. 绑定
                dgvSalary.DataSource = null;
                dgvSalary.Columns.Clear();
                dgvSalary.DataSource = report;

                // 4. 格式化
                FormatGrid(report);
            } catch (Exception ex) {
                MessageBox.Show("查询出错: " + ex.Message);
            } finally {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 格式化 DataGridView 的显示效果
        /// </summary>
        /// <param name="dt">数据源 DataTable</param>
        private void FormatGrid(DataTable dt) {
            // 设置表头为灰色
            dgvSalary.EnableHeadersVisualStyles = false;
            dgvSalary.ColumnHeadersDefaultCellStyle.BackColor = Color.White; // 设置背景为浅灰
            dgvSalary.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;     // 设置文字为黑色
            dgvSalary.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.White; // 防止点击表头变蓝
            if (!dgvSalary.ColumnHeadersDefaultCellStyle.Font.Bold) {
                dgvSalary.ColumnHeadersDefaultCellStyle.Font = new Font(dgvSalary.Font, FontStyle.Bold); // 设置表头字体加粗
            }

            // 1. 先进行通用设置（金额格式、Caption映射）
            foreach (DataGridViewColumn col in dgvSalary.Columns) {
                // 如果 DataTable 中有设置 Caption，就用它
                // 注意：MonthStr 默认 Caption 也是 MonthStr，所以这里会先把表头设为英文
                if (dt.Columns.Contains(col.Name) && !string.IsNullOrEmpty(dt.Columns[col.Name].Caption)) {
                    col.HeaderText = dt.Columns[col.Name].Caption;
                }

                // 设置表头居中对齐
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                // 禁用列排序功能
                col.SortMode = DataGridViewColumnSortMode.NotSortable;

                // 设置金额列的格式 (保留2位小数，靠右对齐)
                if (col.Name.StartsWith("Item_") || col.Name == "TotalAmount") {
                    col.DefaultCellStyle.Format = "N2";
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }

            // 2. 【关键修正】最后强制覆盖特殊列的表头名称
            if (dgvSalary.Columns.Contains("MonthStr")) {
                dgvSalary.Columns["MonthStr"].HeaderText = "时间";
                dgvSalary.Columns["MonthStr"].Width = 100;
                dgvSalary.Columns["MonthStr"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvSalary.Columns["MonthStr"].Frozen = true; // 冻结时间列
            }

            if (dgvSalary.Columns.Contains("EmployeeName")) {
                dgvSalary.Columns["EmployeeName"].HeaderText = "姓名";
                dgvSalary.Columns["EmployeeName"].Width = 100;
                dgvSalary.Columns["EmployeeName"].Frozen = true; // 冻结姓名列
            }

            // 3. 隐藏不需要显示的列
            if (dgvSalary.Columns.Contains("DeptName")) {
                dgvSalary.Columns["DeptName"].Visible = false;
            }

            if (dgvSalary.Columns.Contains("RowType")) {
                dgvSalary.Columns["RowType"].Visible = false;
            }
        }

        /// <summary>
        /// DataGridView 行绘制前事件处理，根据行类型设置不同的背景色和字体
        /// </summary>
        private void dgvSalary_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e) {
            if (e.RowIndex < 0) return;
            DataRowView drv = dgvSalary.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (drv != null && drv.Row.Table.Columns.Contains("RowType")) {
                int type = Convert.ToInt32(drv["RowType"]);
                if (type == 1) {
                    //dgvSalary.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.AliceBlue; // 个人小计
                } else if (type == 2) {
                    // 部门小计
                    //dgvSalary.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGray;
                    dgvSalary.Rows[e.RowIndex].DefaultCellStyle.Font = new Font(dgvSalary.Font, FontStyle.Bold);
                } else if (type == 3) {
                    // 全厂总计
                    //dgvSalary.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGray;
                    dgvSalary.Rows[e.RowIndex].DefaultCellStyle.Font = new Font(dgvSalary.Font, FontStyle.Bold);
                }
            }
        }

        /// <summary>
        /// 条件设置按钮点击事件处理，打开高级筛选条件窗体
        /// </summary>
        private void btnCondition_Click(object sender, EventArgs e) {
            if (_wpfCondition == null) {
                _wpfCondition = new WpfSalaryCondition(_condition);

                _wpfCondition.ApplySelect += (newCond) => {
                    _condition = newCond;
                    btnCondition.Text = _condition.HasFilter ? "条件设置*" : "条件设置";
                    PerformQuery();
                };

                _wpfCondition.Closed += (s, args) => {
                    _wpfCondition = null;
                };

                _wpfCondition.Show();
            } else {
                // 把它带到最前面，防止用户找不到
                _wpfCondition.WindowState = System.Windows.WindowState.Normal;
                _wpfCondition.Activate();
            }
        }

        /// <summary>
        /// 导出按钮点击事件处理，将查询结果导出为 EXCEL 文件
        /// </summary>
        private void btnExport_Click(object sender, EventArgs e) {
            if (dgvSalary.Rows.Count == 0) {
                MessageBox.Show("没有数据可导出。", "提示");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            // 改为 .xlsx 格式
            sfd.Filter = "Excel 文件 (*.xlsx)|*.xlsx";
            // 文件名保持之前的逻辑
            sfd.FileName = $"薪资月报表_{cbYear1.Text}{cbMonth1.Text}-{cbYear2.Text}{cbMonth2.Text}.xlsx";

            if (sfd.ShowDialog() == DialogResult.OK) {
                try {
                    // 调用我们刚写的 NPOI 工具类
                    ExcelHelper.ExportToExcel(dgvSalary, sfd.FileName);

                    if (MessageBox.Show("导出成功！是否立即打开文件？", "成功", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                        System.Diagnostics.Process.Start(sfd.FileName); // 自动打开 Excel
                    }
                } catch (Exception ex) {
                    // 如果文件被占用（比如用户开着旧文件没关），这里会报错
                    MessageBox.Show("导出失败: " + ex.Message + "\n请检查文件是否被占用。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSeq_Click(object sender, EventArgs e) {
            if (_popupSeq != null) {
                _popupSeq.Show(btnSeq, 0, btnSeq.Height);
            }
        }

        private void btnDept_Click(object sender, EventArgs e) {
             if (_popupDept != null) {
                _popupDept.Show(btnDept, 0, btnDept.Height);
            }
        }

        private void btnPost_Click(object sender, EventArgs e) {
             if (_popupPost != null) {
                _popupPost.Show(btnPost, 0, btnPost.Height);
            }
        }
    }
}