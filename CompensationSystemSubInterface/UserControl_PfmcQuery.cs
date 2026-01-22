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
    /// 员工绩效查询用户控件，提供绩效数据查询、筛选和导出功能
    /// </summary>
    public partial class UserControl_PfmcQuery : UserControl {
        /// <summary>
        /// 绩效服务实例
        /// </summary>
        private PfmcService _service = new PfmcService();

        /// <summary>
        /// 当前的查询筛选条件
        /// </summary>
        private PfmcQueryCondition _condition = new PfmcQueryCondition();

        /// <summary>
        /// 高级筛选条件窗体实例（WPF版本）
        /// </summary>
        private WpfEmpCondition _wpfCondition = null;

        // WPF 筛选树控件
        private WpfFilterPanel _treeYear;
        private WpfFilterPanel _treeRslt;
        private WpfFilterPanel _treeSeq;
        private WpfFilterPanel _treeDept;
        private WpfFilterPanel _treePost;

        // 下拉弹窗
        private ToolStripDropDown _popupYear;
        private ToolStripDropDown _popupRslt;
        private ToolStripDropDown _popupSeq;
        private ToolStripDropDown _popupDept;
        private ToolStripDropDown _popupPost;

        /// <summary>
        /// 初始化绩效查询用户控件
        /// </summary>
        public UserControl_PfmcQuery() {
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
        /// 用户控件加载事件处理，执行默认查询
        /// </summary>
        private void UserControl_PfmcQuery_Load(object sender, EventArgs e) {
            if (this.DesignMode) return;
            
            InitFilterControls(); // 初始化筛选控件数据
            
            // 加载时默认查询所有
            PerformQuery();
        }

        /// <summary>
        /// 初始化筛选控件（使用 ToolStripDropDown + ElementHost）
        /// </summary>
        private void InitFilterControls() {
            // 1. 初始化年度树
            _treeYear = new WpfFilterPanel();
            _treeYear.LoadYears();
            _treeYear.SelectionChanged += ids => {
                _condition.Years = ids;
                UpdateButtonText(btnYear, "年度", _treeYear);
            };
            _popupYear = CreatePopup(_treeYear, 150, 200);
            
            // 2. 初始化结果树
            _treeRslt = new WpfFilterPanel();
            _treeRslt.LoadResults();
            _treeRslt.SelectionChanged += ids => {
                // 结果使用文本而不是ID
                _condition.Results = _treeRslt.GetSelectedTexts();
                UpdateButtonText(btnRslt, "结果", _treeRslt);
            };
            _popupRslt = CreatePopup(_treeRslt, 150, 180);

            // 3. 序列
            int popWidth = 250, popHeight = 300;
            _treeSeq = new WpfFilterPanel();
            _treeSeq.LoadSequences();
            _treeSeq.SelectionChanged += ids => {
                _condition.SequenceIds = ids;
                UpdateButtonText(btnSeq, "序列", _treeSeq);
                // 序列变化后级联刷新部门
                _treeDept?.LoadDepartments(ids.Count > 0 ? ids : null);
                _condition.DepartmentIds.Clear();
                UpdateButtonText(btnDept, "部门", _treeDept);
                RefreshConditionWindowEmployees();
            };
            _popupSeq = CreatePopup(_treeSeq, popWidth, popHeight);

            // 4. 部门
            _treeDept = new WpfFilterPanel();
            _treeDept.LoadDepartments(null);
            _treeDept.SelectionChanged += ids => {
                _condition.DepartmentIds = ids;
                UpdateButtonText(btnDept, "部门", _treeDept);
                RefreshConditionWindowEmployees();
            };
            _popupDept = CreatePopup(_treeDept, popWidth, popHeight);

            // 5. 职务
            _treePost = new WpfFilterPanel();
            _treePost.LoadPositions();
            _treePost.SelectionChanged += ids => {
                _condition.PositionIds = ids;
                UpdateButtonText(btnPost, "职务", _treePost);
                RefreshConditionWindowEmployees();
            };
            _popupPost = CreatePopup(_treePost, popWidth, popHeight);

            // 初始化按钮文本
            UpdateButtonText(btnYear, "年度", _treeYear);
            UpdateButtonText(btnRslt, "结果", _treeRslt);
            UpdateButtonText(btnSeq, "序列", _treeSeq);
            UpdateButtonText(btnDept, "部门", _treeDept);
            UpdateButtonText(btnPost, "职务", _treePost);
        }

        /// <summary>
        /// 当外部筛选条件变化时，同步更新条件设置窗体中的员工列表
        /// </summary>
        private void RefreshConditionWindowEmployees() {
            _wpfCondition?.RefreshFilterConditions(new EmpCondition {
                SequenceIds = _condition.SequenceIds,
                DepartmentIds = _condition.DepartmentIds,
                PositionIds = _condition.PositionIds
            });
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
        /// 执行绩效查询并显示结果
        /// </summary>
        private void PerformQuery() {
            try {
                this.Cursor = Cursors.WaitCursor;

                // 1. 获取原始数据
                DataTable raw = _service.GetRawPfmcData(txtName.Text.Trim(), _condition);
                // 2. 透视处理
                DataTable report = _service.BuildPivotReport(raw);

                dgvSalary.DataSource = report;
                FormatGrid();

            } catch (Exception ex) {
                LogManager.Error("查询绩效信息失败", ex);
                MessageBox.Show("查询出错: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            // 关闭持续自动列宽（改用一次性计算提升性能）
            dgvSalary.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            
            // 统一表头样式
            dgvSalary.EnableHeadersVisualStyles = false;
            dgvSalary.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
            dgvSalary.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvSalary.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.White;
            dgvSalary.ColumnHeadersDefaultCellStyle.Font = new Font("微软雅黑", 12F, FontStyle.Bold);

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
            }

            // 设置前2列显示顺序：姓名, 编号
            if (dgvSalary.Columns["姓名"] != null) dgvSalary.Columns["姓名"].DisplayIndex = 0;
            if (dgvSalary.Columns["编号"] != null) dgvSalary.Columns["编号"].DisplayIndex = 1;

            // 冻结前2列（编号列）
            if (dgvSalary.Columns["编号"] != null) dgvSalary.Columns["编号"].Frozen = true;

            // 一次性计算列宽（解决高分屏列宽过窄问题）
            dgvSalary.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
        }

        /// <summary>
        /// 条件设置按钮点击事件处理，打开或激活员工筛选条件窗体
        /// </summary>
        private void btnCondition_Click(object sender, EventArgs e) {
            if (_wpfCondition == null) {
                // 使用通用的 WpfEmpCondition，不传部门筛选条件
                _wpfCondition = new WpfEmpCondition(_condition.EmployeeIds, null);
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
            sfd.FileName = $"员工绩效表_{DateTime.Now:yyyyMMdd}.xlsx";

            if (sfd.ShowDialog() == DialogResult.OK) {
                try {
                    // 直接复用之前的 ExcelHelper
                    ExcelHelper.ExportToExcel(dgvSalary, sfd.FileName);

                    if (MessageBox.Show("导出成功！是否立即打开文件？", "成功", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                } catch (Exception ex) {
                    LogManager.Error("导出绩效信息失败", ex);
                    MessageBox.Show("导出失败: " + ex.Message + "\n请检查文件是否被占用。", "错误");
                }
            }
        }

        private void btnYear_Click(object sender, EventArgs e) {
            if (_popupYear != null) {
                _popupYear.Show(btnYear, 0, btnYear.Height);
            }
        }

        private void btnRslt_Click(object sender, EventArgs e) {
            if (_popupRslt != null) {
                _popupRslt.Show(btnRslt, 0, btnRslt.Height);
            }
        }

        private void btnSeq_Click(object sender, EventArgs e) {
            if (_popupSeq != null && !_popupSeq.Visible) {
                _popupSeq.Show(btnSeq, 0, btnSeq.Height);
            }
        }

        private void btnDept_Click(object sender, EventArgs e) {
            if (_popupDept != null && !_popupDept.Visible) {
                _popupDept.Show(btnDept, 0, btnDept.Height);
            }
        }

        private void btnPost_Click(object sender, EventArgs e) {
            if (_popupPost != null && !_popupPost.Visible) {
                _popupPost.Show(btnPost, 0, btnPost.Height);
            }
        }
    }
}