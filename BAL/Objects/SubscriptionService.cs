using BAL.DAL;
using BAL.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Objects
{
    public class SubscriptionService
    {


        public void GenrateBill(DateTime _currentDate)
        {
            var _dal = new SubscriptionDAL();
            var companies = _dal.GetCompaniesTobill();
            var rates = _dal.GetRates(_currentDate);
            foreach (var company in companies)
            {
                var _lastMonth = _currentDate;//.AddMonths(-1);
                var firstDateOfMonth = new DateTime(_lastMonth.Year, _lastMonth.Month, 1);
                var lastDateofMonth = firstDateOfMonth.AddMonths(1).AddDays(-1);
                var challans = _dal.GetMonthlyChallans(firstDateOfMonth, lastDateofMonth, company.RbnClientId, company.CompanyId);
                if (challans.Count > 0)
                {
                    if (challans.First().Challans > 0)
                    {
                        var billing = GenerateBill(rates, challans.First().Challans, company, firstDateOfMonth);
                        _dal.SaveBillig(billing);
                    }
                }
            }
            // var challans= _dal.GetMonthlyChallans()
        }


        RentacBillingDTO GenerateBill(List<SubscriptionRatesDTO> _rates, int challans, CompanyDTO company, DateTime from)
        {
            var _listItems = new List<RentacBillingDetailsDTO>();
            decimal billed = 0;
            foreach (var rate in _rates.OrderBy(o => o.Start))
            {
                billed = _listItems.Sum(o => o.Quantity);
                var item = new RentacBillingDetailsDTO();
                if (challans > rate.End)
                {
                    item.Quantity = rate.End - rate.Start;
                }
                else
                {
                    item.Quantity = challans - billed;
                }
                //if (item.Quantity == 0)
                //{
                //    continue;
                //}
                item.Range = rate.Start.ToString() + " - " + rate.End.ToString();
                item.ItemType = "Challan";
                item.Name = "Challan";
                item.Rate = rate.Rate;
                item.Amount = item.Quantity * item.Rate;
                item.CreationDate = DateTime.Now;
                item.Remarks = "Challans item";
                _listItems.Add(item);
            }
            var billingDTO = new RentacBillingDTO();
            billingDTO.BillingAddress = company.Address1 + " " + company.Address2;
            billingDTO.BillingCity = String.IsNullOrEmpty(company.City) ? "" : company.City;
            billingDTO.BillingPinCode = Convert.ToInt32(company.ZipCode);
            if (company.State != null && company.State.ToLower().Contains("delhi"))
            {
                billingDTO.SGSTRate = 9;
                billingDTO.CGSTRate = 9;
            }
            else
            {
                billingDTO.IGSTRate = 18;
            }
            billingDTO.SubTotal = _listItems.Sum(o => o.Amount);
            billingDTO.IGST = billingDTO.IGSTRate * billingDTO.SubTotal / 100;
            billingDTO.SGST = billingDTO.SGSTRate * billingDTO.SubTotal / 100;
            billingDTO.CGST = billingDTO.CGSTRate * billingDTO.SubTotal / 100;
            billingDTO.Tax = billingDTO.IGST + billingDTO.CGST + billingDTO.SGST;
            billingDTO.TotalItems = _listItems.Count();
            billingDTO.TotalAmount = billingDTO.SubTotal + billingDTO.Tax;
            billingDTO.Month = from.Month;
            billingDTO.Year = from.Year;
            billingDTO.CreationDate = DateTime.Now;
            billingDTO.DueDate = DateTime.Now.AddDays(7);
            billingDTO.ClientId = company.RbnClientId;
            billingDTO.CompanyId = company.CompanyId;

            billingDTO.Details = _listItems;
            billingDTO.Remarks = "";
            return billingDTO;

        }

        /// <summary>
        /// Get Unpaid invoice
        /// </summary>
        /// <param name="bill"></param>
        /// <returns></returns>
        public List<RentacBillingDTO> GetInvoice(RentacBillingFilterDTO filter)
        {
            var _dal = new SubscriptionDAL();
            return _dal.GetInvoice(filter);
        }

        //public List<RentacBillingDTO> GetPendingInvoice(RentacBillingFilterDTO bill)
        //{
        //    var _dal = new SubscriptionDAL();
        //    return _dal.GetPendingInvoice(bill);
        //}

        public int updateInvoice(List<RentacBillingDTO> bills)
        {
            var _dal = new SubscriptionDAL();
            return _dal.updateInvoice(bills);
        }

        //public string GetByIdToPrint(RentacBillingDTO bill)
        //{
        //    var _dal = new SubscriptionDAL();
        //    var ds = _dal.GetByIdToPrint(bill);
        //    return ds.GetXml();
        //}

        public DataSet GetByIdToPrint(RentacBillingDTO bill)
        {
            var _dal = new SubscriptionDAL();
            var ds = _dal.GetByIdToPrint(bill);
            return ds;
        }
    }
}
