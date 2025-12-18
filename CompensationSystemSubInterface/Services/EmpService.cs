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
        /// 实例化加密对象
        /// </summary>
        private Sm4Crypto _sm4 = new Sm4Crypto();


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
        /// 查询员工详细信息
        /// </summary>
        /// <param name="keyword">关键字搜索（姓名）</param>
        /// <param name="cond">高级筛选条件（部门、员工）</param>
        /// <returns>员工信息数据表</returns>
        public DataTable GetEmpMaintData(string keyword, EmpMaintCondition cond) {
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

        /// <summary>
        /// 获取单个员工的详细信息（用于修改界面回显）
        /// </summary>
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
        /// 获取基础配置数据（用于下拉框）
        /// 根据不同表名应用不同的筛选条件和排序
        /// </summary>
        /// <param name="tableName">表名：ZX_config_bm, ZX_config_xl, ZX_config_gw, ZX_config_cj</param>
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
        /// 更新员工信息（包含加密逻辑）
        /// </summary>
        public void UpdateEmpBasicInfo(EmployeeDetail emp) {
            // 1. 加密敏感字段
            // 注意：如果输入为空，则存入空字符串或处理为 DBNull，视数据库约束而定
            string encIdCard = "";
            if (!string.IsNullOrEmpty(emp.IdCard)) {
                encIdCard = _sm4.Encrypt_ECB_Str(emp.IdCard.Trim());
            }

            string encBankCard = "";
            if (!string.IsNullOrEmpty(emp.BankCard)) {
                encBankCard = _sm4.Encrypt_ECB_Str(emp.BankCard.Trim());
            }

            // 2. 构建 SQL
            // 这里的字段名必须与数据库列名完全一致
            string sql = @"
                UPDATE ZX_config_yg 
                SET 
                    xingming = @Name,
                    xingbie = @Gender,
                    minzu = @Nation,
                    zhengzhimm = @Politic,
                    hunyinzk = @Marital,
                    shuxing = @Zodiac,
                    hujidz = @HujiAddr,
                    xianzhuzhi = @CurrentAddr,
                    lianxidh = @Phone,
                    
                    xueli = @Education,
                    xuewei = @Degree,
                    renyuanlb = @PersonType,
                    zhuanyejs = @Tech,
                    zhichengdj = @TitleLevel,
                    zhuanyejn = @Skill,

                    shenfenzheng = @IdCard, -- 加密后
                    gongzikh = @BankCard    -- 加密后
                WHERE id = @Id";

            // 3. 执行更新
            SqlHelper.ExecuteNonQuery(sql,
                new SqlParameter("@Id", emp.Id),
                new SqlParameter("@Name", emp.Name ?? ""),
                new SqlParameter("@Gender", emp.Gender ?? ""),
                new SqlParameter("@Nation", emp.Nation ?? ""),
                new SqlParameter("@Politic", emp.Politic ?? ""),
                new SqlParameter("@Marital", emp.Marital ?? ""),
                new SqlParameter("@Zodiac", emp.Zodiac ?? ""),
                new SqlParameter("@HujiAddr", emp.HujiAddr ?? ""),
                new SqlParameter("@CurrentAddr", emp.CurrentAddr ?? ""),
                new SqlParameter("@Phone", emp.Phone ?? ""),

                new SqlParameter("@Education", emp.Education ?? ""),
                new SqlParameter("@Degree", emp.Degree ?? ""),
                new SqlParameter("@PersonType", emp.PersonType ?? ""),
                new SqlParameter("@Tech", emp.TechSpecialty ?? ""),
                new SqlParameter("@TitleLevel", emp.TitleLevel ?? ""),
                new SqlParameter("@Skill", emp.Skill ?? ""),

                // 传入加密后的密文
                new SqlParameter("@IdCard", encIdCard),
                new SqlParameter("@BankCard", encBankCard)
            );
        }

        /// <summary>
        /// 执行员工变动（核心事务：插入变动记录 + 更新员工表）
        /// </summary>
        public void ExecuteEmployeeChange(int empId, string changeType, DateTime changeTime,
                                          int newBm, int newXl, int newZw, int newCj,
                                          DateTime? wageStart, DateTime? wageEnd,
                                          string oldBmName, string oldXlName, string oldZwName, string oldCjName) {
            // 开启事务处理
            using (SqlConnection conn = new SqlConnection(SqlHelper.ConnString)) { // 需将 ConnString 在 SqlHelper 中改为 public 或提供获取方法
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction()) {
                    try {
                        // 1. 获取新部门名称等文本（为了存入变动表的文本字段）
                        // 这里为了简化，假设前端传进来或者在存储过程中查，这里演示先获取ID
                        // 实际开发中，ZX_yuangong_change 表最好存 ID，存文本是历史遗留设计

                        // 2. 插入变动记录
                        string insertSql = @"
                            INSERT INTO ZX_yuangong_change 
                            (ygid, changeType, changeTime, 
                             oldbm, oldxl, oldzw, oldcj, 
                             newbmid, newxlid, newzwid, newcjid, 
                             wageStart, wageEnd, CreateTime)
                            VALUES 
                            (@YgId, @Type, @Time, 
                             @OldBm, @OldXl, @OldZw, @OldCj, 
                             @NewBmId, @NewXlId, @NewZwId, @NewCjId, 
                             @WageStart, @WageEnd, GETDATE())";

                        // 注意：这里需要根据 newId 反查出 newName 存入 newbm 字段，或者表结构里有 newbmid
                        // 假设你的表结构同时有 newbm (文本) 和 newbmid (ID)
                        // 为简化代码，这里省略根据ID查Name的步骤，实际项目中需要补全

                        SqlHelper.ExecuteNonQuery(trans, insertSql,
                            new SqlParameter("@YgId", empId),
                            new SqlParameter("@Type", changeType),
                            new SqlParameter("@Time", changeTime),
                            new SqlParameter("@OldBm", oldBmName),
                            new SqlParameter("@OldXl", oldXlName),
                            new SqlParameter("@OldZw", oldZwName),
                            new SqlParameter("@OldCj", oldCjName),
                            new SqlParameter("@NewBmId", newBm),
                            new SqlParameter("@NewXlId", newXl),
                            new SqlParameter("@NewZwId", newZw),
                            new SqlParameter("@NewCjId", newCj),
                            new SqlParameter("@WageStart", wageStart ?? (object)DBNull.Value),
                            new SqlParameter("@WageEnd", wageEnd ?? (object)DBNull.Value)
                        );

                        // 3. 更新员工主表
                        string updateSql = @"
                            UPDATE ZX_config_yg 
                            SET bmid=@Bm, xlid=@Xl, gwid=@Zw, cjid=@Cj 
                            WHERE id=@Id";

                        if (changeType == "离职") {
                            // 如果是离职，还需要更新 zaizhi 状态和离职时间
                            updateSql = @"
                                UPDATE ZX_config_yg 
                                SET bmid=@Bm, xlid=@Xl, gwid=@Zw, cjid=@Cj, 
                                    zaizhi=0, lizhisj=@Time 
                                WHERE id=@Id";
                        }

                        SqlHelper.ExecuteNonQuery(trans, updateSql,
                            new SqlParameter("@Bm", newBm),
                            new SqlParameter("@Xl", newXl),
                            new SqlParameter("@Zw", newZw),
                            new SqlParameter("@Cj", newCj),
                            new SqlParameter("@Time", changeTime), // 仅离职用到
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

        public EmployeeDetail GetEmpDetailObj(int empId) {
            DataRow row = GetEmpDetail(empId); // 调用你之前写好的 SQL 方法
            if (row == null) return null;

            // 手动映射，或者使用 AutoMapper
            return new EmployeeDetail {
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
                Birthday = row["chushengrq"] as DateTime?,
                IdCard = row["shenfenzheng"].ToString(), // 此时已解密
                IdStart = row["qishisfzrq"] as DateTime?,
                IdEnd = row["jieshusfzrq"] as DateTime?,

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
                WorkStart = row["gongzuosj"] as DateTime?,
                JoinDate = row["rusisj"] as DateTime?,
                PostDate = row["gangweisj"] as DateTime?,
                ResignDate = row["lizhisj"] as DateTime?,

                // 学历技能
                Education = row["xueli"].ToString(),
                Degree = row["xuewei"].ToString(),
                TechSpecialty = row["zhuanyejs"].ToString(),
                TitleLevel = row["zhichengdj"].ToString(),
                TitleDate = row["zhichengsj"] as DateTime?,
                Skill = row["zhuanyejn"].ToString(),
                SkillDate = row["jinengsj"] as DateTime?,

                // 联系
                Phone = row["lianxidh"].ToString(),
                BankCard = row["gongzikh"].ToString(), // 此时已解密
                HujiAddr = row["hujidz"].ToString(),
                CurrentAddr = row["xianzhuzhi"].ToString()
            };
        }

        // 获取配置表数据的通用方法
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
        //
    }
}