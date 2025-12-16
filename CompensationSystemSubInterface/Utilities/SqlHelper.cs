using System;
using System.Collections.Generic;
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
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public static readonly string ConnString =
            "Data Source=192.168.100.16;Initial Catalog=jzcw_t;User ID=PEPTest;Password=Test1511*;";

        /// <summary>
        /// 执行 SQL 查询并返回 DataTable
        /// </summary>
        /// <param name="sql">要执行的 SQL 语句</param>
        /// <param name="parameters">SQL 参数数组，可选</param>
        /// <returns>查询结果的 DataTable</returns>
        /// <exception cref="Exception">当数据库操作失败时抛出异常</exception>
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
                        throw new Exception("数据库错误: " + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 执行 SQL 查询并返回结果集中第一行第一列的值
        /// </summary>
        /// <param name="sql">要执行的 SQL 语句</param>
        /// <param name="parameters">SQL 参数数组，可选</param>
        /// <returns>查询结果的第一行第一列的值；如果结果集为空则返回 null</returns>
        /// <exception cref="Exception">当数据库操作失败时抛出异常</exception>
        public static object ExecuteScalar(string sql, params SqlParameter[] parameters) {
            using (SqlConnection conn = new SqlConnection(ConnString)) {
                using (SqlCommand cmd = new SqlCommand(sql, conn)) {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    try {
                        conn.Open();
                        return cmd.ExecuteScalar();
                    } catch (Exception ex) {
                        throw new Exception("数据库错误: " + ex.Message);
                    }
                }
            }
        }
        // 2. 【新增】支持事务的 ExecuteNonQuery
        public static int ExecuteNonQuery(string sql, params SqlParameter[] parameters) {
            using (SqlConnection conn = new SqlConnection(ConnString)) {
                using (SqlCommand cmd = new SqlCommand(sql, conn)) {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        // 3. 【新增】重载方法：接收外部传入的 Transaction (用于 Service 层控制事务)
        public static int ExecuteNonQuery(SqlTransaction trans, string sql, params SqlParameter[] parameters) {
            // 注意：cmd 必须绑定 Connection 和 Transaction
            using (SqlCommand cmd = new SqlCommand(sql, trans.Connection, trans)) {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteNonQuery();
            }
        }//
    }
}
