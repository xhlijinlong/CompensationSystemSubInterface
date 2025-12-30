using CompensationSystemSubInterface.Models;
using CompensationSystemSubInterface.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace CompensationSystemSubInterface {
    /// <summary>
    /// WpfSalaryCondition.xaml 的交互逻辑
    /// 薪资查询高级筛选条件窗体（WPF版本）
    /// </summary>
    public partial class WpfSalaryCondition : Window {
        /// <summary>
        /// 获取当前的筛选条件
        /// </summary>
        public SalaryQueryCondition CurrentCondition { get; private set; }

        /// <summary>
        /// 定义事件: 点击应用时将最新的条件传给主界面
        /// </summary>
        public event Action<SalaryQueryCondition> ApplySelect;

        /// <summary>
        /// 树形结构的根节点集合
        /// </summary>
        public ObservableCollection<ConditionTreeNode> TreeRoots { get; set; } = new ObservableCollection<ConditionTreeNode>();

        // Cache root node references for each category for convenient operations
        private ConditionTreeNode _empRoot;
        private ConditionTreeNode _itemRoot;

        // Static regex for level parsing (compiled for better performance)
        private static readonly Regex LevelNumberRegex = new Regex(@"\d+", RegexOptions.Compiled);

        // Persistent tracking of selected Salary Item IDs (including hidden items)
        private HashSet<int> _persistentItemIds = new HashSet<int>();

        // Persistent tracking of selected Employee IDs (including hidden items during search)
        private HashSet<int> _persistentEmpIds = new HashSet<int>();

        /// <summary>
        /// 初始化薪资筛选条件窗体
        /// </summary>
        /// <param name="existing">现有的筛选条件，如果为 null 则创建新的筛选条件对象</param>
        public WpfSalaryCondition(SalaryQueryCondition existing) {
            InitializeComponent();

            CurrentCondition = existing != null ? existing.Clone() : new SalaryQueryCondition();
            
            // Initialize persistent item IDs from existing condition
            if (CurrentCondition.SalaryItemIds != null) {
                _persistentItemIds = new HashSet<int>(CurrentCondition.SalaryItemIds);
            }

            // Initialize persistent employee IDs from existing condition
            if (CurrentCondition.EmployeeIds != null) {
                _persistentEmpIds = new HashSet<int>(CurrentCondition.EmployeeIds);
            }

            // 绑定数据源
            treeConditions.ItemsSource = TreeRoots;

            InitializeStructure();
            LoadStaticData(); // Load non-cascading static data (positions, levels, salary items)
            RefreshCascadingData(); // Load cascading data (departments, employees)

            RestoreSelection();
        }

        /// <summary>
        /// 初始化树形结构根节点
        /// </summary>
        private void InitializeStructure() {
            TreeRoots.Clear();

            _empRoot = new ConditionTreeNode { Name = "员工", NodeType = ConditionNodeType.Category };
            TreeRoots.Add(_empRoot);

            _itemRoot = new ConditionTreeNode { Name = "薪资项目", NodeType = ConditionNodeType.Category };
            TreeRoots.Add(_itemRoot);
        }

        /// <summary>
        /// Load static data (salary items only - sequences, positions, levels are now in main control)
        /// </summary>
        private void LoadStaticData() {
            try {
                // Load salary items (with special grouping)
                LoadSalaryItems();

            } catch (Exception ex) {
                MessageBox.Show("加载静态数据出错: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Refresh employee data based on filter conditions from main control (CurrentCondition)
        /// </summary>
        private void RefreshCascadingData() {
            try {
                // Get filter conditions from CurrentCondition (passed from main control)
                List<int> selectedSeqIds = CurrentCondition.SequenceIds ?? new List<int>();
                List<int> selectedDeptIds = CurrentCondition.DepartmentIds ?? new List<int>();
                List<int> selectedPostIds = CurrentCondition.PositionIds ?? new List<int>();

                // Note: No user input in SQL query - all data comes from database or controlled sources
                string empSql = "SELECT id, xingming, xlid, bmid, gwid FROM ZX_config_yg WHERE zaizhi=1 ORDER BY xuhao";
                DataTable dtEmp = SqlHelper.ExecuteDataTable(empSql);

                // Update persistent employee IDs from currently visible checked items before clearing
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
                    int empId = Convert.ToInt32(dr["id"]);

                    // Filter by sequence (from main control)
                    if (selectedSeqIds.Count > 0 && !selectedSeqIds.Contains(xlid)) continue;
                    // Filter by department (from main control)
                    if (selectedDeptIds.Count > 0 && !selectedDeptIds.Contains(bmid)) continue;
                    // Filter by position (from main control)
                    if (selectedPostIds.Count > 0 && !selectedPostIds.Contains(gwid)) continue;
                    // Filter by search text (employees only)
                    if (!string.IsNullOrEmpty(searchText) && !name.Contains(searchText)) continue;

                    var child = new ConditionTreeNode {
                        Name = name,
                        Id = empId,
                        NodeType = ConditionNodeType.Item,
                        Parent = _empRoot,
                        // Restore checked state from persistent IDs
                        IsChecked = _persistentEmpIds.Contains(empId)
                    };
                    _empRoot.Children.Add(child);
                }
                _empRoot.UpdateDisplayText();

            } catch (Exception ex) {
                // Avoid showing too many error dialogs during initialization or frequent operations
                System.Diagnostics.Debug.WriteLine($"RefreshCascadingData error: {ex}");
            }
        }

        /// <summary>
        /// Load salary items and group them by QueryType
        /// </summary>
        private void LoadSalaryItems() {
            _itemRoot.Children.Clear();
            // Note: Added QueryType column to query
            string sql = "SELECT ItemId AS id, ItemName, QueryType FROM ZX_SalaryItems WHERE IsEnabled=1 ORDER BY DisplayOrder ASC";
            DataTable dt = SqlHelper.ExecuteDataTable(sql);

            var groups = dt.AsEnumerable()
                .GroupBy(r => {
                    var queryType = r["QueryType"];
                    return queryType == DBNull.Value || string.IsNullOrWhiteSpace(queryType.ToString()) 
                              ? "其他" : queryType.ToString();
                });

            foreach (var group in groups) {
                var groupNode = new ConditionTreeNode {
                    Name = group.Key,
                    NodeType = ConditionNodeType.Group,
                    Parent = _itemRoot
                };

                foreach (var row in group) {
                    var child = new ConditionTreeNode {
                        Name = row["ItemName"].ToString(),
                        Id = Convert.ToInt32(row["id"]),
                        NodeType = ConditionNodeType.Item,
                        Parent = groupNode
                    };
                    groupNode.Children.Add(child);
                }
                groupNode.UpdateDisplayText();
                _itemRoot.Children.Add(groupNode);
            }
            _itemRoot.UpdateDisplayText();
        }

        /// <summary>
        /// 通用加载数据方法
        /// </summary>
        private void LoadTreeData(ConditionTreeNode parent, string sql, string displayField, string valueField) {
            parent.Children.Clear();
            DataTable dt = SqlHelper.ExecuteDataTable(sql);
            foreach (DataRow dr in dt.Rows) {
                var child = new ConditionTreeNode {
                    Name = dr[displayField].ToString(),
                    Id = Convert.ToInt32(dr[valueField]),
                    NodeType = ConditionNodeType.Item,
                    IsChecked = false,
                    Parent = parent
                };
                parent.Children.Add(child);
            }
            parent.UpdateDisplayText();
        }

        /// <summary>
        /// Restore selection state based on CurrentCondition (only for employee and salary items)
        /// </summary>
        private void RestoreSelection() {
            // Refresh employee list based on filter conditions from main control
            RefreshCascadingData();
            
            // Restore employee selections
            SetChecks(_empRoot, CurrentCondition.EmployeeIds);
            
            // Restore salary items (two-level structure)
            SetChecksRecursive(_itemRoot, CurrentCondition.SalaryItemIds);
        }

        private void SetChecks(ConditionTreeNode parent, List<int> ids) {
            if (ids == null) return;
            foreach (var child in parent.Children) {
                child.IsChecked = ids.Contains(child.Id);
            }
            parent.UpdateCheckState();
        }

        // Recursively set checked state (for handling grouped nodes)
        private void SetChecksRecursive(ConditionTreeNode parent, List<int> ids) {
            if (ids == null) return;
            foreach (var child in parent.Children) {
                if (child.NodeType == ConditionNodeType.Group) {
                    SetChecksRecursive(child, ids);
                } else {
                    child.IsChecked = ids.Contains(child.Id);
                }
            }
            parent.UpdateCheckState();
        }

        /// <summary>
        /// CheckBox click event handler
        /// </summary>
        private void CheckBox_Click(object sender, RoutedEventArgs e) {
            var cb = sender as CheckBox;
            var node = cb?.DataContext as ConditionTreeNode;
            if (node == null) return;

            // Handle check logic
            HandleCheckLogic(node);

            // Update persistent item IDs for Salary Items
            if (IsDescendantOf(node, _itemRoot)) {
                UpdatePersistentItemIds(node);
            }

            // Update persistent employee IDs for Employees
            if (IsDescendantOf(node, _empRoot)) {
                UpdatePersistentEmpIds(node);
            }
            // Note: Sequence/department/position/level are now in main control, no cascading needed here
        }

        /// <summary>
        /// Update persistent item IDs based on checked state changes
        /// </summary>
        private void UpdatePersistentItemIds(ConditionTreeNode node) {
            if (node.NodeType == ConditionNodeType.Item) {
                if (node.IsChecked == true) {
                    _persistentItemIds.Add(node.Id);
                } else {
                    _persistentItemIds.Remove(node.Id);
                }
            } else if (node.NodeType == ConditionNodeType.Category || node.NodeType == ConditionNodeType.Group) {
                // Recursively update all child items
                foreach (var child in node.Children) {
                    UpdatePersistentItemIds(child);
                }
            }
        }

        /// <summary>
        /// Update persistent employee IDs based on checked state changes
        /// </summary>
        private void UpdatePersistentEmpIds(ConditionTreeNode node) {
            if (node.NodeType == ConditionNodeType.Item) {
                if (node.IsChecked == true) {
                    _persistentEmpIds.Add(node.Id);
                } else {
                    _persistentEmpIds.Remove(node.Id);
                }
            } else if (node.NodeType == ConditionNodeType.Category) {
                // Update all child items
                foreach (var child in node.Children) {
                    UpdatePersistentEmpIds(child);
                }
            }
        }

        // Recursively set checked state
        private void HandleCheckLogic(ConditionTreeNode node) {
            if (node.NodeType == ConditionNodeType.Category || node.NodeType == ConditionNodeType.Group) {
                bool isChecked = node.IsChecked == true;
                foreach (var child in node.Children) {
                    child.IsChecked = isChecked;
                    if (child.NodeType == ConditionNodeType.Group) {
                        HandleCheckLogic(child); // Recursively handle deep grouping
                    }
                }
            } else {
                node.Parent?.UpdateCheckState();
                // If parent is a group, the group's parent (Category) also needs updating
                if (node.Parent?.Parent != null) {
                    node.Parent.Parent.UpdateCheckState();
                }
            }
        }

        private bool IsDescendantOf(ConditionTreeNode node, ConditionTreeNode root) {
            if (node == root) return true;
            if (node.Parent == null) return false;
            return IsDescendantOf(node.Parent, root);
        }

        /// <summary>
        /// Employee search textbox text changed event handler
        /// </summary>
        private void txtSearchEmp_TextChanged(object sender, TextChangedEventArgs e) {
            // Trigger employee list refresh (uses filter logic in RefreshCascadingData)
            RefreshCascadingData();
        }

        /// <summary>
        /// Salary item search textbox text changed event handler
        /// </summary>
        private void txtSearchItem_TextChanged(object sender, TextChangedEventArgs e) {
            string text = txtSearchItem.Text.Trim();
            // Filter salary items (local filtering)
            FilterSalaryItems(text);
        }

        private void FilterSalaryItems(string keyword) {
            // Reload all salary items to restore complete list (important when clearing a previous search)
            LoadSalaryItems();
            
            if (string.IsNullOrEmpty(keyword)) {
                // No filter, just restore checked state from persistent IDs
                SetChecksRecursive(_itemRoot, _persistentItemIds.ToList());
                return;
            }

            // Iterate in reverse order to remove non-matching nodes
            for (int i = _itemRoot.Children.Count - 1; i >= 0; i--) {
                var group = _itemRoot.Children[i];
                bool groupHasMatch = false;

                for (int j = group.Children.Count - 1; j >= 0; j--) {
                    var item = group.Children[j];
                    if (!item.Name.Contains(keyword)) {
                        group.Children.RemoveAt(j);
                    } else {
                        groupHasMatch = true;
                    }
                }

                if (!groupHasMatch) {
                    _itemRoot.Children.RemoveAt(i);
                }
                // Note: Auto-expand groups with matches would require binding IsExpanded in XAML
                // This is left for UI implementation
            }
            
            // Restore checked state from persistent IDs after filtering
            SetChecksRecursive(_itemRoot, _persistentItemIds.ToList());
            _itemRoot.UpdateDisplayText();
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

        private void ProcessContextMenuAction(object sender, Action<ConditionTreeNode> action) {
            var node = GetContextMenuNode(sender);
            if (node == null) return;

            // Find the target container (Category or Group)
            var targetNode = (node.NodeType == ConditionNodeType.Category || node.NodeType == ConditionNodeType.Group) 
                             ? node : node.Parent;
            
            if (targetNode == null) return;

            // Apply action recursively to all descendants
            ApplyActionRecursively(targetNode, action);
            targetNode.UpdateCheckState();
            
            // Update parent if exists
            if (targetNode.Parent != null) targetNode.Parent.UpdateCheckState();

            // Update persistent item IDs if this is under Salary Items
            if (IsDescendantOf(targetNode, _itemRoot)) {
                UpdatePersistentItemIds(targetNode);
            }

            // Update persistent employee IDs if this is under Employees
            if (IsDescendantOf(targetNode, _empRoot)) {
                UpdatePersistentEmpIds(targetNode);
            }
            // Note: Sequence/department/position/level are now in main control, no cascading needed here
        }

        private void ApplyActionRecursively(ConditionTreeNode node, Action<ConditionTreeNode> action) {
            foreach (var child in node.Children) {
                if (child.NodeType == ConditionNodeType.Item) {
                    action(child);
                } else if (child.NodeType == ConditionNodeType.Group) {
                    // Recursively apply to group's children
                    ApplyActionRecursively(child, action);
                    child.UpdateCheckState();
                }
            }
        }

        private ConditionTreeNode GetContextMenuNode(object sender) {
            var menuItem = sender as MenuItem;
            var contextMenu = menuItem?.Parent as ContextMenu;
            var stackPanel = contextMenu?.PlacementTarget as StackPanel;
            return stackPanel?.DataContext as ConditionTreeNode;
        }

        private void btnReset_Click(object sender, RoutedEventArgs e) {
            InitializeStructure();
            LoadStaticData();
            RefreshCascadingData();
        }

        private void btnApply_Click(object sender, RoutedEventArgs e) {
            // Note: Sequence/department/position IDs are managed by main control, not modified here
            // Note: Using _persistentEmpIds to include hidden selected employees during search
            CurrentCondition.EmployeeIds = _persistentEmpIds.ToList();
            // Note: Using _persistentItemIds instead of GetCheckedIdsRecursive to include hidden selected items
            CurrentCondition.SalaryItemIds = _persistentItemIds.ToList();

            ApplySelect?.Invoke(CurrentCondition);
        }

        private List<int> GetCheckedIds(ConditionTreeNode parent) {
            return parent.Children
                .Where(c => c.IsChecked == true && c.NodeType == ConditionNodeType.Item)
                .Select(c => c.Id)
                .ToList();
        }
        
        private List<int> GetCheckedIdsRecursive(ConditionTreeNode parent) {
            var ids = new List<int>();
            foreach(var child in parent.Children) {
                if(child.NodeType == ConditionNodeType.Item && child.IsChecked == true) {
                    ids.Add(child.Id);
                } else if (child.NodeType == ConditionNodeType.Group) {
                    ids.AddRange(GetCheckedIdsRecursive(child));
                }
            }
            return ids;
        }

        /// <summary>
        /// 关闭按钮点击事件处理
        /// </summary>
        private void btnClose_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }

    #region 数据模型

    public enum ConditionNodeType {
        Category,
        Group, // 新增 Group 类型
        Item
    }

    public class ConditionTreeNode : INotifyPropertyChanged {
        public string Name { get; set; }
        public int Id { get; set; }
        public ConditionNodeType NodeType { get; set; }
        public ConditionTreeNode Parent { get; set; }
        public ObservableCollection<ConditionTreeNode> Children { get; set; } = new ObservableCollection<ConditionTreeNode>();

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

        public string DisplayText {
            get {
                // Category and Group both display statistics
                if ((NodeType == ConditionNodeType.Category || NodeType == ConditionNodeType.Group) && Children.Count > 0) {
                    // Recursively calculate checked count (if contains Groups, need to count Items under Groups)
                    int checkedCount = CountCheckedItems(this);
                    int totalCount = CountTotalItems(this);
                    return $"{Name} ({checkedCount}/{totalCount})";
                }
                return Name;
            }
        }
        
        private int CountCheckedItems(ConditionTreeNode node) {
            int count = 0;
            foreach(var child in node.Children) {
                if (child.NodeType == ConditionNodeType.Item) {
                    if (child.IsChecked == true) count++;
                } else {
                    count += CountCheckedItems(child);
                }
            }
            return count;
        }

        private int CountTotalItems(ConditionTreeNode node) {
            int count = 0;
            foreach(var child in node.Children) {
                if (child.NodeType == ConditionNodeType.Item) {
                    count++;
                } else {
                    count += CountTotalItems(child);
                }
            }
            return count;
        }

        public string FontWeight => (NodeType == ConditionNodeType.Category || NodeType == ConditionNodeType.Group) ? "Bold" : "Normal";

        public void UpdateCheckState() {
            if (Children.Count == 0) return;

            bool allChecked = Children.All(c => c.IsChecked == true);
            bool anyChecked = Children.Any(c => c.IsChecked == true || c.IsChecked == null); // Include indeterminate

            if (allChecked) IsChecked = true;
            else if (anyChecked) IsChecked = null; // Indeterminate state
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