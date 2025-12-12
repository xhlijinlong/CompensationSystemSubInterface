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
        private EmpCgQueryCondition _condition = new EmpCgQueryCondition();

        /// <summary>
        /// 员工变动筛选条件窗体实例
        /// </summary>
        private FrmEmpCgCondition _frmCondition = null;

        /// <summary>
        /// 初始化员工变动查询用户控件
        /// </summary>
        public UserControl_EmpCgQuery() {
            InitializeComponent();
            dgvSalary.AlternatingRowsDefaultCellStyle.BackColor = Color.AliceBlue;
        }

        /// <summary>
        /// 用户控件加载事件处理，初始化日期下拉框并执行默认查询
        /// </summary>
        private void UserControl_EmpCgQuery_Load(object sender, EventArgs e) {
            if (this.DesignMode) return;
            InitDateCombos();
            // 默认查最近一个月，或者今年
            cbYear1.Text = DateTime.Now.Year.ToString();
            cbMonth1.Text = "01";
            cbYear2.Text = DateTime.Now.Year.ToString();
            cbMonth2.Text = DateTime.Now.Month.ToString("00");

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
            dgvSalary.EnableHeadersVisualStyles = false;
            dgvSalary.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            dgvSalary.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvSalary.ColumnHeadersDefaultCellStyle.Font = new Font(dgvSalary.Font, FontStyle.Bold);
            dgvSalary.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.LightGray;

            foreach (DataGridViewColumn col in dgvSalary.Columns) {
                if (col.Name == "id" || col.Name == "ygid") {
                    col.Visible = false;
                    continue;
                }

                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.SortMode = DataGridViewColumnSortMode.NotSortable;

                // 日期列格式化
                if (col.Name.Contains("时间") || col.Name.Contains("日期")) {
                    col.DefaultCellStyle.Format = "yyyy-MM-dd";
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
            }

            // 冻结
            if (dgvSalary.Columns.Count > 2) dgvSalary.Columns[1].Frozen = true; // 冻结姓名
        }

        /// <summary>
        /// 条件设置按钮点击事件处理，打开或激活员工变动筛选条件窗体
        /// </summary>
        private void btnCondition_Click(object sender, EventArgs e) {
            if (_frmCondition == null || _frmCondition.IsDisposed) {
                _frmCondition = new FrmEmpCgCondition(_condition);
                _frmCondition.ApplySelect += (newCond) => {
                    _condition = newCond;
                    btnCondition.Text = _condition.HasFilter ? "条件设置 *" : "条件设置";
                    PerformQuery();
                };
                _frmCondition.Show(this);
            } else {
                _frmCondition.WindowState = FormWindowState.Normal;
                _frmCondition.Activate();
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
    }
}