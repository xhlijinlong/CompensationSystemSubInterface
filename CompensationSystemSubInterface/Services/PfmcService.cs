using CompensationSystemSubInterface.Models;
using CompensationSystemSubInterface.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sm4Encode;

namespace CompensationSystemSubInterface.Services {
    /// <summary>
    /// 绩效信息查询服务类
    /// </summary>
    public class PfmcService {
        // 1. 获取近5年年份列表 (用于初始化筛选条件)
        public List<int> GetRecentYears(int count = 5) {
            List<int> years = new List<int>();
            int currentYear = DateTime.Now.Year;
            for (int i = 0; i < count; i++) {
                years.Add(currentYear - i);
            }
            return years;
        }

        // 2. 获取所有考核结果类型 (用于初始化筛选条件)
        // 硬编码或查库均可，这里按你需求硬编码
        public List<string> GetAssessmentResults() {
            return new List<string> { "优秀", "合格", "基本合格", "不合格" };
        }

        // 3. 获取原始数据 (纵向)
        public DataTable GetRawPfmcData(string keyword, PfmcQueryCondition cond) {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                SELECT 
                    yg.yuangongbh AS 'EmployeeNo',
                    yg.xingming AS 'EmployeeName',
                    jx.[year] AS 'Year',
                    jx.Assessment AS 'Result',
                    yg.id AS 'EmployeeId'
                FROM ZX_Assessment jx
                JOIN ZX_config_yg yg ON jx.ygid = yg.id 
                WHERE yg.zaizhi = 1 
            ");

            List<SqlParameter> ps = new List<SqlParameter>();

            // 姓名搜索
            if (!string.IsNullOrWhiteSpace(keyword)) {
                sb.Append(" AND (yg.xingming LIKE @Key OR yg.yuangongbh LIKE @Key)");
                ps.Add(new SqlParameter("@Key", "%" + keyword.Trim() + "%"));
            }

            // 高级筛选
            if (cond != null) {
                if (cond.EmployeeIds.Count > 0)
                    sb.Append($" AND yg.id IN ({string.Join(",", cond.EmployeeIds)})");

                if (cond.Years.Count > 0)
                    sb.Append($" AND jx.[year] IN ({string.Join(",", cond.Years)})");

                if (cond.Results.Count > 0) {
                    // 字符串列表需要加单引号
                    string resultStr = string.Join(",", cond.Results.Select(r => $"'{r}'"));
                    sb.Append($" AND jx.Assessment IN ({resultStr})");
                }
            }

            sb.Append(" ORDER BY yg.xuhao, jx.[year] DESC");

            return SqlHelper.ExecuteDataTable(sb.ToString(), ps.ToArray());
        }

        // 4. 【核心】构建透视表 (横向展示)
        public DataTable BuildPivotReport(DataTable rawData) {
            DataTable dt = new DataTable();

            // 固定列
            dt.Columns.Add("员工号", typeof(string));
            dt.Columns.Add("姓名", typeof(string));

            // 动态列：找出数据中出现的所有年份，并排序
            var years = rawData.AsEnumerable()
                               .Select(r => r.Field<int>("Year"))
                               .Distinct()
                               .OrderByDescending(y => y) // OrderByDescending 最近年份在左边，或者 OrderBy(y=>y) 最近年份在右边
                               .ToList();

            foreach (var year in years) {
                dt.Columns.Add(year.ToString() + "年", typeof(string)); // 列名: 2025年
            }

            if (rawData.Rows.Count == 0) return dt;

            // 分组填充
            var empGroups = rawData.AsEnumerable()
                                   .GroupBy(r => new {
                                       Id = r.Field<int>("EmployeeId"),
                                       No = r.Field<string>("EmployeeNo"),
                                       Name = r.Field<string>("EmployeeName")
                                   });

            foreach (var group in empGroups) {
                DataRow newRow = dt.NewRow();
                newRow["员工号"] = group.Key.No;
                newRow["姓名"] = group.Key.Name;

                foreach (var row in group) {
                    int y = row.Field<int>("Year");
                    string colName = y.ToString() + "年";
                    // 如果这年有多次考核？通常一年一次，直接覆盖
                    newRow[colName] = row.Field<string>("Result");
                }
                dt.Rows.Add(newRow);
            }

            return dt;
        }
    }
}