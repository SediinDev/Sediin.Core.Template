using ClosedXML.Excel;
using Sediin.Core.Mvc.Helpers.ReflectionHelpers;
using System.Data;

namespace Sediin.Core.Mvc.Helpers.ExcelHelper
{
    public static class Excel
    {
        public static byte[] CreateExcelFromList<T>(IEnumerable<T> model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            using (var wb = new XLWorkbook())
            {
                var dt = Reflection.ListToDataTable<T>(model);
                wb.Worksheets.Add(dt);
                wb.Worksheet(1).Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public static byte[] CreateExcelFromDataTable(DataTable dt)
        {
            if (dt == null)
                throw new ArgumentNullException(nameof(dt));

            using (var wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                wb.Worksheet(1).Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        // Altri metodi simili per lista di DataTable, base64 ecc puoi aggiungerli qui...
    }
}
