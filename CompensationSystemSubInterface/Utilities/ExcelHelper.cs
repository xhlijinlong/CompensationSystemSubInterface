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
                // 1. 创建工作簿
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("薪资数据");

                // ====================================================
                // 2. 预定义样式
                // ====================================================

                // 【关键辅助函数】将 System.Drawing.Color 转换为 NPOI 的 XSSFColor (RGB模式)
                XSSFColor GetXlColor(Color c) {
                    return new XSSFColor(new byte[] { c.R, c.G, c.B });
                }

                // [表头样式]：灰色背景 (RGB) + 加粗
                ICellStyle headerStyle = workbook.CreateCellStyle();
                // 强制转换为 XSSFCellStyle 以使用 RGB 设置方法
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

                // [通用数据样式]
                ICellStyle dataStyle = workbook.CreateCellStyle();
                dataStyle.BorderBottom = BorderStyle.Thin;
                dataStyle.BorderTop = BorderStyle.Thin;
                dataStyle.BorderLeft = BorderStyle.Thin;
                dataStyle.BorderRight = BorderStyle.Thin;
                dataStyle.VerticalAlignment = VerticalAlignment.Center;

                // [货币数字样式]
                ICellStyle currencyStyle = workbook.CreateCellStyle();
                currencyStyle.CloneStyleFrom(dataStyle);
                IDataFormat format = workbook.CreateDataFormat();
                currencyStyle.DataFormat = format.GetFormat("#,##0.00");
                currencyStyle.Alignment = HorizontalAlignment.Right;

                // --- 高亮样式定义 (使用精准 RGB) ---

                // [个人小计 (浅蓝 - AliceBlue)]
                ICellStyle styleSubEmp = workbook.CreateCellStyle();
                styleSubEmp.CloneStyleFrom(currencyStyle);
                // 使用 AliceBlue 的 RGB 值
                ((XSSFCellStyle)styleSubEmp).SetFillForegroundColor(GetXlColor(Color.AliceBlue));
                styleSubEmp.FillPattern = FillPattern.SolidForeground;

                // [部门统计 (浅灰 - LightGray)]
                ICellStyle styleSubDept = workbook.CreateCellStyle();
                styleSubDept.CloneStyleFrom(currencyStyle);
                ((XSSFCellStyle)styleSubDept).SetFillForegroundColor(GetXlColor(Color.LightGray));
                styleSubDept.FillPattern = FillPattern.SolidForeground;
                IFont boldFont = workbook.CreateFont();
                boldFont.IsBold = true;
                styleSubDept.SetFont(boldFont);

                // [总计 (浅灰 - LightGray)] -> 你之前说要把总计也改成灰色
                ICellStyle styleGrand = workbook.CreateCellStyle();
                styleGrand.CloneStyleFrom(styleSubDept);
                ((XSSFCellStyle)styleGrand).SetFillForegroundColor(GetXlColor(Color.LightGray));
                styleGrand.FillPattern = FillPattern.SolidForeground;

                // ====================================================
                // 3. 写入表头
                // ====================================================
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

                // ====================================================
                // 4. 写入数据行
                // ====================================================
                for (int i = 0; i < dgv.Rows.Count; i++) {
                    DataGridViewRow dgvRow = dgv.Rows[i];
                    IRow excelRow = sheet.CreateRow(i + 1);

                    // 获取 RowType (从数据源获取，防止 UI 隐藏列导致取不到)
                    int rowType = 0;
                    if (dgvRow.DataBoundItem is DataRowView drv) {
                        if (drv.Row.Table.Columns.Contains("RowType")) {
                            var val = drv["RowType"];
                            if (val != null && val != DBNull.Value) {
                                int.TryParse(val.ToString(), out rowType);
                            }
                        }
                    }

                    // 选择样式
                    ICellStyle currentNumStyle = currencyStyle;
                    ICellStyle currentTxtStyle = dataStyle;

                    if (rowType == 1) { currentNumStyle = styleSubEmp; currentTxtStyle = styleSubEmp; } else if (rowType == 2) { currentNumStyle = styleSubDept; currentTxtStyle = styleSubDept; } else if (rowType == 3) { currentNumStyle = styleGrand; currentTxtStyle = styleGrand; }

                    // 填充单元格
                    for (int j = 0; j < dgv.Columns.Count; j++) {
                        if (dgvToExcelMap[j] == -1) continue;

                        ICell cell = excelRow.CreateCell(dgvToExcelMap[j]);
                        object val = dgvRow.Cells[j].Value;
                        string strVal = val != null ? val.ToString() : "";

                        bool isCurrencyCol = dgv.Columns[j].Name.StartsWith("Item_") || dgv.Columns[j].Name == "TotalAmount";

                        if (isCurrencyCol && double.TryParse(strVal, out double numVal)) {
                            cell.SetCellValue(numVal);
                            cell.CellStyle = currentNumStyle;
                        } else {
                            cell.SetCellValue(strVal);
                            // 如果是高亮行但不是数字格（如"小计"文字），左对齐更美观
                            if (rowType > 0) {
                                ICellStyle txtStyle = workbook.CreateCellStyle();
                                txtStyle.CloneStyleFrom(currentTxtStyle);
                                txtStyle.Alignment = HorizontalAlignment.Left;
                                cell.CellStyle = txtStyle;
                            } else {
                                cell.CellStyle = currentTxtStyle;
                            }
                        }
                    }
                }

                // ====================================================
                // 5. 收尾工作
                // ====================================================
                for (int i = 0; i < excelColIndex; i++) {
                    sheet.AutoSizeColumn(i);
                    int currentWidth = sheet.GetColumnWidth(i);
                    if (currentWidth > 50 * 256) sheet.SetColumnWidth(i, 50 * 256);
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
