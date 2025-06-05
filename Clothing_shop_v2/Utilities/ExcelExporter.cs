using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;

namespace Clothing_shop_v2.Utilities
{
    public static class ExcelExporter
    {
        public static byte[] ExportToExcel<T>(
            IEnumerable<T> data,
            List<string> columnHeaders,
            List<Func<T, int, object>> columnSelectors,
            string worksheetName = "Sheet1",
            bool includeBorders = true)
        {
            if (data == null || !data.Any() || columnHeaders == null || columnSelectors == null)
            {
                throw new ArgumentException("Dữ liệu hoặc cấu hình không hợp lệ.");
            }

            if (columnHeaders.Count != columnSelectors.Count)
            {
                throw new ArgumentException("Số lượng tiêu đề cột và ánh xạ cột phải bằng nhau.");
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add(worksheetName);

                // Thiết lập tiêu đề cột
                for (int i = 0; i < columnHeaders.Count; i++)
                {
                    worksheet.Cell(1, i + 1).Value = columnHeaders[i];
                }

                // Định dạng tiêu đề
                var headerRange = worksheet.Range(1, 1, 1, columnHeaders.Count);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                // Thêm dữ liệu
                int row = 2;
                int index = 0;
                foreach (var item in data)
                {
                    for (int col = 0; col < columnSelectors.Count; col++)
                    {
                        var value = columnSelectors[col](item, index);
                        if (value is DateTime date)
                        {
                            worksheet.Cell(row, col + 1).Value = date;
                            worksheet.Cell(row, col + 1).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";
                        }
                        else
                        {
                            worksheet.Cell(row, col + 1).Value = value?.ToString() ?? "-";
                        }
                    }
                    row++;
                    index++;
                }

                // Thêm viền
                if (includeBorders)
                {
                    var dataRange = worksheet.Range(1, 1, row - 1, columnHeaders.Count);
                    dataRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                    dataRange.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                }

                // Tự động điều chỉnh kích thước cột
                worksheet.Columns().AdjustToContents();

                // Xuất file
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
    }
}