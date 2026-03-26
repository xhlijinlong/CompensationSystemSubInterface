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
        private EmpCondition _condition = new EmpCondition();

        // 姓名筛选控件
        private WpfEmpCondition _wpfCondition;
        private ToolStripDropDown _popupCondition;

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
        private WpfFilterPanel _treeBirthday;
        private WpfFilterPanel _treeWorkDate;
        private WpfFilterPanel _treeHiredate;
        private WpfFilterPanel _treePositionDate;
        private WpfFilterPanel _treeAge;
        private WpfFilterPanel _treeSkill;
        private WpfFilterPanel _treeTechnology;

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
        private ToolStripDropDown _popupBirthday;
        private ToolStripDropDown _popupWorkDate;
        private ToolStripDropDown _popupHiredate;
        private ToolStripDropDown _popupPositionDate;
        private ToolStripDropDown _popupAge;
        private ToolStripDropDown _popupSkill;
        private ToolStripDropDown _popupTechnology;

        // 部门限制（由主程序设置，用于部门主任查看本部门数据）
        private int _departmentId = -1; //-1 所有部门

        // ===== 排序相关字段 =====
        /// <summary>
        /// 当前排序的列名（null 表示无排序，使用原始顺序）
        /// </summary>
        private string _currentSortColumn = null;

        /// <summary>
        /// 年龄列的三态排序状态: 0=无排序, 1=降序(大→小), 2=升序(小→大)
        /// </summary>
        private int _ageSortState = 0;

        /// <summary>
        /// 保存原始查询数据的 DataTable 副本（用于恢复初始排序）
        /// </summary>
        private DataTable _originalData = null;

        /// <summary>
        /// 可排序列及其自定义排序优先级字典
        /// </summary>
        private static readonly Dictionary<string, Dictionary<string, int>> _enumSortOrders = new Dictionary<string, Dictionary<string, int>> {
            { "性别", new Dictionary<string, int> { {"男", 0}, {"女", 1} } },
            { "政治面貌", new Dictionary<string, int> { {"中共党员", 0}, {"团员", 1}, {"群众", 2} } },
            { "学历", new Dictionary<string, int> { {"博士研究生", 0}, {"硕士研究生", 1}, {"本科", 2}, {"专科", 3}, {"中专", 4}, {"高中及以下", 5} } },
            { "学位", new Dictionary<string, int> { {"博士", 0}, {"硕士", 1}, {"学士", 2}, {"无", 3} } },
            { "序列", new Dictionary<string, int> { {"管理序列", 0}, {"生产序列", 1} } },
        };

        /// <summary>
        /// 日期类型的可排序列
        /// </summary>
        private static readonly HashSet<string> _dateSortColumns = new HashSet<string> {
            "出生日期", "参加工作时间", "入社时间"
        };

        /// <summary>
        /// 获取或设置限定的部门ID
        /// 设为大于0的值时，锁定查询范围为该部门，并禁用序列和部门筛选按钮
        /// 默认值-1表示不限制，显示所有部门
        /// </summary>
        public int DepartmentId {
            get => _departmentId;
            set => _departmentId = value;
        }

        /// <summary>
        /// 初始化员工信息查询用户控件
        /// </summary>
        public UserControl_EmpQuery() {
            InitializeComponent();



            // 搜索框回车触发查询
            txtName.KeyDown += (s, e) => {
                if (e.KeyCode == Keys.Enter) {
                    PerformQuery();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            };

            // 注册表头点击排序事件
            dgvSalary.ColumnHeaderMouseClick += dgvSalary_ColumnHeaderMouseClick;
        }

        /// <summary>
        /// 用户控件加载事件处理，执行默认查询
        /// </summary>
        private void UserControl_EmpQuery_Load(object sender, EventArgs e) {
            if (this.DesignMode) return;

            InitFilterControls(); // 初始化筛选控件数据

            // 如果主程序传入了部门ID，则锁定到该部门
            if (_departmentId > 0) {
                _condition.DepartmentIds = new List<int> { _departmentId };
                btnSeq.Enabled = false;   // 禁用序列筛选
                btnDept.Enabled = false;  // 禁用部门筛选
            }

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

            // 11. 出生日期（年月二级树）
            _treeBirthday = new WpfFilterPanel();
            _treeBirthday.LoadDateYearMonths("chushengrq", "出生日期");
            _treeBirthday.SelectionChanged += ids => {
                _condition.BirthdayYearMonths = _treeBirthday.GetAllLeafSelectedTexts();
                UpdateButtonText(btnBirthday, "出生日期", _treeBirthday);
                RefreshConditionWindowEmployees();
            };
            _popupBirthday = CreatePopup(_treeBirthday, popWidth, popHeight);

            // 12. 参加工作时间（年月二级树）
            _treeWorkDate = new WpfFilterPanel();
            _treeWorkDate.LoadDateYearMonths("gongzuosj", "参加工作时间");
            _treeWorkDate.SelectionChanged += ids => {
                _condition.WorkDateYearMonths = _treeWorkDate.GetAllLeafSelectedTexts();
                UpdateButtonText(btnWorkDate, "参加工作时间", _treeWorkDate);
                RefreshConditionWindowEmployees();
            };
            _popupWorkDate = CreatePopup(_treeWorkDate, popWidth, popHeight);

            // 13. 入社时间（年月二级树）
            _treeHiredate = new WpfFilterPanel();
            _treeHiredate.LoadDateYearMonths("rusisj", "入社时间");
            _treeHiredate.SelectionChanged += ids => {
                _condition.HireDateYearMonths = _treeHiredate.GetAllLeafSelectedTexts();
                UpdateButtonText(btnHiredate, "入社时间", _treeHiredate);
                RefreshConditionWindowEmployees();
            };
            _popupHiredate = CreatePopup(_treeHiredate, popWidth, popHeight);

            // 14. 任现岗位时间（年月二级树）
            _treePositionDate = new WpfFilterPanel();
            _treePositionDate.LoadDateYearMonths("gangweisj", "任现岗位时间");
            _treePositionDate.SelectionChanged += ids => {
                _condition.PositionDateYearMonths = _treePositionDate.GetAllLeafSelectedTexts();
                UpdateButtonText(btnPositionDate, "任现岗位时间", _treePositionDate);
                RefreshConditionWindowEmployees();
            };
            _popupPositionDate = CreatePopup(_treePositionDate, popWidth, popHeight);

            // 15. 年龄段
            _treeAge = new WpfFilterPanel();
            _treeAge.LoadAgeRanges();
            _treeAge.SelectionChanged += ids => {
                _condition.AgeRanges = _treeAge.GetSelectedTexts();
                UpdateButtonText(btnAge, "年龄", _treeAge);
                RefreshConditionWindowEmployees();
            };
            _popupAge = CreatePopup(_treeAge, 200, 250);

            // 16. 专业技能（多选）
            _treeSkill = new WpfFilterPanel();
            _treeSkill.LoadSkills();
            _treeSkill.SelectionChanged += ids => {
                _condition.Skills = _treeSkill.GetSelectedTexts();
                UpdateButtonText(btnSkill, "专业技能", _treeSkill);
                RefreshConditionWindowEmployees();
            };
            _popupSkill = CreatePopup(_treeSkill, popWidth, popHeight);

            // 17. 专业技术（多选）
            _treeTechnology = new WpfFilterPanel();
            _treeTechnology.LoadTechnologies();
            _treeTechnology.SelectionChanged += ids => {
                _condition.Technologies = _treeTechnology.GetSelectedTexts();
                UpdateButtonText(btnTechnology, "专业技术", _treeTechnology);
                RefreshConditionWindowEmployees();
            };
            _popupTechnology = CreatePopup(_treeTechnology, popWidth, popHeight);

            // 姓名筛选
            _wpfCondition = new WpfEmpCondition(_condition.EmployeeIds, _condition.DepartmentIds);
            _wpfCondition.RefreshFilterConditions(_condition);
            _popupCondition = CreatePopup(_wpfCondition, 300, 400);
            _popupCondition.Closed += (s, args) => {
                _condition.EmployeeIds = _wpfCondition.SelectedEmployeeIds;
                UpdateConditionButtonText();
            };

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
            UpdateButtonText(btnBirthday, "出生日期", _treeBirthday);
            UpdateButtonText(btnWorkDate, "参加工作时间", _treeWorkDate);
            UpdateButtonText(btnHiredate, "入社时间", _treeHiredate);
            UpdateButtonText(btnPositionDate, "任现岗位时间", _treePositionDate);
            UpdateButtonText(btnAge, "年龄", _treeAge);
            UpdateButtonText(btnSkill, "专业技能", _treeSkill);
            UpdateButtonText(btnTechnology, "专业技术", _treeTechnology);
            UpdateConditionButtonText();
        }

        /// <summary>
        /// 当外部筛选条件变化时，同步更新条件设置窗体中的员工列表
        /// </summary>
        private void RefreshConditionWindowEmployees() {
            _wpfCondition?.RefreshFilterConditions(_condition);
        }

        /// <summary>
        /// 更新姓名按钮文本
        /// </summary>
        private void UpdateConditionButtonText() {
            if (_wpfCondition == null) return;
            int count = _wpfCondition.GetSelectedCount();
            bool isAll = _wpfCondition.IsAllSelected();
            if (count == 0 || isAll) btnCondition.Text = "姓名";
            else btnCondition.Text = "姓名*";
        }

        /// <summary>
        /// 创建包含 WPF 控件的下拉弹窗
        /// </summary>
        private ToolStripDropDown CreatePopup(System.Windows.UIElement wpfContent, int width, int height) {
            ElementHost host = new ElementHost {
                AutoSize = false,
                Size = new System.Drawing.Size(width, height),
                Child = wpfContent,
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

                // 1. 调用 Service 获取数据
                string keyword = txtName.Text.Trim();
                DataTable dt = _service.GetEmpData(keyword, _condition);

                // 2. 保存原始数据副本（用于恢复初始排序）
                _originalData = dt.Copy();
                _currentSortColumn = null;
                _ageSortState = 0;

                // 3. 绑定数据
                dgvSalary.DataSource = null;
                dgvSalary.Columns.Clear();
                dgvSalary.DataSource = dt;

                // 4. 格式化界面
                FormatGrid();

                // 5. 更新状态栏
                lblStatus.Text = $"共查询到 {dt.Rows.Count} 条记录";

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
            if (dgvSalary.Columns["工资卡号"] != null) {
                if (_departmentId > 0) {
                    dgvSalary.Columns["工资卡号"].Visible = false; // 部门主任模式隐藏工资卡号
                } else {
                    dgvSalary.Columns["工资卡号"].Width = wideColumnWidth;
                }
            }
            if (dgvSalary.Columns["出生日期"] != null) dgvSalary.Columns["出生日期"].Width = phoneColumnWidth;
            if (dgvSalary.Columns["参加工作时间"] != null) dgvSalary.Columns["参加工作时间"].Width = phoneColumnWidth;
            if (dgvSalary.Columns["入社时间"] != null) dgvSalary.Columns["入社时间"].Width = phoneColumnWidth;
            if (dgvSalary.Columns["任现岗位时间"] != null) dgvSalary.Columns["任现岗位时间"].Width = phoneColumnWidth;
            if (dgvSalary.Columns["取得时间"] != null) dgvSalary.Columns["取得时间"].Width = phoneColumnWidth;
            if (dgvSalary.Columns["技能时间"] != null) dgvSalary.Columns["技能时间"].Width = phoneColumnWidth;
            if (dgvSalary.Columns["专业技能"] != null) dgvSalary.Columns["专业技能"].Width = phoneColumnWidth;

            // 窄列宽度调整
            int nameColumnWidth = DpiHelper.ScaleWidth(this, 64);
            int narrowColumnWidth = DpiHelper.ScaleWidth(this, 56);
            if (dgvSalary.Columns["姓名"] != null) dgvSalary.Columns["姓名"].Width = nameColumnWidth;
            if (dgvSalary.Columns["民族"] != null) dgvSalary.Columns["民族"].Width = nameColumnWidth;
            if (dgvSalary.Columns["序号"] != null) dgvSalary.Columns["序号"].Width = narrowColumnWidth;
            if (dgvSalary.Columns["性别"] != null) dgvSalary.Columns["性别"].Width = narrowColumnWidth;
            if (dgvSalary.Columns["学位"] != null) dgvSalary.Columns["学位"].Width = narrowColumnWidth;
            if (dgvSalary.Columns["层级"] != null) dgvSalary.Columns["层级"].Visible = false;
            if (dgvSalary.Columns["属相"] != null) dgvSalary.Columns["属相"].Width = narrowColumnWidth;
            if (dgvSalary.Columns["年龄"] != null) dgvSalary.Columns["年龄"].Width = narrowColumnWidth;

            // 冻结前4列（员工编号, 部门, 职务, 姓名）
            if (dgvSalary.Columns["姓名"] != null) dgvSalary.Columns["姓名"].Frozen = true;

            // 标记可排序列：蓝色表头文字 + 排序方向箭头
            UpdateSortIndicators();
        }

        /// <summary>
        /// 更新可排序列的表头样式：蓝色文字提示可排序，活跃排序列显示方向箭头
        /// </summary>
        private void UpdateSortIndicators() {
            Color sortableColor = Color.FromArgb(0, 102, 204); // 蓝色

            foreach (DataGridViewColumn col in dgvSalary.Columns) {
                string colName = col.Name;
                bool isSortable = _enumSortOrders.ContainsKey(colName) || _dateSortColumns.Contains(colName) || colName == "年龄";

                if (isSortable) {
                    // 蓝色表头文字表示可排序
                    col.HeaderCell.Style.ForeColor = sortableColor;

                    // 如果是当前排序列，显示排序方向箭头
                    if (_currentSortColumn == colName) {
                        string arrow = "";
                        if (colName == "年龄") {
                            arrow = _ageSortState == 1 ? " ▼" : (_ageSortState == 2 ? " ▲" : "");
                        } else if (_dateSortColumns.Contains(colName)) {
                            arrow = " ▲"; // 日期默认升序
                        } else {
                            arrow = " ▲"; // 枚举按优先级排序
                        }
                        col.HeaderText = colName + arrow;
                    } else {
                        col.HeaderText = colName;
                    }
                }
            }
        }

        /// <summary>
        /// 条件设置按钮点击事件处理，打开或激活员工筛选条件窗体
        /// </summary>
        private void btnCondition_Click(object sender, EventArgs e) {
            _popupCondition?.Show(btnCondition, 0, btnCondition.Height);
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
                    // 导出前去掉排序箭头，避免污染 Excel 表头
                    var savedHeaders = new Dictionary<int, string>();
                    foreach (DataGridViewColumn col in dgvSalary.Columns) {
                        if (col.HeaderText.EndsWith(" ▲") || col.HeaderText.EndsWith(" ▼")) {
                            savedHeaders[col.Index] = col.HeaderText;
                            col.HeaderText = col.Name;
                        }
                    }

                    ExcelHelper.ExportToExcel(dgvSalary, sfd.FileName);

                    // 导出后恢复排序箭头
                    foreach (var kv in savedHeaders) {
                        dgvSalary.Columns[kv.Key].HeaderText = kv.Value;
                    }

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

        private void btnBirthday_Click(object sender, EventArgs e) {
            _popupBirthday?.Show(btnBirthday, 0, btnBirthday.Height);
        }

        private void btnWorkDate_Click(object sender, EventArgs e) {
            _popupWorkDate?.Show(btnWorkDate, 0, btnWorkDate.Height);
        }

        private void btnHiredate_Click(object sender, EventArgs e) {
            _popupHiredate?.Show(btnHiredate, 0, btnHiredate.Height);
        }

        private void btnPositionDate_Click(object sender, EventArgs e) {
            _popupPositionDate?.Show(btnPositionDate, 0, btnPositionDate.Height);
        }

        private void btnAge_Click(object sender, EventArgs e) {
            _popupAge?.Show(btnAge, 0, btnAge.Height);
        }

        private void btnSkill_Click(object sender, EventArgs e) {
            _popupSkill?.Show(btnSkill, 0, btnSkill.Height);
        }

        private void btnTechnology_Click(object sender, EventArgs e) {
            _popupTechnology?.Show(btnTechnology, 0, btnTechnology.Height);
        }

        // ===== 表头排序逻辑 =====

        /// <summary>
        /// DataGridView 列头点击事件处理，实现自定义排序
        /// </summary>
        private void dgvSalary_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
            if (_originalData == null || _originalData.Rows.Count == 0) return;

            string columnName = dgvSalary.Columns[e.ColumnIndex].Name;

            // 判断是否为可排序列
            bool isEnumColumn = _enumSortOrders.ContainsKey(columnName);
            bool isDateColumn = _dateSortColumns.Contains(columnName);
            bool isAgeColumn = columnName == "年龄";

            if (!isEnumColumn && !isDateColumn && !isAgeColumn) return;

            // 处理年龄列的三态排序
            if (isAgeColumn) {
                HandleAgeSortClick(columnName);
                return;
            }

            // 其他列：两态切换（排序 ↔ 恢复初始）
            if (_currentSortColumn == columnName) {
                // 已在排序状态，恢复初始排序
                RestoreOriginalOrder();
            } else {
                // 应用排序
                _currentSortColumn = columnName;
                _ageSortState = 0;

                if (isEnumColumn) {
                    ApplyEnumSort(columnName);
                } else if (isDateColumn) {
                    ApplyDateSort(columnName);
                }
            }
        }

        /// <summary>
        /// 处理年龄列的三态排序: 降序(大→小) → 升序(小→大) → 恢复初始
        /// </summary>
        private void HandleAgeSortClick(string columnName) {
            // 如果当前排序列不是年龄，从头开始
            if (_currentSortColumn != columnName) {
                _ageSortState = 0;
            }

            _ageSortState++;
            _currentSortColumn = columnName;

            if (_ageSortState == 1) {
                // 第一次点击：从大到小
                ApplySimpleSort(columnName, "DESC");
            } else if (_ageSortState == 2) {
                // 第二次点击：从小到大
                ApplySimpleSort(columnName, "ASC");
            } else {
                // 第三次点击：恢复初始排序
                _ageSortState = 0;
                RestoreOriginalOrder();
            }
        }

        /// <summary>
        /// 恢复初始排序（使用原始数据副本）
        /// </summary>
        private void RestoreOriginalOrder() {
            _currentSortColumn = null;
            _ageSortState = 0;

            DataTable dt = _originalData.Copy();
            dgvSalary.DataSource = null;
            dgvSalary.Columns.Clear();
            dgvSalary.DataSource = dt;
            FormatGrid();
        }

        /// <summary>
        /// 对日期/数值列应用简单排序
        /// </summary>
        private void ApplySimpleSort(string columnName, string direction) {
            DataTable dt = _originalData.Copy();
            DataView dv = dt.DefaultView;
            dv.Sort = $"[{columnName}] {direction}";
            DataTable sorted = dv.ToTable();

            dgvSalary.DataSource = null;
            dgvSalary.Columns.Clear();
            dgvSalary.DataSource = sorted;
            FormatGrid();
        }

        /// <summary>
        /// 对日期列应用升序排序（从早到晚）
        /// </summary>
        private void ApplyDateSort(string columnName) {
            ApplySimpleSort(columnName, "ASC");
        }

        /// <summary>
        /// 对枚举类型列应用自定义排序（按预定义优先级排序）
        /// </summary>
        private void ApplyEnumSort(string columnName) {
            if (!_enumSortOrders.ContainsKey(columnName)) return;

            var sortOrder = _enumSortOrders[columnName];
            DataTable dt = _originalData.Copy();

            // 添加临时排序辅助列
            string sortKeyCol = "_sort_key_";
            dt.Columns.Add(sortKeyCol, typeof(int));

            foreach (DataRow row in dt.Rows) {
                string val = row[columnName]?.ToString() ?? "";
                row[sortKeyCol] = sortOrder.ContainsKey(val) ? sortOrder[val] : 999;
            }

            // 按辅助列排序
            DataView dv = dt.DefaultView;
            dv.Sort = $"[{sortKeyCol}] ASC";
            DataTable sorted = dv.ToTable();

            // 移除辅助列
            sorted.Columns.Remove(sortKeyCol);

            dgvSalary.DataSource = null;
            dgvSalary.Columns.Clear();
            dgvSalary.DataSource = sorted;
            FormatGrid();
        }
    }
}