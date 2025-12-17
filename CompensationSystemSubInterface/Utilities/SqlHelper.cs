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
    /// </summary>
    public class SqlHelper {
        // 暂时直接写死，等以后集成到主程序（EXE）里再换回 ConfigurationManager
        public static readonly string ConnString =
            "Data Source=192.168.100.16;Initial Catalog=jzcw_t;User ID=PEPTest;Password=Test1511*;";
        // 1. 从配置文件读取连接字符串，避免硬编码
        //public static readonly string ConnString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString
        //                                           ?? throw new Exception("未在配置文件中找到名为 DefaultConnection 的连接字符串");

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
                        // 2. 保留原始异常堆栈
                        throw new Exception("ExecuteDataTable error: " + ex.Message, ex);
                    }
                }
            }
        }

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

        // 不带事务
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

        // 带事务 (注意：这里不 try-catch，让 Service 层控制事务的回滚)
        public static int ExecuteNonQuery(SqlTransaction trans, string sql, params SqlParameter[] parameters) {
            // 3. 检查事务是否有效
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
