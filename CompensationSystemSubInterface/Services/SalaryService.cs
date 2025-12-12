using CompensationSystemSubInterface.Models;
using CompensationSystemSubInterface.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompensationSystemSubInterface.Services {
    /// <summary>
    /// 薪资数据查询服务类
    /// </summary>
    public class SalaryService {
        /// <summary>
        /// 获取最近的一个薪资月份（用于界面默认值）
        /// </summary>
        /// <returns>最近的薪资月份；如果没有数据则返回 null</returns>
        public DateTime? GetLatestSalaryMonth() {
            string sql = "SELECT TOP 1 SalaryMonth FROM ZX_SalaryHeaders ORDER BY SalaryMonth DESC";
            object result = SqlHelper.ExecuteScalar(sql);
            if (result != null && result != DBNull.Value) return Convert.ToDateTime(result);
            return null;
        }

        /// <summary>
        /// 获取所有启用的薪资项目
        /// </summary>
        /// <returns>包含 ItemId、ItemName 和 IsEveryMonth 字段的数据表</returns>
        public DataTable GetSalaryItems() {
            string sql = "SELECT ItemId, ItemName, IsEveryMonth FROM ZX_SalaryItems WHERE IsEnabled=1 ORDER BY DisplayOrder";
            return SqlHelper.ExecuteDataTable(sql);
        }

        /// <summary>
        /// 查询原始薪资数据（纵向格式）
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="keyword">关键字搜索（员工姓名或员工编号）</param>
        /// <param name="cond">高级筛选条件，包含部门、序列、职务、层级和员工ID列表</param>
        /// <returns>包含薪资明细的原始数据表，包括员工信息、薪资项目和金额等字段</returns>
        public DataTable GetRawSalaryData(DateTime startDate, DateTime endDate, string keyword, SalaryQueryCondition cond) {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                SELECT h.SalaryMonth, yg.id AS EmployeeId, yg.xingming AS EmployeeName,
                       si.ItemId, si.ItemName, d.Amount, bm.bmname AS DeptName,
                       h.DisplayOrder, si.DisplayOrder AS ItemOrder
                FROM ZX_SalaryHeaders h
                JOIN ZX_SalaryDetails d ON h.SalaryId = d.SalaryId
                JOIN ZX_SalaryItems si ON d.ItemId = si.ItemId
                JOIN ZX_config_yg yg ON h.EmployeeId = yg.id
                JOIN ZX_config_bm bm ON yg.bmid = bm.id
                JOIN ZX_config_xl xl ON yg.xlid = xl.id
                JOIN ZX_config_gw gw ON yg.gwid = gw.id
                JOIN ZX_config_cj cj ON yg.cjid = cj.id
                WHERE bm.IsEnabled=1 AND bm.DeleteType=0 
                  AND yg.zaizhi=1 AND si.IsEnabled=1
                  AND h.SalaryMonth BETWEEN @StartDate AND @EndDate
            ");

            List<SqlParameter> ps = new List<SqlParameter>();
            ps.Add(new SqlParameter("@StartDate", startDate));
            ps.Add(new SqlParameter("@EndDate", endDate));

            // 姓名搜索
            if (!string.IsNullOrWhiteSpace(keyword)) {
                sb.Append(" AND (yg.xingming LIKE @Key OR yg.yuangongbh LIKE @Key)");
                ps.Add(new SqlParameter("@Key", "%" + keyword.Trim() + "%"));
            }

            // 高级筛选 SQL 拼接
            if (cond.DepartmentIds.Count > 0) sb.Append($" AND bm.id IN ({string.Join(",", cond.DepartmentIds)})");
            if (cond.SequenceIds.Count > 0) sb.Append($" AND xl.id IN ({string.Join(",", cond.SequenceIds)})");
            if (cond.PositionIds.Count > 0) sb.Append($" AND gw.id IN ({string.Join(",", cond.PositionIds)})");
            if (cond.LevelIds.Count > 0) sb.Append($" AND cj.id IN ({string.Join(",", cond.LevelIds)})");
            if (cond.EmployeeIds.Count > 0) sb.Append($" AND yg.id IN ({string.Join(",", cond.EmployeeIds)})");

            // 排序：先按展示顺序，再按月份, 再按薪资项目
            sb.Append(" ORDER BY h.DisplayOrder, h.SalaryMonth, si.DisplayOrder");

            return SqlHelper.ExecuteDataTable(sb.ToString(), ps.ToArray());
        }

        /// <summary>
        /// 构建透视表格式的薪资报表（横向格式）
        /// </summary>
        /// <param name="rawData">原始薪资数据（纵向格式）</param>
        /// <param name="allItems">所有薪资项目信息</param>
        /// <returns>
        /// 透视后的报表数据表，包含以下特点：
        /// <list type="bullet">
        /// <item><description>每个薪资项目作为一列显示</description></item>
        /// <item><description>自动计算个人小计、部门总计和全厂总计</description></item>
        /// <item><description>仅显示启用的薪资项目或有数据的项目</description></item>
        /// <item><description>包含隐藏的 RowType 列标识行类型（0=明细，1=个人小计，2=部门总计，3=全厂总计）</description></item>
        /// </list>
        /// </returns>
        /// <summary>
        /// 构建透视表格式的薪资报表
        /// </summary>
        public DataTable BuildReportData(DataTable rawData, DataTable allItems, SalaryQueryCondition cond) {
            DataTable dt = new DataTable();

            // 获取用户勾选的 ID
            List<int> userSelectedIds = cond != null ? cond.SalaryItemIds : new List<int>();
            bool hasUserFilter = userSelectedIds.Count > 0;
            bool isSelectAll = hasUserFilter && (userSelectedIds.Count == allItems.Rows.Count);

            // 决定是否显示“合计”列 (规则: 有勾选 且 非全选)
            bool showTotalColumn = hasUserFilter && !isSelectAll;

            // 找出在当前数据中“有值”的项目 ID 集合
            HashSet<int> itemsWithData = new HashSet<int>();
            foreach (DataRow r in rawData.Rows) {
                decimal val = r["Amount"] != DBNull.Value ? Convert.ToDecimal(r["Amount"]) : 0;
                if (val != 0) itemsWithData.Add(Convert.ToInt32(r["ItemId"]));
            }

            // 最终确定要显示的 ID 列表
            List<int> finalColIds = new List<int>();
            Dictionary<int, string> colMap = new Dictionary<int, string>();
            Dictionary<int, string> colCaption = new Dictionary<int, string>();

            foreach (DataRow item in allItems.Rows) {
                int id = Convert.ToInt32(item["ItemId"]);

                // 如果用户有筛选，且该 ID 不在筛选列表中，直接跳过
                if (hasUserFilter && !userSelectedIds.Contains(id)) continue;

                bool isEveryMonth = Convert.ToInt32(item["IsEveryMonth"]) == 1;

                // 显示规则：(每月必显) OR (数据里有值)
                if (isEveryMonth || itemsWithData.Contains(id)) {
                    finalColIds.Add(id);
                    colCaption[id] = item["ItemName"].ToString();
                }
            }

            dt.Columns.Add("MonthStr", typeof(string));
            dt.Columns.Add("DeptName", typeof(string));
            dt.Columns.Add("EmployeeName", typeof(string));

            // 添加动态列 (按照 allItems 的原生顺序添加)
            foreach (int id in finalColIds) {
                string colName = "Item_" + id;
                if (!dt.Columns.Contains(colName)) {
                    DataColumn dc = dt.Columns.Add(colName, typeof(decimal));
                    dc.Caption = colCaption[id];
                    dc.DefaultValue = 0;
                    colMap[id] = colName;
                }
            }

            // 添加合计列
            if (showTotalColumn) {
                dt.Columns.Add("TotalAmount", typeof(decimal)).Caption = "各项合计";
            }

            //dt.Columns.Add("RowType", typeof(int)).ColumnMapping = MappingType.Hidden;
            dt.Columns.Add("RowType", typeof(int));

            if (rawData.Rows.Count == 0) return dt;

            var deptGroups = rawData.AsEnumerable().GroupBy(r => r.Field<string>("DeptName"));

            Dictionary<string, decimal> grandTotals = new Dictionary<string, decimal>();
            decimal grandRowTotal = 0;

            // 存放部门合计，实现"部门小计在最后"
            List<DataRow> deptTotalRows = new List<DataRow>();

            foreach (var deptGroup in deptGroups) {
                string deptName = deptGroup.Key;
                Dictionary<string, decimal> deptTotals = new Dictionary<string, decimal>();
                decimal deptRowTotal = 0;

                var empGroups = deptGroup.GroupBy(r => new { Id = r.Field<int>("EmployeeId"), Name = r.Field<string>("EmployeeName") });

                foreach (var empGroup in empGroups) {
                    Dictionary<string, decimal> empTotals = new Dictionary<string, decimal>();
                    decimal empRowTotal = 0;

                    var monthGroups = empGroup.GroupBy(r => r.Field<DateTime>("SalaryMonth"));
                    foreach (var monthGroup in monthGroups) {
                        DataRow row = dt.NewRow();
                        row["MonthStr"] = monthGroup.Key.ToString("yyyy年MM月");
                        row["DeptName"] = deptName;
                        row["EmployeeName"] = empGroup.Key.Name;
                        row["RowType"] = 0; // 明细

                        decimal rowSum = 0;
                        foreach (var d in monthGroup) {
                            int id = d.Field<int>("ItemId");

                            if (colMap.ContainsKey(id)) {
                                decimal amt = d.Field<decimal>("Amount");
                                string cName = colMap[id];
                                row[cName] = amt;

                                if (!empTotals.ContainsKey(cName)) empTotals[cName] = 0;
                                empTotals[cName] += amt;

                                if (showTotalColumn) rowSum += amt;
                            }
                        }

                        if (showTotalColumn) row["TotalAmount"] = rowSum;
                        empRowTotal += rowSum;
                        dt.Rows.Add(row);
                    }

                    // --- 个人小计 ---
                    DataRow subEmp = dt.NewRow();
                    subEmp["DeptName"] = deptName;
                    subEmp["EmployeeName"] = "小计";
                    subEmp["RowType"] = 1;
                    foreach (var kv in empTotals) {
                        subEmp[kv.Key] = kv.Value;
                        if (!deptTotals.ContainsKey(kv.Key)) deptTotals[kv.Key] = 0;
                        deptTotals[kv.Key] += kv.Value;
                    }
                    if (showTotalColumn) subEmp["TotalAmount"] = empRowTotal;
                    deptRowTotal += empRowTotal;
                    dt.Rows.Add(subEmp);
                }

                // --- 部门小计 ---
                DataRow subDept = dt.NewRow();
                subDept["DeptName"] = deptName;
                subDept["EmployeeName"] = deptName + " 小计";
                subDept["RowType"] = 2;
                foreach (var kv in deptTotals) {
                    subDept[kv.Key] = kv.Value;
                    if (!grandTotals.ContainsKey(kv.Key)) grandTotals[kv.Key] = 0;
                    grandTotals[kv.Key] += kv.Value;
                }
                if (showTotalColumn) subDept["TotalAmount"] = deptRowTotal;
                grandRowTotal += deptRowTotal;

                // 添加到部门人员之后
                //dt.Rows.Add(subDept);
                // 加入暂存列表, 最后统一添加
                deptTotalRows.Add(subDept);

            } // End Dept Loop

            // 统一追加部门合计行
            foreach (var row in deptTotalRows) {
                dt.Rows.Add(row);
            }

            // --- 全厂总计 ---
            DataRow subGrand = dt.NewRow();
            subGrand["EmployeeName"] = "总计";
            subGrand["RowType"] = 3;
            foreach (var kv in grandTotals) subGrand[kv.Key] = kv.Value;
            if (showTotalColumn) subGrand["TotalAmount"] = grandRowTotal;
            dt.Rows.Add(subGrand);

            return dt;
        }


        /// <summary>
        /// 构建【薪酬统计】报表数据（按员工汇总，无月份明细）
        /// </summary>
        public DataTable BuildStatisticsReportData(DataTable rawData, DataTable allItems, SalaryQueryCondition cond) {
            DataTable dt = new DataTable();

            // 确定要显示的列 (逻辑同 BuildReportData，完全一致)
            List<int> userSelectedIds = cond != null ? cond.SalaryItemIds : new List<int>();
            bool hasUserFilter = userSelectedIds.Count > 0;
            bool isSelectAll = hasUserFilter && (userSelectedIds.Count == allItems.Rows.Count);
            bool showTotalColumn = hasUserFilter && !isSelectAll;

            HashSet<int> itemsWithData = new HashSet<int>();
            foreach (DataRow r in rawData.Rows) {
                decimal val = r["Amount"] != DBNull.Value ? Convert.ToDecimal(r["Amount"]) : 0;
                if (val != 0) itemsWithData.Add(Convert.ToInt32(r["ItemId"]));
            }

            List<int> finalColIds = new List<int>();
            Dictionary<int, string> colMap = new Dictionary<int, string>();
            Dictionary<int, string> colCaption = new Dictionary<int, string>();

            foreach (DataRow item in allItems.Rows) {
                int id = Convert.ToInt32(item["ItemId"]);
                if (hasUserFilter && !userSelectedIds.Contains(id)) continue;
                bool isEveryMonth = Convert.ToInt32(item["IsEveryMonth"]) == 1;

                if (isEveryMonth || itemsWithData.Contains(id)) {
                    finalColIds.Add(id);
                    colCaption[id] = item["ItemName"].ToString();
                }
            }

            // 构建 DataTable 结构 (增加序号列)
            dt.Columns.Add("Seq", typeof(int)); // 序号
            dt.Columns.Add("DeptName", typeof(string));
            dt.Columns.Add("EmployeeName", typeof(string));

            foreach (int id in finalColIds) {
                string colName = "Item_" + id;
                if (!dt.Columns.Contains(colName)) {
                    DataColumn dc = dt.Columns.Add(colName, typeof(decimal));
                    dc.Caption = colCaption[id];
                    dc.DefaultValue = 0;
                    colMap[id] = colName;
                }
            }

            if (showTotalColumn) {
                dt.Columns.Add("TotalAmount", typeof(decimal)).Caption = "合计";
            }

            //dt.Columns.Add("RowType", typeof(int)).ColumnMapping = MappingType.Hidden;
            dt.Columns.Add("RowType", typeof(int));

            if (rawData.Rows.Count == 0) return dt;

            // 填充数据 (按员工汇总)
            var deptGroups = rawData.AsEnumerable().GroupBy(r => r.Field<string>("DeptName"));

            Dictionary<string, decimal> grandTotals = new Dictionary<string, decimal>();
            decimal grandRowTotal = 0;
            int serialNumber = 1; // 全局序号计数器

            List<DataRow> deptTotalRows = new List<DataRow>();

            foreach (var deptGroup in deptGroups) {
                string deptName = deptGroup.Key;
                Dictionary<string, decimal> deptTotals = new Dictionary<string, decimal>();
                decimal deptRowTotal = 0;

                var empGroups = deptGroup.GroupBy(r => new { Id = r.Field<int>("EmployeeId"), Name = r.Field<string>("EmployeeName") });

                foreach (var empGroup in empGroups) {
                    // --- 直接创建一行员工数据，不再有月份循环 ---
                    DataRow row = dt.NewRow();
                    row["Seq"] = serialNumber++; // 填充序号
                    row["DeptName"] = deptName;
                    row["EmployeeName"] = empGroup.Key.Name;
                    row["RowType"] = 0; // 普通数据行

                    decimal rowSum = 0;

                    // 遍历该员工的所有原始数据记录进行累加
                    foreach (var d in empGroup) {
                        int id = d.Field<int>("ItemId");

                        if (colMap.ContainsKey(id)) {
                            decimal amt = d.Field<decimal>("Amount");
                            string cName = colMap[id];

                            // 累加到当前行
                            decimal currentVal = row[cName] != DBNull.Value ? Convert.ToDecimal(row[cName]) : 0;
                            row[cName] = currentVal + amt;

                            // 累加到部门合计
                            if (!deptTotals.ContainsKey(cName)) deptTotals[cName] = 0;
                            deptTotals[cName] += amt;

                            // 累加行合计
                            if (showTotalColumn) rowSum += amt;
                        }
                    }

                    if (showTotalColumn) row["TotalAmount"] = rowSum;
                    deptRowTotal += rowSum;

                    dt.Rows.Add(row);
                }

                // --- 部门小计 ---
                DataRow subDept = dt.NewRow();
                // 序号留空或不显示
                subDept["DeptName"] = deptName;
                subDept["EmployeeName"] = deptName + " 统计";
                subDept["RowType"] = 2; // 保持 2 (灰色高亮)
                foreach (var kv in deptTotals) {
                    subDept[kv.Key] = kv.Value;
                    if (!grandTotals.ContainsKey(kv.Key)) grandTotals[kv.Key] = 0;
                    grandTotals[kv.Key] += kv.Value;
                }
                if (showTotalColumn) subDept["TotalAmount"] = deptRowTotal;
                grandRowTotal += deptRowTotal;

                deptTotalRows.Add(subDept);

            } // End Dept Loop

            // 追加部门合计
            foreach (var row in deptTotalRows) dt.Rows.Add(row);

            // --- 全厂总计 ---
            DataRow subGrand = dt.NewRow();
            subGrand["EmployeeName"] = "总计";
            subGrand["RowType"] = 3; // 保持 3 (金色高亮)
            foreach (var kv in grandTotals) subGrand[kv.Key] = kv.Value;
            if (showTotalColumn) subGrand["TotalAmount"] = grandRowTotal;
            dt.Rows.Add(subGrand);

            return dt;
        }
    }
}