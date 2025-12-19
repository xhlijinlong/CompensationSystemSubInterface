using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompensationSystemSubInterface.Utilities {
    /// <summary>
    /// SQL Server 数据库访问辅助类
    /// 提供数据库连接和常用操作的封装方法
    /// </summary>
    public class SqlHelper {
        // 0. 暂时直接写死，等以后集成到主程序（EXE）里再换回 ConfigurationManager
        //public static readonly string ConnString =
        //    "Data Source=192.168.100.16;Initial Catalog=jzcw_t;User ID=PEPTest;Password=Test1511*;";

        // 1. 从配置文件读取连接字符串，避免硬编码
        //public static readonly string ConnString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString
        //                                           ?? throw new Exception("未在配置文件中找到名为 DefaultConnection 的连接字符串");

        /// <summary>
        /// 数据库连接字符串的私有字段（默认测试环境地址）
        /// </summary>
        private static string _connString = "Data Source=192.168.100.16;Initial Catalog=jzcw_t;User ID=PEPTest;Password=Test1511*;";
        
        /// <summary>
        /// 数据库连接字符串（公开静态属性，供主程序动态赋值）
        /// 可通过 Set 方法在运行时修改连接字符串，以适应不同环境
        /// </summary>
        public static string ConnString {
            get { return _connString; }
            set { _connString = value; }
        }

        /// <summary>
        /// 执行查询语句，返回数据表
        /// 使用 SqlDataAdapter 填充 DataTable
        /// </summary>
        /// <param name="sql">SQL 查询语句</param>
        /// <param name="parameters">SQL 参数数组（可选）</param>
        /// <returns>查询结果的 DataTable 对象</returns>
        /// <exception cref="Exception">数据库操作失败时抛出异常，包含原始错误信息</exception>
        public static DataTable ExecuteDataTable(string sql, params SqlParameter[] parameters) {
            using (SqlConnection conn = new SqlConnection(ConnString)) {
                using (SqlCommand cmd = new SqlCommand(sql, conn)) {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    try {
                        conn.Open();
                        DataTable dt = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd)) {
                            da.Fill(dt);
                        }
                        return dt;
                    } catch (Exception ex) {
                        // 保留原始异常堆栈
                        throw new Exception("ExecuteDataTable error: " + ex.Message, ex);
                    }
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回结果集中第一行第一列的值
        /// 适用于执行聚合函数（如 COUNT、MAX、MIN 等）或返回单个值的查询
        /// </summary>
        /// <param name="sql">SQL 查询语句</param>
        /// <param name="parameters">SQL 参数数组（可选）</param>
        /// <returns>查询结果的第一行第一列的值，如果结果集为空则返回 null</returns>
        /// <exception cref="Exception">数据库操作失败时抛出异常，包含原始错误信息</exception>
        public static object ExecuteScalar(string sql, params SqlParameter[] parameters) {
            using (SqlConnection conn = new SqlConnection(ConnString)) {
                using (SqlCommand cmd = new SqlCommand(sql, conn)) {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    try {
                        conn.Open();
                        return cmd.ExecuteScalar();
                    } catch (Exception ex) {
                        throw new Exception("ExecuteScalar error: " + ex.Message, ex);
                    }
                }
            }
        }

        /// <summary>
        /// 执行增删改语句（不带事务）
        /// 适用于单个 INSERT、UPDATE 或 DELETE 操作
        /// </summary>
        /// <param name="sql">SQL 操作语句</param>
        /// <param name="parameters">SQL 参数数组（可选）</param>
        /// <returns>受影响的行数</returns>
        /// <exception cref="Exception">数据库操作失败时抛出异常，包含原始错误信息</exception>
        public static int ExecuteNonQuery(string sql, params SqlParameter[] parameters) {
            using (SqlConnection conn = new SqlConnection(ConnString)) {
                using (SqlCommand cmd = new SqlCommand(sql, conn)) {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    try {
                        conn.Open();
                        return cmd.ExecuteNonQuery();
                    } catch (Exception ex) {
                        throw new Exception("ExecuteNonQuery error: " + ex.Message, ex);
                    }
                }
            }
        }

        /// <summary>
        /// 执行增删改语句（带事务）
        /// 在已存在的事务上下文中执行操作，由调用方控制事务的提交或回滚
        /// 注意：此方法不捕获异常，异常会向上抛出，由 Service 层控制事务的回滚
        /// </summary>
        /// <param name="trans">SQL 事务对象，必须是有效且打开的事务</param>
        /// <param name="sql">SQL 操作语句</param>
        /// <param name="parameters">SQL 参数数组（可选）</param>
        /// <returns>受影响的行数</returns>
        /// <exception cref="ArgumentNullException">事务对象为 null 时抛出</exception>
        /// <exception cref="Exception">事务连接已关闭或数据库操作失败时抛出异常</exception>
        public static int ExecuteNonQuery(SqlTransaction trans, string sql, params SqlParameter[] parameters) {
            // 检查事务是否有效
            if (trans == null) throw new ArgumentNullException(nameof(trans));
            if (trans.Connection == null) throw new Exception("事务对应的连接已关闭，无法执行命令。");

            using (SqlCommand cmd = new SqlCommand(sql, trans.Connection, trans)) {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                // 注意：不需要 conn.Open()，因为事务存在的前提是连接已打开
                return cmd.ExecuteNonQuery();
            }
        }
    }
}
