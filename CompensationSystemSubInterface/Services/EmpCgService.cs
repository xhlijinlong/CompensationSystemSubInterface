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
    /// 员工变动信息查询服务类
    /// </summary>
    public class EmpCgService {
        /// <summary>
        /// 查询员工变动信息
        /// </summary>
        public DataTable GetEmpCgData(DateTime startDate, DateTime endDate, string keyword, EmpCgQueryCondition cond) {
            StringBuilder sb = new StringBuilder();

            sb.Append(@"
                SELECT 
                    yg.yuangongbh AS '员工号',
                    yg.xingming AS '姓名',
                    cg.oldbm AS '原部门',
                    cg.oldxl AS '原序列',
                    cg.oldzw AS '原职务',
                    cg.oldcj AS '原层级',
                    cg.changeType AS '变动项目',
                    cg.changeTime AS '变动时间',
                    cg.newbm AS '新部门',
                    cg.newxl AS '新序列',
                    cg.newzw AS '新职务',
                    cg.newcj AS '新层级',
                    cg.wageStart AS '起薪时间',
                    cg.id, cg.ygid
                FROM 
                    ZX_yuangong_change cg
                    JOIN ZX_config_yg yg ON cg.ygid = yg.id 
                    JOIN ZX_config_bm bm ON yg.bmid = bm.id 
                WHERE 
                    yg.zaizhi = 1 
                    AND cg.changeTime BETWEEN @StartDate AND @EndDate
            ");

            List<SqlParameter> ps = new List<SqlParameter>();
            ps.Add(new SqlParameter("@StartDate", startDate));
            ps.Add(new SqlParameter("@EndDate", endDate));

            // 姓名搜索
            if (!string.IsNullOrWhiteSpace(keyword)) {
                sb.Append(" AND (yg.xingming LIKE @Key OR yg.yuangongbh LIKE @Key)");
                ps.Add(new SqlParameter("@Key", "%" + keyword.Trim() + "%"));
            }

            // 高级筛选
            if (cond != null) {
                if (cond.DepartmentIds.Count > 0)
                    sb.Append($" AND bm.id IN ({string.Join(",", cond.DepartmentIds)})");

                if (cond.EmployeeIds.Count > 0)
                    sb.Append($" AND yg.id IN ({string.Join(",", cond.EmployeeIds)})");
            }

            sb.Append(" ORDER BY yg.xuhao, cg.changeTime DESC"); // 按人排序，同一个人按时间倒序

            return SqlHelper.ExecuteDataTable(sb.ToString(), ps.ToArray());
        }
    }
}