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
    /// 离职员工信息查询用户控件，提供离职员工信息查询、筛选和导出功能
    /// </summary>
    public partial class UserControl_EmpRsQuery : UserControl {
        /// <summary>
        /// 员工服务实例
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
        private WpfFilterPanel _treeGender;
        private WpfFilterPanel _treeEthnic;
        private WpfFilterPanel _treeZodiac;
        private WpfFilterPanel _treePolitic;
        private WpfFilterPanel _treeEducation;
        private WpfFilterPanel _treeDegree;
        private WpfFilterPanel _treeTitleLevel;

        // 下拉弹窗
        private ToolStripDropDown _popupSeq;
        private ToolStripDropDown _popupDept;
        private ToolStripDropDown _popupPost;
        private ToolStripDropDown _popupGender;
        private ToolStripDropDown _popupEthnic;
        private ToolStripDropDown _popupZodiac;
        private ToolStripDropDown _popupPolitic;
        private ToolStripDropDown _popupEducation;
        private ToolStripDropDown _popupDegree;
        private ToolStripDropDown _popupTitleLevel;

        /// <summary>
        /// 初始化员工信息查询用户控件
        /// </summary>
        public UserControl_EmpRsQuery() {
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
        private void UserControl_EmpQuery_Load(object sender, EventArgs e) {
            if (this.DesignMode) return;

            InitFilterControls(); // 初始化筛选控件数据

            // 加载时默认查询所有
            PerformQuery();
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
                // 序列变化后级联刷新部门
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

            // 4. 性别
            _treeGender = new WpfFilterPanel();
            _treeGender.LoadGenders();
            _treeGender.SelectionChanged += ids => {
                _condition.Genders = _treeGender.GetSelectedTexts();
                UpdateButtonText(btnGender, "性别", _treeGender);
                RefreshConditionWindowEmployees();
            };
            _popupGender = CreatePopup(_treeGender, 150, 150);

            // 5. 民族
            _treeEthnic = new WpfFilterPanel();
            _treeEthnic.LoadEthnics();
            _treeEthnic.SelectionChanged += ids => {
                _condition.Ethnics = _treeEthnic.GetSelectedTexts();
                UpdateButtonText(btnEthnic, "民族", _treeEthnic);
                RefreshConditionWindowEmployees();
            };
            _popupEthnic = CreatePopup(_treeEthnic, popWidth, popHeight);

            // 6. 属相
            _treeZodiac = new WpfFilterPanel();
            _treeZodiac.LoadZodiacs();
            _treeZodiac.SelectionChanged += ids => {
                _condition.Zodiacs = _treeZodiac.GetSelectedTexts();
                UpdateButtonText(btnChineseZodiac, "属相", _treeZodiac);
                RefreshConditionWindowEmployees();
            };
            _popupZodiac = CreatePopup(_treeZodiac, popWidth, popHeight);

            // 7. 政治面貌
            _treePolitic = new WpfFilterPanel();
            _treePolitic.LoadPolitics();
            _treePolitic.SelectionChanged += ids => {
                _condition.Politics = _treePolitic.GetSelectedTexts();
                UpdateButtonText(btnPS, "政治面貌", _treePolitic);
                RefreshConditionWindowEmployees();
            };
            _popupPolitic = CreatePopup(_treePolitic, popWidth, popHeight);

            // 8. 学历
            _treeEducation = new WpfFilterPanel();
            _treeEducation.LoadEducations();
            _treeEducation.SelectionChanged += ids => {
                _condition.Educations = _treeEducation.GetSelectedTexts();
                UpdateButtonText(btnEducation, "学历", _treeEducation);
                RefreshConditionWindowEmployees();
            };
            _popupEducation = CreatePopup(_treeEducation, popWidth, popHeight);

            // 9. 学位
            _treeDegree = new WpfFilterPanel();
            _treeDegree.LoadDegrees();
            _treeDegree.SelectionChanged += ids => {
                _condition.Degrees = _treeDegree.GetSelectedTexts();
                UpdateButtonText(btnDegree, "学位", _treeDegree);
                RefreshConditionWindowEmployees();
            };
            _popupDegree = CreatePopup(_treeDegree, popWidth, popHeight);

            // 10. 职称等级
            _treeTitleLevel = new WpfFilterPanel();
            _treeTitleLevel.LoadTitleLevels();
            _treeTitleLevel.SelectionChanged += ids => {
                _condition.TitleLevels = _treeTitleLevel.GetSelectedTexts();
                UpdateButtonText(btnTitleLevel, "职称等级", _treeTitleLevel);
                RefreshConditionWindowEmployees();
            };
            _popupTitleLevel = CreatePopup(_treeTitleLevel, popWidth, popHeight);

            // 初始化按钮文本
            UpdateButtonText(btnSeq, "序列", _treeSeq);
            UpdateButtonText(btnDept, "部门", _treeDept);
            UpdateButtonText(btnPost, "职务", _treePost);
            UpdateButtonText(btnGender, "性别", _treeGender);
            UpdateButtonText(btnEthnic, "民族", _treeEthnic);
            UpdateButtonText(btnChineseZodiac, "属相", _treeZodiac);
            UpdateButtonText(btnEducation, "学历", _treeEducation);
            UpdateButtonText(btnDegree, "学位", _treeDegree);
            UpdateButtonText(btnTitleLevel, "职称等级", _treeTitleLevel);
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
        /// 执行员工信息查询并显示结果
        /// </summary>
        private void PerformQuery() {
            try {
                this.Cursor = Cursors.WaitCursor;

                // 1. 调用 Service 获取离职员工数据
                string keyword = txtName.Text.Trim();
                DataTable dt = _service.GetEmpRsData(keyword, _condition);

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
                // 隐藏 ID 列
                if (col.Name == "id" || col.Name == "bmid" || col.Name == "xlid" || col.Name == "gwid") {
                    col.Visible = false;
                    continue;
                }

                // 设置 DPI 缩放列宽
                col.Width = scaledWidth;

                // 居中对齐
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.SortMode = DataGridViewColumnSortMode.NotSortable;

                // 处理日期列格式 (只显示 yyyy-MM-dd)
                if (col.Name.Contains("日期") || col.Name.Contains("时间")) {
                    col.DefaultCellStyle.Format = "yyyy-MM-dd";
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
            }

            // 特殊列宽度调整（证件号码、工资卡号需要更宽）
            int wideColumnWidth = DpiHelper.ScaleWidth(this, 200);
            int phoneColumnWidth = DpiHelper.ScaleWidth(this, 120);
            if (dgvSalary.Columns["证件号码"] != null) dgvSalary.Columns["证件号码"].Width = wideColumnWidth;
            if (dgvSalary.Columns["联系电话"] != null) dgvSalary.Columns["联系电话"].Width = phoneColumnWidth;
            if (dgvSalary.Columns["工资卡号"] != null) dgvSalary.Columns["工资卡号"].Width = wideColumnWidth;

            // 冻结前4列（员工编号, 部门, 职务, 姓名）
            if (dgvSalary.Columns["姓名"] != null) dgvSalary.Columns["姓名"].Frozen = true;
        }

        /// <summary>
        /// 条件设置按钮点击事件处理，打开或激活员工筛选条件窗体
        /// </summary>
        private void btnCondition_Click(object sender, EventArgs e) {
            if (_wpfCondition == null) {
                _wpfCondition = new WpfEmpCondition(_condition.EmployeeIds, _condition.DepartmentIds, true);
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
            sfd.FileName = $"离职员工信息表_{DateTime.Now:yyyyMMdd}.xlsx";

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

        private void btnDept_Click(object sender, EventArgs e) {
            _popupDept?.Show(btnDept, 0, btnDept.Height);
        }

        private void btnSeq_Click(object sender, EventArgs e) {
            _popupSeq?.Show(btnSeq, 0, btnSeq.Height);
        }

        private void btnPost_Click(object sender, EventArgs e) {
            _popupPost?.Show(btnPost, 0, btnPost.Height);
        }

        private void btnGender_Click(object sender, EventArgs e) {
            _popupGender?.Show(btnGender, 0, btnGender.Height);
        }

        private void btnEthnic_Click(object sender, EventArgs e) {
            _popupEthnic?.Show(btnEthnic, 0, btnEthnic.Height);
        }

        private void btnPS_Click(object sender, EventArgs e) {
            _popupPolitic?.Show(btnPS, 0, btnPS.Height);
        }

        private void btnEducation_Click(object sender, EventArgs e) {
            _popupEducation?.Show(btnEducation, 0, btnEducation.Height);
        }

        private void btnDegree_Click(object sender, EventArgs e) {
            _popupDegree?.Show(btnDegree, 0, btnDegree.Height);
        }

        private void btnTitleLevel_Click(object sender, EventArgs e) {
            _popupTitleLevel?.Show(btnTitleLevel, 0, btnTitleLevel.Height);
        }

        private void btnChineseZodiac_Click(object sender, EventArgs e) {
            _popupZodiac?.Show(btnChineseZodiac, 0, btnChineseZodiac.Height);
        }
    }
}