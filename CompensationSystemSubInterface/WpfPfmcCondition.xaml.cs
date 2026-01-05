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
    /// 绩效查询条件设置窗体（WPF版本）
    /// 只包含员工筛选功能，年度和考核结果已移到主界面按钮
    /// </summary>
    public partial class WpfPfmcCondition : Window {
        /// <summary>
        /// 获取当前的筛选条件
        /// </summary>
        public PfmcQueryCondition CurrentCondition { get; private set; }

        /// <summary>
        /// 定义事件: 点击应用时将最新的条件传给主界面
        /// </summary>
        public event Action<PfmcQueryCondition> ApplySelect;

        /// <summary>
        /// 树形结构的根节点集合
        /// </summary>
        public ObservableCollection<PfmcConditionTreeNode> TreeRoots { get; set; } = new ObservableCollection<PfmcConditionTreeNode>();

        // 员工根节点
        private PfmcConditionTreeNode _empRoot;

        // 持久化选中的员工ID（包括搜索时隐藏的项）
        private HashSet<int> _persistentEmpIds = new HashSet<int>();

        /// <summary>
        /// 初始化绩效筛选条件窗体
        /// </summary>
        /// <param name="existing">现有的筛选条件</param>
        public WpfPfmcCondition(PfmcQueryCondition existing) {
            InitializeComponent();

            CurrentCondition = existing != null ? existing.Clone() : new PfmcQueryCondition();

            // 初始化持久化ID
            if (CurrentCondition.EmployeeIds != null) {
                _persistentEmpIds = new HashSet<int>(CurrentCondition.EmployeeIds);
            }

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

            _empRoot = new PfmcConditionTreeNode { Name = "员工", NodeType = PfmcConditionNodeType.Category };
            TreeRoots.Add(_empRoot);
        }

        /// <summary>
        /// 刷新员工数据
        /// </summary>
        private void RefreshEmployeeData() {
            try {
                string empSql = "SELECT id, xingming FROM ZX_config_yg WHERE zaizhi=1 ORDER BY xuhao";
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
                    string name = dr["xingming"].ToString();
                    int empId = Convert.ToInt32(dr["id"]);

                    // 按搜索文本筛选
                    if (!string.IsNullOrEmpty(searchText) && !name.Contains(searchText)) continue;

                    var child = new PfmcConditionTreeNode {
                        Name = name,
                        Id = empId,
                        NodeType = PfmcConditionNodeType.Item,
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
        /// CheckBox 点击事件处理
        /// </summary>
        private void CheckBox_Click(object sender, RoutedEventArgs e) {
            var cb = sender as CheckBox;
            var node = cb?.DataContext as PfmcConditionTreeNode;
            if (node == null) return;

            HandleCheckLogic(node);
            UpdatePersistentEmpIds(node);
        }

        private void HandleCheckLogic(PfmcConditionTreeNode node) {
            if (node.NodeType == PfmcConditionNodeType.Category) {
                bool isChecked = node.IsChecked == true;
                foreach (var child in node.Children) {
                    child.IsChecked = isChecked;
                }
            } else {
                node.Parent?.UpdateCheckState();
            }
        }

        private void UpdatePersistentEmpIds(PfmcConditionTreeNode node) {
            if (node.NodeType == PfmcConditionNodeType.Item) {
                if (node.IsChecked == true) {
                    _persistentEmpIds.Add(node.Id);
                } else {
                    _persistentEmpIds.Remove(node.Id);
                }
            } else if (node.NodeType == PfmcConditionNodeType.Category) {
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

        private void ProcessContextMenuAction(object sender, Action<PfmcConditionTreeNode> action) {
            var node = GetContextMenuNode(sender);
            if (node == null) return;

            var targetNode = node.NodeType == PfmcConditionNodeType.Category ? node : node.Parent;
            if (targetNode == null) return;

            foreach (var child in targetNode.Children) {
                if (child.NodeType == PfmcConditionNodeType.Item) {
                    action(child);
                }
            }
            targetNode.UpdateCheckState();

            // 更新持久化ID
            UpdatePersistentEmpIds(targetNode);
        }

        private PfmcConditionTreeNode GetContextMenuNode(object sender) {
            var menuItem = sender as MenuItem;
            var contextMenu = menuItem?.Parent as ContextMenu;
            var stackPanel = contextMenu?.PlacementTarget as StackPanel;
            return stackPanel?.DataContext as PfmcConditionTreeNode;
        }

        private void btnReset_Click(object sender, RoutedEventArgs e) {
            _persistentEmpIds.Clear();
            InitializeStructure();
            RefreshEmployeeData();
        }

        private void btnApply_Click(object sender, RoutedEventArgs e) {
            CurrentCondition.EmployeeIds = _persistentEmpIds.ToList();
            ApplySelect?.Invoke(CurrentCondition);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }

    #region 数据模型

    public enum PfmcConditionNodeType {
        Category,
        Item
    }

    public class PfmcConditionTreeNode : INotifyPropertyChanged {
        public string Name { get; set; }
        public int Id { get; set; }
        public PfmcConditionNodeType NodeType { get; set; }
        public PfmcConditionTreeNode Parent { get; set; }
        public ObservableCollection<PfmcConditionTreeNode> Children { get; set; } = new ObservableCollection<PfmcConditionTreeNode>();

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

        public bool IsThreeState => NodeType == PfmcConditionNodeType.Category;

        public string DisplayText {
            get {
                if (NodeType == PfmcConditionNodeType.Category && Children.Count > 0) {
                    int checkedCount = Children.Count(c => c.IsChecked == true);
                    int totalCount = Children.Count;
                    return $"{Name} ({checkedCount}/{totalCount})";
                }
                return Name;
            }
        }

        public string FontWeight => NodeType == PfmcConditionNodeType.Category ? "Bold" : "Normal";

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
