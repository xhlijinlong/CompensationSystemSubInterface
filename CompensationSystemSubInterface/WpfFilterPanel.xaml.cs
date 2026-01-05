using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CompensationSystemSubInterface.Utilities;

namespace CompensationSystemSubInterface {
    /// <summary>
    /// WPF 筛选树控件
    /// </summary>
    public partial class WpfFilterPanel : UserControl {
        private FilterTreeNode _root;
        public event Action<List<int>> SelectionChanged;

        public WpfFilterPanel() {
            InitializeComponent();
        }

        #region 数据加载

        public void LoadSequences() {
            _root = new FilterTreeNode { DisplayText = "全部序列", IsThreeState = true, FontWeight = FontWeights.Bold };
            
            string sql = "SELECT id, xlname FROM ZX_config_xl WHERE IsEnabled=1 AND DeleteType=0";
            DataTable dt = SqlHelper.ExecuteDataTable(sql);

            foreach (DataRow row in dt.Rows) {
                _root.Children.Add(new FilterTreeNode {
                    Id = Convert.ToInt32(row["id"]),
                    DisplayText = row["xlname"].ToString(),
                    Parent = _root
                });
            }
            BindTree();
        }

        public void LoadDepartments(List<int> limitSeqIds) {
            _root = new FilterTreeNode { DisplayText = "全部部门", IsThreeState = true, FontWeight = FontWeights.Bold };
            
            // 如果是在重新加载（级联触发），应该尽量保留之前的选中状态吗？
            // 简单起见，这里每次 Sequence 变动都重置 Department 列表，
            // 实际上 Department 的 ID 是固定的，如果只是过滤显示，可以做更复杂的逻辑。
            // 但为了保证数据一致性，重新加载时最好清空选中，或者只保留仍在列表中的 ID。
            // 这里我们简化处理，重新加载。

            string sql = "SELECT id, bmname, xlid FROM ZX_config_bm WHERE IsEnabled=1 AND DeleteType=0 ORDER BY DisplayOrder";
            DataTable dtAll = SqlHelper.ExecuteDataTable(sql);

            foreach (DataRow row in dtAll.Rows) {
                int xlid = row["xlid"] != DBNull.Value ? Convert.ToInt32(row["xlid"]) : 0;

                // 筛选逻辑
                if (limitSeqIds != null && limitSeqIds.Count > 0 && !limitSeqIds.Contains(xlid)) continue;

                _root.Children.Add(new FilterTreeNode {
                    Id = Convert.ToInt32(row["id"]),
                    DisplayText = row["bmname"].ToString(),
                    Parent = _root
                });
            }
            BindTree();
        }

        public void LoadPositions() {
            _root = new FilterTreeNode { DisplayText = "全部岗位", IsThreeState = true, FontWeight = FontWeights.Bold };

            string sql = "SELECT id, gwname FROM ZX_config_gw WHERE IsEnabled=1 AND DeleteType=0 ORDER BY DisplayOrder";
            DataTable dt = SqlHelper.ExecuteDataTable(sql);

            foreach (DataRow row in dt.Rows) {
                _root.Children.Add(new FilterTreeNode {
                    Id = Convert.ToInt32(row["id"]),
                    DisplayText = row["gwname"].ToString(),
                    Parent = _root
                });
            }
            BindTree();
        }

        private void BindTree() {
            treeMain.ItemsSource = new ObservableCollection<FilterTreeNode> { _root };
        }

        #endregion

        #region 逻辑处理

        public List<int> GetSelectedIds() {
            if (_root == null) return new List<int>();
            return _root.Children.Where(x => x.IsChecked == true).Select(x => x.Id).ToList();
        }

        public int GetSelectedCount() {
            if (_root == null) return 0;
            return _root.Children.Count(x => x.IsChecked == true);
        }

        public bool IsAllSelected() {
            if (_root == null || _root.Children.Count == 0) return false;
            return _root.Children.All(x => x.IsChecked == true);
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e) {
            var cb = sender as CheckBox;
            var node = cb?.DataContext as FilterTreeNode;
            if (node == null) return;

            if (node.Children.Count > 0) {
                // 父节点点击
                bool isChecked = node.IsChecked == true;
                foreach (var child in node.Children) {
                    child.IsChecked = isChecked;
                }
            } else {
                // 子节点点击
                UpdateParentState(node.Parent);
            }

            SelectionChanged?.Invoke(GetSelectedIds());
        }

        private void UpdateParentState(FilterTreeNode parent) {
            if (parent == null || parent.Children.Count == 0) return;

            bool allChecked = parent.Children.All(x => x.IsChecked == true);
            bool anyChecked = parent.Children.Any(x => x.IsChecked == true);

            if (allChecked) parent.IsChecked = true;
            else if (anyChecked) parent.IsChecked = null;
            else parent.IsChecked = false;
        }

        #endregion
    }

    // 复用 FilterTreeNode
    // (如果已在其他地方定义，这里不需要再次定义，但为了保险，我先假设它在命名空间下共享或者作为嵌套类)
    // 看起来它是在命名空间下定义的，所以这里不需要 Duplicate。
    // 但是之前的 WpfFilterPanel.xaml.cs 把它定义在了 Namespace 下。
    // 如果我把这个文件覆盖了，那个类定义也没了。所以我需要重新包含它。

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
