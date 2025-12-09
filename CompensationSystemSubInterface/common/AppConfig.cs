namespace CompensationSystemSubInterface.Common {

    internal class AppConfig {
        // 管控配置地址
        internal const string controlInfoUrl = "http://192.168.111.40/ResourcesCenter/control.json";

        // 应用程序启动路径
        public static string appStartupPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? string.Empty;

        // 内网检测 IP
        public static string intranetCheckIp = "10.50.5.1";
    }
}
