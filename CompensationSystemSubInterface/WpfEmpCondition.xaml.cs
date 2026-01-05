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
    /// 通用员工筛选条件窗体（WPF版本）
    /// 只包含员工筛选功能，带搜索框
    /// </summary>
    public partial class WpfEmployeeCondition : Window {
        /// <summary>
        /// 获取当前选中的员工ID列表
        /// </summary>
        public List<int> SelectedEmployeeIds => _persistentEmpIds.ToList();

        /// <summary>
        /// 定义事件: 点击应用时将最新的条件传给主界面
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

        // 部门筛选条件（从主界面传入）
        private List<int> _departmentIds = new List<int>();

        /// <summary>
        /// 初始化员工筛选条件窗体
        /// </summary>
        /// <param name="existingIds">现有的员工ID列表</param>
        /// <param name="departmentIds">部门筛选条件（可选）</param>
        public WpfEmployeeCondition(List<int> existingIds, List<int> departmentIds = null) {
            InitializeComponent();

            // 初始化持久化ID
            if (existingIds != null) {
                _persistentEmpIds = new HashSet<int>(existingIds);
            }

            _departmentIds = departmentIds ?? new List<int>();

            // 绑定数据源
            treeConditions.ItemsSource = TreeRoots;

            InitializeStructure();
            RefreshEmployeeData();
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
                string empSql = "SELECT id, xingming, bmid FROM ZX_config_yg WHERE zaizhi=1 ORDER BY xuhao";
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
                    int bmid = dr["bmid"] != DBNull.Value ? Convert.ToInt32(dr["bmid"]) : 0;
                    string name = dr["xingming"].ToString();
                    int empId = Convert.ToInt32(dr["id"]);

                    // 按部门筛选
                    if (_departmentIds.Count > 0 && !_departmentIds.Contains(bmid)) continue;
                    // 按搜索文本筛选
                    if (!string.IsNullOrEmpty(searchText) && !name.Contains(searchText)) continue;

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
            }
        }

        /// <summary>
        /// 外部调用：更新部门筛选条件并刷新员工列表
        /// </summary>
        public void RefreshFilterConditions(List<int> departmentIds) {
            _departmentIds = departmentIds ?? new List<int>();
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
        }

        private void HandleCheckLogic(EmpConditionTreeNode node) {
            if (node.NodeType == EmpConditionNodeType.Category) {
                bool isChecked = node.IsChecked == true;
                foreach (var child in node.Children) {
                    child.IsChecked = isChecked;
                }
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
        }

        private EmpConditionTreeNode GetContextMenuNode(object sender) {
            var menuItem = sender as MenuItem;
            var contextMenu = menuItem?.Parent as ContextMenu;
            var stackPanel = contextMenu?.PlacementTarget as StackPanel;
            return stackPanel?.DataContext as EmpConditionTreeNode;
        }

        private void btnReset_Click(object sender, RoutedEventArgs e) {
            _persistentEmpIds.Clear();
            InitializeStructure();
            RefreshEmployeeData();
        }

        private void btnApply_Click(object sender, RoutedEventArgs e) {
            ApplySelect?.Invoke(_persistentEmpIds.ToList());
        }

        private void btnClose_Click(object sender, RoutedEventArgs e) {
            this.Close();
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
