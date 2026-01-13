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
            _root = new FilterTreeNode { DisplayText = "全部职务", IsThreeState = true, FontWeight = FontWeights.Bold };

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

        public void LoadYears() {
            _root = new FilterTreeNode { DisplayText = "全部年度", IsThreeState = true, FontWeight = FontWeights.Bold };

            int curYear = DateTime.Now.Year;
            for (int i = 0; i < 5; i++) {
                int year = curYear - i;
                _root.Children.Add(new FilterTreeNode {
                    Id = year,
                    DisplayText = year.ToString() + "年",
                    Parent = _root
                });
            }
            BindTree();
        }

        public void LoadResults() {
            _root = new FilterTreeNode { DisplayText = "全部结果", IsThreeState = true, FontWeight = FontWeights.Bold };

            string[] results = new[] { "优秀", "合格", "基本合格", "不合格" };
            int id = 1;
            foreach (var result in results) {
                _root.Children.Add(new FilterTreeNode {
                    Id = id++,
                    DisplayText = result,
                    Parent = _root
                });
            }
            BindTree();
        }

        /// <summary>
        /// 加载性别（程序生成）
        /// </summary>
        public void LoadGenders() {
            _root = new FilterTreeNode { DisplayText = "全部性别", IsThreeState = true, FontWeight = FontWeights.Bold };
            string[] genders = new[] { "男", "女" };
            int id = 1;
            foreach (var g in genders) {
                _root.Children.Add(new FilterTreeNode { Id = id++, DisplayText = g, Parent = _root });
            }
            BindTree();
        }

        /// <summary>
        /// 加载属相（12生肖）（程序生成）
        /// </summary>
        public void LoadZodiacs() {
            _root = new FilterTreeNode { DisplayText = "全部属相", IsThreeState = true, FontWeight = FontWeights.Bold };
            string[] zodiacs = new[] { "鼠", "牛", "虎", "兔", "龙", "蛇", "马", "羊", "猴", "鸡", "狗", "猪" };
            int id = 1;
            foreach (var z in zodiacs) {
                _root.Children.Add(new FilterTreeNode { Id = id++, DisplayText = z, Parent = _root });
            }
            BindTree();
        }

        /// <summary>
        /// 加载民族（从数据库DISTINCT查询）
        /// </summary>
        public void LoadEthnics() {
            _root = new FilterTreeNode { DisplayText = "全部民族", IsThreeState = true, FontWeight = FontWeights.Bold };
            string sql = "SELECT DISTINCT minzu FROM ZX_config_yg WHERE minzu IS NOT NULL AND minzu <> '' ORDER BY minzu";
            DataTable dt = SqlHelper.ExecuteDataTable(sql);
            int id = 1;
            foreach (DataRow row in dt.Rows) {
                _root.Children.Add(new FilterTreeNode { Id = id++, DisplayText = row["minzu"].ToString(), Parent = _root });
            }
            BindTree();
        }

        /// <summary>
        /// 加载政治面貌（从数据库DISTINCT查询）
        /// </summary>
        public void LoadPolitics() {
            _root = new FilterTreeNode { DisplayText = "全部政治面貌", IsThreeState = true, FontWeight = FontWeights.Bold };
            string sql = "SELECT DISTINCT zhengzhimm FROM ZX_config_yg WHERE zhengzhimm IS NOT NULL AND zhengzhimm <> '' ORDER BY zhengzhimm";
            DataTable dt = SqlHelper.ExecuteDataTable(sql);
            int id = 1;
            foreach (DataRow row in dt.Rows) {
                _root.Children.Add(new FilterTreeNode { Id = id++, DisplayText = row["zhengzhimm"].ToString(), Parent = _root });
            }
            BindTree();
        }

        /// <summary>
        /// 加载学历（从数据库DISTINCT查询）
        /// </summary>
        public void LoadEducations() {
            _root = new FilterTreeNode { DisplayText = "全部学历", IsThreeState = true, FontWeight = FontWeights.Bold };
            string sql = "SELECT DISTINCT xueli FROM ZX_config_yg WHERE xueli IS NOT NULL AND xueli <> '' ORDER BY xueli";
            DataTable dt = SqlHelper.ExecuteDataTable(sql);
            int id = 1;
            foreach (DataRow row in dt.Rows) {
                _root.Children.Add(new FilterTreeNode { Id = id++, DisplayText = row["xueli"].ToString(), Parent = _root });
            }
            BindTree();
        }

        /// <summary>
        /// 加载学位（从数据库DISTINCT查询）
        /// </summary>
        public void LoadDegrees() {
            _root = new FilterTreeNode { DisplayText = "全部学位", IsThreeState = true, FontWeight = FontWeights.Bold };
            string sql = "SELECT DISTINCT xuewei FROM ZX_config_yg WHERE xuewei IS NOT NULL AND xuewei <> '' ORDER BY xuewei";
            DataTable dt = SqlHelper.ExecuteDataTable(sql);
            int id = 1;
            foreach (DataRow row in dt.Rows) {
                _root.Children.Add(new FilterTreeNode { Id = id++, DisplayText = row["xuewei"].ToString(), Parent = _root });
            }
            BindTree();
        }

        /// <summary>
        /// 加载职称等级（从数据库DISTINCT查询）
        /// </summary>
        public void LoadTitleLevels() {
            _root = new FilterTreeNode { DisplayText = "全部职称等级", IsThreeState = true, FontWeight = FontWeights.Bold };
            string sql = "SELECT DISTINCT zhichengdj FROM ZX_config_yg WHERE zhichengdj IS NOT NULL AND zhichengdj <> '' ORDER BY zhichengdj";
            DataTable dt = SqlHelper.ExecuteDataTable(sql);
            int id = 1;
            foreach (DataRow row in dt.Rows) {
                _root.Children.Add(new FilterTreeNode { Id = id++, DisplayText = row["zhichengdj"].ToString(), Parent = _root });
            }
            BindTree();
        }

        /// <summary>
        /// 获取选中项的显示文本列表（用于考核结果等字符串类型筛选）
        /// </summary>
        public List<string> GetSelectedTexts() {
            if (_root == null) return new List<string>();
            return _root.Children.Where(x => x.IsChecked == true).Select(x => x.DisplayText).ToList();
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

    /// <summary>
    /// 筛选树节点数据模型
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
