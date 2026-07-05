using System;
using System.Data;
using System.Globalization;
using System.Linq;

namespace ReportViewer.Helpers
{
    public static class BillingTaxPrintHelper
    {
        public sealed class HeaderTaxAmounts
        {
            public decimal FreightTax { get; set; }
            public decimal ChargesTax { get; set; }
            public decimal BreakageTax { get; set; }
        }

        public static bool HasDynamicTaxes(DataTable taxes)
        {
            return taxes != null && taxes.Rows.Count > 0;
        }

        public static decimal GetDecimal(DataRow row, params string[] columnNames)
        {
            if (row == null || columnNames == null)
                return 0m;

            foreach (var columnName in columnNames)
            {
                if (string.IsNullOrEmpty(columnName) || !row.Table.Columns.Contains(columnName))
                    continue;

                var value = row[columnName];
                if (value == null || value == DBNull.Value)
                    continue;

                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                }
            }

            return 0m;
        }

        public static string GetTaxName(DataRow row)
        {
            return Convert.ToString(GetColumnValue(row, "TaxName", "Name")) ?? string.Empty;
        }

        public static decimal GetTaxAmount(DataRow row)
        {
            return GetDecimal(row, "TaxAmount", "Amount");
        }

        public static decimal GetTaxRate(DataRow row)
        {
            return GetDecimal(row, "Rate");
        }

        public static decimal SumTaxAmount(DataTable taxes)
        {
            if (!HasDynamicTaxes(taxes))
                return 0m;

            return taxes.AsEnumerable().Sum(GetTaxAmount);
        }

        public static int CountTaxDisplayRows(DataTable taxes, HeaderTaxAmounts headerTaxes)
        {
            var count = HasDynamicTaxes(taxes) ? taxes.Rows.Count : 0;
            if (headerTaxes == null)
                return count;

            if (headerTaxes.FreightTax > 0)
                count++;
            if (headerTaxes.ChargesTax > 0)
                count++;
            if (headerTaxes.BreakageTax > 0)
                count++;

            return count;
        }

        public static HeaderTaxAmounts GetHeaderTaxAmounts(DataRow firstRow)
        {
            return new HeaderTaxAmounts
            {
                FreightTax = GetDecimal(firstRow, "FreightTax"),
                ChargesTax = GetDecimal(firstRow, "ChargesTax", "chargesTax"),
                BreakageTax = GetDecimal(firstRow, "BreakageTax")
            };
        }

        public static string FormatAmount(decimal amount)
        {
            return amount.ToString("N", CultureInfo.InvariantCulture);
        }

        public static void ApplyLegacyQuotationFreightTaxFolding(DataSet ds)
        {
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return;

            var lineTaxes = ds.Tables.Count > 1 ? ds.Tables[1] : null;
            if (HasDynamicTaxes(lineTaxes))
                return;

            var row = ds.Tables[0].Rows[0];
            if (GetDecimal(row, "IGST") > 0)
            {
                row["IGST"] = GetDecimal(row, "IGST") + GetDecimal(row, "FreightTax") + GetDecimal(row, "chargesTax");
            }
            else if (GetDecimal(row, "SGST") > 0)
            {
                row["SGST"] = GetDecimal(row, "SGST") + GetDecimal(row, "FreightTax") / 2 + GetDecimal(row, "chargesTax") / 2;
                row["CGST"] = GetDecimal(row, "CGST") + GetDecimal(row, "FreightTax") / 2 + GetDecimal(row, "chargesTax") / 2;
            }
        }

        public static int BuildQuotationRowsToSpan(DataRow firstRow, DataTable lineTaxes)
        {
            var rowsToSpan = 6;
            rowsToSpan += GetDecimal(firstRow, "charge1") > 0 ? 1 : 0;
            rowsToSpan += GetDecimal(firstRow, "charge2") > 0 ? 1 : 0;
            rowsToSpan += GetDecimal(firstRow, "charge3") > 0 ? 1 : 0;
            rowsToSpan += GetDecimal(firstRow, "charge4") > 0 ? 1 : 0;
            rowsToSpan += GetDecimal(firstRow, "charge5") > 0 ? 1 : 0;
            rowsToSpan += GetDecimal(firstRow, "DiscountAmount") > 0 ? 1 : 0;

            if (HasDynamicTaxes(lineTaxes))
            {
                rowsToSpan = rowsToSpan - 3 + lineTaxes.Rows.Count;
            }

            return rowsToSpan;
        }

        static object GetColumnValue(DataRow row, params string[] columnNames)
        {
            if (row == null || columnNames == null)
                return null;

            foreach (var columnName in columnNames)
            {
                if (string.IsNullOrEmpty(columnName) || !row.Table.Columns.Contains(columnName))
                    continue;

                var value = row[columnName];
                if (value != null && value != DBNull.Value)
                    return value;
            }

            return null;
        }
    }
}
