using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using CompensationSystemSubInterface.Utilities;

namespace CompensationSystemSubInterface {
    /// <summary>
    /// WPF 筛选面板控件，用于序列/部门/岗位多选
    /// 使用弹出式树形结构，与条件设置窗体风格一致
    /// </summary>
    public partial class WpfFilterPanel : UserControl {
        // 树形节点
        private FilterTreeNode _seqRoot;
        private FilterTreeNode _deptRoot;
        private FilterTreeNode _postRoot;

        /// <summary>
        /// 选中的序列 ID 变化事件
        /// </summary>
        public event Action<List<int>> SequenceSelectionChanged;

        /// <summary>
        /// 选中的部门 ID 变化事件
        /// </summary>
        public event Action<List<int>> DepartmentSelectionChanged;

        /// <summary>
        /// 选中的岗位 ID 变化事件
        /// </summary>
        public event Action<List<int>> PositionSelectionChanged;

        public WpfFilterPanel() {
            InitializeComponent();
        }

        #region 数据加载

        /// <summary>
        /// 加载序列数据
        /// </summary>
        public void LoadSequences() {
            _seqRoot = new FilterTreeNode { DisplayText = "序列", IsThreeState = true, FontWeight = FontWeights.Bold };
            
            string sql = "SELECT id, xlname FROM ZX_config_xl WHERE IsEnabled=1 AND DeleteType=0";
            DataTable dt = SqlHelper.ExecuteDataTable(sql);

            foreach (DataRow row in dt.Rows) {
                _seqRoot.Children.Add(new FilterTreeNode {
                    Id = Convert.ToInt32(row["id"]),
                    DisplayText = row["xlname"].ToString(),
                    Parent = _seqRoot
                });
            }

            treeSeq.ItemsSource = new ObservableCollection<FilterTreeNode> { _seqRoot };
            UpdateButtonText(btnSeq, "序列", _seqRoot);
        }

        /// <summary>
        /// 加载部门数据，根据选中的序列进行筛选
        /// </summary>
        public void LoadDepartments() {
            // 保存当前选中的部门 ID
            var currentSelectedIds = _deptRoot?.Children
                .Where(x => x.IsChecked == true)
                .Select(x => x.Id).ToList() ?? new List<int>();

            _deptRoot = new FilterTreeNode { DisplayText = "部门", IsThreeState = true, FontWeight = FontWeights.Bold };

            var selectedSeqIds = GetSelectedSequenceIds();

            string sql = "SELECT id, bmname, xlid FROM ZX_config_bm WHERE IsEnabled=1 AND DeleteType=0 ORDER BY DisplayOrder";
            DataTable dtAll = SqlHelper.ExecuteDataTable(sql);

            foreach (DataRow row in dtAll.Rows) {
                int xlid = row["xlid"] != DBNull.Value ? Convert.ToInt32(row["xlid"]) : 0;

                // 如果选择了序列，则过滤部门
                if (selectedSeqIds.Count > 0 && !selectedSeqIds.Contains(xlid)) continue;

                int id = Convert.ToInt32(row["id"]);
                _deptRoot.Children.Add(new FilterTreeNode {
                    Id = id,
                    DisplayText = row["bmname"].ToString(),
                    IsChecked = currentSelectedIds.Contains(id),
                    Parent = _deptRoot
                });
            }

            // 更新父节点状态
            UpdateParentState(_deptRoot);

            treeDept.ItemsSource = new ObservableCollection<FilterTreeNode> { _deptRoot };
            UpdateButtonText(btnDept, "部门", _deptRoot);
        }

        /// <summary>
        /// 加载岗位数据
        /// </summary>
        public void LoadPositions() {
            _postRoot = new FilterTreeNode { DisplayText = "岗位", IsThreeState = true, FontWeight = FontWeights.Bold };

            string sql = "SELECT id, gwname FROM ZX_config_gw WHERE IsEnabled=1 AND DeleteType=0 ORDER BY DisplayOrder";
            DataTable dt = SqlHelper.ExecuteDataTable(sql);

            foreach (DataRow row in dt.Rows) {
                _postRoot.Children.Add(new FilterTreeNode {
                    Id = Convert.ToInt32(row["id"]),
                    DisplayText = row["gwname"].ToString(),
                    Parent = _postRoot
                });
            }

            treePost.ItemsSource = new ObservableCollection<FilterTreeNode> { _postRoot };
            UpdateButtonText(btnPost, "岗位", _postRoot);
        }

        #endregion

        #region 获取选中项

        /// <summary>
        /// 获取选中的序列 ID 列表
        /// </summary>
        public List<int> GetSelectedSequenceIds() {
            if (_seqRoot == null) return new List<int>();
            return _seqRoot.Children.Where(x => x.IsChecked == true).Select(x => x.Id).ToList();
        }

        /// <summary>
        /// 获取选中的部门 ID 列表
        /// </summary>
        public List<int> GetSelectedDepartmentIds() {
            if (_deptRoot == null) return new List<int>();
            return _deptRoot.Children.Where(x => x.IsChecked == true).Select(x => x.Id).ToList();
        }

        /// <summary>
        /// 获取选中的岗位 ID 列表
        /// </summary>
        public List<int> GetSelectedPositionIds() {
            if (_postRoot == null) return new List<int>();
            return _postRoot.Children.Where(x => x.IsChecked == true).Select(x => x.Id).ToList();
        }

        #endregion

        #region 按钮事件

        private void btnSeq_Checked(object sender, RoutedEventArgs e) {
            popupSeq.IsOpen = true;
        }

        private void btnSeq_Unchecked(object sender, RoutedEventArgs e) {
            popupSeq.IsOpen = false;
        }

        private void popupSeq_Closed(object sender, EventArgs e) {
            btnSeq.IsChecked = false;
        }

        private void btnDept_Checked(object sender, RoutedEventArgs e) {
            popupDept.IsOpen = true;
        }

        private void btnDept_Unchecked(object sender, RoutedEventArgs e) {
            popupDept.IsOpen = false;
        }

        private void popupDept_Closed(object sender, EventArgs e) {
            btnDept.IsChecked = false;
        }

        private void btnPost_Checked(object sender, RoutedEventArgs e) {
            popupPost.IsOpen = true;
        }

        private void btnPost_Unchecked(object sender, RoutedEventArgs e) {
            popupPost.IsOpen = false;
        }

        private void popupPost_Closed(object sender, EventArgs e) {
            btnPost.IsChecked = false;
        }

        #endregion

        #region CheckBox 事件

        private void SeqCheckBox_Click(object sender, RoutedEventArgs e) {
            var cb = sender as CheckBox;
            var node = cb?.DataContext as FilterTreeNode;
            if (node == null) return;

            // 如果点击的是父节点，级联子节点
            if (node.Children.Count > 0) {
                bool isChecked = node.IsChecked == true;
                foreach (var child in node.Children) {
                    child.IsChecked = isChecked;
                }
            } else {
                // 如果点击的是子节点，更新父节点状态
                UpdateParentState(node.Parent);
            }

            UpdateButtonText(btnSeq, "序列", _seqRoot);
            
            // 刷新部门列表并触发事件
            LoadDepartments();
            SequenceSelectionChanged?.Invoke(GetSelectedSequenceIds());
            UpdateStatus();
        }

        private void DeptCheckBox_Click(object sender, RoutedEventArgs e) {
            var cb = sender as CheckBox;
            var node = cb?.DataContext as FilterTreeNode;
            if (node == null) return;

            // 如果点击的是父节点，级联子节点
            if (node.Children.Count > 0) {
                bool isChecked = node.IsChecked == true;
                foreach (var child in node.Children) {
                    child.IsChecked = isChecked;
                }
            } else {
                // 如果点击的是子节点，更新父节点状态
                UpdateParentState(node.Parent);
            }

            UpdateButtonText(btnDept, "部门", _deptRoot);
            DepartmentSelectionChanged?.Invoke(GetSelectedDepartmentIds());
            UpdateStatus();
        }

        private void PostCheckBox_Click(object sender, RoutedEventArgs e) {
            var cb = sender as CheckBox;
            var node = cb?.DataContext as FilterTreeNode;
            if (node == null) return;

            // 如果点击的是父节点，级联子节点
            if (node.Children.Count > 0) {
                bool isChecked = node.IsChecked == true;
                foreach (var child in node.Children) {
                    child.IsChecked = isChecked;
                }
            } else {
                // 如果点击的是子节点，更新父节点状态
                UpdateParentState(node.Parent);
            }

            UpdateButtonText(btnPost, "岗位", _postRoot);
            PositionSelectionChanged?.Invoke(GetSelectedPositionIds());
            UpdateStatus();
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 更新父节点状态
        /// </summary>
        private void UpdateParentState(FilterTreeNode parent) {
            if (parent == null || parent.Children.Count == 0) return;

            bool allChecked = parent.Children.All(x => x.IsChecked == true);
            bool anyChecked = parent.Children.Any(x => x.IsChecked == true);

            if (allChecked) {
                parent.IsChecked = true;
            } else if (anyChecked) {
                parent.IsChecked = null;
            } else {
                parent.IsChecked = false;
            }
        }

        /// <summary>
        /// 更新按钮文本（显示选中数量）
        /// </summary>
        private void UpdateButtonText(ToggleButton btn, string baseName, FilterTreeNode root) {
            if (root == null) {
                btn.Content = baseName;
                return;
            }

            int checkedCount = root.Children.Count(x => x.IsChecked == true);
            if (checkedCount == 0) {
                btn.Content = baseName;
            } else if (checkedCount == root.Children.Count) {
                btn.Content = $"{baseName} (全部)";
            } else {
                btn.Content = $"{baseName} ({checkedCount})";
            }
        }

        /// <summary>
        /// 更新状态栏
        /// </summary>
        private void UpdateStatus() {
            int seqCount = GetSelectedSequenceIds().Count;
            int deptCount = GetSelectedDepartmentIds().Count;
            int postCount = GetSelectedPositionIds().Count;

            var parts = new List<string>();
            if (seqCount > 0) parts.Add($"{seqCount}个序列");
            if (deptCount > 0) parts.Add($"{deptCount}个部门");
            if (postCount > 0) parts.Add($"{postCount}个岗位");

            lblStatus.Text = parts.Count > 0 ? $"已选: {string.Join(", ", parts)}" : "";
        }

        #endregion
    }

    /// <summary>
    /// 筛选树节点
    /// </summary>
    public class FilterTreeNode : INotifyPropertyChanged {
        private bool? _isChecked = false;

        public int Id { get; set; }
        public string DisplayText { get; set; }
        public bool IsThreeState { get; set; } = false;
        public FontWeight FontWeight { get; set; } = FontWeights.Normal;
        public FilterTreeNode Parent { get; set; }
        public ObservableCollection<FilterTreeNode> Children { get; set; } = new ObservableCollection<FilterTreeNode>();

        public bool? IsChecked {
            get => _isChecked;
            set {
                if (_isChecked != value) {
                    _isChecked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsChecked)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
