using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace BAL.Helpers
{
    public static class QuotationPrintTaxHelper
    {
        sealed class TaxGroup
        {
            public int TaxId { get; set; }
            public string TaxName { get; set; }
            public decimal Rate { get; set; }
            public decimal Amount { get; set; }
        }

        public static void ReplaceQuotationTaxTableForPrint(DataSet ds)
        {
            if (ds == null || ds.Tables.Count <= 1 || ds.Tables[0].Rows.Count == 0)
                return;

            var lineTaxes = ds.Tables[1];
            if (lineTaxes == null || lineTaxes.Rows.Count == 0)
                return;

            var aggregated = BuildAggregatedQuotationTaxTable(lineTaxes, ds.Tables[0].Rows[0]);
            var tableName = lineTaxes.TableName;
            ds.Tables.RemoveAt(1);
            aggregated.TableName = tableName;
            ds.Tables.Add(aggregated);
        }

        public static void PrepareSaleBillTaxTableForPrint(DataSet ds)
        {
            ReplaceQuotationTaxTableForPrint(ds);
            UpdateSaleBillPrintTotalFromTaxTable(ds);
        }

        public static void UpdateSaleBillPrintTotalFromTaxTable(DataSet ds)
        {
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return;

            var lineTaxes = ds.Tables.Count > 1 ? ds.Tables[1] : null;
            if (lineTaxes == null || lineTaxes.Rows.Count == 0)
                return;

            var headerRow = ds.Tables[0].Rows[0];
            var subTotal = ds.Tables[0].AsEnumerable().Sum(r => GetDecimal(r, "SubTotal"));
            var discount = GetDecimal(headerRow, "discount", "Discount");
            var freight = GetDecimal(headerRow, "Freight");
            var otherCharges = GetDecimal(headerRow, "OtherChargeAmount");
            var taxTotal = lineTaxes.AsEnumerable().Sum(r => GetDecimal(r, "Amount", "TaxAmount"));

            var total = subTotal - discount + freight + otherCharges + taxTotal;
            var rounded = Math.Round(total, 0, MidpointRounding.AwayFromZero);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                SetDecimalIfExists(row, "Total", total);
                SetDecimalIfExists(row, "RoundedAmount", rounded);
                SetDecimalIfExists(row, "TaxAmount", taxTotal);
            }
        }

        static void SetDecimalIfExists(DataRow row, string columnName, decimal value)
        {
            if (row == null || string.IsNullOrEmpty(columnName) || !row.Table.Columns.Contains(columnName))
                return;

            row[columnName] = value;
        }

        public static DataTable BuildAggregatedQuotationTaxTable(DataTable lineTaxes, DataRow headerRow)
        {
            var result = CreatePrintTaxTable();
            if (lineTaxes == null || lineTaxes.Rows.Count == 0)
                return result;

            var groups = lineTaxes.AsEnumerable()
                .GroupBy(GetGroupKey)
                .Select(g =>
                {
                    var first = g.First();
                    return new TaxGroup
                    {
                        TaxId = GetInt(first, "TaxId"),
                        TaxName = GetString(first, "TaxName", "Name"),
                        Rate = GetDecimal(first, "Rate"),
                        Amount = g.Sum(r => GetDecimal(r, "Amount", "TaxAmount"))
                    };
                })
                .OrderBy(g => g.TaxName ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var extra = GetDecimal(headerRow, "FreightTax") + GetDecimal(headerRow, "ChargesTax", "chargesTax");
            DistributeExtraTaxByRate(groups, extra);

            foreach (var group in groups)
            {
                var row = result.NewRow();
                row["TaxId"] = group.TaxId;
                row["TaxName"] = group.TaxName ?? string.Empty;
                row["Rate"] = group.Rate;
                row["Amount"] = group.Amount;
                row["TaxAmount"] = group.Amount;
                result.Rows.Add(row);
            }

            return result;
        }

        static void DistributeExtraTaxByRate(List<TaxGroup> groups, decimal extra)
        {
            if (extra <= 0 || groups == null || groups.Count == 0)
                return;

            var totalRate = groups.Sum(g => g.Rate);
            if (totalRate <= 0)
                return;

            decimal distributed = 0;
            for (var i = 0; i < groups.Count; i++)
            {
                decimal share;
                if (i == groups.Count - 1)
                {
                    share = extra - distributed;
                }
                else
                {
                    share = Math.Round(extra * groups[i].Rate / totalRate, 2, MidpointRounding.AwayFromZero);
                    distributed += share;
                }

                groups[i].Amount += share;
            }
        }

        static DataTable CreatePrintTaxTable()
        {
            var table = new DataTable();
            table.Columns.Add("TaxId", typeof(int));
            table.Columns.Add("TaxName", typeof(string));
            table.Columns.Add("Rate", typeof(decimal));
            table.Columns.Add("Amount", typeof(decimal));
            table.Columns.Add("TaxAmount", typeof(decimal));
            return table;
        }

        static string GetGroupKey(DataRow row)
        {
            if (row.Table.Columns.Contains("TaxId") && row["TaxId"] != DBNull.Value)
            {
                return "id:" + Convert.ToInt32(row["TaxId"]);
            }

            var name = GetString(row, "TaxName", "Name");
            var rate = GetDecimal(row, "Rate");
            return "nr:" + name + "|" + rate.ToString(CultureInfo.InvariantCulture);
        }

        static decimal GetDecimal(DataRow row, params string[] columnNames)
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

        static int GetInt(DataRow row, string columnName)
        {
            if (row == null || string.IsNullOrEmpty(columnName) || !row.Table.Columns.Contains(columnName))
                return 0;

            var value = row[columnName];
            if (value == null || value == DBNull.Value)
                return 0;

            try
            {
                return Convert.ToInt32(value);
            }
            catch
            {
                return 0;
            }
        }

        static string GetString(DataRow row, params string[] columnNames)
        {
            if (row == null || columnNames == null)
                return string.Empty;

            foreach (var columnName in columnNames)
            {
                if (string.IsNullOrEmpty(columnName) || !row.Table.Columns.Contains(columnName))
                    continue;

                var value = row[columnName];
                if (value != null && value != DBNull.Value)
                    return Convert.ToString(value);
            }

            return string.Empty;
        }
    }
}
