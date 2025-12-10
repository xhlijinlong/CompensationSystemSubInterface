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
    /// 员工信息查询服务类
    /// </summary>
    public class EmpService {
        /// <summary>
        /// 查询员工详细信息
        /// </summary>
        /// <param name="keyword">关键字搜索（姓名）</param>
        /// <param name="cond">高级筛选条件（部门、员工）</param>
        /// <returns>员工信息数据表</returns>
        public DataTable GetEmpData(string keyword, EmpQueryCondition cond) {
            StringBuilder sb = new StringBuilder();

            // 拼接 SQL 语句 (直接使用你提供的 SQL)
            sb.Append(@"
                SELECT 
                    yg.yuangongbh AS '员工号',
                    yg.xingming AS '姓名',
                    bm.bmname AS '部门',
                    xl.xlname AS '序列',
                    gw.gwname AS '职务',
                    cj.cjname AS '层级',
                    yg.xingbie AS '性别',
                    yg.minzu AS '民族',
                    yg.zhengzhimm AS '政治面貌',
                    yg.xueli AS '学历',
                    yg.xuewei AS '学位',
                    yg.chushengrq AS '出生日期',
                    yg.gongzuosj AS '参加工作时间',
                    yg.rusisj AS '入社时间',
                    yg.gangweisj AS '任现岗位时间',
                    yg.shenfenzheng AS '证件号码',
                    yg.zhuanyejs AS '专业技术',
                    yg.zhichengdj AS '职称等级',
                    yg.zhichengsj AS '取得时间',
                    yg.zhuanyejn AS '专业技能',
                    yg.jinengsj AS '技能时间',
                    yg.lianxidh AS '联系电话',
                    yg.nianling AS '年龄',
                    yg.renyuanlb AS '人员类别',
                    yg.shuxing AS '属相',
                    yg.hunyinzk AS '婚姻状况',
                    yg.qishisfzrq AS '身份证起始',
                    yg.jieshusfzrq AS '身份证截止',
                    yg.hujidz AS '户籍地址',
                    yg.xianzhuzhi AS '现住址',
                    yg.gongzikh AS '工资卡号',
                    yg.lizhisj AS '离职日期',
                    yg.id, yg.bmid, yg.xlid, yg.gwid, yg.cjid
                FROM 
                    ZX_config_yg yg
                    LEFT JOIN ZX_config_bm bm ON yg.bmid = bm.id
                    LEFT JOIN ZX_config_xl xl ON yg.xlid = xl.id
                    LEFT JOIN ZX_config_gw gw ON yg.gwid = gw.id
                    LEFT JOIN ZX_config_cj cj ON yg.cjid = cj.id
                WHERE 
                    bm.IsEnabled=1 AND bm.DeleteType=0 
                    AND yg.zaizhi=1
            ");

            List<SqlParameter> ps = new List<SqlParameter>();

            // 1. 姓名模糊搜索
            if (!string.IsNullOrWhiteSpace(keyword)) {
                sb.Append(" AND (yg.xingming LIKE @Key OR yg.yuangongbh LIKE @Key)");
                ps.Add(new SqlParameter("@Key", "%" + keyword.Trim() + "%"));
            }

            // 2. 高级筛选条件
            if (cond != null) {
                if (cond.DepartmentIds.Count > 0)
                    sb.Append($" AND bm.id IN ({string.Join(",", cond.DepartmentIds)})");

                if (cond.EmployeeIds.Count > 0)
                    sb.Append($" AND yg.id IN ({string.Join(",", cond.EmployeeIds)})");
            }

            // 3. 排序
            sb.Append(" ORDER BY yg.xuhao");

            return SqlHelper.ExecuteDataTable(sb.ToString(), ps.ToArray());
        }
    }
}