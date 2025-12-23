using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace CompensationSystemSubInterface.Common {

    /// <summary>
    /// 日志管理器，提供调试输出和文件日志记录功能
    /// </summary>
    public static class LogManager {
        /// <summary>
        /// 调试信息的前缀标识
        /// </summary>
        internal const string Prefix = "[Debug Info] >>> ";

        /// <summary>
        /// 日志文件存储目录路径
        /// 存储在用户的应用数据目录：...\AppData\Roaming\PaySys\Logs\
        /// </summary>
        private static readonly string LogDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "PaySys",
            "Logs"
        );

        /// <summary>
        /// 当前日志文件的完整路径
        /// 格式：ps_yyyyMMdd.log（如 ps_20251114.log）
        /// 注意: CleanOldLogs 删除日志的时候需要与日志名格式匹配
        /// </summary>
        private static readonly string LogFilePath = Path.Combine(
            LogDirectory,
            $"ps_{DateTime.Now:yyyyMMdd}.log"
        );

        /// <summary>
        /// 文件写入操作的锁对象，确保线程安全
        /// </summary>
        private static readonly object FileLock = new object();

        /// <summary>
        /// 静态构造函数：确保日志目录存在
        /// </summary>
        static LogManager() {
            try {
                if (!Directory.Exists(LogDirectory)) {
                    Directory.CreateDirectory(LogDirectory);
                }
            } catch (Exception ex) {
                Debug.WriteLine($"{Prefix}无法创建日志目录: {ex.Message}");
            }
        }

        /// <summary>
        /// 调试信息（仅输出到 Debug 窗口）
        /// </summary>
        /// <param name="message">要输出的调试消息</param>
        public static void Print(string message) {
            Debug.WriteLine(Prefix + message);
        }

        /// <summary>
        /// 记录关键日志到本地文件（同时输出到 Debug 窗口）
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="level">日志级别（INFO/WARN/ERROR）</param>
        public static void Info(string message, string level = "INFO") {
            try {
                // 构建日志条目
                string logEntry = FormatLogEntry(message, level);

                // 输出到 Debug 窗口
                Debug.WriteLine(logEntry);

                // 写入文件（线程安全）
                lock (FileLock) {
                    File.AppendAllText(LogFilePath, logEntry + Environment.NewLine, Encoding.UTF8);
                }
            } catch (Exception ex) {
                Debug.WriteLine($"{Prefix}日志写入失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 记录错误日志到文件
        /// </summary>
        /// <param name="message">错误信息</param>
        public static void Error(string message) {
            Info(message, "ERROR");
        }

        /// <summary>
        /// 记录错误日志到文件（包含异常堆栈）
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <param name="ex">异常对象</param>
        public static void Error(string message, Exception ex) {
            string fullMessage = $"{message}\n异常详情: {ex}";
            Info(fullMessage, "ERROR");
        }

        /// <summary>
        /// 记录警告日志到文件
        /// </summary>
        /// <param name="message">警告信息</param>
        public static void Warn(string message) {
            Info(message, "WARN");
        }

        /// <summary>
        /// 格式化日志条目
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="level">日志级别</param>
        /// <returns>格式化后的日志条目字符串</returns>
        private static string FormatLogEntry(string message, string level) {
            return $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
        }

        /// <summary>
        /// 清理超过指定天数的旧日志文件
        /// </summary>
        /// <param name="daysToKeep">保留的天数（默认7天）</param>
        public static void CleanOldLogs(int daysToKeep = 7) {
            try {
                if (!Directory.Exists(LogDirectory)) return;

                var files = Directory.GetFiles(LogDirectory, "ps_*.log");
                var cutoffDate = DateTime.Now.AddDays(-daysToKeep);

                foreach (var file in files) {
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.LastWriteTime < cutoffDate) {
                        File.Delete(file);
                        Debug.WriteLine($"{Prefix}已删除旧日志: {fileInfo.Name}");
                    }
                }
            } catch (Exception ex) {
                Debug.WriteLine($"{Prefix}清理旧日志失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取当前日志文件路径
        /// </summary>
        /// <returns>当前日志文件的完整路径</returns>
        public static string GetLogFilePath() {
            return LogFilePath;
        }
    }
}
