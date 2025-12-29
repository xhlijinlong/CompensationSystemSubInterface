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

        /// <summary>
        /// 初始化薪资筛选条件窗体
        /// </summary>
        /// <param name="existing">现有的筛选条件，如果为 null 则创建新的筛选条件对象</param>
        public WpfSalaryCondition(SalaryQueryCondition existing) {
            InitializeComponent();

            CurrentCondition = existing != null ? existing.Clone() : new SalaryQueryCondition();

            // 绑定数据源
            treeConditions.ItemsSource = TreeRoots;

            LoadData();
            RestoreSelection();
        }

        /// <summary>
        /// 加载基础数据到树形结构
        /// </summary>
        private void LoadData() {
            try {
                TreeRoots.Clear();

                // 1. 序列
                var seqRoot = new ConditionTreeNode { Name = "序列", NodeType = ConditionNodeType.Category };
                LoadTreeData(seqRoot, 
                    "SELECT id, xlname FROM ZX_config_xl WHERE IsEnabled=1 AND DeleteType=0", 
                    "xlname", "id");
                TreeRoots.Add(seqRoot);

                // 2. 部门
                var deptRoot = new ConditionTreeNode { Name = "部门", NodeType = ConditionNodeType.Category };
                LoadTreeData(deptRoot, 
                    "SELECT id, bmname FROM ZX_config_bm WHERE IsEnabled=1 AND DeleteType=0 ORDER BY DisplayOrder", 
                    "bmname", "id");
                TreeRoots.Add(deptRoot);

                // 3. 岗位
                var postRoot = new ConditionTreeNode { Name = "岗位", NodeType = ConditionNodeType.Category };
                LoadTreeData(postRoot, 
                    "SELECT id, gwname FROM ZX_config_gw WHERE IsEnabled=1 AND DeleteType=0 ORDER BY DisplayOrder", 
                    "gwname", "id");
                TreeRoots.Add(postRoot);

                // 4. 层级
                var levelRoot = new ConditionTreeNode { Name = "层级", NodeType = ConditionNodeType.Category };
                LoadTreeData(levelRoot, 
                    "SELECT id, cjname FROM ZX_config_cj WHERE DeleteType=0", 
                    "cjname", "id");
                TreeRoots.Add(levelRoot);

                // 5. 员工
                var empRoot = new ConditionTreeNode { Name = "员工", NodeType = ConditionNodeType.Category };
                LoadTreeData(empRoot, 
                    "SELECT id, xingming FROM ZX_config_yg WHERE zaizhi=1 ORDER BY xuhao", 
                    "xingming", "id");
                TreeRoots.Add(empRoot);

                // 6. 薪资项目
                var itemRoot = new ConditionTreeNode { Name = "薪资项目", NodeType = ConditionNodeType.Category };
                LoadTreeData(itemRoot, 
                    "SELECT ItemId AS id, ItemName FROM ZX_SalaryItems WHERE IsEnabled=1 ORDER BY DisplayOrder ASC", 
                    "ItemName", "id");
                TreeRoots.Add(itemRoot);

            } catch (Exception ex) {
                MessageBox.Show("加载数据出错: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 通用方法：从数据库加载数据并添加到父节点
        /// </summary>
        private void LoadTreeData(ConditionTreeNode parent, string sql, string displayField, string valueField) {
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
            // 更新父节点显示的计数
            parent.UpdateDisplayText();
        }

        /// <summary>
        /// 根据当前筛选条件恢复各节点的选中状态
        /// </summary>
        private void RestoreSelection() {
            if (TreeRoots.Count < 6) return;

            // 序列
            SetChecks(TreeRoots[0], CurrentCondition.SequenceIds);
            // 部门
            SetChecks(TreeRoots[1], CurrentCondition.DepartmentIds);
            // 岗位
            SetChecks(TreeRoots[2], CurrentCondition.PositionIds);
            // 层级
            SetChecks(TreeRoots[3], CurrentCondition.LevelIds);
            // 员工
            SetChecks(TreeRoots[4], CurrentCondition.EmployeeIds);
            // 薪资项目
            SetChecks(TreeRoots[5], CurrentCondition.SalaryItemIds);
        }

        /// <summary>
        /// 根据 ID 列表设置节点的选中状态
        /// </summary>
        private void SetChecks(ConditionTreeNode parent, List<int> ids) {
            foreach (var child in parent.Children) {
                child.IsChecked = ids.Contains(child.Id);
            }
            parent.UpdateCheckState();
        }

        /// <summary>
        /// CheckBox 点击事件处理
        /// </summary>
        private void CheckBox_Click(object sender, RoutedEventArgs e) {
            var cb = sender as CheckBox;
            var node = cb?.DataContext as ConditionTreeNode;
            if (node == null) return;

            if (node.NodeType == ConditionNodeType.Category) {
                // 父节点：向下联动，全选/取消所有子节点
                bool isChecked = node.IsChecked == true;
                foreach (var child in node.Children) {
                    child.IsChecked = isChecked;
                }
            } else {
                // 子节点：向上联动，更新父节点状态
                node.Parent?.UpdateCheckState();
            }
        }

        /// <summary>
        /// 右键菜单 - 全选
        /// </summary>
        private void ContextMenu_SelectAll_Click(object sender, RoutedEventArgs e) {
            var node = GetContextMenuNode(sender);
            if (node == null) return;

            var targetNode = node.NodeType == ConditionNodeType.Category ? node : node.Parent;
            if (targetNode == null) return;

            foreach (var child in targetNode.Children) {
                child.IsChecked = true;
            }
            targetNode.UpdateCheckState();
        }

        /// <summary>
        /// 右键菜单 - 反选
        /// </summary>
        private void ContextMenu_Invert_Click(object sender, RoutedEventArgs e) {
            var node = GetContextMenuNode(sender);
            if (node == null) return;

            var targetNode = node.NodeType == ConditionNodeType.Category ? node : node.Parent;
            if (targetNode == null) return;

            foreach (var child in targetNode.Children) {
                child.IsChecked = !(child.IsChecked == true);
            }
            targetNode.UpdateCheckState();
        }

        /// <summary>
        /// 右键菜单 - 取消全选
        /// </summary>
        private void ContextMenu_ClearAll_Click(object sender, RoutedEventArgs e) {
            var node = GetContextMenuNode(sender);
            if (node == null) return;

            var targetNode = node.NodeType == ConditionNodeType.Category ? node : node.Parent;
            if (targetNode == null) return;

            foreach (var child in targetNode.Children) {
                child.IsChecked = false;
            }
            targetNode.UpdateCheckState();
        }

        /// <summary>
        /// 获取右键菜单对应的节点
        /// </summary>
        private ConditionTreeNode GetContextMenuNode(object sender) {
            var menuItem = sender as MenuItem;
            var contextMenu = menuItem?.Parent as ContextMenu;
            var stackPanel = contextMenu?.PlacementTarget as StackPanel;
            return stackPanel?.DataContext as ConditionTreeNode;
        }

        /// <summary>
        /// 重置按钮点击事件处理，清空所有选中状态
        /// </summary>
        private void btnReset_Click(object sender, RoutedEventArgs e) {
            foreach (var root in TreeRoots) {
                foreach (var child in root.Children) {
                    child.IsChecked = false;
                }
                root.UpdateCheckState();
            }
        }

        /// <summary>
        /// 应用按钮点击事件处理，保存筛选条件并通知主界面
        /// </summary>
        private void btnApply_Click(object sender, RoutedEventArgs e) {
            if (TreeRoots.Count < 6) return;

            // 收集各维度选中的ID
            CurrentCondition.SequenceIds = GetCheckedIds(TreeRoots[0]);
            CurrentCondition.DepartmentIds = GetCheckedIds(TreeRoots[1]);
            CurrentCondition.PositionIds = GetCheckedIds(TreeRoots[2]);
            CurrentCondition.LevelIds = GetCheckedIds(TreeRoots[3]);
            CurrentCondition.EmployeeIds = GetCheckedIds(TreeRoots[4]);
            CurrentCondition.SalaryItemIds = GetCheckedIds(TreeRoots[5]);

            // 触发事件通知主界面
            ApplySelect?.Invoke(CurrentCondition);
        }

        /// <summary>
        /// 获取父节点下已选中子节点的 ID 列表
        /// </summary>
        private List<int> GetCheckedIds(ConditionTreeNode parent) {
            return parent.Children
                .Where(c => c.IsChecked == true)
                .Select(c => c.Id)
                .ToList();
        }

        /// <summary>
        /// 关闭按钮点击事件处理
        /// </summary>
        private void btnClose_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }

    #region 数据模型

    /// <summary>
    /// 条件树节点类型
    /// </summary>
    public enum ConditionNodeType {
        /// <summary>分类节点（如：序列、部门等）</summary>
        Category,
        /// <summary>具体项目节点</summary>
        Item
    }

    /// <summary>
    /// 条件树节点
    /// </summary>
    public class ConditionTreeNode : INotifyPropertyChanged {
        /// <summary>节点名称</summary>
        public string Name { get; set; }

        /// <summary>节点ID（仅对 Item 类型有效）</summary>
        public int Id { get; set; }

        /// <summary>节点类型</summary>
        public ConditionNodeType NodeType { get; set; }

        /// <summary>父节点引用</summary>
        public ConditionTreeNode Parent { get; set; }

        /// <summary>子节点集合</summary>
        public ObservableCollection<ConditionTreeNode> Children { get; set; } = new ObservableCollection<ConditionTreeNode>();

        private bool? _isChecked = false;
        /// <summary>是否选中（支持三态：true/false/null）</summary>
        public bool? IsChecked {
            get => _isChecked;
            set {
                if (_isChecked != value) {
                    _isChecked = value;
                    OnPropertyChanged(nameof(IsChecked));
                }
            }
        }

        /// <summary>显示文本（分类节点显示选中数量）</summary>
        public string DisplayText {
            get {
                if (NodeType == ConditionNodeType.Category && Children.Count > 0) {
                    int checkedCount = Children.Count(c => c.IsChecked == true);
                    return $"{Name} ({checkedCount}/{Children.Count})";
                }
                return Name;
            }
        }

        /// <summary>字体粗细（分类节点加粗）</summary>
        public string FontWeight => NodeType == ConditionNodeType.Category ? "Bold" : "Normal";

        /// <summary>
        /// 更新父节点的选中状态（根据子节点状态）
        /// </summary>
        public void UpdateCheckState() {
            if (Children.Count == 0) return;

            bool allChecked = Children.All(c => c.IsChecked == true);
            bool anyChecked = Children.Any(c => c.IsChecked == true);

            if (allChecked)
                IsChecked = true;
            else if (anyChecked)
                IsChecked = null; // 半选状态
            else
                IsChecked = false;

            UpdateDisplayText();
        }

        /// <summary>
        /// 更新显示文本（通知UI刷新）
        /// </summary>
        public void UpdateDisplayText() {
            OnPropertyChanged(nameof(DisplayText));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    #endregion
}