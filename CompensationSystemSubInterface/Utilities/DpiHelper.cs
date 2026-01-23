using System.Drawing;
using System.Windows.Forms;

namespace CompensationSystemSubInterface.Utilities {
    /// <summary>
    /// DPI 缩放辅助类，用于处理高 DPI 显示器下的尺寸缩放
    /// </summary>
    public static class DpiHelper {
        /// <summary>
        /// 根据当前 DPI 缩放宽度值
        /// 在实际缩放因子基础上增加 0.25 的偏移量，以确保列宽足够显示内容
        /// 例如：125% 缩放时按 150% 计算，150% 缩放时按 175% 计算
        /// </summary>
        /// <param name="control">用于获取 DPI 的控件</param>
        /// <param name="baseWidth">100% DPI (96 DPI) 下的基准宽度</param>
        /// <returns>缩放后的宽度值</returns>
        public static int ScaleWidth(Control control, int baseWidth) {
            using (Graphics g = control.CreateGraphics()) {
                float scaleFactor = g.DpiX / 96f;
                // 增加 0.25 的偏移量，确保列宽足够
                float adjustedScaleFactor = scaleFactor + 0.25f;
                return (int)(baseWidth * adjustedScaleFactor);
            }
        }
    }
}
