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

        // 缓存各分类的根节点引用，方便后续操作
        private ConditionTreeNode _seqRoot;
        private ConditionTreeNode _deptRoot;
        private ConditionTreeNode _postRoot;
        private ConditionTreeNode _levelRoot;
        private ConditionTreeNode _empRoot;
        private ConditionTreeNode _itemRoot;

        /// <summary>
        /// 初始化薪资筛选条件窗体
        /// </summary>
        /// <param name="existing">现有的筛选条件，如果为 null 则创建新的筛选条件对象</param>
        public WpfSalaryCondition(SalaryQueryCondition existing) {
            InitializeComponent();

            CurrentCondition = existing != null ? existing.Clone() : new SalaryQueryCondition();

            // 绑定数据源
            treeConditions.ItemsSource = TreeRoots;

            InitializeStructure();
            LoadStaticData(); // 加载不随级联变化的静态数据（岗位、层级、薪资项目）
            RefreshCascadingData(); // 加载级联数据（部门、员工）

            RestoreSelection();
        }

        /// <summary>
        /// 初始化树形结构根节点
        /// </summary>
        private void InitializeStructure() {
            TreeRoots.Clear();

            _seqRoot = new ConditionTreeNode { Name = "序列", NodeType = ConditionNodeType.Category };
            TreeRoots.Add(_seqRoot);

            _deptRoot = new ConditionTreeNode { Name = "部门", NodeType = ConditionNodeType.Category };
            TreeRoots.Add(_deptRoot);

            _postRoot = new ConditionTreeNode { Name = "岗位", NodeType = ConditionNodeType.Category };
            TreeRoots.Add(_postRoot);

            _levelRoot = new ConditionTreeNode { Name = "层级", NodeType = ConditionNodeType.Category };
            TreeRoots.Add(_levelRoot);

            _empRoot = new ConditionTreeNode { Name = "员工", NodeType = ConditionNodeType.Category };
            TreeRoots.Add(_empRoot);

            _itemRoot = new ConditionTreeNode { Name = "薪资项目", NodeType = ConditionNodeType.Category };
            TreeRoots.Add(_itemRoot);
        }

        /// <summary>
        /// 加载静态数据（序列、岗位、层级、薪资项目）
        /// </summary>
        private void LoadStaticData() {
            try {
                // 1. 序列 (基础筛选源)
                LoadTreeData(_seqRoot,
                    "SELECT id, xlname FROM ZX_config_xl WHERE IsEnabled=1 AND DeleteType=0",
                    "xlname", "id");

                // 2. 岗位
                LoadTreeData(_postRoot,
                    "SELECT id, gwname FROM ZX_config_gw WHERE IsEnabled=1 AND DeleteType=0 ORDER BY DisplayOrder",
                    "gwname", "id");

                // 3. 层级 (特殊分组处理)
                LoadLevels();

                // 4. 薪资项目 (特殊分组处理)
                LoadSalaryItems();

            } catch (Exception ex) {
                MessageBox.Show("加载静态数据出错: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 加载层级数据并按 1-10, 11-20 分组
        /// </summary>
        private void LoadLevels() {
            _levelRoot.Children.Clear();
            string sql = "SELECT id, cjname FROM ZX_config_cj WHERE DeleteType=0";
            DataTable dt = SqlHelper.ExecuteDataTable(sql);

            // 内存中分组
            var groups = dt.AsEnumerable()
                .Select(r => new {
                    Id = Convert.ToInt32(r["id"]),
                    Name = r["cjname"].ToString()
                })
                .GroupBy(x => {
                    // 解析数字，例如 "1级" -> 1
                    var match = Regex.Match(x.Name, @"\d+");
                    if (match.Success && int.TryParse(match.Value, out int level)) {
                        int groupStart = ((level - 1) / 10) * 10 + 1;
                        return $"{groupStart}-{groupStart + 9}";
                    }
                    return "其他";
                })
                .OrderBy(g => g.Key.Length).ThenBy(g => g.Key); // 简单排序

            foreach (var group in groups) {
                var groupNode = new ConditionTreeNode {
                    Name = group.Key,
                    NodeType = ConditionNodeType.Group,
                    Parent = _levelRoot
                };

                foreach (var item in group.OrderBy(i => i.Id)) { // 假设ID顺序即为层级顺序
                    var child = new ConditionTreeNode {
                        Name = item.Name,
                        Id = item.Id,
                        NodeType = ConditionNodeType.Item,
                        Parent = groupNode
                    };
                    groupNode.Children.Add(child);
                }
                groupNode.UpdateDisplayText();
                _levelRoot.Children.Add(groupNode);
            }
            _levelRoot.UpdateDisplayText();
        }

        /// <summary>
        /// 加载薪资项目并按 QueryType 分组
        /// </summary>
        private void LoadSalaryItems() {
            _itemRoot.Children.Clear();
            // 注意：增加了 QueryType 列的查询
            string sql = "SELECT ItemId AS id, ItemName, QueryType FROM ZX_SalaryItems WHERE IsEnabled=1 ORDER BY DisplayOrder ASC";
            DataTable dt = SqlHelper.ExecuteDataTable(sql);

            var groups = dt.AsEnumerable()
                .GroupBy(r => r["QueryType"] == DBNull.Value || string.IsNullOrWhiteSpace(r["QueryType"].ToString()) 
                              ? "其他" : r["QueryType"].ToString());

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
        /// 刷新级联数据（部门、员工），基于当前选中的序列和部门
        /// </summary>
        private void RefreshCascadingData() {
            try {
                // 获取当前选中的序列ID
                List<int> selectedSeqIds = GetCheckedIds(_seqRoot);
                
                // 1. 刷新部门 (依赖序列)
                // 仅当序列有选中时进行筛选，否则显示全部（或根据需求逻辑调整）
                // 需求：选中序列...应用到下面的部门(xlid等于选择序列)
                // 如果没有选中任何序列，通常显示所有部门，或者空？这里假设显示所有。
                // 实际上级联通常意味着：如果选了父级，子级只显示相关的。如果不选父级，显示全部。
                
                string deptSql = "SELECT id, bmname, xlid FROM ZX_config_bm WHERE IsEnabled=1 AND DeleteType=0 ORDER BY DisplayOrder";
                DataTable dtDept = SqlHelper.ExecuteDataTable(deptSql);
                
                // 暂存当前选中的部门ID，以便刷新后恢复（如果还在列表中）
                var currentSelectedDeptIds = GetCheckedIds(_deptRoot);
                
                _deptRoot.Children.Clear();
                foreach (DataRow dr in dtDept.Rows) {
                    int xlid = dr["xlid"] != DBNull.Value ? Convert.ToInt32(dr["xlid"]) : 0;
                    // 如果有选序列，且该部门不属于选中序列，则跳过
                    if (selectedSeqIds.Count > 0 && !selectedSeqIds.Contains(xlid)) continue;

                    var child = new ConditionTreeNode {
                        Name = dr["bmname"].ToString(),
                        Id = Convert.ToInt32(dr["id"]),
                        NodeType = ConditionNodeType.Item,
                        Parent = _deptRoot,
                        // 恢复选中状态
                        IsChecked = currentSelectedDeptIds.Contains(Convert.ToInt32(dr["id"]))
                    };
                    _deptRoot.Children.Add(child);
                }
                _deptRoot.UpdateDisplayText();

                // 2. 刷新员工 (依赖序列和部门)
                // 员工筛选条件：(没有选序列 OR xlid in seqs) AND (没有选部门 OR bmid in depts)
                List<int> selectedDeptIds = GetCheckedIds(_deptRoot); // 获取刷新后的选中部门

                string empSql = "SELECT id, xingming, xlid, bmid FROM ZX_config_yg WHERE zaizhi=1 ORDER BY xuhao";
                DataTable dtEmp = SqlHelper.ExecuteDataTable(empSql);

                // 暂存员工选中状态
                var currentSelectedEmpIds = GetCheckedIds(_empRoot);
                string searchText = txtSearch?.Text?.Trim();

                _empRoot.Children.Clear();
                foreach (DataRow dr in dtEmp.Rows) {
                    int xlid = dr["xlid"] != DBNull.Value ? Convert.ToInt32(dr["xlid"]) : 0;
                    int bmid = dr["bmid"] != DBNull.Value ? Convert.ToInt32(dr["bmid"]) : 0;
                    string name = dr["xingming"].ToString();

                    // 序列筛选
                    if (selectedSeqIds.Count > 0 && !selectedSeqIds.Contains(xlid)) continue;
                    // 部门筛选
                    if (selectedDeptIds.Count > 0 && !selectedDeptIds.Contains(bmid)) continue;
                    // 模糊搜索筛选 (仅针对员工)
                    if (!string.IsNullOrEmpty(searchText) && !name.Contains(searchText)) continue;

                    var child = new ConditionTreeNode {
                        Name = name,
                        Id = Convert.ToInt32(dr["id"]),
                        NodeType = ConditionNodeType.Item,
                        Parent = _empRoot,
                        IsChecked = currentSelectedEmpIds.Contains(Convert.ToInt32(dr["id"]))
                    };
                    _empRoot.Children.Add(child);
                }
                _empRoot.UpdateDisplayText();

            } catch (Exception ex) {
                // 避免在初始化或频繁操作时弹出过多错误，可以记录日志
                Console.WriteLine(ex.Message);
            }
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
        /// 恢复选中状态
        /// </summary>
        private void RestoreSelection() {
            SetChecks(_seqRoot, CurrentCondition.SequenceIds);
            
            // 恢复后可能触发了级联逻辑，需要确保部门和员工列表正确
            // 但 SetChecks 只是设置 IsChecked，不触发 Click 事件
            // 所以我们需要手动调用一次刷新
            RefreshCascadingData();

            // 再次设置部门和员工的选中状态，因为 RefreshCascadingData 可能重置了节点
            SetChecks(_deptRoot, CurrentCondition.DepartmentIds);
            RefreshCascadingData(); // 部门变化可能影响员工
            
            SetChecks(_postRoot, CurrentCondition.PositionIds);
            
            // 恢复层级 (两层结构)
            SetChecksRecursive(_levelRoot, CurrentCondition.LevelIds);
            
            SetChecks(_empRoot, CurrentCondition.EmployeeIds);
            
            // 恢复薪资项目 (两层结构)
            SetChecksRecursive(_itemRoot, CurrentCondition.SalaryItemIds);
        }

        private void SetChecks(ConditionTreeNode parent, List<int> ids) {
            if (ids == null) return;
            foreach (var child in parent.Children) {
                child.IsChecked = ids.Contains(child.Id);
            }
            parent.UpdateCheckState();
        }

        // 递归设置选中状态（用于处理分组情况）
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
        /// CheckBox 点击事件
        /// </summary>
        private void CheckBox_Click(object sender, RoutedEventArgs e) {
            var cb = sender as CheckBox;
            var node = cb?.DataContext as ConditionTreeNode;
            if (node == null) return;

            // 处理选中逻辑
            HandleCheckLogic(node);

            // 级联处理
            // 如果操作的是 序列 节点 (或其子节点)，刷新 部门 和 员工
            if (IsDescendantOf(node, _seqRoot)) {
                RefreshCascadingData();
            }
            // 如果操作的是 部门 节点 (或其子节点)，刷新 员工
            else if (IsDescendantOf(node, _deptRoot)) {
                RefreshCascadingData();
            }
        }

        // 递归设置选中
        private void HandleCheckLogic(ConditionTreeNode node) {
            if (node.NodeType == ConditionNodeType.Category || node.NodeType == ConditionNodeType.Group) {
                bool isChecked = node.IsChecked == true;
                foreach (var child in node.Children) {
                    child.IsChecked = isChecked;
                    if (child.NodeType == ConditionNodeType.Group) {
                        HandleCheckLogic(child); // 递归处理深层分组
                    }
                }
            } else {
                node.Parent?.UpdateCheckState();
                // 如果父级是组，组的父级（Category）也需要更新
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
        /// 搜索框文本变化事件
        /// </summary>
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e) {
            string text = txtSearch.Text.Trim();
            
            // 1. 触发员工列表刷新（利用 RefreshCascadingData 中的过滤逻辑）
            RefreshCascadingData();

            // 2. 筛选薪资项目 (本地筛选)
            FilterSalaryItems(text);
        }

        private void FilterSalaryItems(string keyword) {
            // 先重新加载所有薪资项目以恢复完整列表
            LoadSalaryItems();
            
            if (string.IsNullOrEmpty(keyword)) return;

            // 倒序遍历删除不匹配的节点
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
                } else {
                    // 如果组内有匹配项，默认展开（需要 TreeViewItem 样式绑定 IsExpanded，这里简化处理或需在 Model 增加 IsExpanded 属性）
                    // 假设用户手动展开
                }
            }
            _itemRoot.UpdateDisplayText();
        }

        // ... (ContextMenu methods remain mostly the same, ensuring they call HandleCheckLogic logic effectively) ...

        
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

            // 找到操作的目标集合容器（Category 或者 Group）
            var targetNode = (node.NodeType == ConditionNodeType.Category || node.NodeType == ConditionNodeType.Group) 
                             ? node : node.Parent;
            
            if (targetNode == null) return;

            foreach (var child in targetNode.Children) {
                action(child);
                if (child.NodeType == ConditionNodeType.Group) {
                    // 如果是组，递归应用
                    foreach(var grandChild in child.Children) action(grandChild);
                    child.UpdateCheckState();
                }
            }
            targetNode.UpdateCheckState();
            
            // 如果有更上级
            if (targetNode.Parent != null) targetNode.Parent.UpdateCheckState();

            // 触发级联
            if (IsDescendantOf(targetNode, _seqRoot) || IsDescendantOf(targetNode, _deptRoot)) {
                RefreshCascadingData();
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
            CurrentCondition.SequenceIds = GetCheckedIds(_seqRoot);
            CurrentCondition.DepartmentIds = GetCheckedIds(_deptRoot);
            CurrentCondition.PositionIds = GetCheckedIds(_postRoot);
            CurrentCondition.LevelIds = GetCheckedIdsRecursive(_levelRoot);
            CurrentCondition.EmployeeIds = GetCheckedIds(_empRoot);
            CurrentCondition.SalaryItemIds = GetCheckedIdsRecursive(_itemRoot);

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
                // Category 和 Group 都显示统计
                if ((NodeType == ConditionNodeType.Category || NodeType == ConditionNodeType.Group) && Children.Count > 0) {
                    // 递归计算选中数（如果包含Group，需要计算Group下的Item）
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
            bool anyChecked = Children.Any(c => c.IsChecked == true || c.IsChecked == null); // 包含半选

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