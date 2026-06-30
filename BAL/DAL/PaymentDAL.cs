using BAL.DTO;
using BAL.Objects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DAL
{
    public class PaymentDAL
    {
        public int CreateOrder(PaymentOrderDTO payment)
        {
            SQL objSql = new SQL();
            try
            {

                objSql.BeginTransaction();
                objSql.NewCommand();

                objSql.AddParameter("@amount", DbType.Double, ParameterDirection.Input, 0, payment.Amount);
                objSql.AddParameter("@uniqueId", DbType.String, ParameterDirection.Input, 0, payment.UniqueId);
                objSql.AddParameter("@creationDate", DbType.DateTime, ParameterDirection.Input, 0, payment.CreationDate);
                objSql.AddParameter("@status", DbType.String, ParameterDirection.Input, 0, payment.Status);
                objSql.AddParameter("@clientId", DbType.Int32, ParameterDirection.Input, 0, payment.ClientId);
                objSql.AddParameter("@error", DbType.Int32, ParameterDirection.Input, 0, payment.Error);
                objSql.AddParameter("@PackageId", DbType.Int32, ParameterDirection.Input, 0, payment.PackageId);
                objSql.AddParameter("@payerName", DbType.String, ParameterDirection.Input, 0, payment.PayerName);
                objSql.AddParameter("@payerMobile", DbType.String, ParameterDirection.Input, 0, payment.PayerMobile);
                objSql.AddParameter("@paymentType", DbType.String, ParameterDirection.Input, 0, payment.PaymentType);
                objSql.AddParameter("@payeeCompanyId", DbType.Int32, ParameterDirection.Input, 0, payment.PayeeCompanyId);
                objSql.AddParameter("@payerLedgerId", DbType.Int32, ParameterDirection.Input, 0, payment.PayerLedgerId);


                var order = objSql.ExecuteNonQuery(CREATE_ORDER);
                objSql.Commit();
                objSql.Dispose();
                return order;
            }
            catch (Exception ex)
            {
                objSql.Rollback();
                objSql.Dispose();
                throw ex;
            }
        }
        public int UpdateOrder(PaymentOrderDTO payment)
        {
            SQL objSql = new SQL();
            try
            {

                objSql.BeginTransaction();
                objSql.NewCommand();


                if (payment.PaymentDate != DateTime.MinValue)
                {
                    objSql.AddParameter("@paymentDate", DbType.DateTime, ParameterDirection.Input, 0, payment.PaymentDate);
                }

                objSql.AddParameter("@status", DbType.String, ParameterDirection.Input, 0, payment.Status);
                objSql.AddParameter("@error", DbType.String, ParameterDirection.Input, 0, payment.Error);
                objSql.AddParameter("@payment_id", DbType.String, ParameterDirection.Input, 0, payment.payment_id);
                objSql.AddParameter("@payment_signature", DbType.String, ParameterDirection.Input, 0, payment.payment_signature);
                int ret = 0;
                objSql.AddParameter("@orderId", DbType.String, ParameterDirection.Input, 0, payment.orderId);

                objSql.AddParameter("@uniqueId", DbType.String, ParameterDirection.Input, 0, payment.UniqueId);
                ret = objSql.ExecuteNonQuery(UPDATE_ORDER);



                objSql.Commit();
                objSql.Dispose();
                return ret;
            }
            catch (Exception ex)
            {
                objSql.Rollback();
                objSql.Dispose();
                throw ex;
            }
        }

        public PaymentOrderDTO UpdateOrderByOrderId(PaymentOrderDTO payment)
        {
            SQL objSql = new SQL();
            try
            {

                objSql.BeginTransaction();
                objSql.NewCommand();


                if (payment.PaymentDate != DateTime.MinValue)
                {
                    objSql.AddParameter("@paymentDate", DbType.DateTime, ParameterDirection.Input, 0, payment.PaymentDate);
                }

                objSql.AddParameter("@status", DbType.String, ParameterDirection.Input, 0, payment.Status);
                objSql.AddParameter("@error", DbType.String, ParameterDirection.Input, 0, payment.Error);
                objSql.AddParameter("@payment_id", DbType.String, ParameterDirection.Input, 0, payment.payment_id);
                objSql.AddParameter("@payment_signature", DbType.String, ParameterDirection.Input, 0, payment.payment_signature);


                objSql.AddParameter("@orderId", DbType.String, ParameterDirection.Input, 0, payment.orderId);
                objSql.AddParameter("@description", DbType.String, ParameterDirection.Input, 0, payment.description);
                objSql.AddParameter("@code", DbType.String, ParameterDirection.Input, 0, payment.code);
                objSql.AddParameter("@source", DbType.String, ParameterDirection.Input, 0, payment.source);
                objSql.AddParameter("@reason", DbType.String, ParameterDirection.Input, 0, payment.reason);

                DataSet ds = objSql.ExecuteDataSet(UPDATEBYORDERID_ORDER);



                objSql.Commit();
                objSql.Dispose();
                if (payment.Status == "success")
                {
                    var ret = (from d in ds.Tables[0].AsEnumerable()
                               select new PaymentOrderDTO
                               {
                                   payment_id = d.Field<string>("payment_id"),
                                   Amount = d.Field<decimal>("amount"),
                                   PaymentDate = d.Field<DateTime>("paymentdate"),
                                   PackageId = d.Field<Int32>("PackageId"),
                                   Status = d.Field<string>("status"),
                                   ClientId = d.Field<Int32>("clientId"),
                                   PaymentType = d["payment_type"] == DBNull.Value ? "" : d.Field<string>("payment_type"),
                                   PayeeCompanyId = d["payeeCompanyId"] == DBNull.Value ? 0 : d.Field<Int32>("payeeCompanyId"),
                                   PayerMobile = d["payerMobile"] == DBNull.Value ? "" : d.Field<string>("payerMobile"),
                                   PayerLedgerId = d["PayerLedgerId"] == DBNull.Value ? 0 : d.Field<Int32>("PayerLedgerId"),
                               }
                             ).FirstOrDefault();
                    return ret;
                }
                else return payment;

            }
            catch (Exception ex)
            {
                objSql.Rollback();
                objSql.Dispose();
                throw ex;
            }
        }

        public PaymentOrderDTO GetOrderByPaymentId(string payment_id)
        {
            using (var sql = new SQL())
            {
                sql.AddParameter("@payment_id", DbType.String, ParameterDirection.Input, 0, payment_id);
                var ds = sql.ExecuteDataSet(GET_ORDER);

                var o = (from d in ds.Tables[0].AsEnumerable()
                         select new PaymentOrderDTO
                         {
                             payment_id = d.Field<string>("payment_id"),
                             Amount = d.Field<decimal>("amount"),
                             PaymentDate = d.Field<DateTime>("paymentdate"),
                             PackageId = d.Field<Int32>("PackageId")
                         }
                         ).FirstOrDefault();
                return o;

            }
        }
        public List<PaymentOrderDTO> GetUnsettledPayments(int clientId, DateTime fromDate, DateTime endDate)
        {
            using (var sql = new SQL())
            {
                sql.AddParameter("@clientId", DbType.Int32, ParameterDirection.Input, 0, clientId);
                sql.AddParameter("@fromDate", DbType.DateTime, ParameterDirection.Input, 0, fromDate);
                sql.AddParameter("@endDate", DbType.DateTime, ParameterDirection.Input, 0, endDate);
                var ds = sql.ExecuteDataSet(UNSETTLED_PAYMENTS);

                var o = (from d in ds.Tables[0].AsEnumerable()
                         select new PaymentOrderDTO
                         {
                             payment_id = d.Field<string>("payment_id"),
                             Amount = d.Field<decimal>("amount"),
                             PaymentDate = d.Field<DateTime>("paymentDate")
                         }
                         ).ToList();
                return o;

            }
        }
        //public List<RentacBillingDTO> GetUnsettledBills(int clientId, DateTime fromDate, DateTime endDate)
        //{
        //    using (var sql = new SQL())
        //    {
        //        sql.AddParameter("@clientId", DbType.Int32, ParameterDirection.Input, 0, clientId);
        //        sql.AddParameter("@fromDate", DbType.DateTime, ParameterDirection.Input, 0, fromDate);
        //        sql.AddParameter("@endDate", DbType.DateTime, ParameterDirection.Input, 0, endDate);
        //        var ds = sql.ExecuteDataSet(UNSETTLED_PAYMENTS);

        //        var o = (from d in ds.Tables[0].AsEnumerable()
        //                 select new RentacBillingDTO
        //                 {
        //                     Balance = d["balacne"] == null ? 0:  d.Field<decimal>("Balance"),
        //                     BillingId = d.Field<int>("id"),
        //                     PaymentDate = d.Field<DateTime>("paymentDate")

        //                 }
        //                 ).ToList();
        //        return o;

        //    }
        //}
        public List<PaymentOrderDTO> AllPayments(int clientId, DateTime fromDate, DateTime endDate)
        {
            using (var sql = new SQL())
            {
                sql.AddParameter("@clientId", DbType.Int32, ParameterDirection.Input, 0, clientId);
                sql.AddParameter("@fromDate", DbType.DateTime, ParameterDirection.Input, 0, fromDate);
                sql.AddParameter("@endDate", DbType.DateTime, ParameterDirection.Input, 0, endDate);
                var ds = sql.ExecuteDataSet(ALL_PAYMENTS);

                var o = (from d in ds.Tables[0].AsEnumerable()
                         select new PaymentOrderDTO
                         {
                             PackageId = d.Field<int>("packageId"),
                             Amount = d.Field<decimal>("amount"),
                             InvoiceNumber = d.Field<string>("InvoiceNumber"),

                             Status = d.Field<string>("status"),
                             PaymentDate = d.Field<DateTime>("paymentDate")
                         }
                         ).ToList();
                return o;

            }
        }
        public List<PaymentOrderDTO> PartyPayments(string mobile)
        {
            using (var sql = new SQL())
            {

                sql.AddParameter("@partyMobile", DbType.String, ParameterDirection.Input, 0, mobile);
                var ds = sql.ExecuteDataSet(PARTY_PAYMENTS);

                var o = (from d in ds.Tables[0].AsEnumerable()
                         select new PaymentOrderDTO
                         {
                             Amount = d.Field<decimal>("amount"),
                             PaymentDate = d.Field<DateTime>("paymentdate"),
                             PayeeCompany = d.IsNull("PayeeCompany") ? "" : d.Field<string>("PayeeCompany"),
                             Status = d.Field<string>("status"),                        
                             PaymentType = d["payment_type"] == DBNull.Value ? "" : d.Field<string>("payment_type"),                        
                             PayerMobile = d["payerMobile"] == DBNull.Value ? "" : d.Field<string>("payerMobile"),
                             PayerLedgerId = d["PayerLedgerId"] == DBNull.Value ? 0 : d.Field<Int32>("PayerLedgerId"),
                         }
                         ).ToList();
                return o;

            }
        }
        #region procedures
        const string CREATE_ORDER = "p_payments_ins";
        const string UPDATE_ORDER = "p_payments_update";
        const string UPDATEBYORDERID_ORDER = "p_payments_ByOrderId";
        const string GET_ORDER = "p_payments_sel";
        const string UNSETTLED_PAYMENTS = "p_unsettledPayments";
        const string ALL_PAYMENTS = "p_payments_all";
        const string PARTY_PAYMENTS = "p_partyPayments";
        #endregion
    }
}
