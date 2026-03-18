using CompensationSystemSubInterface.Models;
using CompensationSystemSubInterface.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CompensationSystemSubInterface {
    /// <summary>
    /// 通用员工筛选条件控件（WPF UserControl 版本）
    /// 只包含员工筛选功能，带搜索框，CheckBox 即时触发筛选
    /// </summary>
    public partial class WpfEmpCondition : UserControl {
        /// <summary>
        /// 获取当前选中的员工ID列表
        /// </summary>
        public List<int> SelectedEmployeeIds => _persistentEmpIds.ToList();

        /// <summary>
        /// 定义事件: CheckBox 变化时将最新的条件传给主界面
        /// </summary>
        public event Action<List<int>> ApplySelect;

        /// <summary>
        /// 树形结构的根节点集合
        /// </summary>
        public ObservableCollection<EmpConditionTreeNode> TreeRoots { get; set; } = new ObservableCollection<EmpConditionTreeNode>();

        // 员工根节点
        private EmpConditionTreeNode _empRoot;

        // 持久化选中的员工ID（包括搜索时隐藏的项）
        private HashSet<int> _persistentEmpIds = new HashSet<int>();

        // 筛选条件（从主界面传入）
        private EmpCondition _filterCondition = new EmpCondition();

        // 是否显示离职员工（false=在职，true=离职）
        private bool _showTerminatedEmployees = false;

        // 是否正在刷新数据（防止搜索时触发多余事件）
        private bool _isRefreshing = false;

        /// <summary>
        /// 初始化员工筛选条件控件
        /// </summary>
        /// <param name="existingIds">现有的员工ID列表</param>
        /// <param name="departmentIds">部门筛选条件（可选）</param>
        /// <param name="showTerminatedEmployees">是否显示离职员工（默认false=在职员工）</param>
        public WpfEmpCondition(List<int> existingIds, List<int> departmentIds = null, bool showTerminatedEmployees = false) {
            InitializeComponent();

            // 设置是否显示离职员工
            _showTerminatedEmployees = showTerminatedEmployees;

            // 初始化持久化ID
            if (existingIds != null) {
                _persistentEmpIds = new HashSet<int>(existingIds);
            }

            // 初始化筛选条件
            if (departmentIds != null && departmentIds.Count > 0) {
                _filterCondition.DepartmentIds = departmentIds;
            }

            // 绑定数据源
            treeConditions.ItemsSource = TreeRoots;

            InitializeStructure();
            RefreshEmployeeData();
        }

        /// <summary>
        /// 获取选中的员工数量
        /// </summary>
        public int GetSelectedCount() {
            return _persistentEmpIds.Count;
        }

        /// <summary>
        /// 判断是否全选（持久化ID为空表示没有筛选，等同于全选）
        /// </summary>
        public bool IsAllSelected() {
            return _persistentEmpIds.Count == 0;
        }

        /// <summary>
        /// 初始化树形结构根节点
        /// </summary>
        private void InitializeStructure() {
            TreeRoots.Clear();

            _empRoot = new EmpConditionTreeNode { Name = "员工", NodeType = EmpConditionNodeType.Category };
            TreeRoots.Add(_empRoot);
        }

        /// <summary>
        /// 刷新员工数据
        /// </summary>
        private void RefreshEmployeeData() {
            try {
                _isRefreshing = true;

                // 查询所有需要的字段，根据_showTerminatedEmployees决定查询在职或离职员工
                int zaizhi = _showTerminatedEmployees ? 0 : 1;
                string empSql = $@"SELECT y.id, y.xingming, y.yonghuming, y.xlid, y.bmid, y.gwid, 
                                  y.xingbie, y.minzu, y.shuxing, y.zhengzhimm, 
                                  y.xueli, y.xuewei, y.zhichengdj,
                                  y.chushengrq, y.gongzuosj, y.rusisj, y.gangweisj,
                                  y.nianling, y.zhuanyejn, y.zhuanyejs
                                  FROM ZX_config_yg y WHERE y.zaizhi={zaizhi} ORDER BY y.xuhao";
                DataTable dtEmp = SqlHelper.ExecuteDataTable(empSql);

                // 更新持久化ID
                foreach (var child in _empRoot.Children) {
                    if (child.IsChecked == true) {
                        _persistentEmpIds.Add(child.Id);
                    } else {
                        _persistentEmpIds.Remove(child.Id);
                    }
                }

                string searchText = txtSearchEmp?.Text?.Trim() ?? string.Empty;

                _empRoot.Children.Clear();
                foreach (DataRow dr in dtEmp.Rows) {
                    int xlid = dr["xlid"] != DBNull.Value ? Convert.ToInt32(dr["xlid"]) : 0;
                    int bmid = dr["bmid"] != DBNull.Value ? Convert.ToInt32(dr["bmid"]) : 0;
                    int gwid = dr["gwid"] != DBNull.Value ? Convert.ToInt32(dr["gwid"]) : 0;
                    string name = dr["xingming"].ToString();
                    string yonghuming = dr["yonghuming"]?.ToString() ?? "";
                    int empId = Convert.ToInt32(dr["id"]);
                    string xingbie = dr["xingbie"]?.ToString() ?? "";
                    string minzu = dr["minzu"]?.ToString() ?? "";
                    string shuxing = dr["shuxing"]?.ToString() ?? "";
                    string zhengzhimm = dr["zhengzhimm"]?.ToString() ?? "";
                    string xueli = dr["xueli"]?.ToString() ?? "";
                    string xuewei = dr["xuewei"]?.ToString() ?? "";
                    string zhichengdj = dr["zhichengdj"]?.ToString() ?? "";

                    // 按序列筛选
                    if (_filterCondition.SequenceIds.Count > 0 && !_filterCondition.SequenceIds.Contains(xlid)) continue;
                    // 按部门筛选
                    if (_filterCondition.DepartmentIds.Count > 0 && !_filterCondition.DepartmentIds.Contains(bmid)) continue;
                    // 按职务筛选
                    if (_filterCondition.PositionIds.Count > 0 && !_filterCondition.PositionIds.Contains(gwid)) continue;
                    // 按性别筛选
                    if (_filterCondition.Genders.Count > 0 && !_filterCondition.Genders.Contains(xingbie)) continue;
                    // 按民族筛选
                    if (_filterCondition.Ethnics.Count > 0 && !_filterCondition.Ethnics.Contains(minzu)) continue;
                    // 按属相筛选
                    if (_filterCondition.Zodiacs.Count > 0 && !_filterCondition.Zodiacs.Contains(shuxing)) continue;
                    // 按政治面貌筛选
                    if (_filterCondition.Politics.Count > 0 && !_filterCondition.Politics.Contains(zhengzhimm)) continue;
                    // 按学历筛选
                    if (_filterCondition.Educations.Count > 0 && !_filterCondition.Educations.Contains(xueli)) continue;
                    // 按学位筛选
                    if (_filterCondition.Degrees.Count > 0 && !_filterCondition.Degrees.Contains(xuewei)) continue;
                    // 按职称等级筛选
                    if (_filterCondition.TitleLevels.Count > 0 && !_filterCondition.TitleLevels.Contains(zhichengdj)) continue;

                    // 按出生日期年月筛选
                    if (_filterCondition.BirthdayYearMonths.Count > 0) {
                        DateTime? dt2 = dr["chushengrq"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["chushengrq"]) : null;
                        if (dt2 == null || !_filterCondition.BirthdayYearMonths.Contains(dt2.Value.ToString("yyyy-MM"))) continue;
                    }
                    // 按参加工作时间年月筛选
                    if (_filterCondition.WorkDateYearMonths.Count > 0) {
                        DateTime? dt2 = dr["gongzuosj"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["gongzuosj"]) : null;
                        if (dt2 == null || !_filterCondition.WorkDateYearMonths.Contains(dt2.Value.ToString("yyyy-MM"))) continue;
                    }
                    // 按入社时间年月筛选
                    if (_filterCondition.HireDateYearMonths.Count > 0) {
                        DateTime? dt2 = dr["rusisj"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["rusisj"]) : null;
                        if (dt2 == null || !_filterCondition.HireDateYearMonths.Contains(dt2.Value.ToString("yyyy-MM"))) continue;
                    }
                    // 按任现岗位时间年月筛选
                    if (_filterCondition.PositionDateYearMonths.Count > 0) {
                        DateTime? dt2 = dr["gangweisj"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["gangweisj"]) : null;
                        if (dt2 == null || !_filterCondition.PositionDateYearMonths.Contains(dt2.Value.ToString("yyyy-MM"))) continue;
                    }
                    // 按年龄段筛选
                    if (_filterCondition.AgeRanges.Count > 0) {
                        int age = dr["nianling"] != DBNull.Value ? Convert.ToInt32(dr["nianling"]) : -1;
                        bool matched = false;
                        foreach (var range in _filterCondition.AgeRanges) {
                            if (range == "35岁以下" && age >= 0 && age < 35) { matched = true; break; }
                            else if (range == "35-39" && age >= 35 && age <= 39) { matched = true; break; }
                            else if (range == "40-44" && age >= 40 && age <= 44) { matched = true; break; }
                            else if (range == "45-49" && age >= 45 && age <= 49) { matched = true; break; }
                            else if (range == "50-54" && age >= 50 && age <= 54) { matched = true; break; }
                            else if (range == "55岁以上" && age >= 55) { matched = true; break; }
                        }
                        if (!matched) continue;
                    }
                    // 按专业技能筛选
                    if (_filterCondition.Skills.Count > 0) {
                        string skill = dr["zhuanyejn"]?.ToString() ?? "";
                        if (!_filterCondition.Skills.Contains(skill)) continue;
                    }
                    // 按专业技术筛选
                    if (_filterCondition.Technologies.Count > 0) {
                        string tech = dr["zhuanyejs"]?.ToString() ?? "";
                        if (!_filterCondition.Technologies.Contains(tech)) continue;
                    }

                    // 按搜索文本筛选（姓名或用户名）
                    if (!string.IsNullOrEmpty(searchText) && !name.Contains(searchText) && !yonghuming.Contains(searchText)) continue;

                    var child = new EmpConditionTreeNode {
                        Name = name,
                        Id = empId,
                        NodeType = EmpConditionNodeType.Item,
                        Parent = _empRoot,
                        IsChecked = _persistentEmpIds.Contains(empId)
                    };
                    _empRoot.Children.Add(child);
                }
                _empRoot.UpdateDisplayText();

            } catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine($"RefreshEmployeeData error: {ex}");
            } finally {
                _isRefreshing = false;
            }
        }

        /// <summary>
        /// 外部调用：更新筛选条件并刷新员工列表
        /// </summary>
        public void RefreshFilterConditions(EmpCondition condition) {
            _filterCondition = condition ?? new EmpCondition();
            RefreshEmployeeData();
        }

        /// <summary>
        /// 外部调用：更新部门筛选条件并刷新员工列表（兼容旧接口）
        /// </summary>
        public void RefreshFilterConditions(List<int> departmentIds) {
            _filterCondition.DepartmentIds = departmentIds ?? new List<int>();
            RefreshEmployeeData();
        }

        /// <summary>
        /// CheckBox 点击事件处理
        /// </summary>
        private void CheckBox_Click(object sender, RoutedEventArgs e) {
            var cb = sender as CheckBox;
            var node = cb?.DataContext as EmpConditionTreeNode;
            if (node == null) return;

            HandleCheckLogic(node);
            UpdatePersistentEmpIds(node);

            // 即时触发筛选事件
            if (!_isRefreshing) {
                ApplySelect?.Invoke(_persistentEmpIds.ToList());
            }
        }

        private void HandleCheckLogic(EmpConditionTreeNode node) {
            if (node.NodeType == EmpConditionNodeType.Category) {
                bool isChecked = node.IsChecked == true;
                foreach (var child in node.Children) {
                    child.IsChecked = isChecked;
                }
                // 更新父节点的显示文本（选择/全部数字）
                node.UpdateDisplayText();
            } else {
                node.Parent?.UpdateCheckState();
            }
        }

        private void UpdatePersistentEmpIds(EmpConditionTreeNode node) {
            if (node.NodeType == EmpConditionNodeType.Item) {
                if (node.IsChecked == true) {
                    _persistentEmpIds.Add(node.Id);
                } else {
                    _persistentEmpIds.Remove(node.Id);
                }
            } else if (node.NodeType == EmpConditionNodeType.Category) {
                foreach (var child in node.Children) {
                    UpdatePersistentEmpIds(child);
                }
            }
        }

        private void txtSearchEmp_TextChanged(object sender, TextChangedEventArgs e) {
            RefreshEmployeeData();
        }

        private void ContextMenu_SelectAll_Click(object sender, RoutedEventArgs e) {
            ProcessContextMenuAction(sender, child => child.IsChecked = true);
        }

        private void ContextMenu_Invert_Click(object sender, RoutedEventArgs e) {
            ProcessContextMenuAction(sender, child => child.IsChecked = !(child.IsChecked == true));
        }

        private void ContextMenu_ClearAll_Click(object sender, RoutedEventArgs e) {
            ProcessContextMenuAction(sender, child => child.IsChecked = false);
        }

        private void ProcessContextMenuAction(object sender, Action<EmpConditionTreeNode> action) {
            var node = GetContextMenuNode(sender);
            if (node == null) return;

            var targetNode = node.NodeType == EmpConditionNodeType.Category ? node : node.Parent;
            if (targetNode == null) return;

            foreach (var child in targetNode.Children) {
                if (child.NodeType == EmpConditionNodeType.Item) {
                    action(child);
                }
            }
            targetNode.UpdateCheckState();

            // 更新持久化ID
            UpdatePersistentEmpIds(targetNode);

            // 即时触发筛选事件
            ApplySelect?.Invoke(_persistentEmpIds.ToList());
        }

        private EmpConditionTreeNode GetContextMenuNode(object sender) {
            var menuItem = sender as MenuItem;
            var contextMenu = menuItem?.Parent as ContextMenu;
            var stackPanel = contextMenu?.PlacementTarget as StackPanel;
            return stackPanel?.DataContext as EmpConditionTreeNode;
        }
    }

    #region 数据模型

    public enum EmpConditionNodeType {
        Category,
        Item
    }

    public class EmpConditionTreeNode : INotifyPropertyChanged {
        public string Name { get; set; }
        public int Id { get; set; }
        public EmpConditionNodeType NodeType { get; set; }
        public EmpConditionTreeNode Parent { get; set; }
        public ObservableCollection<EmpConditionTreeNode> Children { get; set; } = new ObservableCollection<EmpConditionTreeNode>();

        private bool? _isChecked = false;
        public bool? IsChecked {
            get => _isChecked;
            set {
                if (_isChecked != value) {
                    _isChecked = value;
                    OnPropertyChanged(nameof(IsChecked));
                }
            }
        }

        public bool IsThreeState => NodeType == EmpConditionNodeType.Category;

        public string DisplayText {
            get {
                if (NodeType == EmpConditionNodeType.Category && Children.Count > 0) {
                    int checkedCount = Children.Count(c => c.IsChecked == true);
                    int totalCount = Children.Count;
                    return $"{Name} ({checkedCount}/{totalCount})";
                }
                return Name;
            }
        }

        public string FontWeight => NodeType == EmpConditionNodeType.Category ? "Bold" : "Normal";

        public void UpdateCheckState() {
            if (Children.Count == 0) return;

            bool allChecked = Children.All(c => c.IsChecked == true);
            bool anyChecked = Children.Any(c => c.IsChecked == true || c.IsChecked == null);

            if (allChecked) IsChecked = true;
            else if (anyChecked) IsChecked = null;
            else IsChecked = false;

            UpdateDisplayText();
        }

        public void UpdateDisplayText() {
            OnPropertyChanged(nameof(DisplayText));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    #endregion
}
