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
    /// 员工信息维护用户控件
    /// 提供员工信息查询、筛选、导出、维护和变动功能
    /// </summary>
    public partial class UserControl_EmpMaint : UserControl {
        /// <summary>
        /// 员工服务实例，用于数据库操作
        /// </summary>
        private EmpService _service = new EmpService();

        /// <summary>
        /// 当前的查询筛选条件（部门、员工）
        /// </summary>
        private EmpCondition _condition = new EmpCondition();

        /// <summary>
        /// 高级筛选条件窗体实例（WPF版本）
        /// </summary>
        private WpfEmpCondition _wpfCondition = null;

        // WPF 筛选树控件
        private WpfFilterPanel _treeDept;

        // 下拉弹窗
        private ToolStripDropDown _popupDept;

        /// <summary>
        /// 构造函数：初始化员工信息维护用户控件
        /// 设置 DataGridView 的交替行颜色样式
        /// </summary>
        public UserControl_EmpMaint() {
            InitializeComponent();
            // 员工信息表不需要复杂的行颜色逻辑 (RowPrePaint)，只需交替色即可
            //dgvSalary.AlternatingRowsDefaultCellStyle.BackColor = Color.AliceBlue;
            
            // 当控件销毁时关闭WPF弹窗
            this.HandleDestroyed += (s, e) => {
                _wpfCondition?.Close();
            };
        }

        /*/// <summary>
        /// 用户控件加载事件处理，执行默认查询
        /// </summary>
        private void UserControl_EmpMaint_Load(object sender, EventArgs e) {
            if (this.DesignMode) return;
            // 加载时默认查询所有
            PerformQuery();
        }*/

        /// <summary>
        /// 查询按钮点击事件处理
        /// 执行员工信息查询操作
        /// </summary>
        private void btnQuery_Click(object sender, EventArgs e) {
            PerformQuery();
        }

        /// <summary>
        /// 执行员工信息查询并显示结果到 DataGridView
        /// 包含异常处理和等待光标显示
        /// </summary>
        private void PerformQuery() {
            try {
                this.Cursor = Cursors.WaitCursor;

                // 1. 调用 Service 获取数据
                string keyword = txtName.Text.Trim();
                DataTable dt = _service.GetEmpMaintData(keyword, _condition);

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
        /// 格式化 DataGridView 的显示效果
        /// 包括表头样式、隐藏ID列、日期格式化和冻结列设置
        /// </summary>
        private void FormatGrid() {
            // 统一表头样式
            dgvSalary.EnableHeadersVisualStyles = false;
            dgvSalary.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
            dgvSalary.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvSalary.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.White;
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
        /// 条件设置按钮点击事件处理
        /// 打开或激活员工筛选条件窗体
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
        /// 导出按钮点击事件处理
        /// 将查询结果导出为 Excel 文件并提示是否打开
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

        /// <summary>
        /// 用户控件加载事件处理
        /// 执行默认查询并绑定双击事件和右键菜单
        /// </summary>
        private void UserControl_EmpMaint_Load(object sender, EventArgs e) {
            if (this.DesignMode) return;
            
            InitFilterControls(); // 初始化筛选控件数据
            PerformQuery();

            // 绑定双击事件
            dgvSalary.CellDoubleClick += DgvSalary_CellDoubleClick;

            // 绑定右键菜单
            ContextMenuStrip cms = new ContextMenuStrip();
            ToolStripMenuItem tsmiMod = new ToolStripMenuItem("维护");
            tsmiMod.Click += btnMaint_Click; // 复用修改按钮逻辑
            ToolStripMenuItem tsmiCg = new ToolStripMenuItem("变动");
            tsmiCg.Click += btnCg_Click; // 复用变动按钮逻辑
            cms.Items.Add(tsmiMod);
            cms.Items.Add(tsmiCg);
            dgvSalary.ContextMenuStrip = cms;
        }

        /// <summary>
        /// 初始化筛选控件（使用 ToolStripDropDown + ElementHost）
        /// </summary>
        private void InitFilterControls() {
            // 初始化部门树
            _treeDept = new WpfFilterPanel();
            _treeDept.LoadDepartments(null);
            _treeDept.SelectionChanged += ids => {
                _condition.DepartmentIds = ids;
                UpdateButtonText(btnDept, "部门", _treeDept);
                // 同步更新条件设置窗体中的员工列表
                RefreshConditionWindowEmployees();
            };
            _popupDept = CreatePopup(_treeDept, 250, 300);

            // 初始化按钮文本
            UpdateButtonText(btnDept, "部门", _treeDept);
        }

        /// <summary>
        /// 当外部筛选条件变化时，同步更新条件设置窗体中的员工列表
        /// </summary>
        private void RefreshConditionWindowEmployees() {
            _wpfCondition?.RefreshFilterConditions(_condition.DepartmentIds);
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
        /// DataGridView 单元格双击事件处理
        /// 双击员工行时打开维护窗口
        /// </summary>
        private void DgvSalary_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
            if (e.RowIndex >= 0) {
                OpenMaintWindow();
            }
        }

        /// <summary>
        /// 维护按钮点击事件处理
        /// 打开员工信息维护窗口
        /// </summary>
        private void btnMaint_Click(object sender, EventArgs e) {
            OpenMaintWindow();
        }

        /// <summary>
        /// 变动按钮点击事件处理
        /// 打开员工变动窗口
        /// </summary>
        private void btnCg_Click(object sender, EventArgs e) {
            OpenChangeWindow();
        }

        /// <summary>
        /// 打开员工信息维护窗口（WPF）
        /// 选择当前选中的员工进行信息维护，维护成功后刷新列表
        /// </summary>
        private void OpenMaintWindow() {
            if (dgvSalary.SelectedRows.Count == 0) {
                MessageBox.Show("请先选择一名员工");
                return;
            }

            // 获取选中员工ID (假设隐藏列 "id" 存在)
            int empId = Convert.ToInt32(dgvSalary.SelectedRows[0].Cells["id"].Value);

            // 打开 WPF 窗口
            WpfEmpMaint win = new WpfEmpMaint(empId);

            // 监听窗口内的"变动"请求 (如果在WpfEmpMaint内部处理了，这里就不需要特殊处理，只要刷新即可)
            bool? result = win.ShowDialog();

            if (result == true) {
                PerformQuery(); // 刷新列表
            }
        }

        /// <summary>
        /// 打开员工变动窗口（WPF）
        /// 选择当前选中的员工进行组织变动或离职操作，操作成功后刷新列表
        /// </summary>
        private void OpenChangeWindow() {
            if (dgvSalary.SelectedRows.Count == 0) {
                MessageBox.Show("请先选择一名员工");
                return;
            }
            int empId = Convert.ToInt32(dgvSalary.SelectedRows[0].Cells["id"].Value);

            WpfEmpCg win = new WpfEmpCg(empId);
            bool? result = win.ShowDialog();

            if (result == true) {
                PerformQuery(); // 刷新列表
            }
        }

        private void btnDept_Click(object sender, EventArgs e) {
            if (_popupDept != null) {
                _popupDept.Show(btnDept, 0, btnDept.Height);
            }
        }
    }
}