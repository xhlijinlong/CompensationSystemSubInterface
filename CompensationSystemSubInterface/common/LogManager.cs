using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace CompensationSystemSubInterface.Common {

    public static class LogManager {
        internal const string Prefix = "[Debug Info] >>> ";

        // 日志文件路径 存储在用户的应用数据目录
        // ...\AppData\Roaming\PdfTextDiff\Logs\PdfTextDiff_20251114.log
        private static readonly string LogDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "PdfTextDiff",
            "Logs"
        );

        private static readonly string LogFilePath = Path.Combine(
            LogDirectory,
            $"PdfTextDiff_{DateTime.Now:yyyyMMdd}.log"
        );

        // 文件日志锁对象
        private static readonly object FileLock = new object();

        // 静态构造函数：确保日志目录存在
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
        public static void Error(string message) {
            Info(message, "ERROR");
        }

        /// <summary>
        /// 记录错误日志到文件（包含异常堆栈）
        /// </summary>
        public static void Error(string message, Exception ex) {
            string fullMessage = $"{message}\n异常详情: {ex}";
            Info(fullMessage, "ERROR");
        }

        /// <summary>
        /// 记录警告日志到文件
        /// </summary>
        public static void Warn(string message) {
            Info(message, "WARN");
        }

        /// <summary>
        /// 格式化日志条目
        /// </summary>
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

                var files = Directory.GetFiles(LogDirectory, "WordAddIn_*.log");
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
        public static string GetLogFilePath() {
            return LogFilePath;
        }
    }
}
