using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BorderStyle = NPOI.SS.UserModel.BorderStyle;
using HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment;

namespace CompensationSystemSubInterface.Utilities {
    /// <summary>
    /// Excel 辅助类
    /// </summary>
    public class ExcelHelper {
        /// <summary>
        /// 将 DataGridView 的内容导出为带格式的 Excel (.xlsx)
        /// </summary>
        public static bool ExportToExcel(DataGridView dgv, string filePath) {
            try {
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("数据导出");

                // ==================== 样式定义 ====================
                XSSFColor GetXlColor(Color c) {
                    return new XSSFColor(new byte[] { c.R, c.G, c.B });
                }

                // 1. 表头
                ICellStyle headerStyle = workbook.CreateCellStyle();
                ((XSSFCellStyle)headerStyle).SetFillForegroundColor(GetXlColor(Color.LightGray));
                headerStyle.FillPattern = FillPattern.SolidForeground;
                headerStyle.Alignment = HorizontalAlignment.Center;
                headerStyle.BorderBottom = BorderStyle.Thin;
                headerStyle.BorderTop = BorderStyle.Thin;
                headerStyle.BorderLeft = BorderStyle.Thin;
                headerStyle.BorderRight = BorderStyle.Thin;
                IFont headerFont = workbook.CreateFont();
                headerFont.IsBold = true;
                headerStyle.SetFont(headerFont);

                // 2. 基础数据
                ICellStyle dataStyle = workbook.CreateCellStyle();
                dataStyle.BorderBottom = BorderStyle.Thin;
                dataStyle.BorderTop = BorderStyle.Thin;
                dataStyle.BorderLeft = BorderStyle.Thin;
                dataStyle.BorderRight = BorderStyle.Thin;
                dataStyle.VerticalAlignment = VerticalAlignment.Center;

                // 3. 货币 (数字)
                ICellStyle currencyStyle = workbook.CreateCellStyle();
                currencyStyle.CloneStyleFrom(dataStyle);
                IDataFormat format = workbook.CreateDataFormat();
                currencyStyle.DataFormat = format.GetFormat("#,##0.00");
                currencyStyle.Alignment = HorizontalAlignment.Right;

                // 4. 【日期 (不含时分秒)】
                ICellStyle dateStyle = workbook.CreateCellStyle();
                dateStyle.CloneStyleFrom(dataStyle);
                dateStyle.DataFormat = format.GetFormat("yyyy-MM-dd");
                dateStyle.Alignment = HorizontalAlignment.Center;

                // 5. 【新增：时间 (含时分秒)】
                ICellStyle dateTimeStyle = workbook.CreateCellStyle();
                dateTimeStyle.CloneStyleFrom(dataStyle);
                // 设置为通用的时间格式
                dateTimeStyle.DataFormat = format.GetFormat("yyyy-MM-dd HH:mm:ss");
                dateTimeStyle.Alignment = HorizontalAlignment.Center;

                // --- 高亮样式 ---
                // (为了代码简洁，这里略写高亮样式的定义，请保留你之前代码中的 styleSubEmp, styleSubDept, styleGrand 逻辑)
                // 务必保留之前的高亮样式定义代码...
                ICellStyle styleSubEmp = workbook.CreateCellStyle();
                styleSubEmp.CloneStyleFrom(currencyStyle);
                ((XSSFCellStyle)styleSubEmp).SetFillForegroundColor(GetXlColor(Color.AliceBlue));
                styleSubEmp.FillPattern = FillPattern.SolidForeground;

                ICellStyle styleSubDept = workbook.CreateCellStyle();
                styleSubDept.CloneStyleFrom(currencyStyle);
                ((XSSFCellStyle)styleSubDept).SetFillForegroundColor(GetXlColor(Color.LightGray));
                styleSubDept.FillPattern = FillPattern.SolidForeground;
                IFont boldFont = workbook.CreateFont();
                boldFont.IsBold = true;
                styleSubDept.SetFont(boldFont);

                ICellStyle styleGrand = workbook.CreateCellStyle();
                styleGrand.CloneStyleFrom(styleSubDept);
                ((XSSFCellStyle)styleGrand).SetFillForegroundColor(GetXlColor(Color.LightGray));
                styleGrand.FillPattern = FillPattern.SolidForeground;

                // ==================== 写入表头 ====================
                IRow headerRow = sheet.CreateRow(0);
                int excelColIndex = 0;
                int[] dgvToExcelMap = new int[dgv.Columns.Count];

                for (int i = 0; i < dgv.Columns.Count; i++) {
                    if (dgv.Columns[i].Visible) {
                        ICell cell = headerRow.CreateCell(excelColIndex);
                        cell.SetCellValue(dgv.Columns[i].HeaderText);
                        cell.CellStyle = headerStyle;
                        sheet.SetColumnWidth(excelColIndex, 15 * 256);
                        dgvToExcelMap[i] = excelColIndex;
                        excelColIndex++;
                    } else {
                        dgvToExcelMap[i] = -1;
                    }
                }

                // ==================== 写入数据 ====================
                for (int i = 0; i < dgv.Rows.Count; i++) {
                    DataGridViewRow dgvRow = dgv.Rows[i];
                    IRow excelRow = sheet.CreateRow(i + 1);

                    int rowType = 0;
                    if (dgvRow.DataBoundItem is DataRowView drv) {
                        if (drv.Row.Table.Columns.Contains("RowType")) {
                            var val = drv["RowType"];
                            if (val != null && val != DBNull.Value) int.TryParse(val.ToString(), out rowType);
                        }
                    }

                    // 确定当前行的高亮样式
                    ICellStyle baseStyle = currencyStyle; // 默认数字样式
                    // ... (这里也略写样式选择逻辑，请保留你之前的 if rowType == 1 ... 代码)
                    if (rowType == 1) baseStyle = styleSubEmp;
                    else if (rowType == 2) baseStyle = styleSubDept;
                    else if (rowType == 3) baseStyle = styleGrand;

                    for (int j = 0; j < dgv.Columns.Count; j++) {
                        if (dgvToExcelMap[j] == -1) continue;

                        ICell cell = excelRow.CreateCell(dgvToExcelMap[j]);
                        object val = dgvRow.Cells[j].Value;
                        string strVal = val != null ? val.ToString() : "";

                        // 1. 处理日期类型
                        if (val != null && val != DBNull.Value && val is DateTime dateVal) {
                            cell.SetCellValue(dateVal);

                            // 【智能判断】：检查 DataGridView 这一列的格式设置
                            string colFmt = dgv.Columns[j].DefaultCellStyle.Format;

                            // 如果界面上明确设了 "yyyy-MM-dd"，或者列名里包含“日期”且不包含“时间”
                            // 这里我们主要依赖 Format 属性，这最准确
                            if (colFmt == "yyyy-MM-dd" || colFmt == "d") {
                                cell.CellStyle = dateStyle; // 只显示日期
                            } else {
                                cell.CellStyle = dateTimeStyle; // 显示完整时间
                            }
                        }
                        // 2. 处理数字类型
                        else {
                            bool isCurrencyCol = dgv.Columns[j].Name.StartsWith("Item_") || dgv.Columns[j].Name == "TotalAmount";
                            if (isCurrencyCol && double.TryParse(strVal, out double numVal)) {
                                cell.SetCellValue(numVal);
                                cell.CellStyle = baseStyle; // 使用数字/高亮样式
                            } else {
                                cell.SetCellValue(strVal);
                                // 文本样式处理...
                                if (rowType > 0) {
                                    ICellStyle txtStyle = workbook.CreateCellStyle();
                                    txtStyle.CloneStyleFrom(dataStyle);
                                    // 复制背景色
                                    if (baseStyle.FillForegroundColorColor is XSSFColor c)
                                        ((XSSFCellStyle)txtStyle).SetFillForegroundColor(c);
                                    else
                                        txtStyle.FillForegroundColor = baseStyle.FillForegroundColor;

                                    txtStyle.FillPattern = FillPattern.SolidForeground;
                                    txtStyle.Alignment = HorizontalAlignment.Left;
                                    cell.CellStyle = txtStyle;
                                } else {
                                    cell.CellStyle = dataStyle;
                                }
                            }
                        }
                    }
                }

                // ==================== 收尾 ====================
                for (int i = 0; i < excelColIndex; i++) {
                    sheet.AutoSizeColumn(i);
                    int w = sheet.GetColumnWidth(i);
                    if (w > 50 * 256) sheet.SetColumnWidth(i, 50 * 256);
                }
                sheet.CreateFreezePane(2, 1);

                try {
                    using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write)) {
                        workbook.Write(fs);
                    }
                } catch (IOException) {
                    throw new Exception("文件正被另一个程序打开，请先关闭 Excel 文件后再试！");
                }
                return true;
            } catch (Exception ex) {
                throw;
            }
        }
    }
}
