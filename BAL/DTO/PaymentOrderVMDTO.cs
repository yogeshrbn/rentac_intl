using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class PaymentOrderVMDTO
    {
        public decimal Amount { get; set; }

    }
    public class PackagePurchaseDTO
    {

        public int PackageId { get; set; }
        public string MonthlyYearly { get; set; }
    }
    public class PaymentOrderUpdateVMDTO
    {
        public string order_id { get; set; }
        public string payment_id { get; set; }
        public string payment_signature { get; set; }
        public string description { get; set; }
        public string reason { get; set; }

        /// <summary>
        /// it comes only when payment is failed
        /// </summary>
        public string code { get; set; }
        public string source { get; set; }



    }

    public class CreatePaymentOrderDto
    {
        public decimal Amount { get; set; }
        public int CompanyId { get; set; }
        public int PayerLedgerId { get; set; }
    }

    public class PaymentOrderDTO
    {
        public string RazorPayKey { get; set; }
        public decimal Amount { get; set; }
        public string orderId { get; set; }
        public string Status { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime PaymentDate { get; set; }
        public string UniqueId { get; set; }
        public int ClientId { get; set; }
        public int CompanyId { get; set; }
        public int PayeeCompanyId { get; set; }
        public int PayerLedgerId { get; set; }
        public string Error { get; set; }
        /// <summary>
        /// Razorpay payment Id
        /// </summary>s
        public string payment_id { get; set; }
        public string payment_signature { get; set; }
        public string InvoiceNumber { get; set; }
        public int PackageId { get; set; }
        public string description { get; set; }
        public string reason { get; set; }
        public string code { get; set; }
        public string source { get; set; }
        public string PayerMobile { get; set; }
        public string PayerName { get; set; }
        public string PayeeCompany { get; set; }

        public string PaymentType { get; set; }
    }
    public class PaymentOrderResponse
    {
        public string id { get; set; }
        public string entity { get; set; }
        public double amount { get; set; }
        public string currency { get; set; }
        public string receipt { get; set; }
        public string offer_id { get; set; }
        public string status { get; set; }
        public string attempts { get; set; }
        public string[] notes { get; set; }
        public string created_at { get; set; }

        public PaymentOrderErrorResponse error { get; set; }

    }

    public class PaymentOrderErrorResponse
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string source { get; set; }
        public string step { get; set; }
        public string reason { get; set; }
        public string field { get; set; }
    }

    public class PaymentOrderResponseVM
    {
        public string orderId { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
    }

    //public class RazorPayWebHookDto
    //{
    //    public string account_id { get; set; }
    //    public string event { get; set; }

    //}
    //public class RazorPayWebHookDtoPayLoad
    //{

    //}
    //public class RazorPayWebHookDtoPayLoadPayment
    //{

    //}
    //public class RazorPayWebHookDtoPayLoadPaymentEntity
    //{

    //}
}
