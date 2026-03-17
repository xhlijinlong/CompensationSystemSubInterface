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
        /// 加载在职状态（程序生成）
        /// Id对应数据库zaizhi字段: 1=在职, 0=离职
        /// </summary>
        public void LoadEmploymentStatus() {
            _root = new FilterTreeNode { DisplayText = "全部状态", IsThreeState = true, FontWeight = FontWeights.Bold };
            // Id直接使用数据库中的zaizhi值
            _root.Children.Add(new FilterTreeNode { Id = 1, DisplayText = "在职", Parent = _root });
            _root.Children.Add(new FilterTreeNode { Id = 0, DisplayText = "离职", Parent = _root });
            BindTree();
        }

        /// <summary>
        /// 加载日期年月（从数据库查询实际存在的年月，构建二级树：年→月）
        /// </summary>
        /// <param name="dbFieldName">数据库字段名（如 chushengrq, gongzuosj 等）</param>
        /// <param name="displayName">显示名称（如 "出生日期"）</param>
        public void LoadDateYearMonths(string dbFieldName, string displayName) {
            _root = new FilterTreeNode { DisplayText = "全部" + displayName, IsThreeState = true, FontWeight = FontWeights.Bold };

            string sql = $@"SELECT DISTINCT YEAR({dbFieldName}) AS y, MONTH({dbFieldName}) AS m 
                           FROM ZX_config_yg 
                           WHERE {dbFieldName} IS NOT NULL AND {dbFieldName} > '1900-01-01' AND zaizhi=1 
                           ORDER BY y, m";
            DataTable dt = SqlHelper.ExecuteDataTable(sql);

            // 按年分组构建二级树
            var yearGroups = new Dictionary<int, FilterTreeNode>();
            int id = 1;
            foreach (DataRow row in dt.Rows) {
                int year = Convert.ToInt32(row["y"]);
                int month = Convert.ToInt32(row["m"]);

                if (!yearGroups.ContainsKey(year)) {
                    var yearNode = new FilterTreeNode {
                        Id = year,
                        DisplayText = year + "年",
                        IsThreeState = true,
                        FontWeight = FontWeights.Bold,
                        IsExpanded = false,
                        Parent = _root
                    };
                    _root.Children.Add(yearNode);
                    yearGroups[year] = yearNode;
                }

                var monthNode = new FilterTreeNode {
                    Id = id++,
                    DisplayText = year.ToString("D4") + "-" + month.ToString("D2"),
                    Parent = yearGroups[year]
                };
                yearGroups[year].Children.Add(monthNode);
            }
            BindTree();
        }

        /// <summary>
        /// 加载年龄段（固定6个档位）
        /// </summary>
        public void LoadAgeRanges() {
            _root = new FilterTreeNode { DisplayText = "全部年龄", IsThreeState = true, FontWeight = FontWeights.Bold };
            string[] ranges = new[] { "35岁以下", "35-39", "40-44", "45-49", "50-54", "55岁以上" };
            int id = 1;
            foreach (var r in ranges) {
                _root.Children.Add(new FilterTreeNode { Id = id++, DisplayText = r, Parent = _root });
            }
            BindTree();
        }

        /// <summary>
        /// 加载专业技能（从数据库DISTINCT查询）
        /// </summary>
        public void LoadSkills() {
            _root = new FilterTreeNode { DisplayText = "全部专业技能", IsThreeState = true, FontWeight = FontWeights.Bold };
            string sql = "SELECT DISTINCT zhuanyejn FROM ZX_config_yg WHERE zhuanyejn IS NOT NULL AND zhuanyejn <> '' AND zaizhi=1 ORDER BY zhuanyejn";
            DataTable dt = SqlHelper.ExecuteDataTable(sql);
            int id = 1;
            foreach (DataRow row in dt.Rows) {
                _root.Children.Add(new FilterTreeNode { Id = id++, DisplayText = row["zhuanyejn"].ToString(), Parent = _root });
            }
            BindTree();
        }

        /// <summary>
        /// 加载专业技术（从数据库DISTINCT查询）
        /// </summary>
        public void LoadTechnologies() {
            _root = new FilterTreeNode { DisplayText = "全部专业技术", IsThreeState = true, FontWeight = FontWeights.Bold };
            string sql = "SELECT DISTINCT zhuanyejs FROM ZX_config_yg WHERE zhuanyejs IS NOT NULL AND zhuanyejs <> '' AND zaizhi=1 ORDER BY zhuanyejs";
            DataTable dt = SqlHelper.ExecuteDataTable(sql);
            int id = 1;
            foreach (DataRow row in dt.Rows) {
                _root.Children.Add(new FilterTreeNode { Id = id++, DisplayText = row["zhuanyejs"].ToString(), Parent = _root });
            }
            BindTree();
        }

        /// <summary>
        /// 获取选中项的显示文本列表（用于考核结果等字符串类型筛选）
        /// 仅获取一级子节点
        /// </summary>
        public List<string> GetSelectedTexts() {
            if (_root == null) return new List<string>();
            return _root.Children.Where(x => x.IsChecked == true).Select(x => x.DisplayText).ToList();
        }

        /// <summary>
        /// 获取所有叶子节点中选中项的显示文本列表（用于二级树，如日期年月）
        /// 递归遍历所有层级，只返回没有子节点（叶子节点）的选中项
        /// </summary>
        public List<string> GetAllLeafSelectedTexts() {
            if (_root == null) return new List<string>();
            var result = new List<string>();
            CollectLeafTexts(_root, result);
            return result;
        }

        private void CollectLeafTexts(FilterTreeNode node, List<string> result) {
            foreach (var child in node.Children) {
                if (child.Children.Count > 0) {
                    CollectLeafTexts(child, result);
                } else if (child.IsChecked == true) {
                    result.Add(child.DisplayText);
                }
            }
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
            // 对于二级树，部分选中（IsChecked==null）也应计入已选中
            return _root.Children.Count(x => x.IsChecked == true || x.IsChecked == null);
        }

        public bool IsAllSelected() {
            if (_root == null || _root.Children.Count == 0) return false;
            return _root.IsChecked == true;
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e) {
            var cb = sender as CheckBox;
            var node = cb?.DataContext as FilterTreeNode;
            if (node == null) return;

            if (node.Children.Count > 0) {
                // 父节点点击 - 递归设置所有子孙节点
                bool isChecked = node.IsChecked == true;
                SetAllChildrenChecked(node, isChecked);
                // 更新父节点状态（如果有父节点）
                if (node.Parent != null) {
                    UpdateParentState(node.Parent);
                }
            } else {
                // 叶子节点点击 - 逐级向上更新父节点状态
                UpdateParentState(node.Parent);
            }

            SelectionChanged?.Invoke(GetSelectedIds());
        }

        /// <summary>
        /// 递归设置所有子节点的选中状态
        /// </summary>
        private void SetAllChildrenChecked(FilterTreeNode node, bool isChecked) {
            foreach (var child in node.Children) {
                child.IsChecked = isChecked;
                if (child.Children.Count > 0) {
                    SetAllChildrenChecked(child, isChecked);
                }
            }
        }

        private void UpdateParentState(FilterTreeNode parent) {
            if (parent == null || parent.Children.Count == 0) return;

            bool allChecked = parent.Children.All(x => x.IsChecked == true);
            bool anyChecked = parent.Children.Any(x => x.IsChecked == true || x.IsChecked == null);

            if (allChecked) parent.IsChecked = true;
            else if (anyChecked) parent.IsChecked = null;
            else parent.IsChecked = false;

            // 递归向上更新
            if (parent.Parent != null) {
                UpdateParentState(parent.Parent);
            }
        }

        #endregion
    }

    /// <summary>
    /// 筛选树节点数据模型
    /// </summary>

    public class FilterTreeNode : INotifyPropertyChanged {
        private bool? _isChecked = false;
        private bool _isExpanded = true;

        public int Id { get; set; }
        public string DisplayText { get; set; }
        public bool IsThreeState { get; set; } = false;
        public FontWeight FontWeight { get; set; } = FontWeights.Normal;
        public FilterTreeNode Parent { get; set; }
        public ObservableCollection<FilterTreeNode> Children { get; set; } = new ObservableCollection<FilterTreeNode>();

        /// <summary>
        /// 节点是否展开（默认为true，日期年月的年节点设为false）
        /// </summary>
        public bool IsExpanded {
            get => _isExpanded;
            set {
                if (_isExpanded != value) {
                    _isExpanded = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsExpanded)));
                }
            }
        }

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
