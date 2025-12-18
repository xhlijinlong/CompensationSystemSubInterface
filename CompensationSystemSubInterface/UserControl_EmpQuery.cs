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
    /// 员工信息查询用户控件，提供员工信息查询、筛选和导出功能
    /// </summary>
    public partial class UserControl_EmpQuery : UserControl {
        /// <summary>
        /// 员工服务实例
        /// </summary>
        private EmpService _service = new EmpService();

        /// <summary>
        /// 当前的查询筛选条件
        /// </summary>
        private EmpQueryCondition _condition = new EmpQueryCondition();

        /// <summary>
        /// 员工筛选条件窗体实例
        /// </summary>
        private FrmEmpCondition _frmCondition = null;

        /// <summary>
        /// 初始化员工信息查询用户控件
        /// </summary>
        public UserControl_EmpQuery() {
            InitializeComponent();
            // 员工信息表不需要复杂的行颜色逻辑 (RowPrePaint)，只需交替色即可
            dgvSalary.AlternatingRowsDefaultCellStyle.BackColor = Color.AliceBlue;
        }

        /// <summary>
        /// 用户控件加载事件处理，执行默认查询
        /// </summary>
        private void UserControl_EmpQuery_Load(object sender, EventArgs e) {
            if (this.DesignMode) return;
            // 加载时默认查询所有
            PerformQuery();
        }

        /// <summary>
        /// 查询按钮点击事件处理
        /// </summary>
        private void btnQuery_Click(object sender, EventArgs e) {
            PerformQuery();
        }

        /// <summary>
        /// 执行员工信息查询并显示结果
        /// </summary>
        private void PerformQuery() {
            try {
                this.Cursor = Cursors.WaitCursor;

                // 1. 调用 Service 获取数据
                string keyword = txtName.Text.Trim();
                DataTable dt = _service.GetEmpData(keyword, _condition);

                // 2. 绑定数据
                dgvSalary.DataSource = null;
                dgvSalary.Columns.Clear();
                dgvSalary.DataSource = dt;

                // 3. 格式化界面
                FormatGrid();

            } catch (Exception ex) {
                LogManager.Error("查询员工信息失败", ex);
                MessageBox.Show("查询出错: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } finally {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 格式化 DataGridView 的显示效果，包括表头样式、列属性和冻结列
        /// </summary>
        private void FormatGrid() {
            // 统一表头样式
            dgvSalary.EnableHeadersVisualStyles = false;
            dgvSalary.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            dgvSalary.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvSalary.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.LightGray;
            dgvSalary.ColumnHeadersDefaultCellStyle.Font = new Font(dgvSalary.Font, FontStyle.Bold);

            // 设置列属性
            foreach (DataGridViewColumn col in dgvSalary.Columns) {
                // 隐藏 ID 列
                if (col.Name == "id" || col.Name == "bmid" || col.Name == "xlid" || col.Name == "gwid" || col.Name == "cjid") {
                    col.Visible = false;
                    continue;
                }

                // 居中对齐
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.SortMode = DataGridViewColumnSortMode.NotSortable;

                // 处理日期列格式 (只显示 yyyy-MM-dd)
                if (col.Name.Contains("日期") || col.Name.Contains("时间")) {
                    col.DefaultCellStyle.Format = "yyyy-MM-dd";
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                if (col.Name.Contains("身份证起始") || col.Name.Contains("身份证截止")) {
                    col.DefaultCellStyle.Format = "yyyy-MM-dd";
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                // 处理日期列格式 (只显示 yyyy-MM-dd HH:mm:ss)
                //if (col.Name == "打卡时间") {
                //    col.DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
                //    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                //}
            }

            // 冻结前几列 (员工号、姓名、部门)
            if (dgvSalary.Columns.Count > 3) {
                if (dgvSalary.Columns["姓名"] != null) dgvSalary.Columns["姓名"].Frozen = true;
            }
        }

        /// <summary>
        /// 条件设置按钮点击事件处理，打开或激活员工筛选条件窗体
        /// </summary>
        private void btnCondition_Click(object sender, EventArgs e) {
            if (_frmCondition == null || _frmCondition.IsDisposed) {
                _frmCondition = new FrmEmpCondition(_condition);
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
            sfd.FileName = $"员工信息表_{DateTime.Now:yyyyMMdd}.xlsx";

            if (sfd.ShowDialog() == DialogResult.OK) {
                try {
                    // 直接复用之前的 ExcelHelper
                    ExcelHelper.ExportToExcel(dgvSalary, sfd.FileName);

                    if (MessageBox.Show("导出成功！是否立即打开文件？", "成功", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                } catch (Exception ex) {
                    LogManager.Error("导出员工信息失败", ex);
                    MessageBox.Show("导出失败: " + ex.Message + "\n请检查文件是否被占用。", "错误");
                }
            }
        }
    }
}