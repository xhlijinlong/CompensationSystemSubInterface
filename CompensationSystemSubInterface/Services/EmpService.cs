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
    /// 员工信息查询服务类
    /// </summary>
    public class EmpService {
        /// <summary>
        /// SM4加密解密对象实例 (用于敏感信息的加密和解密)
        /// </summary>
        private Sm4Crypto _sm4 = new Sm4Crypto();


        /// <summary>
        /// 查询员工详细信息 (用于员工信息查询界面)
        /// </summary>
        /// <param name="keyword">关键字搜索（姓名或员工编号）</param>
        /// <param name="cond">高级筛选条件（部门、员工）</param>
        /// <returns>包含员工信息的数据表</returns>
        public DataTable GetEmpData(string keyword, EmpCondition cond) {
            StringBuilder sb = new StringBuilder();

            // 拼接 SQL 语句 (直接使用你提供的 SQL)
            sb.Append(@"
                SELECT 
                    yg.yuangongbh AS '员工编号',
                    bm.bmname AS '部门',
                    gw.gwname AS '职务',
                    yg.xingming AS '姓名',
                    yg.xingbie AS '性别',
                    yg.minzu AS '民族',
                    yg.zhengzhimm AS '政治面貌',
                    yg.xueli AS '学历',
                    yg.xuewei AS '学位',
                    NULLIF(yg.chushengrq, '1900-01-01') AS '出生日期',
                    NULLIF(yg.gongzuosj, '1900-01-01') AS '参加工作时间',
                    NULLIF(yg.rusisj, '1900-01-01') AS '入社时间',
                    NULLIF(yg.gangweisj, '1900-01-01') AS '任现岗位时间',
                    yg.shenfenzheng AS '证件号码',
                    yg.zhuanyejs AS '专业技术',
                    yg.zhichengdj AS '职称等级',
                    NULLIF(yg.zhichengsj, '1900-01-01') AS '取得时间',
                    yg.zhuanyejn AS '专业技能',
                    NULLIF(yg.jinengsj, '1900-01-01') AS '技能时间',
                    xl.xlname AS '序列',
                    cj.cjname AS '层级',
                    yg.shuxing AS '属相',
                    yg.nianling AS '年龄',
                    yg.lianxidh AS '联系电话',
                    yg.gongzikh AS '工资卡号',
                    yg.id, yg.bmid, yg.xlid, yg.gwid
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
                sb.Append(" AND (yg.xingming LIKE @Key OR yg.yuangongbh LIKE @Key OR yg.yonghuming LIKE @Key)");
                ps.Add(new SqlParameter("@Key", "%" + keyword.Trim() + "%"));
            }

            // 2. 高级筛选条件
            if (cond != null) {
                if (cond.SequenceIds.Count > 0)
                    sb.Append($" AND xl.id IN ({string.Join(",", cond.SequenceIds)})");

                if (cond.DepartmentIds.Count > 0)
                    sb.Append($" AND bm.id IN ({string.Join(",", cond.DepartmentIds)})");

                if (cond.PositionIds.Count > 0)
                    sb.Append($" AND gw.id IN ({string.Join(",", cond.PositionIds)})");

                if (cond.EmployeeIds.Count > 0)
                    sb.Append($" AND yg.id IN ({string.Join(",", cond.EmployeeIds)})");

                if (cond.Genders.Count > 0)
                    sb.Append($" AND yg.xingbie IN ({string.Join(",", cond.Genders.Select(x => "'" + x.Replace("'", "''") + "'"))})");

                if (cond.Ethnics.Count > 0)
                    sb.Append($" AND yg.minzu IN ({string.Join(",", cond.Ethnics.Select(x => "'" + x.Replace("'", "''") + "'"))})");

                if (cond.Zodiacs.Count > 0)
                    sb.Append($" AND yg.shuxing IN ({string.Join(",", cond.Zodiacs.Select(x => "'" + x.Replace("'", "''") + "'"))})");

                if (cond.Politics.Count > 0)
                    sb.Append($" AND yg.zhengzhimm IN ({string.Join(",", cond.Politics.Select(x => "'" + x.Replace("'", "''") + "'"))})");

                if (cond.Educations.Count > 0)
                    sb.Append($" AND yg.xueli IN ({string.Join(",", cond.Educations.Select(x => "'" + x.Replace("'", "''") + "'"))})");

                if (cond.Degrees.Count > 0)
                    sb.Append($" AND yg.xuewei IN ({string.Join(",", cond.Degrees.Select(x => "'" + x.Replace("'", "''") + "'"))})");

                if (cond.TitleLevels.Count > 0)
                    sb.Append($" AND yg.zhichengdj IN ({string.Join(",", cond.TitleLevels.Select(x => "'" + x.Replace("'", "''") + "'"))})");

                // 日期年月筛选（格式 "yyyy-MM"）
                if (cond.BirthdayYearMonths.Count > 0)
                    sb.Append($" AND FORMAT(yg.chushengrq, 'yyyy-MM') IN ({string.Join(",", cond.BirthdayYearMonths.Select(x => "'" + x.Replace("'", "''") + "'"))})");

                if (cond.WorkDateYearMonths.Count > 0)
                    sb.Append($" AND FORMAT(yg.gongzuosj, 'yyyy-MM') IN ({string.Join(",", cond.WorkDateYearMonths.Select(x => "'" + x.Replace("'", "''") + "'"))})");

                if (cond.HireDateYearMonths.Count > 0)
                    sb.Append($" AND FORMAT(yg.rusisj, 'yyyy-MM') IN ({string.Join(",", cond.HireDateYearMonths.Select(x => "'" + x.Replace("'", "''") + "'"))})");

                if (cond.PositionDateYearMonths.Count > 0)
                    sb.Append($" AND FORMAT(yg.gangweisj, 'yyyy-MM') IN ({string.Join(",", cond.PositionDateYearMonths.Select(x => "'" + x.Replace("'", "''") + "'"))})");

                // 年龄段筛选
                if (cond.AgeRanges.Count > 0) {
                    var ageClauses = new List<string>();
                    foreach (var range in cond.AgeRanges) {
                        if (range == "35岁以下") ageClauses.Add("yg.nianling < 35");
                        else if (range == "35-39") ageClauses.Add("yg.nianling BETWEEN 35 AND 39");
                        else if (range == "40-44") ageClauses.Add("yg.nianling BETWEEN 40 AND 44");
                        else if (range == "45-49") ageClauses.Add("yg.nianling BETWEEN 45 AND 49");
                        else if (range == "50-54") ageClauses.Add("yg.nianling BETWEEN 50 AND 54");
                        else if (range == "55岁以上") ageClauses.Add("yg.nianling >= 55");
                    }
                    if (ageClauses.Count > 0)
                        sb.Append($" AND ({string.Join(" OR ", ageClauses)})");
                }

                // 专业技能筛选
                if (cond.Skills.Count > 0)
                    sb.Append($" AND yg.zhuanyejn IN ({string.Join(",", cond.Skills.Select(x => "'" + x.Replace("'", "''") + "'"))})");

                // 专业技术筛选
                if (cond.Technologies.Count > 0)
                    sb.Append($" AND yg.zhuanyejs IN ({string.Join(",", cond.Technologies.Select(x => "'" + x.Replace("'", "''") + "'"))})");
            }

            // 3. 排序
            sb.Append(" ORDER BY yg.xuhao");

            //return SqlHelper.ExecuteDataTable(sb.ToString(), ps.ToArray());

            DataTable dt = SqlHelper.ExecuteDataTable(sb.ToString(), ps.ToArray());

            // 3. 遍历解密
            if (dt.Rows.Count > 0) {
                // 为了提高性能，先检查是否有需要解密的列，避免每次循环都 Contains
                bool hasIdCard = dt.Columns.Contains("证件号码");
                bool hasBankCard = dt.Columns.Contains("工资卡号");

                if (hasIdCard || hasBankCard) {
                    foreach (DataRow row in dt.Rows) {
                        try {
                            if (hasIdCard && row["证件号码"] != DBNull.Value) {
                                string cipher = row["证件号码"].ToString().Trim();
                                if (!string.IsNullOrEmpty(cipher)) {
                                    // 解密逻辑：调用 DLL 中的方法
                                    row["证件号码"] = _sm4.Decrypt_ECB_Str(cipher);
                                }
                            }

                            if (hasBankCard && row["工资卡号"] != DBNull.Value) {
                                string cipher = row["工资卡号"].ToString().Trim();
                                if (!string.IsNullOrEmpty(cipher)) {
                                    // 解密逻辑
                                    row["工资卡号"] = _sm4.Decrypt_ECB_Str(cipher);
                                }
                            }
                        } catch {
                            // 解密失败时不处理，保持密文显示，防止报错崩溃
                            // row["证件号码"] = "解密失败"; 
                        }
                    }
                }
            }

            return dt;
        }

        /// <summary>
        /// 查询离职员工详细信息 (用于离职员工信息查询界面)
        /// </summary>
        /// <param name="keyword">关键字搜索（姓名或员工编号）</param>
        /// <param name="cond">高级筛选条件（部门、员工）</param>
        /// <returns>包含离职员工信息的数据表</returns>
        public DataTable GetEmpRsData(string keyword, EmpCondition cond) {
            StringBuilder sb = new StringBuilder();

            // 拼接 SQL 语句 - 查询离职员工 (zaizhi=0)
            sb.Append(@"
                SELECT 
                    yg.yuangongbh AS '员工编号',
                    bm.bmname AS '部门',
                    gw.gwname AS '职务',
                    yg.xingming AS '姓名',
                    yg.xingbie AS '性别',
                    yg.minzu AS '民族',
                    yg.zhengzhimm AS '政治面貌',
                    yg.xueli AS '学历',
                    yg.xuewei AS '学位',
                    NULLIF(yg.chushengrq, '1900-01-01') AS '出生日期',
                    NULLIF(yg.gongzuosj, '1900-01-01') AS '参加工作时间',
                    NULLIF(yg.rusisj, '1900-01-01') AS '入社时间',
                    NULLIF(yg.gangweisj, '1900-01-01') AS '任现岗位时间',
                    yg.shenfenzheng AS '证件号码',
                    yg.zhuanyejs AS '专业技术',
                    yg.zhichengdj AS '职称等级',
                    NULLIF(yg.zhichengsj, '1900-01-01') AS '取得时间',
                    yg.zhuanyejn AS '专业技能',
                    NULLIF(yg.jinengsj, '1900-01-01') AS '技能时间',
                    xl.xlname AS '序列',
                    cj.cjname AS '层级',
                    yg.shuxing AS '属相',
                    yg.nianling AS '年龄',
                    yg.lianxidh AS '联系电话',
                    yg.gongzikh AS '工资卡号',
                    NULLIF(yg.lizhisj, '1900-01-01') AS '离职日期',
                    yg.id, yg.bmid, yg.xlid, yg.gwid
                FROM 
                    ZX_config_yg yg
                    LEFT JOIN ZX_config_bm bm ON yg.bmid = bm.id
                    LEFT JOIN ZX_config_xl xl ON yg.xlid = xl.id
                    LEFT JOIN ZX_config_gw gw ON yg.gwid = gw.id
                    LEFT JOIN ZX_config_cj cj ON yg.cjid = cj.id
                WHERE 
                    yg.zaizhi=0
            ");

            List<SqlParameter> ps = new List<SqlParameter>();

            // 1. 姓名模糊搜索
            if (!string.IsNullOrWhiteSpace(keyword)) {
                sb.Append(" AND (yg.xingming LIKE @Key OR yg.yuangongbh LIKE @Key OR yg.yonghuming LIKE @Key)");
                ps.Add(new SqlParameter("@Key", "%" + keyword.Trim() + "%"));
            }

            // 2. 高级筛选条件
            if (cond != null) {
                if (cond.SequenceIds.Count > 0)
                    sb.Append($" AND xl.id IN ({string.Join(",", cond.SequenceIds)})");

                if (cond.DepartmentIds.Count > 0)
                    sb.Append($" AND bm.id IN ({string.Join(",", cond.DepartmentIds)})");

                if (cond.PositionIds.Count > 0)
                    sb.Append($" AND gw.id IN ({string.Join(",", cond.PositionIds)})");

                if (cond.EmployeeIds.Count > 0)
                    sb.Append($" AND yg.id IN ({string.Join(",", cond.EmployeeIds)})");

                if (cond.Genders.Count > 0)
                    sb.Append($" AND yg.xingbie IN ({string.Join(",", cond.Genders.Select(x => "'" + x.Replace("'", "''") + "'"))})");

                if (cond.Ethnics.Count > 0)
                    sb.Append($" AND yg.minzu IN ({string.Join(",", cond.Ethnics.Select(x => "'" + x.Replace("'", "''") + "'"))})");

                if (cond.Zodiacs.Count > 0)
                    sb.Append($" AND yg.shuxing IN ({string.Join(",", cond.Zodiacs.Select(x => "'" + x.Replace("'", "''") + "'"))})");

                if (cond.Politics.Count > 0)
                    sb.Append($" AND yg.zhengzhimm IN ({string.Join(",", cond.Politics.Select(x => "'" + x.Replace("'", "''") + "'"))})");

                if (cond.Educations.Count > 0)
                    sb.Append($" AND yg.xueli IN ({string.Join(",", cond.Educations.Select(x => "'" + x.Replace("'", "''") + "'"))})");

                if (cond.Degrees.Count > 0)
                    sb.Append($" AND yg.xuewei IN ({string.Join(",", cond.Degrees.Select(x => "'" + x.Replace("'", "''") + "'"))})");

                if (cond.TitleLevels.Count > 0)
                    sb.Append($" AND yg.zhichengdj IN ({string.Join(",", cond.TitleLevels.Select(x => "'" + x.Replace("'", "''") + "'"))})");
            }

            // 3. 排序 - 按离职日期倒序
            sb.Append(" ORDER BY yg.lizhisj DESC, yg.xuhao");

            DataTable dt = SqlHelper.ExecuteDataTable(sb.ToString(), ps.ToArray());

            // 4. 遍历解密
            if (dt.Rows.Count > 0) {
                bool hasIdCard = dt.Columns.Contains("证件号码");
                bool hasBankCard = dt.Columns.Contains("工资卡号");

                if (hasIdCard || hasBankCard) {
                    foreach (DataRow row in dt.Rows) {
                        try {
                            if (hasIdCard && row["证件号码"] != DBNull.Value) {
                                string cipher = row["证件号码"].ToString().Trim();
                                if (!string.IsNullOrEmpty(cipher)) {
                                    row["证件号码"] = _sm4.Decrypt_ECB_Str(cipher);
                                }
                            }

                            if (hasBankCard && row["工资卡号"] != DBNull.Value) {
                                string cipher = row["工资卡号"].ToString().Trim();
                                if (!string.IsNullOrEmpty(cipher)) {
                                    row["工资卡号"] = _sm4.Decrypt_ECB_Str(cipher);
                                }
                            }
                        } catch {
                            // 解密失败时不处理
                        }
                    }
                }
            }

            return dt;
        }

        /// <summary>
        /// 查询员工详细信息 (用于员工维护界面)
        /// </summary>
        /// <param name="keyword">关键字搜索（姓名或员工编号）</param>
        /// <param name="cond">高级筛选条件（部门、员工）</param>
        /// <returns>包含员工信息的数据表</returns>
        public DataTable GetEmpMaintData(string keyword, EmpCondition cond) {
            StringBuilder sb = new StringBuilder();

            // 拼接 SQL 语句 (直接使用你提供的 SQL)
            sb.Append(@"
                SELECT 
                    yg.yuangongbh AS '员工编号',
                    bm.bmname AS '部门',
                    gw.gwname AS '职务',
                    yg.xingming AS '姓名',
                    yg.xingbie AS '性别',
                    yg.minzu AS '民族',
                    yg.zhengzhimm AS '政治面貌',
                    yg.xueli AS '学历',
                    yg.xuewei AS '学位',
                    NULLIF(yg.chushengrq, '1900-01-01') AS '出生日期',
                    NULLIF(yg.gongzuosj, '1900-01-01') AS '参加工作时间',
                    NULLIF(yg.rusisj, '1900-01-01') AS '入社时间',
                    NULLIF(yg.gangweisj, '1900-01-01') AS '任现岗位时间',
                    yg.shenfenzheng AS '证件号码',
                    yg.zhuanyejs AS '专业技术',
                    yg.zhichengdj AS '职称等级',
                    NULLIF(yg.zhichengsj, '1900-01-01') AS '取得时间',
                    yg.zhuanyejn AS '专业技能',
                    NULLIF(yg.jinengsj, '1900-01-01') AS '技能时间',
                    xl.xlname AS '序列',
                    cj.cjname AS '层级',
                    yg.shuxing AS '属相',
                    yg.nianling AS '年龄',
                    yg.lianxidh AS '联系电话',
                    yg.gongzikh AS '工资卡号',
                    yg.xuhao AS '序号',
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
                sb.Append(" AND (yg.xingming LIKE @Key OR yg.yuangongbh LIKE @Key OR yg.yonghuming LIKE @Key)");
                ps.Add(new SqlParameter("@Key", "%" + keyword.Trim() + "%"));
            }

            // 2. 高级筛选条件
            if (cond != null) {
                if (cond.SequenceIds.Count > 0)
                    sb.Append($" AND xl.id IN ({string.Join(",", cond.SequenceIds)})");

                if (cond.DepartmentIds.Count > 0)
                    sb.Append($" AND bm.id IN ({string.Join(",", cond.DepartmentIds)})");

                if (cond.PositionIds.Count > 0)
                    sb.Append($" AND gw.id IN ({string.Join(",", cond.PositionIds)})");

                if (cond.EmployeeIds.Count > 0)
                    sb.Append($" AND yg.id IN ({string.Join(",", cond.EmployeeIds)})");

                if (cond.Genders.Count > 0)
                    sb.Append($" AND yg.xingbie IN ({string.Join(",", cond.Genders.Select(x => "'" + x.Replace("'", "''") + "'"))})");

                if (cond.Ethnics.Count > 0)
                    sb.Append($" AND yg.minzu IN ({string.Join(",", cond.Ethnics.Select(x => "'" + x.Replace("'", "''") + "'"))})");

                if (cond.Zodiacs.Count > 0)
                    sb.Append($" AND yg.shuxing IN ({string.Join(",", cond.Zodiacs.Select(x => "'" + x.Replace("'", "''") + "'"))})");

                if (cond.Politics.Count > 0)
                    sb.Append($" AND yg.zhengzhimm IN ({string.Join(",", cond.Politics.Select(x => "'" + x.Replace("'", "''") + "'"))})");

                if (cond.Educations.Count > 0)
                    sb.Append($" AND yg.xueli IN ({string.Join(",", cond.Educations.Select(x => "'" + x.Replace("'", "''") + "'"))})");

                if (cond.Degrees.Count > 0)
                    sb.Append($" AND yg.xuewei IN ({string.Join(",", cond.Degrees.Select(x => "'" + x.Replace("'", "''") + "'"))})");

                if (cond.TitleLevels.Count > 0)
                    sb.Append($" AND yg.zhichengdj IN ({string.Join(",", cond.TitleLevels.Select(x => "'" + x.Replace("'", "''") + "'"))})");
            }

            // 3. 排序
            sb.Append(" ORDER BY yg.xuhao");

            //return SqlHelper.ExecuteDataTable(sb.ToString(), ps.ToArray());

            DataTable dt = SqlHelper.ExecuteDataTable(sb.ToString(), ps.ToArray());

            // 3. 遍历解密
            if (dt.Rows.Count > 0) {
                // 为了提高性能，先检查是否有需要解密的列，避免每次循环都 Contains
                bool hasIdCard = dt.Columns.Contains("证件号码");
                bool hasBankCard = dt.Columns.Contains("工资卡号");

                if (hasIdCard || hasBankCard) {
                    foreach (DataRow row in dt.Rows) {
                        try {
                            if (hasIdCard && row["证件号码"] != DBNull.Value) {
                                string cipher = row["证件号码"].ToString().Trim();
                                if (!string.IsNullOrEmpty(cipher)) {
                                    // 解密逻辑：调用 DLL 中的方法
                                    row["证件号码"] = _sm4.Decrypt_ECB_Str(cipher);
                                }
                            }

                            if (hasBankCard && row["工资卡号"] != DBNull.Value) {
                                string cipher = row["工资卡号"].ToString().Trim();
                                if (!string.IsNullOrEmpty(cipher)) {
                                    // 解密逻辑
                                    row["工资卡号"] = _sm4.Decrypt_ECB_Str(cipher);
                                }
                            }
                        } catch {
                            // 解密失败时不处理，保持密文显示，防止报错崩溃
                            // row["证件号码"] = "解密失败"; 
                        }
                    }
                }
            }

            return dt;
        }

        /// <summary>
        /// 查询员工变动信息 (用于员工变动查询界面)
        /// </summary>
        /// <param name="startDate">变动开始日期</param>
        /// <param name="endDate">变动结束日期</param>
        /// <param name="keyword">关键字搜索（姓名或员工编号）</param>
        /// <param name="cond">高级筛选条件（部门、员工）</param>
        /// <returns>包含员工变动记录的数据表</returns>
        public DataTable GetEmpCgData(DateTime startDate, DateTime endDate, string keyword, EmpCondition cond) {
            StringBuilder sb = new StringBuilder();

            sb.Append(@"
                SELECT 
                    yg.yuangongbh AS '编号',
                    yg.xingming AS '姓名',
                    cg.oldbm AS '原部门',
                    cg.oldxl AS '原序列',
                    cg.oldzw AS '原职务',
                    cg.oldcj AS '原层级',
                    cg.changeType AS '变动项目',
                    NULLIF(cg.changeTime, '1900-01-01') AS '变动时间',
                    cg.newbm AS '新部门',
                    cg.newxl AS '新序列',
                    cg.newzw AS '新职务',
                    cg.newcj AS '新层级',
                    NULLIF(cg.wageStart, '1900-01-01') AS '起薪时间',
                    cg.id, cg.ygid
                FROM 
                    ZX_yuangong_change cg
                    JOIN ZX_config_yg yg ON cg.ygid = yg.id 
                    JOIN ZX_config_bm bm ON yg.bmid = bm.id 
                WHERE 
                    yg.id > 0
                    AND cg.changeTime BETWEEN @StartDate AND @EndDate
                    AND (cg.DeleteType = 0 OR cg.DeleteType IS NULL)
            ");

            // 参数列表
            List<SqlParameter> ps = new List<SqlParameter>();
            ps.Add(new SqlParameter("@StartDate", startDate));
            ps.Add(new SqlParameter("@EndDate", endDate));


            // 姓名搜索
            if (!string.IsNullOrWhiteSpace(keyword)) {
                sb.Append(" AND (yg.xingming LIKE @Key OR yg.yuangongbh LIKE @Key OR yg.yonghuming LIKE @Key)");
                ps.Add(new SqlParameter("@Key", "%" + keyword.Trim() + "%"));
            }

            // 高级筛选
            if (cond != null) {
                if (cond.SequenceIds.Count > 0)
                    sb.Append($" AND yg.xlid IN ({string.Join(",", cond.SequenceIds)})");

                if (cond.DepartmentIds.Count > 0)
                    sb.Append($" AND bm.id IN ({string.Join(",", cond.DepartmentIds)})");

                if (cond.PositionIds.Count > 0)
                    sb.Append($" AND yg.gwid IN ({string.Join(",", cond.PositionIds)})");

                if (cond.EmployeeIds.Count > 0)
                    sb.Append($" AND yg.id IN ({string.Join(",", cond.EmployeeIds)})");
            }

            sb.Append(" ORDER BY cg.changeTime DESC, yg.xuhao ASC");

            return SqlHelper.ExecuteDataTable(sb.ToString(), ps.ToArray());
        }

        /// <summary>
        /// 获取单个员工的详细信息 (用于修改界面数据回显)
        /// </summary>
        /// <param name="empId">员工ID</param>
        /// <returns>员工数据行，如果不存在则返回null</returns>
        public DataRow GetEmpDetail(int empId) {
            string sql = @"
                SELECT yg.*, 
                       bm.bmname, xl.xlname, gw.gwname, cj.cjname 
                FROM ZX_config_yg yg
                LEFT JOIN ZX_config_bm bm ON yg.bmid = bm.id
                LEFT JOIN ZX_config_xl xl ON yg.xlid = xl.id
                LEFT JOIN ZX_config_gw gw ON yg.gwid = gw.id
                LEFT JOIN ZX_config_cj cj ON yg.cjid = cj.id
                WHERE yg.id = @Id";

            DataTable dt = SqlHelper.ExecuteDataTable(sql, new SqlParameter("@Id", empId));
            if (dt.Rows.Count > 0) {
                DataRow row = dt.Rows[0];
                // 解密敏感信息
                try {
                    if (row["shenfenzheng"] != DBNull.Value)
                        row["shenfenzheng"] = _sm4.Decrypt_ECB_Str(row["shenfenzheng"].ToString());
                    if (row["gongzikh"] != DBNull.Value)
                        row["gongzikh"] = _sm4.Decrypt_ECB_Str(row["gongzikh"].ToString());
                } catch { /* 忽略解密错误 */ }
                return row;
            }
            return null;
        }

        /// <summary>
        /// 获取基础配置数据 (用于下拉框数据绑定)
        /// 根据不同表名应用不同的筛选条件和排序规则
        /// </summary>
        /// <param name="tableName">配置表名称：ZX_config_bm(部门), ZX_config_xl(序列), ZX_config_gw(职务), ZX_config_cj(层级)</param>
        /// <returns>包含id和name列的配置数据表</returns>
        public DataTable GetDictData(string tableName) {
            string sql = "";

            switch (tableName) {
                case "ZX_config_bm":
                    // 部门：有 IsEnabled, DeleteType, 按 DisplayOrder 排序
                    sql = @"SELECT id, bmname as name 
                            FROM ZX_config_bm 
                            WHERE IsEnabled=1 AND DeleteType=0 
                            ORDER BY DisplayOrder ASC";
                    break;

                case "ZX_config_xl":
                    // 序列：有 IsEnabled, DeleteType (未指定排序，默认处理)
                    sql = @"SELECT id, xlname as name 
                            FROM ZX_config_xl 
                            WHERE IsEnabled=1 AND DeleteType=0";
                    break;

                case "ZX_config_gw":
                    // 职务：有 IsEnabled, DeleteType, 按 DisplayOrder 排序
                    sql = @"SELECT id, gwname as name 
                            FROM ZX_config_gw 
                            WHERE IsEnabled=1 AND DeleteType=0 
                            ORDER BY DisplayOrder ASC";
                    break;

                case "ZX_config_cj":
                    // ★层级：只有 DeleteType，没有 IsEnabled★ (这是之前报错的原因)
                    sql = @"SELECT id, cjname as name 
                            FROM ZX_config_cj 
                            WHERE DeleteType=0";
                    break;

                default:
                    // 默认兜底逻辑
                    sql = $"SELECT id, name FROM {tableName}";
                    break;
            }

            return SqlHelper.ExecuteDataTable(sql);
        }

        /// <summary>
        /// 更新员工基本信息 (包含敏感信息的加密处理)
        /// </summary>
        /// <param name="emp">员工详细信息对象，包含所有需要更新的字段</param>
        public void UpdateEmpBasicInfo(EmpDetail emp) {
            // 1. 加密敏感字段 (保持不变)
            string encIdCard = "";
            if (!string.IsNullOrEmpty(emp.IdCard)) {
                encIdCard = _sm4.Encrypt_ECB_Str(emp.IdCard.Trim());
            }
            string encBankCard = "";
            if (!string.IsNullOrEmpty(emp.BankCard)) {
                encBankCard = _sm4.Encrypt_ECB_Str(emp.BankCard.Trim());
            }

            // 2. 构建完整的 UPDATE SQL
            string sql = @"
        UPDATE ZX_config_yg 
        SET 
            xingming = @Name,
            xingbie = @Gender,
            minzu = @Nation,
            zhengzhimm = @Politic,
            shuxing = @Zodiac,
            nianling = @Age,
            chushengrq = @Birthday,
            lianxidh = @Phone,

            xueli = @Education,
            xuewei = @Degree,
            
            gongzuosj = @WorkStart,
            rusisj = @JoinDate,
            gangweisj = @PostDate,
            lizhisj = @ResignDate,

            zhuanyejs = @Tech,
            zhichengdj = @TitleLevel,
            zhichengsj = @TitleDate,
            zhuanyejn = @Skill,
            jinengsj = @SkillDate,

            shenfenzheng = @IdCard,
            gongzikh = @BankCard,

            shiyong = @IsProbation,
            yjbys = @IsFreshGraduate
        WHERE id = @Id";

            // 3. 执行更新 (补全参数)
            SqlHelper.ExecuteNonQuery(sql,
                new SqlParameter("@Id", emp.Id),
                new SqlParameter("@Name", emp.Name ?? ""),
                new SqlParameter("@Gender", emp.Gender ?? ""),
                new SqlParameter("@Nation", emp.Nation ?? ""),
                new SqlParameter("@Politic", emp.Politic ?? ""),
                new SqlParameter("@Zodiac", emp.Zodiac ?? ""),
                new SqlParameter("@Age", emp.Age), // int类型
                new SqlParameter("@Birthday", emp.Birthday ?? (object)DBNull.Value), // DateTime?
                new SqlParameter("@Phone", emp.Phone ?? ""),

                new SqlParameter("@Education", emp.Education ?? ""),
                new SqlParameter("@Degree", emp.Degree ?? ""),

                new SqlParameter("@WorkStart", emp.WorkStart ?? (object)DBNull.Value),
                new SqlParameter("@JoinDate", emp.JoinDate ?? (object)DBNull.Value),
                new SqlParameter("@PostDate", emp.PostDate ?? (object)DBNull.Value),
                new SqlParameter("@ResignDate", emp.ResignDate ?? (object)DBNull.Value),

                new SqlParameter("@Tech", emp.TechSpecialty ?? ""),
                new SqlParameter("@TitleLevel", emp.TitleLevel ?? ""),
                new SqlParameter("@TitleDate", emp.TitleDate.HasValue ? emp.TitleDate.Value.ToString("yyyy-MM-dd") : (object)DBNull.Value),
                new SqlParameter("@Skill", emp.Skill ?? ""),
                new SqlParameter("@SkillDate", emp.SkillDate.HasValue ? emp.SkillDate.Value.ToString("yyyy-MM-dd") : (object)DBNull.Value),

                new SqlParameter("@IdCard", encIdCard),
                new SqlParameter("@BankCard", encBankCard),

                new SqlParameter("@IsProbation", emp.IsProbation ? 1 : 0),
                new SqlParameter("@IsFreshGraduate", emp.IsFreshGraduate ? 1 : 0)
            );
        }

        /// <summary>
        /// 执行员工变动操作 (核心事务：插入变动记录 + 更新员工表)
        /// 使用事务确保数据一致性
        /// </summary>
        /// <param name="empId">员工ID</param>
        /// <param name="changeType">变动类型（如：调动、晋升、离职等）</param>
        /// <param name="changeTime">变动时间</param>
        /// <param name="oldBmId">原部门ID</param>
        /// <param name="oldBmName">原部门名称</param>
        /// <param name="oldXlId">原序列ID</param>
        /// <param name="oldXlName">原序列名称</param>
        /// <param name="oldZwId">原职务ID</param>
        /// <param name="oldZwName">原职务名称</param>
        /// <param name="oldCjId">原层级ID</param>
        /// <param name="oldCjName">原层级名称</param>
        /// <param name="newBm">新部门ID</param>
        /// <param name="newBmName">新部门名称</param>
        /// <param name="newXl">新序列ID</param>
        /// <param name="newXlName">新序列名称</param>
        /// <param name="newZw">新职务ID</param>
        /// <param name="newZwName">新职务名称</param>
        /// <param name="newCj">新层级ID</param>
        /// <param name="newCjName">新层级名称</param>
        /// <param name="wageStart">起薪时间</param>
        /// <param name="wageEnd">结束薪资时间</param>
        public void ExecuteEmployeeChange(int empId, string changeType, DateTime changeTime,
                                          // --- 新增：原信息的ID参数 ---
                                          int? oldBmId, string oldBmName,
                                          int? oldXlId, string oldXlName,
                                          int? oldZwId, string oldZwName,
                                          int? oldCjId, string oldCjName,
                                          // ------------------------
                                          int newBm, string newBmName,
                                          int newXl, string newXlName,
                                          int newZw, string newZwName,
                                          int newCj, string newCjName,
                                          DateTime? wageStart, DateTime? wageEnd) {

            using (SqlConnection conn = new SqlConnection(SqlHelper.ConnString)) {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction()) {
                    try {
                        // 判断变动时间是否为今天（使用数据库时间）
                        bool isToday = changeTime.Date <= DateTime.Today;

                        // 1. 插入变动记录 (包含 IsEffect 和 DeleteType)
                        string insertSql = @"
                    INSERT INTO ZX_yuangong_change 
                    (ygid, changeType, changeTime, 
                        oldbmid, oldbm, oldxlid, oldxl, oldzwid, oldzw, oldcjid, oldcj, 
                        newbmid, newbm, newxlid, newxl, newzwid, newzw, newcjid, newcj, 
                        wageStart, wageEnd, IsEffect, DeleteType)
                    VALUES 
                    (@YgId, @Type, @Time, 
                        @OldBmId, @OldBm, @OldXlId, @OldXl, @OldZwId, @OldZw, @OldCjId, @OldCj, 
                        @NewBmId, @NewBm, @NewXlId, @NewXl, @NewZwId, @NewZw, @NewCjId, @NewCj, 
                        @WageStart, @WageEnd, @IsEffect, 0)";

                        SqlHelper.ExecuteNonQuery(trans, insertSql,
                            new SqlParameter("@YgId", empId),
                            new SqlParameter("@Type", changeType),
                            new SqlParameter("@Time", changeTime),

                            // --- 原信息 (ID + Name) ---
                            new SqlParameter("@OldBmId", oldBmId ?? (object)DBNull.Value),
                            new SqlParameter("@OldBm", oldBmName ?? ""),
                            new SqlParameter("@OldXlId", oldXlId ?? (object)DBNull.Value),
                            new SqlParameter("@OldXl", oldXlName ?? ""),
                            new SqlParameter("@OldZwId", oldZwId ?? (object)DBNull.Value),
                            new SqlParameter("@OldZw", oldZwName ?? ""),
                            new SqlParameter("@OldCjId", oldCjId ?? (object)DBNull.Value),
                            new SqlParameter("@OldCj", oldCjName ?? ""),

                            // --- 新信息 (ID + Name) ---
                            new SqlParameter("@NewBmId", newBm),
                            new SqlParameter("@NewBm", newBmName ?? ""),
                            new SqlParameter("@NewXlId", newXl),
                            new SqlParameter("@NewXl", newXlName ?? ""),
                            new SqlParameter("@NewZwId", newZw),
                            new SqlParameter("@NewZw", newZwName ?? ""),
                            new SqlParameter("@NewCjId", newCj),
                            new SqlParameter("@NewCj", newCjName ?? ""),

                            new SqlParameter("@WageStart", wageStart ?? (object)DBNull.Value),
                            new SqlParameter("@WageEnd", wageEnd ?? (object)DBNull.Value),
                            new SqlParameter("@IsEffect", isToday ? 1 : 0)
                        );

                        // 2. 仅当变动时间是今天（或已过去）才立即更新员工主表
                        if (isToday) {
                            string updateSql = @"
                        UPDATE ZX_config_yg 
                        SET bmid=@Bm, xlid=@Xl, gwid=@Zw, cjid=@Cj, newChange=1 
                        WHERE id=@Id";

                            if (changeType == "离职") {
                                updateSql = @"
                            UPDATE ZX_config_yg 
                            SET bmid=@Bm, xlid=@Xl, gwid=@Zw, cjid=@Cj, 
                                zaizhi=0, lizhisj=@Time, newChange=1 
                            WHERE id=@Id";
                            }

                            SqlHelper.ExecuteNonQuery(trans, updateSql,
                                new SqlParameter("@Bm", newBm),
                                new SqlParameter("@Xl", newXl),
                                new SqlParameter("@Zw", newZw),
                                new SqlParameter("@Cj", newCj),
                                new SqlParameter("@Time", changeTime),
                                new SqlParameter("@Id", empId)
                            );
                        }

                        trans.Commit();
                    } catch {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// 获取员工详细信息对象 (返回强类型的EmployeeDetail对象，用于WPF绑定)
        /// </summary>
        /// <param name="empId">员工ID</param>
        /// <returns>员工详细信息对象，如果不存在则返回null</returns>
        public EmpDetail GetEmpDetailObj(int empId) {
            DataRow row = GetEmpDetail(empId); // 调用你之前写好的 SQL 方法
            if (row == null) return null;

            // 手动映射，或者使用 AutoMapper
            return new EmpDetail {
                Id = Convert.ToInt32(row["id"]),
                EmployeeNo = row["yuangongbh"].ToString(),
                Name = row["xingming"].ToString(),

                // 基础信息
                Gender = row["xingbie"].ToString(),
                Nation = row["minzu"].ToString(),
                Politic = row["zhengzhimm"].ToString(),
                Marital = row["hunyinzk"].ToString(),
                Zodiac = row["shuxing"].ToString(),
                Age = row["nianling"] != DBNull.Value ? Convert.ToInt32(row["nianling"]) : 0,
                //Birthday = row["chushengrq"] as DateTime?,
                Birthday = ConvertToDate(row["chushengrq"]),
                IdCard = row["shenfenzheng"].ToString(), // 此时已解密
                //IdStart = row["qishisfzrq"] as DateTime?,
                IdStart = ConvertToDate(row["qishisfzrq"]),
                //IdEnd = row["jieshusfzrq"] as DateTime?,
                IdEnd = ConvertToDate(row["jieshusfzrq"]),

                // 组织信息 (Value)
                DeptId = row["bmid"] as int?,
                DeptName = row["bmname"].ToString(),
                SeqId = row["xlid"] as int?,
                SeqName = row["xlname"].ToString(),
                JobId = row["gwid"] as int?,
                JobName = row["gwname"].ToString(),
                LevelId = row["cjid"] as int?,
                LevelName = row["cjname"].ToString(),
                PersonType = row["renyuanlb"].ToString(),

                // 日期
                WorkStart = ConvertToDate(row["gongzuosj"]),
                JoinDate = ConvertToDate(row["rusisj"]),
                PostDate = ConvertToDate(row["gangweisj"]),
                ResignDate = ConvertToDate(row["lizhisj"]),

                // 学历技能
                Education = row["xueli"].ToString(),
                Degree = row["xuewei"].ToString(),
                TechSpecialty = row["zhuanyejs"].ToString(),
                TitleLevel = row["zhichengdj"].ToString(),
                //TitleDate = row["zhichengsj"] as DateTime?,
                TitleDate = ConvertToDate(row["zhichengsj"]),
                Skill = row["zhuanyejn"].ToString(),
                //SkillDate = row["jinengsj"] as DateTime?,
                SkillDate = ConvertToDate(row["jinengsj"]),

                // 联系
                Phone = row["lianxidh"].ToString(),
                BankCard = row["gongzikh"].ToString(), // 此时已解密
                HujiAddr = row["hujidz"].ToString(),
                CurrentAddr = row["xianzhuzhi"].ToString(),

                // 状态标记
                IsProbation = row["shiyong"] != DBNull.Value && Convert.ToBoolean(row["shiyong"]),
                IsFreshGraduate = row["yjbys"] != DBNull.Value && Convert.ToBoolean(row["yjbys"])
            };
        }

        /// <summary>
        /// 获取配置表数据的通用方法 (返回ComboItem列表，用于下拉框绑定)
        /// </summary>
        /// <param name="tableName">配置表名称</param>
        /// <returns>ComboItem对象列表</returns>
        public List<ComboItem> GetComboList(string tableName) {
            DataTable dt = GetDictData(tableName);
            List<ComboItem> list = new List<ComboItem>();

            foreach (DataRow row in dt.Rows) {
                list.Add(new ComboItem {
                    // 确保转换安全
                    Id = row["id"] != DBNull.Value ? Convert.ToInt32(row["id"]) : 0,
                    Name = row["name"] != DBNull.Value ? row["name"].ToString() : ""
                });
            }
            return list;
        }

        /// <summary>
        /// 根据序列ID获取部门列表 (用于联动下拉框)
        /// </summary>
        /// <param name="seqId">序列ID，如果为null则返回所有部门</param>
        /// <returns>ComboItem对象列表</returns>
        public List<ComboItem> GetDeptListBySeq(int? seqId) {
            string sql;
            if (seqId.HasValue && seqId.Value > 0) {
                sql = $"SELECT id, bmname AS name FROM ZX_config_bm WHERE IsEnabled=1 AND DeleteType=0 AND xlid={seqId.Value} ORDER BY DisplayOrder";
            } else {
                sql = "SELECT id, bmname AS name FROM ZX_config_bm WHERE IsEnabled=1 AND DeleteType=0 ORDER BY DisplayOrder";
            }

            DataTable dt = SqlHelper.ExecuteDataTable(sql);
            List<ComboItem> list = new List<ComboItem>();
            foreach (DataRow row in dt.Rows) {
                list.Add(new ComboItem {
                    Id = row["id"] != DBNull.Value ? Convert.ToInt32(row["id"]) : 0,
                    Name = row["name"] != DBNull.Value ? row["name"].ToString() : ""
                });
            }
            return list;
        }

        /// <summary>
        /// 辅助方法：将数据库对象转换为可空DateTime类型
        /// 自动处理DBNull和1900-01-01无效日期
        /// </summary>
        /// <param name="obj">数据库中的日期对象</param>
        /// <returns>转换后的可空DateTime，无效日期返回null</returns>
        private DateTime? ConvertToDate(object obj) {
            if (obj == null || obj == DBNull.Value) return null;

            DateTime dt;
            // 1. 如果已经是 DateTime 类型（来自 datetime 列）
            if (obj is DateTime) {
                dt = (DateTime)obj;
            }
            // 2. 如果是字符串（来自 varchar 列），尝试解析
            else if (!DateTime.TryParse(obj.ToString(), out dt)) {
                return null;
            }

            // 3. 核心逻辑：如果是 1900-01-01，视为无效，返回 null
            if (dt.Year == 1900 && dt.Month == 1 && dt.Day == 1) {
                return null;
            }

            return dt;
        }

        /// <summary>
        /// 撤回变动记录
        /// 仅允许撤回指定员工的最新一条未删除记录
        /// </summary>
        /// <param name="changeId">选中的变动记录ID</param>
        /// <param name="empId">员工ID</param>
        /// <param name="empName">员工姓名（用于提示信息）</param>
        public void RevokeChange(int changeId, int empId, string empName) {
            // 1. 查询该员工最新一条未删除的变动记录
            string checkSql = @"
                SELECT TOP 1 id FROM ZX_yuangong_change 
                WHERE ygid = @EmpId AND (DeleteType = 0 OR DeleteType IS NULL)
                ORDER BY id DESC";
            object latestIdObj = SqlHelper.ExecuteScalar(checkSql, new SqlParameter("@EmpId", empId));

            if (latestIdObj == null) {
                throw new Exception("该员工没有可撤销的变动记录！");
            }

            int latestId = Convert.ToInt32(latestIdObj);
            if (latestId != changeId) {
                throw new Exception($"仅可撤销{empName}的最新变动记录！");
            }

            // 2. 获取该变动记录详情
            string detailSql = @"
                SELECT * FROM ZX_yuangong_change WHERE id = @Id";
            DataTable dtChange = SqlHelper.ExecuteDataTable(detailSql, new SqlParameter("@Id", changeId));
            if (dtChange.Rows.Count == 0) {
                throw new Exception("变动记录不存在！");
            }
            DataRow row = dtChange.Rows[0];

            bool isEffect = row["IsEffect"] != DBNull.Value && Convert.ToBoolean(row["IsEffect"]);
            string changeType = row["changeType"].ToString();

            using (SqlConnection conn = new SqlConnection(SqlHelper.ConnString)) {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction()) {
                    try {
                        // 3. 如果已生效，需要回滚员工表
                        if (isEffect) {
                            string rollbackSql;
                            if (changeType == "离职") {
                                // 离职撤回：恢复在职状态和原组织信息
                                rollbackSql = @"
                                    UPDATE ZX_config_yg 
                                    SET bmid = @OldBmId, xlid = @OldXlId, 
                                        gwid = @OldZwId, cjid = @OldCjId,
                                        zaizhi = 1, lizhisj = NULL
                                    WHERE id = @EmpId";
                            } else {
                                // 普通变动撤回：恢复原组织信息
                                rollbackSql = @"
                                    UPDATE ZX_config_yg 
                                    SET bmid = @OldBmId, xlid = @OldXlId, 
                                        gwid = @OldZwId, cjid = @OldCjId
                                    WHERE id = @EmpId";
                            }

                            SqlHelper.ExecuteNonQuery(trans, rollbackSql,
                                new SqlParameter("@OldBmId", row["oldbmid"] != DBNull.Value ? row["oldbmid"] : (object)DBNull.Value),
                                new SqlParameter("@OldXlId", row["oldxlid"] != DBNull.Value ? row["oldxlid"] : (object)DBNull.Value),
                                new SqlParameter("@OldZwId", row["oldzwid"] != DBNull.Value ? row["oldzwid"] : (object)DBNull.Value),
                                new SqlParameter("@OldCjId", row["oldcjid"] != DBNull.Value ? row["oldcjid"] : (object)DBNull.Value),
                                new SqlParameter("@EmpId", empId)
                            );
                        }

                        // 4. 标记变动记录为已撤销
                        string revokeSql = @"
                            UPDATE ZX_yuangong_change SET DeleteType = 1 WHERE id = @Id";
                        SqlHelper.ExecuteNonQuery(trans, revokeSql, new SqlParameter("@Id", changeId));

                        trans.Commit();
                    } catch {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// 检查并应用待生效的变动记录
        /// 查找 IsEffect=0 且 DeleteType=0 且 changeTime <= 当前数据库日期 的记录，逐条生效
        /// </summary>
        public void ApplyPendingChanges() {
            // 查找到期的待生效记录
            string querySql = @"
                SELECT * FROM ZX_yuangong_change 
                WHERE (IsEffect = 0 OR IsEffect IS NULL)
                  AND (DeleteType = 0 OR DeleteType IS NULL)
                  AND CAST(changeTime AS DATE) <= CAST(GETDATE() AS DATE)
                ORDER BY id ASC";

            DataTable dt = SqlHelper.ExecuteDataTable(querySql);
            if (dt.Rows.Count == 0) return;

            foreach (DataRow row in dt.Rows) {
                int changeId = Convert.ToInt32(row["id"]);
                int empId = Convert.ToInt32(row["ygid"]);
                string changeType = row["changeType"].ToString();

                using (SqlConnection conn = new SqlConnection(SqlHelper.ConnString)) {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction()) {
                        try {
                            // 1. 更新员工表
                            string updateSql = @"
                                UPDATE ZX_config_yg 
                                SET bmid = @Bm, xlid = @Xl, gwid = @Zw, cjid = @Cj, newChange = 1
                                WHERE id = @Id";

                            if (changeType == "离职") {
                                updateSql = @"
                                    UPDATE ZX_config_yg 
                                    SET bmid = @Bm, xlid = @Xl, gwid = @Zw, cjid = @Cj,
                                        zaizhi = 0, lizhisj = @Time, newChange = 1
                                    WHERE id = @Id";
                            }

                            SqlHelper.ExecuteNonQuery(trans, updateSql,
                                new SqlParameter("@Bm", row["newbmid"] != DBNull.Value ? row["newbmid"] : (object)DBNull.Value),
                                new SqlParameter("@Xl", row["newxlid"] != DBNull.Value ? row["newxlid"] : (object)DBNull.Value),
                                new SqlParameter("@Zw", row["newzwid"] != DBNull.Value ? row["newzwid"] : (object)DBNull.Value),
                                new SqlParameter("@Cj", row["newcjid"] != DBNull.Value ? row["newcjid"] : (object)DBNull.Value),
                                new SqlParameter("@Time", row["changeTime"]),
                                new SqlParameter("@Id", empId)
                            );

                            // 2. 标记为已生效
                            string effectSql = @"
                                UPDATE ZX_yuangong_change SET IsEffect = 1 WHERE id = @CgId";
                            SqlHelper.ExecuteNonQuery(trans, effectSql, new SqlParameter("@CgId", changeId));

                            trans.Commit();
                        } catch {
                            trans.Rollback();
                            // 单条失败不影响其他记录继续处理
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取员工表当前最大ID
        /// </summary>
        /// <returns>当前最大ID，如果表为空则返回0</returns>
        public int GetMaxId() {
            string sql = "SELECT ISNULL(MAX(id), 0) FROM ZX_config_yg";
            object result = SqlHelper.ExecuteScalar(sql);
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// 获取员工表当前最大序号
        /// </summary>
        /// <returns>当前最大序号，如果表为空则返回0</returns>
        public int GetMaxXuhao() {
            string sql = "SELECT ISNULL(MAX(xuhao), 0) FROM ZX_config_yg";
            object result = SqlHelper.ExecuteScalar(sql);
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// 插入新员工
        /// 自动生成新ID和序号，并设置默认字段值
        /// </summary>
        /// <param name="emp">员工详细信息对象</param>
        public void InsertEmployee(EmpDetail emp) {
            // 1. 获取新ID和序号
            int newId = GetMaxId() + 1;
            int newXuhao = GetMaxXuhao() + 1;

            // 2. 加密敏感字段
            string encIdCard = "";
            if (!string.IsNullOrEmpty(emp.IdCard)) {
                encIdCard = _sm4.Encrypt_ECB_Str(emp.IdCard.Trim());
            }
            string encBankCard = "";
            if (!string.IsNullOrEmpty(emp.BankCard)) {
                encBankCard = _sm4.Encrypt_ECB_Str(emp.BankCard.Trim());
            }

            // 3. 构建INSERT SQL (包含默认字段)
            string sql = @"
        INSERT INTO ZX_config_yg (
            id, yuangongbh, xingming, xingbie, minzu, zhengzhimm,
            chushengrq, nianling, shuxing, shenfenzheng,
            bmid, xlid, gwid, cjid,
            gongzuosj, rusisj, gangweisj,
            xueli, xuewei, zhuanyejs, zhichengdj, zhichengsj, zhuanyejn, jinengsj,
            lianxidh, gongzikh,
            shiyong, yjbys,
            zaizhi, zaigang, fanpin, jiediao, kaoqin, xinchou, xuhao
        ) VALUES (
            @Id, @EmpNo, @Name, @Gender, @Nation, @Politic,
            @Birthday, @Age, @Zodiac, @IdCard,
            @DeptId, @SeqId, @JobId, @LevelId,
            @WorkStart, @JoinDate, @PostDate,
            @Education, @Degree, @Tech, @TitleLevel, @TitleDate, @Skill, @SkillDate,
            @Phone, @BankCard,
            @IsProbation, @IsFreshGraduate,
            1, 1, 0, 0, 1, 1, @Xuhao
        )";

            // 4. 执行插入
            SqlHelper.ExecuteNonQuery(sql,
                new SqlParameter("@Id", newId),
                new SqlParameter("@EmpNo", emp.EmployeeNo ?? ""),
                new SqlParameter("@Name", emp.Name ?? ""),
                new SqlParameter("@Gender", emp.Gender ?? ""),
                new SqlParameter("@Nation", emp.Nation ?? ""),
                new SqlParameter("@Politic", emp.Politic ?? ""),
                new SqlParameter("@Birthday", emp.Birthday ?? (object)DBNull.Value),
                new SqlParameter("@Age", emp.Age),
                new SqlParameter("@Zodiac", emp.Zodiac ?? ""),
                new SqlParameter("@IdCard", encIdCard),

                new SqlParameter("@DeptId", emp.DeptId ?? (object)DBNull.Value),
                new SqlParameter("@SeqId", emp.SeqId ?? (object)DBNull.Value),
                new SqlParameter("@JobId", emp.JobId ?? (object)DBNull.Value),
                new SqlParameter("@LevelId", emp.LevelId ?? (object)DBNull.Value),

                new SqlParameter("@WorkStart", emp.WorkStart ?? (object)DBNull.Value),
                new SqlParameter("@JoinDate", emp.JoinDate ?? (object)DBNull.Value),
                new SqlParameter("@PostDate", emp.PostDate ?? (object)DBNull.Value),

                new SqlParameter("@Education", emp.Education ?? ""),
                new SqlParameter("@Degree", emp.Degree ?? ""),
                new SqlParameter("@Tech", emp.TechSpecialty ?? ""),
                new SqlParameter("@TitleLevel", emp.TitleLevel ?? ""),
                new SqlParameter("@TitleDate", emp.TitleDate.HasValue ? emp.TitleDate.Value.ToString("yyyy-MM-dd") : (object)DBNull.Value),
                new SqlParameter("@Skill", emp.Skill ?? ""),
                new SqlParameter("@SkillDate", emp.SkillDate.HasValue ? emp.SkillDate.Value.ToString("yyyy-MM-dd") : (object)DBNull.Value),

                new SqlParameter("@Phone", emp.Phone ?? ""),
                new SqlParameter("@BankCard", encBankCard),

                new SqlParameter("@IsProbation", emp.IsProbation ? 1 : 0),
                new SqlParameter("@IsFreshGraduate", emp.IsFreshGraduate ? 1 : 0),
                new SqlParameter("@Xuhao", newXuhao)
            );
        }
        /// <summary>
        /// 批量更新员工序号 (用于拖拽排序后保存)
        /// </summary>
        /// <param name="orderList">员工ID和新序号的列表</param>
        public void UpdateEmpOrder(List<KeyValuePair<int, int>> orderList) {
            using (SqlConnection conn = new SqlConnection(SqlHelper.ConnString)) {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction()) {
                    try {
                        string sql = "UPDATE ZX_config_yg SET xuhao = @Xuhao WHERE id = @Id";
                        foreach (var item in orderList) {
                            SqlHelper.ExecuteNonQuery(trans, sql,
                                new SqlParameter("@Id", item.Key),
                                new SqlParameter("@Xuhao", item.Value)
                            );
                        }
                        trans.Commit();
                    } catch {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }
        //
    }
}