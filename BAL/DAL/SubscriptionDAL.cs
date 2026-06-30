using BAL.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DAL
{
    public class SubscriptionDAL
    {
        public List<SubscriptionRatesDTO> GetRates(DateTime dateTime)
        {
            using (SQL objSql = new SQL())
            {

                objSql.AddParameter("@date", DbType.Date, ParameterDirection.Input, 0, dateTime);
                return objSql.ContructList<SubscriptionRatesDTO>(objSql.ExecuteDataSet(GET_RATES));
            }

        }

        public List<MonthlyChallansDTO> GetMonthlyChallans(DateTime fromDate, DateTime endDate, int clientId, int companyId)
        {
            using (SQL objSql = new SQL())
            {

                objSql.AddParameter("@clientId", DbType.Int32, ParameterDirection.Input, 0, clientId);
                objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
                objSql.AddParameter("@fromDate", DbType.Date, ParameterDirection.Input, 0, fromDate);
                objSql.AddParameter("@endDate", DbType.Date, ParameterDirection.Input, 0, endDate);
                return objSql.ContructList<MonthlyChallansDTO>(objSql.ExecuteDataSet(GET_MONTHLYCHALLANS));
            }

        }
        public List<CompanyDTO> GetCompaniesTobill()
        {
            using (SQL objSql = new SQL())
            {
                return objSql.ContructList<CompanyDTO>(objSql.ExecuteDataSet(GET_ALL_COMPANIES));
            }

        }

        public int SaveBillig(RentacBillingDTO bill)
        {
            using (SQL objSql = new SQL())
            {
                try
                {
                    objSql.BeginTransaction();
                    objSql.NewCommand();
                    objSql.AddParameter("@clientId", DbType.Int32, ParameterDirection.Input, 0, bill.ClientId);
                    objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, bill.CompanyId);

                    objSql.AddParameter("@month", DbType.Int32, ParameterDirection.Input, 0, bill.Month);
                    objSql.AddParameter("@year", DbType.Int32, ParameterDirection.Input, 0, bill.Year);
                    objSql.AddParameter("@creationDate", DbType.DateTime, ParameterDirection.Input, 0, bill.CreationDate);
                    objSql.AddParameter("@IGSTRate", DbType.Decimal, ParameterDirection.Input, 0, bill.IGSTRate);
                    objSql.AddParameter("@IGST", DbType.Decimal, ParameterDirection.Input, 0, bill.IGST);
                    objSql.AddParameter("@SGSTRate", DbType.Decimal, ParameterDirection.Input, 0, bill.SGSTRate);
                    objSql.AddParameter("@SGST", DbType.Decimal, ParameterDirection.Input, 0, bill.SGST);
                    objSql.AddParameter("@CGSTRate", DbType.Decimal, ParameterDirection.Input, 0, bill.CGSTRate);
                    objSql.AddParameter("@CGST", DbType.Decimal, ParameterDirection.Input, 0, bill.CGST);
                    objSql.AddParameter("@dueDate", DbType.Date, ParameterDirection.Input, 0, bill.DueDate);
                    objSql.AddParameter("@billingAddress", DbType.String, ParameterDirection.Input, 0, bill.BillingAddress);
                    objSql.AddParameter("@billingCity", DbType.String, ParameterDirection.Input, 0, bill.BillingCity);
                    objSql.AddParameter("@billingPinCode", DbType.Int32, ParameterDirection.Input, 0, bill.BillingPinCode);
                    objSql.AddParameter("@tax", DbType.Decimal, ParameterDirection.Input, 0, bill.Tax);
                    objSql.AddParameter("@totalAmount", DbType.Decimal, ParameterDirection.Input, 0, bill.TotalAmount);
                    objSql.AddParameter("@subtotal", DbType.Decimal, ParameterDirection.Input, 0, bill.SubTotal);

                    objSql.AddParameter("@totalItems", DbType.Decimal, ParameterDirection.Input, 0, bill.TotalItems);
                    objSql.AddParameter("@remarks", DbType.String, ParameterDirection.Input, 0, bill.Remarks);
                    objSql.AddParameter("@packageId", DbType.Int32, ParameterDirection.Input, 0, bill.PackageId);
                    objSql.AddParameter("@lineItemDescription", DbType.String, ParameterDirection.Input, 0, bill.LiteItemDescription);
                    objSql.AddParameter("@payment_id", DbType.String, ParameterDirection.Input, 0, bill.payment_id);

                    bill.BillingId = Convert.ToInt32(objSql.ExecuteScalar(ADD_BILLING));
                    //details section will be used when we implement pay as you go;
                    //if (bill.BillingId > 0)
                    //{
                    //    foreach (var item in bill.Details)
                    //    {
                    //        objSql.NewCommand();

                    //        objSql.AddParameter("@billingId", DbType.Int32, ParameterDirection.Input, 0, bill.BillingId);
                    //        objSql.AddParameter("@challans", DbType.Int32, ParameterDirection.Input, 0, item.Challans);
                    //        objSql.AddParameter("@name", DbType.String, ParameterDirection.Input, 0, item.Name);
                    //        objSql.AddParameter("@remarks", DbType.String, ParameterDirection.Input, 0, item.Remarks);
                    //        objSql.AddParameter("@quantity", DbType.Int32, ParameterDirection.Input, 0, item.Quantity);
                    //        objSql.AddParameter("@rate", DbType.Decimal, ParameterDirection.Input, 0, item.Rate);
                    //        objSql.AddParameter("@amount", DbType.Decimal, ParameterDirection.Input, 0, item.Amount);
                    //        objSql.AddParameter("@creationDate", DbType.DateTime, ParameterDirection.Input, 0, item.CreationDate);
                    //        objSql.AddParameter("@itemType", DbType.String, ParameterDirection.Input, 0, item.ItemType);
                    //        objSql.AddParameter("@range", DbType.String, ParameterDirection.Input, 0, item.Range);


                    //        objSql.ExecuteNonQuery(ADD_BILLINGItems);
                    //    }
                    objSql.Commit();
                    //}
                    //else
                    //{
                    //    objSql.Rollback();
                    //}

                }
                catch (Exception ex)
                {
                    objSql.Rollback();
                }

            }
            return 1;
        }

        /// <summary>
        /// Get bills between a date range
        /// </summary>
        /// <param name="bill"></param>
        /// <returns></returns>
        public List<RentacBillingDTO> GetInvoice(RentacBillingFilterDTO bill)
        {
            using (SQL objSql = new SQL())
            {
                objSql.AddParameter("@clientId", DbType.Int32, ParameterDirection.Input, 0, bill.ClientId);
                objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, bill.CompanyId);
                objSql.AddParameter("@fromDate", DbType.Date, ParameterDirection.Input, 0, bill.FromDate);
                objSql.AddParameter("@toDate", DbType.Date, ParameterDirection.Input, 0, bill.EndDate);
                var ds = objSql.ExecuteDataSet(GET_INVOICE);

                var _list = (from d in ds.Tables[0].AsEnumerable()
                             group d by new
                             {
                                 BillingId = d.Field<int>("BillingId"),
                                 TotalAmount = d["totalAmount"] == DBNull.Value ? 0 : d.Field<decimal>("totalAmount"),
                                 SubTotal = d["subTotal"] == DBNull.Value ? 0 : d.Field<decimal>("subTotal"),
                                 Tax = d.Field<decimal>("tax"),
                                 InvoiceNumber = d.Field<string>("invoiceNumber"),
                                 DueDate = d.Field<DateTime>("dueDate"),
                                 CreationDate = d.Field<DateTime>("creationDate"),
                                 PaymentRefId = d.Field<string>("paymentRefId"),
                                 Paid = d.Field<int>("Paid"),
                                 Month = d.Field<int>("Month"),
                                 Year = d.Field<int>("Year"),
                                 Balance = d["balance"] == DBNull.Value ? 0 : d.Field<decimal>("balance"),
                                 PaymentDate = d.Field<DateTime?>("paymentDate")
                             } into g
                             select new RentacBillingDTO
                             {
                                 BillingId = g.Key.BillingId,
                                 TotalAmount = g.Key.TotalAmount,
                                 SubTotal = g.Key.SubTotal,
                                 Tax = g.Key.Tax,
                                 InvoiceNumber = g.Key.InvoiceNumber,
                                 DueDate = g.Key.DueDate,
                                 CreationDate = g.Key.CreationDate,
                                 PaymentRefId = g.Key.PaymentRefId,
                                 PaymentDate = g.Key.PaymentDate,
                                 Paid = g.Key.Paid,
                                 Month = g.Key.Month,
                                 Year = g.Key.Year,
                                 Balance = g.Key.Balance
                               //  Details = g.Select(o =>
                                  //new RentacBillingDetailsDTO
                                  //{
                                  //    Challans = o.Field<int>("challans"),
                                  //    Range = o.Field<string>("range"),
                                  //    Quantity = o.Field<decimal>("quantity"),
                                  //    Amount = o.Field<decimal>("amount"),
                                  //    Rate = o.Field<decimal>("rate")
                                  //}

                                 //).ToList()
                             }
                        ).ToList();

                return _list;
            }

        }

        /// <summary>
        /// Update bill balance
        /// </summary>
        /// <param name="bills"></param>
        /// <returns></returns>
        public int updateInvoice(List<RentacBillingDTO> bills)
        {
            using (var sql = new SQL())
            {
                try
                {
                    sql.BeginTransaction();
                    foreach (var bill in bills)
                    {
                        sql.NewCommand();
                        sql.AddParameter("@paymentRef_id", DbType.String, ParameterDirection.Input, 0, bill.PaymentRefId);
                        sql.AddParameter("@id", DbType.Int32, ParameterDirection.Input, 0, bill.BillingId);
                        sql.AddParameter("@paymentDate", DbType.DateTime, ParameterDirection.Input, 0, bill.PaymentDate);
                        sql.AddParameter("@balance", DbType.Decimal, ParameterDirection.Input, 0, bill.Balance);
                        var ret = sql.ExecuteNonQuery(UPDATE_PAYMENT_INFO);
                    }
                    sql.Commit();
                    return 1;
                }
                catch (Exception ex)
                {
                    sql.Rollback();
                }
            }
            return 0;
        }

        public List<RentacBillingDTO> GetPendingInvoice(RentacBillingFilterDTO bill)
        {
            using (SQL objSql = new SQL())
            {
                objSql.AddParameter("@clientId", DbType.Int32, ParameterDirection.Input, 0, bill.ClientId);
                objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, bill.CompanyId);
                //objSql.AddParameter("@fromDate", DbType.Date, ParameterDirection.Input, 0, bill.FromDate);
                //objSql.AddParameter("@toDate", DbType.Date, ParameterDirection.Input, 0, bill.EndDate);
                var ds = objSql.ExecuteDataSet(GET_PENDING_BILLS);

                var _list = (from d in ds.Tables[0].AsEnumerable()
                             select new RentacBillingDTO
                             {
                                 BillingId = d.Field<int>("id"),
                                 TotalAmount = d.Field<decimal>("totalAmount"),
                                 SubTotal = d.Field<decimal>("subTotal"),
                                 Tax = d.Field<decimal>("tax"),
                                 InvoiceNumber = d.Field<string>("invoiceNumber"),
                                 DueDate = d.Field<DateTime>("dueDate"),
                                 CreationDate = d.Field<DateTime>("creationDate"),
                                 PaymentRefId = d.Field<string>("paymentRefId"),
                                 Balance = d.Field<decimal>("balance"),
                                 Month = d.Field<int>("Month"),
                                 Year = d.Field<int>("Year"),
                                 PaymentDate = d.Field<DateTime?>("paymentDate")
                             }
                        ).ToList();

                return _list;
            }

        }

        public DataSet GetByIdToPrint(RentacBillingDTO bill)
        {
            using (SQL objSql = new SQL())
            {
                objSql.AddParameter("@billingId", DbType.Int32, ParameterDirection.Input, 0, bill.BillingId);


                return objSql.ExecuteDataSet(GET_BILLBYID);

                //var _list = (from d in ds.Tables[0].AsEnumerable()
                //             group d by new
                //             {
                //                 BillingId = d.Field<int>("id"),
                //                 TotalAmount = d.Field<decimal>("totalAmount"),
                //                 SubTotal = d.Field<decimal>("subTotal"),
                //                 Tax = d.Field<decimal>("tax"),
                //                 InvoiceNumber = d.Field<string>("invoiceNumber"),
                //                 DueDate = d.Field<DateTime>("dueDate"),
                //                 CreationDate = d.Field<DateTime>("creationDate"),
                //                 PaymentRefId = d.Field<string>("paymentRefId"),
                //               //  Paid = d.Field<int>("Paid"),
                //                 Month = d.Field<int>("Month"),
                //                 Year = d.Field<int>("Year"),
                //                 Balance = d.Field<decimal>("balance"),
                //                 PaymentDate = d.Field<DateTime?>("paymentDate")
                //             } into g
                //             select new RentacBillingDTO
                //             {
                //                 BillingId = g.Key.BillingId,
                //                 TotalAmount = g.Key.TotalAmount,
                //                 SubTotal = g.Key.SubTotal,
                //                 Tax = g.Key.Tax,
                //                 InvoiceNumber = g.Key.InvoiceNumber,
                //                 DueDate = g.Key.DueDate,
                //                 CreationDate = g.Key.CreationDate,
                //                 PaymentRefId = g.Key.PaymentRefId,
                //                 PaymentDate = g.Key.PaymentDate,
                //                // Paid = g.Key.Paid,
                //                 Month = g.Key.Month,
                //                 Year = g.Key.Year,
                //                 Balance = g.Key.Balance,
                //                 Details = g.Select(o =>
                //                  new RentacBillingDetailsDTO
                //                  {
                //                      Challans = o.Field<int>("challans"),
                //                      Range = o.Field<string>("range"),
                //                      Quantity = o.Field<decimal>("quantity"),
                //                      Amount = o.Field<decimal>("amount"),
                //                      Rate = o.Field<decimal>("rate")
                //                  }

                //                 ).ToList()
                //             }
                //        ).ToList();
                //return _list.FirstOrDefault();
            }
        }


        #region Procedures
        const string GET_RATES = "p_getBillingRates";
        const string GET_MONTHLYCHALLANS = "p_getMonthlyChallans";
        const string GET_ALL_COMPANIES = "p_Clientcompanies_sel";
        const string ADD_BILLING = "p_rentacbilling_ins";
        const string ADD_BILLINGItems = "p_rentacbillingItems_ins";
        const string GET_INVOICE = "p_getInvoices";
        const string UPDATE_PAYMENT_INFO = "p_rentacBilling_updPaymentInfo";
        const string GET_PENDING_BILLS = "p_getpendingBills";
        const string GET_BILLBYID = "p_clientBillById";

        #endregion
    }
}
