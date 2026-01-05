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
                    NULLIF(yg.chushengrq, '1900-01-01') AS '出生日期',
                    NULLIF(yg.gongzuosj, '1900-01-01') AS '参加工作时间',
                    NULLIF(yg.rusisj, '1900-01-01') AS '入社时间',
                    NULLIF(yg.gangweisj, '1900-01-01') AS '任现岗位时间',
                    yg.shenfenzheng AS '证件号码',
                    yg.zhuanyejs AS '专业技术',
                    yg.zhichengdj AS '职称等级',
                    NULLIF ( yg.zhichengsj, '1900-01-01' ) AS '取得时间',
                    yg.zhuanyejn AS '专业技能',
                    NULLIF(yg.jinengsj, '1900-01-01') AS '技能时间',
                    yg.lianxidh AS '联系电话',
                    yg.nianling AS '年龄',
                    yg.renyuanlb AS '人员类别',
                    yg.shuxing AS '属相',
                    yg.hunyinzk AS '婚姻状况',
                    NULLIF(yg.qishisfzrq, '1900-01-01') AS '身份证起始',
                    NULLIF(yg.jieshusfzrq, '1900-01-01') AS '身份证截止',
                    yg.hujidz AS '户籍地址',
                    yg.xianzhuzhi AS '现住址',
                    yg.gongzikh AS '工资卡号',
                    NULLIF(yg.lizhisj, '1900-01-01') AS '离职日期',
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
                    yg.lianxidh AS '联系电话',
                    yg.nianling AS '年龄',
                    yg.renyuanlb AS '人员类别',
                    yg.shuxing AS '属相',
                    yg.hunyinzk AS '婚姻状况',
                    NULLIF(yg.qishisfzrq, '1900-01-01') AS '身份证起始',
                    NULLIF(yg.jieshusfzrq, '1900-01-01') AS '身份证截止',
                    yg.hujidz AS '户籍地址',
                    yg.xianzhuzhi AS '现住址',
                    yg.gongzikh AS '工资卡号',
                    NULLIF(yg.lizhisj, '1900-01-01') AS '离职日期',
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
                    yg.yuangongbh AS '员工号',
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
                    yg.zaizhi = 1 
                    AND cg.changeTime BETWEEN @StartDate AND @EndDate
            ");

            // 参数列表
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
            hunyinzk = @Marital,
            shuxing = @Zodiac,
            nianling = @Age,
            chushengrq = @Birthday,
            
            hujidz = @HujiAddr,
            xianzhuzhi = @CurrentAddr,
            lianxidh = @Phone,
            
            qishisfzrq = @IdStart,
            jieshusfzrq = @IdEnd,

            xueli = @Education,
            xuewei = @Degree,
            renyuanlb = @PersonType,
            
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
            gongzikh = @BankCard
        WHERE id = @Id";

            // 3. 执行更新 (补全参数)
            SqlHelper.ExecuteNonQuery(sql,
                new SqlParameter("@Id", emp.Id),
                new SqlParameter("@Name", emp.Name ?? ""),
                new SqlParameter("@Gender", emp.Gender ?? ""),
                new SqlParameter("@Nation", emp.Nation ?? ""),
                new SqlParameter("@Politic", emp.Politic ?? ""),
                new SqlParameter("@Marital", emp.Marital ?? ""),
                new SqlParameter("@Zodiac", emp.Zodiac ?? ""),
                new SqlParameter("@Age", emp.Age), // int类型
                new SqlParameter("@Birthday", emp.Birthday ?? (object)DBNull.Value), // DateTime?

                new SqlParameter("@HujiAddr", emp.HujiAddr ?? ""),
                new SqlParameter("@CurrentAddr", emp.CurrentAddr ?? ""),
                new SqlParameter("@Phone", emp.Phone ?? ""),

                new SqlParameter("@IdStart", emp.IdStart ?? (object)DBNull.Value),
                new SqlParameter("@IdEnd", emp.IdEnd ?? (object)DBNull.Value),

                new SqlParameter("@Education", emp.Education ?? ""),
                new SqlParameter("@Degree", emp.Degree ?? ""),
                new SqlParameter("@PersonType", emp.PersonType ?? ""),

                new SqlParameter("@WorkStart", emp.WorkStart ?? (object)DBNull.Value),
                new SqlParameter("@JoinDate", emp.JoinDate ?? (object)DBNull.Value),
                new SqlParameter("@PostDate", emp.PostDate ?? (object)DBNull.Value),
                new SqlParameter("@ResignDate", emp.ResignDate ?? (object)DBNull.Value),

                new SqlParameter("@Tech", emp.TechSpecialty ?? ""),
                new SqlParameter("@TitleLevel", emp.TitleLevel ?? ""),
                //new SqlParameter("@TitleDate", emp.TitleDate ?? (object)DBNull.Value), // 数据库已改datetime，直接传
                new SqlParameter("@TitleDate", emp.TitleDate.HasValue ? emp.TitleDate.Value.ToString("yyyy-MM-dd") : (object)DBNull.Value),
                new SqlParameter("@Skill", emp.Skill ?? ""),
                //new SqlParameter("@SkillDate", emp.SkillDate ?? (object)DBNull.Value),
                new SqlParameter("@SkillDate", emp.SkillDate.HasValue ? emp.SkillDate.Value.ToString("yyyy-MM-dd") : (object)DBNull.Value),

                new SqlParameter("@IdCard", encIdCard),
                new SqlParameter("@BankCard", encBankCard)
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
                        // 1. 插入变动记录 (补全 Old ID 和 New Name)
                        string insertSql = @"
                    INSERT INTO ZX_yuangong_change 
                    (ygid, changeType, changeTime, 
                        oldbmid, oldbm, oldxlid, oldxl, oldzwid, oldzw, oldcjid, oldcj, 
                        newbmid, newbm, newxlid, newxl, newzwid, newzw, newcjid, newcj, 
                        wageStart, wageEnd)
                    VALUES 
                    (@YgId, @Type, @Time, 
                        @OldBmId, @OldBm, @OldXlId, @OldXl, @OldZwId, @OldZw, @OldCjId, @OldCj, 
                        @NewBmId, @NewBm, @NewXlId, @NewXl, @NewZwId, @NewZw, @NewCjId, @NewCj, 
                        @WageStart, @WageEnd)";

                        SqlHelper.ExecuteNonQuery(trans, insertSql,
                            new SqlParameter("@YgId", empId),
                            new SqlParameter("@Type", changeType),
                            new SqlParameter("@Time", changeTime),

                            // --- 原信息 (ID + Name) ---
                            // 注意：ID可能是null，需要处理 DBNull
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
                            new SqlParameter("@WageEnd", wageEnd ?? (object)DBNull.Value)
                        );

                        // 2. 更新员工主表 (逻辑不变) 增加 newChange = 1
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
                //WorkStart = row["gongzuosj"] as DateTime?,
                //JoinDate = row["rusisj"] as DateTime?,
                //PostDate = row["gangweisj"] as DateTime?,
                //ResignDate = row["lizhisj"] as DateTime?,

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
                CurrentAddr = row["xianzhuzhi"].ToString()
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
        //
    }
}