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

                // 1. 表头样式（白色背景、加粗、居中）
                ICellStyle headerStyle = workbook.CreateCellStyle();
                headerStyle.Alignment = HorizontalAlignment.Center;
                headerStyle.BorderBottom = BorderStyle.Thin;
                headerStyle.BorderTop = BorderStyle.Thin;
                headerStyle.BorderLeft = BorderStyle.Thin;
                headerStyle.BorderRight = BorderStyle.Thin;
                IFont headerFont = workbook.CreateFont();
                headerFont.IsBold = true;
                headerStyle.SetFont(headerFont);

                // 2. 基础数据样式（边框）
                ICellStyle dataStyle = workbook.CreateCellStyle();
                dataStyle.BorderBottom = BorderStyle.Thin;
                dataStyle.BorderTop = BorderStyle.Thin;
                dataStyle.BorderLeft = BorderStyle.Thin;
                dataStyle.BorderRight = BorderStyle.Thin;
                dataStyle.VerticalAlignment = VerticalAlignment.Center;

                // 3. 货币样式（数字格式，靠右对齐）
                ICellStyle currencyStyle = workbook.CreateCellStyle();
                currencyStyle.CloneStyleFrom(dataStyle);
                IDataFormat format = workbook.CreateDataFormat();
                currencyStyle.DataFormat = format.GetFormat("#,##0.00");
                currencyStyle.Alignment = HorizontalAlignment.Right;

                // 4. 日期样式（不含时分秒）
                ICellStyle dateStyle = workbook.CreateCellStyle();
                dateStyle.CloneStyleFrom(dataStyle);
                dateStyle.DataFormat = format.GetFormat("yyyy-MM-dd");
                dateStyle.Alignment = HorizontalAlignment.Center;

                // 5. 时间样式（含时分秒）
                ICellStyle dateTimeStyle = workbook.CreateCellStyle();
                dateTimeStyle.CloneStyleFrom(dataStyle);
                dateTimeStyle.DataFormat = format.GetFormat("yyyy-MM-dd HH:mm:ss");
                dateTimeStyle.Alignment = HorizontalAlignment.Center;

                // ==================== 写入表头 ====================
                IRow headerRow = sheet.CreateRow(0);
                int excelColIndex = 0;
                int[] dgvToExcelMap = new int[dgv.Columns.Count];

                // 按照 DisplayIndex 顺序获取列
                var orderedColumns = dgv.Columns.Cast<DataGridViewColumn>()
                    .OrderBy(c => c.DisplayIndex)
                    .ToList();

                // 计算冻结列数（根据DataGridView中实际冻结的可见列数）
                int frozenColCount = 0;
                foreach (var col in orderedColumns) {
                    if (col.Visible && col.Frozen) {
                        frozenColCount++;
                    }
                }

                // 初始化映射为-1
                for (int i = 0; i < dgv.Columns.Count; i++) {
                    dgvToExcelMap[i] = -1;
                }

                // 按DisplayIndex顺序写入列头
                foreach (var col in orderedColumns) {
                    if (col.Visible) {
                        ICell cell = headerRow.CreateCell(excelColIndex);
                        cell.SetCellValue(col.HeaderText);
                        cell.CellStyle = headerStyle;
                        sheet.SetColumnWidth(excelColIndex, 15 * 256);
                        dgvToExcelMap[col.Index] = excelColIndex;
                        excelColIndex++;
                    }
                }

                // ==================== 写入数据 ====================
                for (int i = 0; i < dgv.Rows.Count; i++) {
                    DataGridViewRow dgvRow = dgv.Rows[i];
                    IRow excelRow = sheet.CreateRow(i + 1);

                    // 按DisplayIndex顺序写入单元格
                    foreach (var col in orderedColumns) {
                        if (dgvToExcelMap[col.Index] == -1) continue;

                        ICell cell = excelRow.CreateCell(dgvToExcelMap[col.Index]);
                        object val = dgvRow.Cells[col.Index].Value;
                        string strVal = val != null ? val.ToString() : "";

                        // 1. 处理日期类型
                        if (val != null && val != DBNull.Value && val is DateTime dateVal) {
                            cell.SetCellValue(dateVal);
                            string colFmt = col.DefaultCellStyle.Format;
                            if (colFmt == "yyyy-MM-dd" || colFmt == "d") {
                                cell.CellStyle = dateStyle;
                            } else {
                                cell.CellStyle = dateTimeStyle;
                            }
                        }
                        // 2. 处理数字类型
                        else {
                            bool isCurrencyCol = col.Name.StartsWith("Item_") || col.Name == "TotalAmount";
                            if (isCurrencyCol && double.TryParse(strVal, out double numVal)) {
                                cell.SetCellValue(numVal);
                                cell.CellStyle = currencyStyle;
                            } else {
                                cell.SetCellValue(strVal);
                                cell.CellStyle = dataStyle;
                            }
                        }
                    }
                }

                // ==================== 收尾 ====================
                // 自动调整列宽
                for (int i = 0; i < excelColIndex; i++) {
                    sheet.AutoSizeColumn(i);
                    int w = sheet.GetColumnWidth(i);
                    if (w > 50 * 256) sheet.SetColumnWidth(i, 50 * 256);
                }

                // 根据DataGridView的实际冻结列数设置Excel冻结窗格
                // CreateFreezePane(列数, 行数) - 冻结指定数量的列和第一行（表头）
                if (frozenColCount > 0) {
                    sheet.CreateFreezePane(frozenColCount, 1);
                } else {
                    // 如果没有冻结列，只冻结表头行
                    sheet.CreateFreezePane(0, 1);
                }

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
