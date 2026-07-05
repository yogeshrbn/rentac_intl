using BAL.Common;
using BAL.DAL;
using BAL.DTO;
using BAL.Enums;
using BAL.Exceptions;
using BAL.Models;
using Omu.ValueInjecter;
using Org.BouncyCastle.Crypto.Prng;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace BAL.Objects
{
    public class Billing
    {
        static CultureInfo culture = new CultureInfo("en-GB");
        public async Task<RentBillDto> GenerateBill(BillingDTO bdto, UserDTO userInfo)
        {
            var billData = new RentBillDto();
            BillingDAL objBillingDAL = new BillingDAL();
            bdto.CompanyId = userInfo.DefaultCompanyId;
            //  var data = objBillingDAL.GeneratBill(bdto.LedgerId, bdto.From, bdto.To, bdto.LedgerSiteId, userInfo.FinYearId);
            var data = objBillingDAL.GeneratBill(bdto, userInfo.FinYearId);
            if (bdto.IncludeLostItems)
            {
                billData.LostItems = (await objBillingDAL.GetLossItemsToBill(bdto, userInfo.FinYearId)).ToList();
            }

            var lstBillBalance = objBillingDAL.GetBalanceAffterLastBill(bdto.LedgerId, bdto.LedgerSiteId, bdto.InvoiceId);
            DateTime lastBillDate = new DateTime();

            var config = new Config();
            var configs = config.GetBillingConfig(bdto.CompanyId);
            bool billUptoEndDate = false;
            var recheckbalance = configs.Where(o => o.Key == "recheckbalance").FirstOrDefault();
            var billUptoEndDateConfig = configs.Where(o => o.Key.ToLower() == "billuptoenddate").FirstOrDefault();
            if (billUptoEndDateConfig != null)
            {
                billUptoEndDate = Convert.ToInt16(billUptoEndDateConfig.Value) == 1;
            }
            var blnRecheckBlalance = false;
            if (recheckbalance != null)
            {
                if (recheckbalance.Value.ToUpper() == "TRUE" || recheckbalance.Value == "1")
                {
                    blnRecheckBlalance = true;
                }
            }
            //&& (blnRecheckBlalance == false || data.Count() ==0)
            if (lstBillBalance != null && lstBillBalance.Count > 0
                )
            {
                lastBillDate = Convert.ToDateTime(lstBillBalance[0].To);
                if (lastBillDate < bdto.To)
                {
                    foreach (var f in lstBillBalance)
                    {
                        //  var itemInCurrentBill = data.Where(o => o.ProductId == f.ProductId).OrderBy(o => Convert.ToDateTime(o.To)).LastOrDefault();

                        //var minFirstDateInCurrentBill = data.Min(o => Convert.ToDateTime(o.From));

                        //if (minFirstDateInCurrentBill != null && minFirstDateInCurrentBill.Year > 2000
                        //    && minFirstDateInCurrentBill > Convert.ToDateTime(lstBillBalance.First().To)
                        //    )
                        //{
                        //if (f.CB <= 0) continue;

                        //data.Add(new BillingItemDTO
                        //{
                        //    ProductId = f.ProductId,
                        //    Item = f.Product,
                        //    Quantity = f.CB,
                        //    ItemCategory = 1012,
                        //    ClosingBalance = f.CB,
                        //    From = f.To.AddDays(1),
                        //    TranType = 3,
                        //    Freight = data.Count() > 0 ? data[0].Freight : 0,
                        //    //  To = minFirstDateInCurrentBill.ToShortDateString(),
                        //    Rate = f.Rate
                        //});
                        //}
                    }
                }
            }
            var company = new Company(bdto.CompanyId).GetDetails();
            //if (company.RbnClientId == 83)
            //{
            //    billData.BillingItems = CalculateBillAmount2(bdto, data.OrderBy(o => Convert.ToDateTime(o.From)).ThenBy(o => o.Item).ToList(),
            //        Convert.ToDateTime(bdto.To, culture),
            //        userInfo, lastBillDate
            //        );
            //}else
            //{

            var objLedger = new Ledger(bdto.LedgerId);
            //if (data.Count == 0)
            //{
            // var recheckbalance = configs.Where(o => o.Key == "recheckbalance").FirstOrDefault();
            IEnumerable<BillingItemDTO> balanceAsof1DayBeforeFromDate = null;
            var balanceType = bdto.InvoiceType == 8 ? "hire" : "rent";
            string poNumber = bdto.FilterChallansByPO ? bdto.PONumber : "";
            var dsClientWiseBalance = objLedger.ClientWiseItems(bdto.LedgerId, bdto.LedgerSiteId, bdto.CompanyId,
                bdto.From.AddDays(-1).ToShortDateString(), bdto.From.AddDays(-1).ToShortDateString(), poNumber, balanceType, 1);
            if (dsClientWiseBalance != null)
            {
                balanceAsof1DayBeforeFromDate = (from d in dsClientWiseBalance.Tables[0].AsEnumerable()
                                                 select new BillingItemDTO
                                                 {
                                                     ProductId = d.Field<int>("ProductId"),
                                                     Item = d.Field<string>("Product"),
                                                     ItemCategory = d.Field<int>("ItemCategory"),
                                                     From = bdto.From,
                                                     Rate = 0,
                                                     Quantity = Convert.ToInt32(d.Field<decimal>("ClosingBalance")),
                                                     ClosingBalance = Convert.ToInt32(d.Field<decimal>("ClosingBalance")),
                                                     ExcessQty = Convert.ToInt32(d.Field<decimal>("ExcessQty")),
                                                     Product = d.Field<string>("Product"),
                                                     Freight = 0,
                                                     FreightTax = 0,
                                                 }).ToList();
                //if (recheckbalance != null)
                //{
                //    if (recheckbalance.Value.ToUpper() == "TRUE" || recheckbalance.Value == "1")
                //    {
                bool billNegativeQty = false;
                if (configs != null)
                {
                    var billnegativeConfig = configs.Where(o => o.SubCategory.ToLower() == "billing" &&
                    o.Key.ToLower() == "billnegativeqty").FirstOrDefault();
                    if (billnegativeConfig != null && !String.IsNullOrEmpty(billnegativeConfig.Value))
                    {
                        // billNegativeQty = Convert.ToBoolean(billnegativeConfig.Value);
                        if (!bool.TryParse(billnegativeConfig.Value, out billNegativeQty))
                        {
                            throw new Exception("Invalid setting 'bill negative quantity'. Please fix under Settings > Billing");
                        }
                    }
                }
                foreach (var f in balanceAsof1DayBeforeFromDate)
                {
                    var itemInCurrentBill = data.Where(o => o.ProductId == f.ProductId && o.ChallanId == 0).
                        OrderBy(o => o.From).FirstOrDefault(); //.OrderBy(o=> Convert.ToDateTime( o.From)).FirstOrDefault();

                    //if trying to generate bill in previous dates which is earlier than the last generated bill date.
                    //in that case do not check the last balance otherwise it will re-create the bill of already billed items.
                    if (lstBillBalance != null && lstBillBalance.Count > 0)
                    {
                        lastBillDate = Convert.ToDateTime(lstBillBalance[0].To);
                        // if FilterChallansByPO true then get the last bill genrated of the provided PO.and compare that
                        //it is not done yet.
                        if (lastBillDate > bdto.From
                            && !bdto.FilterChallansByPO
                            )

                        {
                            continue;
                        }
                    }

                    if (f.ProductId == 3389)
                    {

                    }
                    if (itemInCurrentBill != null && itemInCurrentBill.TranType == 3)
                    {
                        itemInCurrentBill.Quantity = f.Quantity;
                        continue;
                    }



                    //var minFirstDateInCurrentBill = data.Min(o => Convert.ToDateTime(o.From));

                    //if (minFirstDateInCurrentBill != null && minFirstDateInCurrentBill.Year > 2000
                    //    && minFirstDateInCurrentBill > Convert.ToDateTime(lstBillBalance.First().To)
                    //    )
                    //{
                    // if (itemInCurrentBill == null) { continue; }
                    if (f.Quantity == 0) continue;
                    if (f.Quantity <= 0 && !billNegativeQty) continue;



                    data.Insert(0, new BillingItemDTO
                    {
                        ProductId = f.ProductId,
                        Item = f.Product,
                        OPB = f.ClosingBalance,
                        OpBalance = f.ClosingBalance,
                        Quantity = f.ClosingBalance,
                        ItemCategory = f.ItemCategory,
                        ClosingBalance = f.ClosingBalance,
                        From = bdto.From,
                        TranType = 3,
                        Freight = 0,
                        //  To = minFirstDateInCurrentBill.ToShortDateString(),
                        Rate = f.Rate
                    });
                    //}
                }
                //    }
                //}
            }
            //}
            var rentLineSource = data.Where(o => !o.IsBreakage).ToList();
            if (billUptoEndDate && data.Count() > 0)
            {
                var r = new RentCalculator();
                billData.BillingItems = await r.CalculateRentPeriods(data, bdto);

            }
            else
            {
                billData.BillingItems = CalculateBillAmount3(bdto, rentLineSource.OrderBy(o => Convert.ToDateTime(o.From)).ThenBy(o => o.Item).ToList(),
                              Convert.ToDateTime(bdto.To, culture),
                              userInfo, lastBillDate
                              );
            }

            billData.BillingItems = billData.BillingItems.Where(o => !o.IsBreakage).ToList();
            if (billData.BillingItems.Count > 0)
            {
                var freightRow = data.Where(o => o.Freight > 0).FirstOrDefault();
                if (freightRow != null)
                {
                    billData.BillingItems[0].Freight = freightRow.Freight;
                    billData.BillingItems[0].FreightTax = freightRow.FreightTax;
                }
                var latestPerProduct = billData.BillingItems
               .GroupBy(r => r.ProductId)
               .Select(g => g.OrderByDescending(r => r.From).First())
               .ToList();

                var balanceAsOnEndDate = objLedger.ClientWiseItems(bdto.LedgerId, bdto.LedgerSiteId, bdto.CompanyId,
                bdto.To.ToShortDateString(), bdto.To.ToShortDateString(), poNumber, balanceType);
                IEnumerable<BillingItemDTO> balanceAsofEndDate = null;
                if (balanceAsOnEndDate != null)
                {
                    balanceAsofEndDate = (from d in balanceAsOnEndDate.Tables[0].AsEnumerable()
                                          select new BillingItemDTO
                                          {
                                              ProductId = d.Field<int>("ProductId"),
                                              Item = d.Field<string>("Product"),
                                              ItemCategory = d.Field<int>("ItemCategory"),
                                              From = bdto.From,
                                              Rate = 0,
                                              Quantity = Convert.ToInt32(d.Field<decimal>("ClosingBalance")),
                                              ClosingBalance = Convert.ToInt32(d.Field<decimal>("ClosingBalance")),
                                              ExcessQty = Convert.ToInt32(d.Field<decimal>("ExcessQty")),
                                              Product = d.Field<string>("Product"),
                                              Freight = 0,
                                              FreightTax = 0,
                                          }).ToList().Where(o => o.ClosingBalance > 0);
                }

                if (billUptoEndDate)
                {
                    foreach (var p in latestPerProduct)
                    {
                        if (balanceAsofEndDate != null)
                        {
                            var blanceItem = balanceAsofEndDate.Where(o => o.ProductId == p.ProductId).FirstOrDefault();
                            if (blanceItem != null)
                            {
                                p.CB = p.ClosingBalance = p.Closingbalance = blanceItem.ClosingBalance;
                            }
                        }
                    }
                }
                var firstRowPerProduct = billData.BillingItems
                 .GroupBy(r => r.ProductId)
                 .Select(g => g.OrderBy(r => r.From).First())
                 .ToList();

                foreach (var p in firstRowPerProduct)
                {
                    if (lstBillBalance != null)
                    {
                        var blanceItem = lstBillBalance.Where(o => o.ProductId == p.ProductId && o.From == p.From).FirstOrDefault();
                        if (blanceItem != null)
                        {
                            p.Rate = 0;
                            p.Amount = 0;
                            p.Days = 0;
                        }
                    }
                }
            }
            //}
            billData.Challans = rentLineSource.Select(o => new BillChallanDto
            {
                Type = o.TranType,
                ChallanId = o.ChallanId,
                ChallanNumber = o.JobNumber,
                LedgerId = o.LedgerId,

            }).Where(o => o.ChallanId > 0 && o.Type != 3).GroupBy(o => o.ChallanId).Select(g => g.First()).ToList();
            var poNumbers = data.Where(o => !String.IsNullOrEmpty(o.PONumber)).Select(o =>
            new BillPODto
            {
                PONumber = o.PONumber
            }
            ).GroupBy(o => o.PONumber).Select(g => g.First()).ToList();

            if (poNumber != null)
            {
                billData.PO = poNumbers;
            }
            Product objProduct = new Product();
            var allProducts = await objProduct.GetAll(bdto.CompanyId);
            var taxCategories = await objBillingDAL.GetAllTaxCategories();

            var client = objLedger.GetDetails();
            var taxes = new List<TaxCategoryDTO>();

            billData.AccountLedger = objLedger.GetAccountLedger(bdto.LedgerId, bdto.From.ToShortDateString(), bdto.To.ToShortDateString(),
                bdto.LedgerSiteId, bdto.CompanyId, userInfo.FinYearId).FirstOrDefault();

            var taxConfig = configs.Where(o => o.Key == "applyTax").FirstOrDefault();

            if (billData.AccountLedger != null)
            {
                if (billData.AccountLedger.OBType == 1)
                {
                    billData.AccountLedger.Balance = billData.AccountLedger.Credit - (billData.AccountLedger.Debit + billData.AccountLedger.OpeningBalance);
                }
                else
                {
                    billData.AccountLedger.Balance = billData.AccountLedger.Credit + billData.AccountLedger.OpeningBalance - billData.AccountLedger.Debit;
                }
                billData.AccountLedger.BalanceType = 2;
                if (billData.AccountLedger.Balance < 0)
                {
                    billData.AccountLedger.BalanceType = 1;
                }
                billData.AccountLedger.Balance = Math.Abs(billData.AccountLedger.Balance);
            }
            if (taxConfig == null)
            {
                bdto.ApplyTax = true;
            }
            billData.ApplyTax = bdto.ApplyTax;
            var site = objLedger.GetSiteById(bdto.LedgerSiteId);
            var clientSateId = client.StateId;
            if (site.UseForBilling == 1)
            {
                clientSateId = site.StateId;
            }
            foreach (var bItem in billData.BillingItems)
            {
                var p = allProducts.Where(o => o.ProductId == bItem.ProductId).FirstOrDefault();
                if (p != null)
                {
                    bItem.TaxCategoryId = p.TaxCategoryId;
                    var tax = taxCategories.Where(o => o.TaxCategoryId == p.TaxCategoryId).FirstOrDefault();
                    if (tax != null)
                    {
                        if (clientSateId == company.StateId)
                        {
                            bItem.SGSTRate = tax.SGST;
                            bItem.CGSTRate = tax.CGST;
                        }
                        else
                        {
                            bItem.IGSTRate = tax.IGST;
                        }
                        if (!bdto.ApplyTax)
                        {
                            bItem.SGSTRate = 0;
                            bItem.CGSTRate = 0;
                            bItem.IGSTRate = 0;
                        }

                        bItem.IGST = (bItem.IGSTRate * bItem.Amount) / 100;
                        bItem.SGST = (bItem.SGSTRate * bItem.Amount) / 100;
                        bItem.CGST = (bItem.CGSTRate * bItem.Amount) / 100;
                    }

                }
            }
            var ledger = new Ledger();

            var rates = ledger.GetProductRates(bdto.LedgerId, bdto.LedgerSiteId, bdto.CompanyId);

            if (bdto.IncludeLostItems)
            {
                billData.LostItems = (await objBillingDAL.GetLossItemsToBill(bdto, userInfo.FinYearId)).ToList();
                foreach (var bItem in billData.LostItems)
                {
                    var p = allProducts.Where(o => o.ProductId == bItem.ProductId).FirstOrDefault();
                    if (p != null)
                    {
                        bItem.TaxCategoryId = p.TaxCategoryId;
                        var rate = rates.Where(o => o.ProductId == bItem.ProductId).FirstOrDefault();
                        if (rate != null)
                        {
                            bItem.Rate = rate.LossRate;
                            bItem.LossCharges = rate.LossRate;
                        }
                        else
                        {
                            bItem.LossCharges = p.LossRate;
                        }
                        bItem.Amount = bItem.Rate * bItem.Quantity;
                        var tax = taxCategories.Where(o => o.TaxCategoryId == p.TaxCategoryId).FirstOrDefault();
                        if (tax != null)
                        {
                            if (clientSateId == company.StateId)
                            {
                                bItem.SGSTRate = tax.SGST;
                                bItem.CGSTRate = tax.CGST;
                            }
                            else
                                bItem.IGSTRate = tax.IGST;


                            if (!bdto.ApplyTax)
                            {
                                bItem.SGSTRate = 0;
                                bItem.CGSTRate = 0;
                                bItem.IGSTRate = 0;
                            }

                            bItem.IGST = (bItem.IGSTRate * bItem.Amount) / 100;
                            bItem.SGST = (bItem.SGSTRate * bItem.Amount) / 100;
                            bItem.CGST = (bItem.CGSTRate * bItem.Amount) / 100;
                        }
                    }
                }
            }

            if (bdto.IncludeBreakageItems)
            {
                billData.Breakage = (await objBillingDAL.GetBreakageForBill(bdto.CompanyId, bdto.LedgerId, bdto.InvoiceId, bdto.LedgerSiteId,
                    bdto.From, bdto.To, bdto.FinYearId)).ToList();
                foreach (var bItem in billData.Breakage)
                {
                    var p = allProducts.Where(o => o.ProductId == bItem.ProductId).FirstOrDefault();
                    if (p != null)
                    {
                        var rate = rates.Where(o => o.ProductId == bItem.ProductId).FirstOrDefault();
                        bItem.TaxCategoryId = p.TaxCategoryId;
                        if (rate != null)
                        {
                            bItem.Rate = rate.DamageRate;
                            bItem.BreakageRate = rate.DamageRate;
                        }
                        else
                        {
                            bItem.Rate = p.BrekageRate;
                            bItem.BreakageRate = p.BrekageRate;
                        }
                        bItem.Amount = bItem.Rate * bItem.Quantity;
                        var tax = taxCategories.Where(o => o.TaxCategoryId == p.TaxCategoryId).FirstOrDefault();
                        if (tax != null)
                        {
                            if (clientSateId == company.StateId)
                            {
                                bItem.SGSTRate = tax.SGST;
                                bItem.CGSTRate = tax.CGST;
                            }
                            else
                                bItem.IGSTRate = tax.IGST;

                            if (!bdto.ApplyTax)
                            {
                                bItem.SGSTRate = 0;
                                bItem.CGSTRate = 0;
                                bItem.IGSTRate = 0;
                            }

                            bItem.IGST = (bItem.IGSTRate * bItem.Amount) / 100;
                            bItem.SGST = (bItem.SGSTRate * bItem.Amount) / 100;
                            bItem.CGST = (bItem.CGSTRate * bItem.Amount) / 100;
                        }

                    }
                }

                var finYearId = bdto.FinYearId > 0 ? bdto.FinYearId : userInfo.FinYearId;
                //billData.BreakageDamageDetails = (await objBillingDAL.GetBreakageDamageDetailsForBill(
                //    bdto.CompanyId, bdto.LedgerId, bdto.InvoiceId, bdto.LedgerSiteId, bdto.From, bdto.To, finYearId)).ToList();
                billData.BreakageDamageDetails = (await objBillingDAL.GetBreakageDamageDetailsForSeparateBill(bdto.CompanyId,
                    bdto.LedgerId, bdto.InvoiceId, bdto.LedgerSiteId)).ToList();
                ApplyGstToBreakageDamageDetails(billData.BreakageDamageDetails, allProducts, taxCategories, clientSateId, company.StateId, bdto.ApplyTax);
            }
            else
            {
                billData.BreakageDamageDetails = new List<BreakageDamageDetailDTO>();
            }

            var dsCharges = objBillingDAL.getOhterChargesToBill(bdto.LedgerSiteId, bdto.CompanyId, bdto.From, bdto.To);
            if (dsCharges != null)
            {
                var charges = (from d in dsCharges.Tables[0].AsEnumerable()
                               select new InvoiceChargeDTO
                               {
                                   Amount = Convert.ToDouble(d["Amount"]),
                                   ChargeId = Convert.ToInt32(d["ChargeId"]),
                                   Name = Convert.ToString(d["Name"]),
                                   ChallanId = Convert.ToInt32(d["ChallanId"]),
                                   ChallanType = Convert.ToByte(d["ChallanType"])

                               }).ToList();

                var otherCharges = new List<InvoiceChargeDTO>();

                foreach (var charge in charges)
                {

                    var challanExists = billData.Challans.Where(o => o.ChallanId == charge.ChallanId && o.Type == charge.ChallanType).FirstOrDefault();
                    if (challanExists != null)
                    {
                        otherCharges.Add(charge);
                    }
                    //only to list all charge types on UI
                    if (charge.Amount == 0)
                    {
                        otherCharges.Add(charge);
                    }

                }
                billData.OtherCharges = (from s in otherCharges
                                         group s by s.ChargeId into g
                                         select new InvoiceChargeDTO
                                         {
                                             ChargeId = g.Key,
                                             Amount = g.Sum(o => o.Amount),
                                             Name = g.First().Name
                                         }

                                        ).ToList();

            }

            return billData;
        }

        /// <summary>Sets IGST/CGST/SGST rates and tax amounts on GRN damage rows from product tax category.</summary>
        private static void ApplyGstToBreakageDamageDetails(
            List<BreakageDamageDetailDTO> details,
            IEnumerable<ProductDTO> allProducts,
            IEnumerable<TaxCategoryDTO> taxCategories,
            int clientStateId,
            int companyStateId,
            bool applyTax)
        {
            if (details == null || details.Count == 0 || allProducts == null || taxCategories == null)
                return;
            foreach (var row in details)
            {
                var p = allProducts.FirstOrDefault(o => o.ProductId == row.ProductId);
                if (p == null)
                    continue;
                var tax = taxCategories.FirstOrDefault(o => o.TaxCategoryId == p.TaxCategoryId);
                if (tax == null)
                    continue;
                double igstRate = 0, cgstRate = 0, sgstRate = 0;
                row.TaxCategoryId = p.TaxCategoryId;
                if (clientStateId == companyStateId)
                {
                    sgstRate = tax.SGST;
                    cgstRate = tax.CGST;
                }
                else
                    igstRate = tax.IGST;
                if (!applyTax)
                {
                    igstRate = cgstRate = sgstRate = 0;
                }
                row.IGSTRate = igstRate;
                row.CGSTRate = cgstRate;
                row.SGSTRate = sgstRate;
                var amt = Convert.ToDouble(row.Cost);
                row.IGST = Math.Round(amt * igstRate / 100.0, 2, MidpointRounding.AwayFromZero);
                row.SGST = Math.Round(amt * sgstRate / 100.0, 2, MidpointRounding.AwayFromZero);
                row.CGST = Math.Round(amt * cgstRate / 100.0, 2, MidpointRounding.AwayFromZero);
            }
        }

        public async Task<BillingDTO> GetByIdForEdit(BillingDTO billdto)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            var dto = await objBillingDAL.GetByIdForEdit(billdto);
            if (dto == null || dto.CompanyId <= 0)
                return dto;
            if (dto.BreakageDamageDetails != null && dto.BreakageDamageDetails.Count > 0)
            {
                var company = new Company(dto.CompanyId).GetDetails();
                var objLedger = new Ledger(dto.LedgerId);
                var client = objLedger.GetDetails();
                var site = objLedger.GetSiteById(dto.LedgerSiteId);
                var clientSateId = client.StateId;
                if (site != null && site.UseForBilling == 1)
                    clientSateId = site.StateId;
                var objProduct = new Product();
                var allProducts = await objProduct.GetAll(dto.CompanyId);
                var taxCategories = await objBillingDAL.GetAllTaxCategories();
                ApplyGstToBreakageDamageDetails(dto.BreakageDamageDetails, allProducts, taxCategories, clientSateId, company.StateId, dto.ApplyTax);
            }
            return dto;
        }
        List<BillingItemDTO> CalculateBillAmount2(BillingDTO bdto, List<BillingItemDTO> items, DateTime to, UserDTO userInfo, DateTime lastBillDate)
        {
            return CalculateBillAmountWithBillEndMode(bdto, items, to, userInfo, lastBillDate, fixBillEndReturnOnLastDay: false);
        }

        List<BillingItemDTO> CalculateBillAmount3(BillingDTO bdto, List<BillingItemDTO> items, DateTime to, UserDTO userInfo, DateTime lastBillDate)
        {
            return CalculateBillAmountWithBillEndMode(bdto, items, to, userInfo, lastBillDate, fixBillEndReturnOnLastDay: true);
        }
        List<BillingItemDTO> CalculateBillAmountWithBillEndMode(BillingDTO bdto, List<BillingItemDTO> items, DateTime to, UserDTO userInfo, DateTime lastBillDate, bool fixBillEndReturnOnLastDay)
        {
            int companyId = userInfo.DefaultCompanyId;
            var config = new Config();

            //  bool chargSentDate = true;
            bool chargeReceivingDate = bdto.ChargeReturnDay == 1;
            bool billNegativeQty = false;
            bool hideBomComponents = false;
            bool hideZeroAmountsItems = false;
            string dayscalctype = "";
            byte dayscalctype_days = 30;
            var configs = config.GetBillingConfig(companyId);
            if (configs != null)
            {
                var billnegativeConfig = configs.Where(o => o.SubCategory.ToLower() == "billing" &&
                o.Key.ToLower() == "billnegativeqty").FirstOrDefault();
                if (billnegativeConfig != null && !String.IsNullOrEmpty(billnegativeConfig.Value))
                {
                    // billNegativeQty = Convert.ToBoolean(billnegativeConfig.Value);
                    if (!bool.TryParse(billnegativeConfig.Value, out billNegativeQty))
                    {
                        throw new Exception("Invalid setting 'bill negative quantity'. Please fix under Settings > Billing");
                    }
                }
                var configHideBomComponents = configs.Where(o => o.SubCategory.ToLower() == "billing"
                && o.Key.ToLower() == "hidezeroamountitem").FirstOrDefault();
                if (configHideBomComponents != null && !String.IsNullOrEmpty(configHideBomComponents.Value))
                {
                    // hideBomComponents = Convert.ToBoolean(configHideBomComponents.Value);
                    if (!bool.TryParse(configHideBomComponents.Value, out hideBomComponents))
                    {
                        throw new Exception("Invalid setting 'Hide bom components'. Please fix under Settings > Billing");
                    }
                }
                var confighideZeroAmountsItems = configs.Where(o => o.SubCategory.ToLower() == "billing"
                && o.Key.ToLower() == "hidebomcomponents").FirstOrDefault();
                if (confighideZeroAmountsItems != null && !String.IsNullOrEmpty(confighideZeroAmountsItems.Value))
                {
                    if (!bool.TryParse(confighideZeroAmountsItems.Value, out hideZeroAmountsItems))
                    {
                        throw new Exception("Invalid setting 'Hide Zero(0) amount items'. Please fix under Settings > Billing");
                    }
                    // hideZeroAmountsItems = Convert.ToBoolean(confighideZeroAmountsItems.Value);

                }
                var configdayscalctype = configs.Where(o => o.SubCategory.ToLower() == "billing" && o.Key.ToLower() == "dayscalctype").FirstOrDefault();
                if (configdayscalctype != null && !String.IsNullOrEmpty(configdayscalctype.Value))
                {
                    dayscalctype = Convert.ToString(configdayscalctype.Value);
                }

                var configdayscalctype_days = configs.Where(o => o.SubCategory.ToLower() == "billing" && o.Key.ToLower() == "dayscalctype_days").FirstOrDefault();
                if (configdayscalctype_days != null && !String.IsNullOrEmpty(configdayscalctype_days.Value)
                     && !String.IsNullOrEmpty(dayscalctype))
                {
                    // dayscalctype_days = Convert.ToByte(configdayscalctype_days.Value);
                    if (!byte.TryParse(configdayscalctype_days.Value, out dayscalctype_days))
                    {
                        throw new Exception("Invalid setting 'Month Days Setup'. Please fix under Settings > Billing");
                    }
                    if (dayscalctype_days <= 0)
                    {
                        throw new Exception("Invalid setting 'Month Days Setup'. Please fix under Settings > Billing");
                    }
                }
            }
            else
            {
                throw new Exception("Invalid bill setting . Please fix under Settings > Billing and save");
            }
            //if (configs != null)
            //{
            //    var chargDates = configs.Where(o => o.SubCategory.ToLower() == "chagedates");
            //    if (chargDates != null)
            //    {
            //        if (chargDates.Where(o => o.Key.ToLower() == "billreceiveddate").Count() > 0)
            //        {
            //            chargeReceivingDate = Convert.ToBoolean(chargDates.Where(o => o.Key.ToLower() == "billreceiveddate").First().Value);
            //        }
            //        if (chargDates.Where(o => o.Key.ToLower() == "billsentdate").Count() > 0)
            //        {
            //            chargSentDate = Convert.ToBoolean(chargDates.Where(o => o.Key.ToLower() == "billsentdate").First().Value);
            //        }
            //    }

            //}

            var uniqueTxn = (from item in items

                             group item by new { item.From, item.ProductId, item.ItemCategory } into g

                             select new BillingItemDTO
                             {
                                 ProductId = g.Key.ProductId,
                                 From = g.Key.From,
                                 Rate = g.First().Rate,

                                 TranType = g.First().TranType,
                                 Id = g.First().Id,
                                 //Rate = g.Key.Rate,
                                 ItemCategory = g.Key.ItemCategory,
                                 Quantity = g.Sum(o => o.Quantity),



                                 ClosingBalance = g.Sum(o => o.ClosingBalance),
                                 QtyCalculation = String.Join(" ",
                                                items.Where(o => o.From == g.Key.From &&
                                                o.ProductId == g.Key.ProductId).Select(o =>
                                                o.Quantity > 0 ? " + " + o.Quantity.ToString() : o.Quantity.ToString()
                                                )),
                                 ExcessQty = g.Sum(o => o.ExcessQty),
                                 Product = g.First().Product,
                                 Item = g.First().Item,
                                 Freight = items.First().Freight,
                                 FreightTax = items.First().FreightTax
                                 //   Id = items.IndexOf(g.Key) + 1,
                                 //GuId = Guid.NewGuid().ToString()
                             }
                             ).ToList().OrderBy(o => o.From).ToList();


            if (items == null || items.Count == 0)
            {
                throw new UDFException("Nothing to bill", ErrorCodes.NOTHING_TO_BILL);
            }
            var freshData = new List<BillingItemDTO>();
            var ledger = new Ledger();
            //var ledgerId = items.First().LedgerId;
            var rates = ledger.GetProductRates(bdto.LedgerId, bdto.LedgerSiteId, companyId);

            var applyUnit2RateConfig = configs.Where(o => o.SubCategory.ToLower() == "billing"
              && o.Key.ToLower() == "applyunit2rate").FirstOrDefault();

            bool applyUnit2Rate = false;
            if (applyUnit2RateConfig != null)
            {
                applyUnit2Rate = applyUnit2RateConfig.Value == "true" || applyUnit2RateConfig.Value == "1";
            }
            for (var i = 0; i <= uniqueTxn.Count() - 1; i++)
            {
                var s = uniqueTxn[i];
                if (s.TranType == 3)
                {
                    s.OPB = s.Quantity;
                }

                //}
                //foreach (var s in uniqueTxn)
                //{
                double qtyToBill = 0;
                var rate = rates.Find(o => o.ProductId == s.ProductId);

                s.Rate = 0;
                if (rate != null)
                {
                    s.Rate = rate.RentRate;

                    if (rate.ApplyUnit2Rate)
                    {
                        s.Rate = rate.UnitSizeRate;
                    }
                }

                if (s.ItemCategory != 1013)
                {

                    var firstProductInList = uniqueTxn.Where(o => o.ProductId == s.ProductId).First();
                    if (s.From > to)
                    {
                        //s.From = to;
                    }
                    s.GuId = Guid.NewGuid().ToString();
                    // var sIndex = uniqueTxn.IndexOf(s);
                    var nextProduct = uniqueTxn.Where(o => o.ProductId == s.ProductId && Convert.ToDateTime(o.From) >= Convert.ToDateTime(s.From)
                    && String.IsNullOrEmpty(o.GuId)
                    ).FirstOrDefault();



                    //if (lastBillDate.Year > 2000)
                    //{
                    //    if (Convert.ToDateTime(s.From, culture) <= lastBillDate)
                    //        s.From = lastBillDate.AddDays(1);//.ToShortDateString();
                    //    //item.From = lastBillDate.ToShortDateString();

                    //}
                    var returnOnBillEndDate = false;
                    if (nextProduct != null)
                    {
                        s.To = nextProduct.From;
                        nextProduct.ClosingBalance = s.ClosingBalance + nextProduct.Quantity;
                        nextProduct.From = s.To;
                        nextProduct.GuId = Guid.NewGuid().ToString();
                        if (fixBillEndReturnOnLastDay && chargeReceivingDate && nextProduct.Quantity < 0
                            && Convert.ToDateTime(s.To).Date == Convert.ToDateTime(to).Date
                            && Convert.ToDateTime(s.From).Date < Convert.ToDateTime(to).Date)
                        {
                            s.To = to.Date.AddDays(-1);
                            nextProduct.From = to.Date;
                            returnOnBillEndDate = true;
                        }
                        // nextProduct.Rate = s.Rate;
                    }
                    else
                    {
                        s.To = to;//.ToShortDateString();

                    }
                    qtyToBill = s.ClosingBalance;
                    if (Convert.ToDateTime(s.From) == Convert.ToDateTime(s.To))
                    {
                        if (s.ClosingBalance == 0)
                        {
                            var sentQty = items.Find(o => o.ProductId == s.ProductId && (Convert.ToDateTime(o.From) == Convert.ToDateTime(s.From) && o.Quantity > 0));
                            if (sentQty != null && chargeReceivingDate)
                            {
                                s.Quantity = sentQty.Quantity;
                                qtyToBill = s.Quantity;
                            }
                        }
                        else if (s.ClosingBalance > 0 && s.Quantity > 0)
                        {
                            var recQty = items.Find(o => o.ProductId == s.ProductId && (Convert.ToDateTime(o.From) == Convert.ToDateTime(s.From) && o.Quantity < 0));
                            if (recQty != null && chargeReceivingDate)
                            {
                                s.Quantity += Math.Abs(recQty.Quantity);
                                qtyToBill = s.Quantity;

                            }
                        }
                        else if (chargeReceivingDate && s.ClosingBalance > 0 && s.Quantity < 0)
                        {
                            // Return on this day: charge on-hand qty before the return (same formula as openingBalance below)
                            qtyToBill = s.ClosingBalance - s.Quantity;
                        }
                    }
                    s.Days = (Convert.ToDateTime(s.To) - Convert.ToDateTime(s.From)).Days;
                    if (s.Days < 0)
                    {
                        s.From = s.To;
                    }
                    if (returnOnBillEndDate && s.Days >= 0)
                        s.Days++;
                    if (!returnOnBillEndDate && nextProduct != null && nextProduct.Quantity < 0)
                    {
                        if (chargeReceivingDate)
                        {
                            // if (s.Quantity > 0)
                            s.Days++;
                            //if (nextProduct.Quantity > 0)
                            nextProduct.From = s.To.AddDays(1);
                        }
                        else
                        {
                            if (s.Quantity < 0 && nextProduct.Quantity < 0)
                            {
                                s.Days--;
                                s.To = s.To.AddDays(-1);
                                nextProduct.From = s.To.AddDays(1);
                            }
                        }

                    }
                    if (nextProduct != null && !chargeReceivingDate)
                    {
                        s.To = nextProduct.From.AddDays(-1);
                        nextProduct.From = s.To.AddDays(1);
                        s.Days = (Convert.ToDateTime(s.To) - Convert.ToDateTime(s.From)).Days + 1;
                    }

                    if (nextProduct == null && Convert.ToDateTime(s.To).Date == Convert.ToDateTime(to).Date)
                    {
                        // s.Days = (Convert.ToDateTime(s.To) - Convert.ToDateTime(s.From)).Days;
                        // if (s.Quantity > 0 || (s.Quantity < 0 && chargeReceivingDate))
                        s.Days++;
                    }



                    if (bdto.RateCalcType == 2 || bdto.RateCalcType == 3)
                    {
                        var d = Convert.ToDateTime(s.From);


                        if (s.Days > Utils.DaysInMonth(d))
                        {
                            var nextMonthEntry = new BillingItemDTO();
                            nextMonthEntry.InjectFrom(s);


                            s.To = Utils.LastDaysOfMonth(d);
                            s.Days = (Convert.ToDateTime(s.To) - Convert.ToDateTime(s.From)).Days;
                            s.Days++;

                            nextMonthEntry.From = s.To.AddDays(1);
                            var currentIndex = uniqueTxn.IndexOf(s);
                            uniqueTxn.Insert(currentIndex + 1, nextMonthEntry);
                        }

                        if (s.Days < Utils.DaysInMonth(d) && bdto.RateCalcType == 2)
                        {
                            if (dayscalctype == "fixed")
                            {
                                s.Rate = Math.Round(s.Rate / dayscalctype_days, 2, MidpointRounding.AwayFromZero);
                            }
                            else
                                s.Rate = Math.Round(s.Rate / Utils.DaysInMonth(d), 2, MidpointRounding.AwayFromZero);

                        }
                        else if (s.Days == Utils.DaysInMonth(d))
                        {
                            //in this case Days=1 will be considered as 1 month
                            s.Days = 1;
                        }

                    }

                    //do not charge if item is set with another item.
                    var isSetWith = items.Where(o => o.ProductId == s.ProductId && o.GroupItemId > 0).FirstOrDefault();
                    if (isSetWith != null)
                    {
                        s.Rate = 0;
                        continue;
                    }

                    var rateAndQty = s.Rate * qtyToBill;
                    if (rate == null)
                    {
                        rate = new ProductRateDTO();
                    }
                    if (rate.ApplyUnit2Rate)
                    {
                        rateAndQty = s.Rate * rate.Size * qtyToBill;
                    }
                    if (bdto.RateCalcType != 3)
                    {
                        s.Amount = rateAndQty * s.Days;
                    }
                    else
                    {
                        s.Amount = rateAndQty;
                    }
                    if (bdto.RateCalcType == 3)
                    {
                        var itemExist = freshData.Where(o => o.ProductId == s.ProductId && o.From.Month == s.To.Month).Count();
                        if (itemExist > 0)
                        {
                            s.Amount = 0;
                        }
                    }
                    double openingBalance = (s.ClosingBalance - s.Quantity);
                    s.QtyCalculation = "";
                    if (openingBalance > 0)
                    {
                        var products = uniqueTxn.Where(o => o.From == s.From &&
                                                    o.ProductId == s.ProductId);
                        if (s.ProductId == 3556)
                        {

                        }
                        if (openingBalance != 0)
                        {
                            s.QtyCalculation += openingBalance.ToString();
                        }
                        foreach (var p in products)
                        {
                            if (p.Quantity > 0 && openingBalance != 0)
                            {
                                s.QtyCalculation += " + " + p.Quantity.ToString();
                            }
                            else if (p.Quantity > 0)
                            {
                                s.QtyCalculation += p.Quantity.ToString();
                            }
                            if (p.Quantity < 0)
                            {
                                s.QtyCalculation += " " + p.Quantity.ToString();
                            }
                        }
                        if (products.Count() == 0)
                        {
                            s.QtyCalculation += s.Quantity.ToString();
                        }

                    }
                    //if (s.Amount <= 0)
                    //{
                    //    continue;
                    //}
                    //if charge returned date is checked and it is a return
                    //if (chargeReceivingDate && s.Quantity < 0)
                    //{
                    //    s.Amount += s.Rate * Math.Abs(s.Quantity) * 1;
                    //}


                }
                else if (s.ItemCategory == 1013)
                {
                    s.Days = 1;
                    s.To = s.From;
                    qtyToBill = s.Quantity;
                    s.Amount = s.Rate * qtyToBill;
                }

                if (!billNegativeQty && s.Amount < 0)
                {
                    continue;
                }
                if (hideZeroAmountsItems && s.Amount == 0)
                {
                    continue;
                }

                freshData.Add(s);
            }


            if (lastBillDate != null)
            {
                //return freshData.Where(o => Convert.ToDateTime(o.From, culture) >= lastBillDate).ToList().OrderBy(o => o.Item).ToList();
                //;
                return freshData.ToList().OrderBy(o => o.Item).ToList();

            }


            return freshData.OrderBy(o => o.Item).ToList();
        }
        /*
        List<BillingItemDTO> CalculateBillUpToEndDate(BillingDTO bdto, List<BillingItemDTO> items, DateTime to, UserDTO userInfo, DateTime lastBillDate, bool fixBillEndReturnOnLastDay)
        {
            int companyId = userInfo.DefaultCompanyId;
            var config = new Config();
            var configs = config.GetBillingConfig(companyId);
            //  bool chargSentDate = true;
            bool chargeReceivingDate = bdto.ChargeReturnDay == 1;
            bool billNegativeQty = false;
            bool hideBomComponents = false;
            bool hideZeroAmountsItems = false;
            string dayscalctype = "";
            byte dayscalctype_days = 30;

            if (configs != null)
            {
                var billnegativeConfig = configs.Where(o => o.SubCategory.ToLower() == "billing" &&
                o.Key.ToLower() == "billnegativeqty").FirstOrDefault();
                if (billnegativeConfig != null && !String.IsNullOrEmpty(billnegativeConfig.Value))
                {
                    // billNegativeQty = Convert.ToBoolean(billnegativeConfig.Value);
                    if (!bool.TryParse(billnegativeConfig.Value, out billNegativeQty))
                    {
                        throw new Exception("Invalid setting 'bill negative quantity'. Please fix under Settings > Billing");
                    }
                }
                var configHideBomComponents = configs.Where(o => o.SubCategory.ToLower() == "billing"
                && o.Key.ToLower() == "hidezeroamountitem").FirstOrDefault();
                if (configHideBomComponents != null && !String.IsNullOrEmpty(configHideBomComponents.Value))
                {
                    // hideBomComponents = Convert.ToBoolean(configHideBomComponents.Value);
                    if (!bool.TryParse(configHideBomComponents.Value, out hideBomComponents))
                    {
                        throw new Exception("Invalid setting 'Hide bom components'. Please fix under Settings > Billing");
                    }
                }
                var confighideZeroAmountsItems = configs.Where(o => o.SubCategory.ToLower() == "billing"
                && o.Key.ToLower() == "hidebomcomponents").FirstOrDefault();
                if (confighideZeroAmountsItems != null && !String.IsNullOrEmpty(confighideZeroAmountsItems.Value))
                {
                    if (!bool.TryParse(confighideZeroAmountsItems.Value, out hideZeroAmountsItems))
                    {
                        throw new Exception("Invalid setting 'Hide Zero(0) amount items'. Please fix under Settings > Billing");
                    }
                    // hideZeroAmountsItems = Convert.ToBoolean(confighideZeroAmountsItems.Value);

                }
                var configdayscalctype = configs.Where(o => o.SubCategory.ToLower() == "billing" && o.Key.ToLower() == "dayscalctype").FirstOrDefault();
                if (configdayscalctype != null && !String.IsNullOrEmpty(configdayscalctype.Value))
                {
                    dayscalctype = Convert.ToString(configdayscalctype.Value);
                }

                var configdayscalctype_days = configs.Where(o => o.SubCategory.ToLower() == "billing" && o.Key.ToLower() == "dayscalctype_days").FirstOrDefault();
                if (configdayscalctype_days != null && !String.IsNullOrEmpty(configdayscalctype_days.Value)
                     && !String.IsNullOrEmpty(dayscalctype))
                {
                    // dayscalctype_days = Convert.ToByte(configdayscalctype_days.Value);
                    if (!byte.TryParse(configdayscalctype_days.Value, out dayscalctype_days))
                    {
                        throw new Exception("Invalid setting 'Month Days Setup'. Please fix under Settings > Billing");
                    }
                    if (dayscalctype_days <= 0)
                    {
                        throw new Exception("Invalid setting 'Month Days Setup'. Please fix under Settings > Billing");
                    }
                }
            }
            else
            {
                throw new Exception("Invalid bill setting . Please fix under Settings > Billing and save");
            }
            //if (configs != null)
            //{
            //    var chargDates = configs.Where(o => o.SubCategory.ToLower() == "chagedates");
            //    if (chargDates != null)
            //    {
            //        if (chargDates.Where(o => o.Key.ToLower() == "billreceiveddate").Count() > 0)
            //        {
            //            chargeReceivingDate = Convert.ToBoolean(chargDates.Where(o => o.Key.ToLower() == "billreceiveddate").First().Value);
            //        }
            //        if (chargDates.Where(o => o.Key.ToLower() == "billsentdate").Count() > 0)
            //        {
            //            chargSentDate = Convert.ToBoolean(chargDates.Where(o => o.Key.ToLower() == "billsentdate").First().Value);
            //        }
            //    }

            //}


            var uniqueTxn = (from item in items

                             group item by new { item.From, item.ProductId, item.ItemCategory } into g

                             select new BillingItemDTO
                             {
                                 ProductId = g.Key.ProductId,
                                 From = g.Key.From,
                                 Rate = g.First().Rate,

                                 TranType = g.First().TranType,
                                 Id = g.First().Id,
                                 //Rate = g.Key.Rate,
                                 ItemCategory = g.Key.ItemCategory,
                                 Quantity = g.Sum(o => o.Quantity),



                                 ClosingBalance = g.Sum(o => o.ClosingBalance),
                                 QtyCalculation = String.Join(" ",
                                                items.Where(o => o.From == g.Key.From &&
                                                o.ProductId == g.Key.ProductId).Select(o =>
                                                o.Quantity > 0 ? " + " + o.Quantity.ToString() : o.Quantity.ToString()
                                                )),
                                 ExcessQty = g.Sum(o => o.ExcessQty),
                                 Product = g.First().Product,
                                 Item = g.First().Item,
                                 Freight = items.First().Freight,
                                 FreightTax = items.First().FreightTax,
                                 //   Id = items.IndexOf(g.Key) + 1,
                                 GuId = Guid.NewGuid().ToString()
                             }
                             ).ToList().OrderBy(o => o.From).ToList();


            if (items == null || items.Count == 0)
            {
                throw new UDFException("Nothing to bill", ErrorCodes.NOTHING_TO_BILL);
            }
            var freshData = new List<BillingItemDTO>();
            var ledger = new Ledger();
            //var ledgerId = items.First().LedgerId;
            var rates = ledger.GetProductRates(bdto.LedgerId, bdto.LedgerSiteId, companyId);

            var applyUnit2RateConfig = configs.Where(o => o.SubCategory.ToLower() == "billing"
              && o.Key.ToLower() == "applyunit2rate").FirstOrDefault();

            bool applyUnit2Rate = false;
            if (applyUnit2RateConfig != null)
            {
                applyUnit2Rate = applyUnit2RateConfig.Value == "true" || applyUnit2RateConfig.Value == "1";
            }
            for (var i = 0; i <= uniqueTxn.Count() - 1; i++)
            {
                var s = uniqueTxn[i];
                if (s.TranType == 3)
                {
                    s.OPB = s.Quantity;
                }
                //}
                //foreach (var s in uniqueTxn)
                //{
                double qtyToBill = 0;
                var rate = rates.Find(o => o.ProductId == s.ProductId);

                s.Rate = 0;
                if (rate != null)
                {
                    s.Rate = rate.RentRate;

                    if (applyUnit2Rate)
                    {
                        s.Rate = rate.UnitSizeRate;
                    }
                }

                if (s.ItemCategory != 1013)
                {

                    var firstProductInList = uniqueTxn.Where(o => o.ProductId == s.ProductId).First();
                    if (s.From > to)
                    {
                        //s.From = to;
                    }
                    // var sIndex = uniqueTxn.IndexOf(s);
                    var nextProduct = uniqueTxn.Where(o => o.ProductId == s.ProductId && Convert.ToDateTime(o.From) > Convert.ToDateTime(s.From)).FirstOrDefault();



                    //if (lastBillDate.Year > 2000)
                    //{
                    //    if (Convert.ToDateTime(s.From, culture) <= lastBillDate)
                    //        s.From = lastBillDate.AddDays(1);//.ToShortDateString();
                    //    //item.From = lastBillDate.ToShortDateString();

                    //}
                    var returnOnBillEndDate = false;
                    if (nextProduct != null)
                    {
                        s.To = to;
                        //s.To = nextProduct.From;
                        //nextProduct.ClosingBalance = s.ClosingBalance + nextProduct.Quantity;
                        //nextProduct.From = s.To;
                        //if (fixBillEndReturnOnLastDay && chargeReceivingDate && nextProduct.Quantity < 0
                        //    && Convert.ToDateTime(s.To).Date == Convert.ToDateTime(to).Date
                        //    && Convert.ToDateTime(s.From).Date < Convert.ToDateTime(to).Date)
                        //{
                        //    s.To = to.Date.AddDays(-1);
                        //    nextProduct.From = to.Date;
                        //    returnOnBillEndDate = true;
                        //}
                        // nextProduct.Rate = s.Rate;
                    }
                    else
                    {
                        s.To = to;//.ToShortDateString();

                    }
                    qtyToBill = s.ClosingBalance;
                    if (Convert.ToDateTime(s.From) == Convert.ToDateTime(s.To))
                    {
                        if (s.ClosingBalance == 0)
                        {
                            var sentQty = items.Find(o => o.ProductId == s.ProductId && (Convert.ToDateTime(o.From) == Convert.ToDateTime(s.From) && o.Quantity > 0));
                            if (sentQty != null && chargeReceivingDate)
                            {
                                s.Quantity = sentQty.Quantity;
                                qtyToBill = s.Quantity;
                            }
                        }
                        else if (s.ClosingBalance > 0 && s.Quantity > 0)
                        {
                            var recQty = items.Find(o => o.ProductId == s.ProductId && (Convert.ToDateTime(o.From) == Convert.ToDateTime(s.From) && o.Quantity < 0));
                            if (recQty != null && chargeReceivingDate)
                            {
                                s.Quantity += Math.Abs(recQty.Quantity);
                                qtyToBill = s.Quantity;

                            }
                        }
                        else if (chargeReceivingDate && s.ClosingBalance > 0 && s.Quantity < 0)
                        {
                            // Return on this day: charge on-hand qty before the return (same formula as openingBalance below)
                            qtyToBill = s.ClosingBalance - s.Quantity;
                        }
                    }
                    s.Days = (Convert.ToDateTime(s.To) - Convert.ToDateTime(s.From)).Days;
                    if (s.Days < 0)
                    {
                        s.From = s.To;
                    }
                    if (returnOnBillEndDate && s.Days >= 0)
                        s.Days++;
                    if (!returnOnBillEndDate && nextProduct != null && nextProduct.Quantity < 0)
                    {
                        if (chargeReceivingDate)
                        {
                            // if (s.Quantity > 0)
                            s.Days++;
                            nextProduct.From = s.To.AddDays(1);
                        }
                        else
                        {
                            if (s.Quantity < 0 && nextProduct.Quantity < 0)
                            {
                                s.Days--;
                                s.To = s.To.AddDays(-1);
                                nextProduct.From = s.To.AddDays(1);
                            }
                        }

                    }
                    if (nextProduct != null && !chargeReceivingDate)
                    {
                        s.To = nextProduct.From.AddDays(-1);
                        nextProduct.From = s.To.AddDays(1);
                        s.Days = (Convert.ToDateTime(s.To) - Convert.ToDateTime(s.From)).Days + 1;
                    }

                    if (nextProduct == null && Convert.ToDateTime(s.To).Date == Convert.ToDateTime(to).Date)
                    {
                        // s.Days = (Convert.ToDateTime(s.To) - Convert.ToDateTime(s.From)).Days;
                        // if (s.Quantity > 0 || (s.Quantity < 0 && chargeReceivingDate))
                        s.Days++;
                    }



                    if (bdto.RateCalcType == 2 || bdto.RateCalcType == 3)
                    {
                        var d = Convert.ToDateTime(s.From);


                        if (s.Days > Utils.DaysInMonth(d))
                        {
                            var nextMonthEntry = new BillingItemDTO();
                            nextMonthEntry.InjectFrom(s);


                            s.To = Utils.LastDaysOfMonth(d);
                            s.Days = (Convert.ToDateTime(s.To) - Convert.ToDateTime(s.From)).Days;
                            s.Days++;

                            nextMonthEntry.From = s.To.AddDays(1);
                            var currentIndex = uniqueTxn.IndexOf(s);
                            uniqueTxn.Insert(currentIndex + 1, nextMonthEntry);
                        }

                        if (s.Days < Utils.DaysInMonth(d) && bdto.RateCalcType == 2)
                        {
                            if (dayscalctype == "fixed")
                            {
                                s.Rate = Math.Round(s.Rate / dayscalctype_days, 2, MidpointRounding.AwayFromZero);
                            }
                            else
                                s.Rate = Math.Round(s.Rate / Utils.DaysInMonth(d), 2, MidpointRounding.AwayFromZero);

                        }
                        else if (s.Days == Utils.DaysInMonth(d))
                        {
                            //in this case Days=1 will be considered as 1 month
                            s.Days = 1;
                        }

                    }

                    //do not charge if item is set with another item.
                    var isSetWith = items.Where(o => o.ProductId == s.ProductId && o.GroupItemId > 0).FirstOrDefault();
                    if (isSetWith != null)
                    {
                        s.Rate = 0;
                        continue;
                    }

                    var rateAndQty = s.Rate * qtyToBill;
                    if (applyUnit2Rate)
                    {
                        rateAndQty = s.Rate * rate.Size * qtyToBill;
                    }
                    if (bdto.RateCalcType != 3)
                    {
                        s.Amount = rateAndQty * s.Days;
                    }
                    else
                    {
                        s.Amount = rateAndQty;
                    }
                    if (bdto.RateCalcType == 3)
                    {
                        var itemExist = freshData.Where(o => o.ProductId == s.ProductId && o.From.Month == s.To.Month).Count();
                        if (itemExist > 0)
                        {
                            s.Amount = 0;
                        }
                    }
                    double openingBalance = (s.ClosingBalance - s.Quantity);
                    s.QtyCalculation = "";
                    if (openingBalance > 0)
                    {
                        var products = uniqueTxn.Where(o => o.From == s.From &&
                                                    o.ProductId == s.ProductId);
                        if (s.ProductId == 3556)
                        {

                        }
                        if (openingBalance != 0)
                        {
                            s.QtyCalculation += openingBalance.ToString();
                        }
                        foreach (var p in products)
                        {
                            if (p.Quantity > 0 && openingBalance != 0)
                            {
                                s.QtyCalculation += " + " + p.Quantity.ToString();
                            }
                            else if (p.Quantity > 0)
                            {
                                s.QtyCalculation += p.Quantity.ToString();
                            }
                            if (p.Quantity < 0)
                            {
                                s.QtyCalculation += " " + p.Quantity.ToString();
                            }
                        }
                        if (products.Count() == 0)
                        {
                            s.QtyCalculation += s.Quantity.ToString();
                        }

                    }
                    //if (s.Amount <= 0)
                    //{
                    //    continue;
                    //}
                    //if charge returned date is checked and it is a return
                    //if (chargeReceivingDate && s.Quantity < 0)
                    //{
                    //    s.Amount += s.Rate * Math.Abs(s.Quantity) * 1;
                    //}


                }
                else if (s.ItemCategory == 1013)
                {
                    s.Days = 1;
                    s.To = s.From;
                    qtyToBill = s.Quantity;
                    s.Amount = s.Rate * qtyToBill;
                }

                if (!billNegativeQty && s.Amount < 0)
                {
                    continue;
                }
                if (hideZeroAmountsItems && s.Amount == 0)
                {
                    continue;
                }

                freshData.Add(s);
            }


            if (lastBillDate != null)
            {
                //return freshData.Where(o => Convert.ToDateTime(o.From, culture) >= lastBillDate).ToList().OrderBy(o => o.Item).ToList();
                //;
                return freshData.ToList().OrderBy(o => o.Item).ToList();

            }


            return freshData.OrderBy(o => o.Item).ToList();
        }

        List<BillingItemDTO> CalculateBillAmount1(BillingDTO bdto, List<BillingItemDTO> items, DateTime to, UserDTO userInfo, DateTime lastBillDate)
        {
            int companyId = userInfo.DefaultCompanyId;
            var config = new Config();
            var configs = config.GetBillingConfig(companyId);
            //  bool chargSentDate = true;
            bool chargeReceivingDate = bdto.ChargeReturnDay == 1;
            bool billNegativeQty = false;
            bool hideBomComponents = false;
            bool hideZeroAmountsItems = false;

            if (configs != null)
            {
                var billnegativeConfig = configs.Where(o => o.SubCategory.ToLower() == "billing" && o.Key.ToLower() == "billnegativeqty").FirstOrDefault();
                if (billnegativeConfig != null)
                {
                    billNegativeQty = Convert.ToBoolean(billnegativeConfig.Value);
                }
                var configHideBomComponents = configs.Where(o => o.SubCategory.ToLower() == "billing" && o.Key.ToLower() == "hidezeroamountitem").FirstOrDefault();
                if (configHideBomComponents != null)
                {
                    hideBomComponents = Convert.ToBoolean(configHideBomComponents.Value);
                }
                var confighideZeroAmountsItems = configs.Where(o => o.SubCategory.ToLower() == "billing" && o.Key.ToLower() == "hidebomcomponents").FirstOrDefault();
                if (confighideZeroAmountsItems != null)
                {
                    hideZeroAmountsItems = Convert.ToBoolean(confighideZeroAmountsItems.Value);
                }
            }
            //if (configs != null)
            //{
            //    var chargDates = configs.Where(o => o.SubCategory.ToLower() == "chagedates");
            //    if (chargDates != null)
            //    {
            //        if (chargDates.Where(o => o.Key.ToLower() == "billreceiveddate").Count() > 0)
            //        {
            //            chargeReceivingDate = Convert.ToBoolean(chargDates.Where(o => o.Key.ToLower() == "billreceiveddate").First().Value);
            //        }
            //        if (chargDates.Where(o => o.Key.ToLower() == "billsentdate").Count() > 0)
            //        {
            //            chargSentDate = Convert.ToBoolean(chargDates.Where(o => o.Key.ToLower() == "billsentdate").First().Value);
            //        }
            //    }

            //}

            var uniqueTxn = (from item in items

                             group item by new { item.From, item.ProductId, item.ItemCategory } into g

                             select new BillingItemDTO
                             {
                                 ProductId = g.Key.ProductId,
                                 From = g.Key.From,
                                 Rate = g.First().Rate,

                                 // TranType = g.Key.TranType,
                                 Id = g.First().Id,
                                 //Rate = g.Key.Rate,
                                 ItemCategory = g.Key.ItemCategory,
                                 Quantity = g.Sum(o => o.Quantity),
                                 ClosingBalance = g.Sum(o => o.ClosingBalance),
                                 Product = g.First().Product,
                                 Item = g.First().Item,
                                 Freight = items.First().Freight,
                                 FreightTax = items.First().FreightTax,
                                 //   Id = items.IndexOf(g.Key) + 1,
                                 GuId = Guid.NewGuid().ToString()
                             }
                             ).ToList().OrderBy(o => o.From).ToList();


            if (items == null || items.Count == 0)
            {
                throw new UDFException("Nothing to bill", ErrorCodes.NOTHING_TO_BILL);
            }
            var freshData = new List<BillingItemDTO>();
            var ledger = new Ledger();
            //var ledgerId = items.First().LedgerId;
            var rates = ledger.GetProductRates(bdto.LedgerId, bdto.LedgerSiteId, companyId);
            foreach (var s in uniqueTxn)
            {
                double qtyToBill = 0;
                var rate = rates.Find(o => o.ProductId == s.ProductId);

                s.Rate = 0;
                if (rate != null)
                {
                    s.Rate = rate.RentRate;
                }

                if (s.ItemCategory != 1013)
                {

                    var firstProductInList = uniqueTxn.Where(o => o.ProductId == s.ProductId).First();

                    // var sIndex = uniqueTxn.IndexOf(s);
                    var nextProduct = uniqueTxn.Where(o => o.ProductId == s.ProductId && Convert.ToDateTime(o.From) > Convert.ToDateTime(s.From)).FirstOrDefault();



                    //if (lastBillDate.Year > 2000)
                    //{
                    //    if (Convert.ToDateTime(s.From, culture) <= lastBillDate)
                    //        s.From = lastBillDate.AddDays(1);//.ToShortDateString();
                    //    //item.From = lastBillDate.ToShortDateString();

                    //}
                    if (nextProduct != null)
                    {
                        s.To = nextProduct.From;
                        nextProduct.ClosingBalance = s.ClosingBalance + nextProduct.Quantity;
                        nextProduct.From = s.To;
                        // nextProduct.Rate = s.Rate;
                    }
                    else
                    {
                        s.To = to;//.ToShortDateString();

                    }
                    qtyToBill = s.ClosingBalance;
                    if (Convert.ToDateTime(s.From) == Convert.ToDateTime(s.To))
                    {
                        if (s.ClosingBalance == 0)
                        {

                            var sentQty = items.Find(o => o.ProductId == s.ProductId && (Convert.ToDateTime(o.From) == Convert.ToDateTime(s.From) && o.Quantity > 0));
                            if (sentQty != null && chargeReceivingDate)
                            {
                                s.Quantity = sentQty.Quantity;
                                qtyToBill = s.Quantity;

                            }
                        }
                        else if (s.ClosingBalance > 0 && s.Quantity > 0)
                        {
                            var recQty = items.Find(o => o.ProductId == s.ProductId && (Convert.ToDateTime(o.From) == Convert.ToDateTime(s.From) && o.Quantity < 0));
                            if (recQty != null && chargeReceivingDate)
                            {
                                s.Quantity += Math.Abs(recQty.Quantity);
                                qtyToBill = s.Quantity;

                            }
                        }
                    }
                    s.Days = (Convert.ToDateTime(s.To) - Convert.ToDateTime(s.From)).Days;
                    if (s.Days < 0)
                    {
                        s.From = s.To;
                    }
                    if (nextProduct == null)
                    {
                        s.Days++;
                    }

                    if (bdto.RateCalcType == 2)
                    {
                        var d = Convert.ToDateTime(s.From);
                        if (s.Days < Utils.DaysInMonth(d))
                        {
                            s.Rate = Math.Round(s.Rate / Utils.DaysInMonth(d), 2, MidpointRounding.AwayFromZero);
                        }
                        else if (s.Days == Utils.DaysInMonth(d))
                        {
                            //in this case Days=1 will be considered as 1 month
                            s.Days = 1;
                        }
                    }

                    //do not charge if item is set with another item.
                    var isSetWith = items.Where(o => o.ProductId == s.ProductId && o.GroupItemId > 0).FirstOrDefault();
                    if (isSetWith != null)
                    {
                        s.Rate = 0;
                        continue;
                    }

                    var rateAndQty = s.Rate * qtyToBill;

                    s.Amount = rateAndQty * s.Days;
                    //if (s.Amount <= 0)
                    //{
                    //    continue;
                    //}
                    //if charge returned date is checked and it is a return
                    if (chargeReceivingDate && s.Quantity < 0)
                    {
                        s.Amount += s.Rate * Math.Abs(s.Quantity) * 1;
                    }


                }
                else if (s.ItemCategory == 1013)
                {
                    s.Days = 1;
                    s.To = s.From;
                    qtyToBill = s.Quantity;
                    s.Amount = s.Rate * qtyToBill;
                }

                if (!billNegativeQty && s.Amount < 0)
                {
                    continue;
                }
                if (hideZeroAmountsItems && s.Amount == 0)
                {
                    continue;
                }

                freshData.Add(s);
            }


            if (lastBillDate != null)
            {
                //return freshData.Where(o => Convert.ToDateTime(o.From, culture) >= lastBillDate).ToList().OrderBy(o => o.Item).ToList();
                //;
                return freshData.ToList().OrderBy(o => o.Item).ToList();

            }


            return freshData.OrderBy(o => o.Item).ToList();
        }

        List<BillingItemDTO> CalculateBillAmount(List<BillingItemDTO> items, DateTime to, LoggedInUserInfo userInfo, DateTime lastBillDate)
        {
            int companyId = userInfo.DefaultCompanyId;
            var config = new Config();
            var configs = config.GetBillingConfig(companyId);
            bool chargSentDate = true;
            bool chargeReceivingDate = false;

            if (configs != null)
            {
                var chargDates = configs.Where(o => o.SubCategory.ToLower() == "chagedates");
                if (chargDates != null)
                {
                    if (chargDates.Where(o => o.Key.ToLower() == "billreceiveddate").Count() > 0)
                    {
                        chargeReceivingDate = Convert.ToBoolean(chargDates.Where(o => o.Key.ToLower() == "billreceiveddate").First().Value);
                    }
                    if (chargDates.Where(o => o.Key.ToLower() == "billsentdate").Count() > 0)
                    {
                        chargSentDate = Convert.ToBoolean(chargDates.Where(o => o.Key.ToLower() == "billsentdate").First().Value);
                    }
                }
            }

            var groupedItems = (from item in items
                                group item by new { item.From, item.ProductId, item.Rate, item.TranType, item.ItemCategory } into g
                                select new BillingItemDTO
                                {
                                    ProductId = g.Key.ProductId,
                                    From = g.Key.From,
                                    TranType = g.Key.TranType,
                                    Id = g.First().Id,
                                    Rate = g.Key.Rate,
                                    ItemCategory = g.Key.ItemCategory,
                                    Quantity = g.Sum(o => o.Quantity),
                                    ClosingBalance = g.First().ClosingBalance,
                                    Product = g.First().Product,
                                    Item = g.First().Item,
                                    Freight = g.First().Freight,
                                    FreightTax = g.First().FreightTax,
                                    //   Id = items.IndexOf(g.Key) + 1,
                                    GuId = Guid.NewGuid().ToString()
                                }
                                ).ToList();
            var freshData = new List<BillingItemDTO>();

            foreach (var item in groupedItems)
            {
                double qtyToBill = 0;
                if (item.ItemCategory != 1013)
                {
                    bool chargeLastDate = chargeReceivingDate;
                    //  var nextEntry = items.Where(o => o.ProductId == item.ProductId && o.Id != item.Id && Convert.ToDateTime(o.From, culture) >= Convert.ToDateTime(item.From, culture)).OrderBy(o => o.From).FirstOrDefault();
                    //var nextEntry = groupedItems.Where(o => o.ProductId == item.ProductId &&  o.Id > item.Id && Convert.ToDateTime(o.From, culture) >= Convert.ToDateTime(item.From, culture)).OrderBy(o => o.From).FirstOrDefault();

                    //var prevEntry = groupedItems.Where(o => o.ProductId == item.ProductId && o.Id < item.Id &&  Convert.ToDateTime(o.From, culture) < Convert.ToDateTime(item.From, culture)).OrderByDescending(o => o.From).FirstOrDefault();

                    var nextEntry = groupedItems.Where(o => o.ProductId == item.ProductId && o.Id > item.Id).OrderBy(o => o.Id).FirstOrDefault();

                    var prevEntry = groupedItems.Where(o => o.ProductId == item.ProductId && o.Id < item.Id).OrderByDescending(o => o.Id).FirstOrDefault();

                    if (item.To == null && nextEntry == null)
                    {
                        item.To = to.ToShortDateString();

                    }

                    if (nextEntry != null)
                    {

                        // item.To = Convert.ToDateTime(nextEntry.From, culture).AddDays(-1).ToShortDateString();
                        item.To = Convert.ToDateTime(nextEntry.From, culture).ToShortDateString();

                    }

                    if (Convert.ToDateTime(item.To, culture) <= lastBillDate)
                    {
                        continue;
                    }

                    if (lastBillDate.Year > 2000)
                    {

                        item.From = lastBillDate.AddDays(1).ToShortDateString();
                        //item.From = lastBillDate.ToShortDateString();

                    }

                    if (prevEntry != null)
                    {

                        if (item.Rate == 0)
                            item.Rate = prevEntry.Rate;
                        string name = item.Item;


                        item.From = Convert.ToDateTime(prevEntry.To, culture).ToShortDateString();

                    }


                    item.ClosingBalance = qtyToBill = getClosingBal(item, groupedItems);

                    if (!chargeReceivingDate)
                    {
                        if (nextEntry != null && nextEntry.TranType == 2)
                        {
                            item.ClosingBalance += nextEntry.Quantity;
                            item.To = nextEntry.From;
                        }
                        if (item.TranType == 2 && nextEntry == null && prevEntry == null)
                        {
                            item.From = item.To;
                        }
                        else if (item.TranType == 2 && nextEntry == null && prevEntry != null)
                        {
                            item.From = prevEntry.To;
                        }

                    }

                    //if (Convert.ToDateTime(item.To, culture) < Convert.ToDateTime(item.From, culture) && item.ClosingBalance > 0)
                    //{
                    //    item.To = item.From;
                    //}
                    if (item.TranType == 2 && chargeReceivingDate)
                    {
                        qtyToBill = item.ClosingBalance + Math.Abs(item.Quantity);
                    }
                    //item.ClosingBalance = qtyToBill;

                    if (item.ClosingBalance == 0 && nextEntry == null)
                    {
                        item.To = item.From;

                    }
                    item.Days = (Convert.ToDateTime(item.To, culture) - Convert.ToDateTime(item.From, culture)).Days;

                    if (item.ClosingBalance > 0 && Convert.ToDateTime(item.To, culture) == to && nextEntry == null)
                    {
                        if (prevEntry == null)
                            item.Days++;
                    }

                    item.Days = item.Days == 0 ? 1 : item.Days;
                    //if not receivin

                    if (Convert.ToDateTime(item.To, culture) > Convert.ToDateTime(item.From, culture))
                    {
                        //var diff = (Convert.ToDateTime(to, culture) - Convert.ToDateTime(item.To, culture)).Days;
                        //if (diff <= 0)
                        // item.Days++;
                    }

                }
                //if consumeables
                else if (item.ItemCategory == 1013)
                {
                    item.Days = 1;
                    item.To = item.From;
                    qtyToBill = item.Quantity;
                }



                item.Amount = item.Days * qtyToBill * item.Rate;
                if (item.Amount == 0)
                {
                    // continue;
                }
                //if (item.TranType == 2  )
                //{

                //    item.To = item.From;
                //    item.Days = 1;
                //    item.Amount = item.Days *  Math.Abs(item.Quantity) * item.Rate;
                //    if (!chargeReceivingDate)
                //    {
                //        item.Amount = 0;
                //    }
                //    freshData.Add(item);
                //}
                //else
                freshData.Add(item);
            }
            // freshData.RemoveAll(o => o.ClosingBalance == 0);
            if (lastBillDate != null)
            {
                return freshData.Where(o => Convert.ToDateTime(o.From, culture) >= lastBillDate).ToList().OrderBy(o => o.Item).ToList();
                ;
            }


            return freshData.OrderBy(o => o.Item).ToList();
        }
*/
        double getClosingBal(BillingItemDTO item, List<BillingItemDTO> items)
        {
            //var x = (from d in items
            //         where d.ProductId == item.ProductId && d.GuId != item.GuId
            //         && Convert.ToDateTime(d.From, culture) <= Convert.ToDateTime(item.From, culture)
            //         select d
            //         ).Sum(o => o.Quantity);
            var x = (from d in items
                     where d.ProductId == item.ProductId
                     && (d.Id < item.Id)
                     // && ( Convert.ToDateTime(d.From, culture) <= Convert.ToDateTime(item.From, culture))

                     select d
                   ).Sum(o => o.Quantity);


            return item.Quantity + x;


        }

        public async Task<RentBillDto> GetLostItemsToBill(BillingDTO bdto, LoggedInUserInfo userInfo)
        {
            var config = new Config();
            Product objProduct = new Product();
            BillingDAL objBillingDAL = new BillingDAL();
            var company = new Company(bdto.CompanyId).GetDetails();

            var allProducts = await objProduct.GetAll(bdto.CompanyId);
            var taxCategories = await objBillingDAL.GetAllTaxCategories();

            var client = new Ledger(bdto.LedgerId).GetDetails();
            var configs = config.GetBillingConfig(bdto.CompanyId);

            var taxConfig = configs.Where(o => o.Key == "applyTax").FirstOrDefault();
            var billData = new RentBillDto();
            if (taxConfig == null)
            {
                bdto.ApplyTax = true;
            }
            //string discouintApplyConfigValue = "aftertax";
            //if (configs != null)
            //{
            //    var discountApplyConfig = configs.Where(o => o.Key == "").FirstOrDefault();
            //    if (discountApplyConfig != null)
            //    {
            //        discouintApplyConfigValue = discountApplyConfig.Value;
            //    }
            //}

            billData.ApplyTax = bdto.ApplyTax;

            billData.BillingItems = (await objBillingDAL.GetLossItemsToBill(bdto, userInfo.FinYearId)).ToList();

            foreach (var bItem in billData.BillingItems)
            {
                var p = allProducts.Where(o => o.ProductId == bItem.ProductId).FirstOrDefault();
                if (p != null)
                {
                    bItem.Rate = p.LossRate;
                    bItem.Amount = bItem.Rate * bItem.Quantity;
                    var tax = taxCategories.Where(o => o.TaxCategoryId == p.TaxCategoryId).FirstOrDefault();
                    if (tax != null)
                    {
                        if (client.StateId == company.StateId)
                        {
                            bItem.SGSTRate = tax.SGST;
                            bItem.CGSTRate = tax.CGST;
                        }
                        else
                            bItem.IGSTRate = tax.IGST;
                        if (!bdto.ApplyTax)
                        {
                            bItem.SGSTRate = 0;
                            bItem.CGSTRate = 0;
                            bItem.IGSTRate = 0;
                        }
                        bItem.IGST = (bItem.IGSTRate * bItem.Amount) / 100;
                        bItem.SGST = (bItem.SGSTRate * bItem.Amount) / 100;
                        bItem.CGST = (bItem.CGSTRate * bItem.Amount) / 100;
                    }
                }
            }

            return billData;
        }

        //public async Task<BillingDTO> SaveBill(BillingDTO billingDto)
        //{
        //    BillingDAL objBillingDAL = new BillingDAL();
        //    return await objBillingDAL.SaveBill(billingDto.LedgerId, billingDto.From, billingDto.To, billingDto.WorkOrderNumber, billingDto);
        //}

        public async Task<bool> CreateInvoice(BillingDTO billingdto)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return await objBillingDAL.Add(billingdto);
        }
        public bool CreateQuotation(QuotationDataDTO billingdto)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return objBillingDAL.AddQuotation(billingdto);

        }
        public async Task<bool> UpdateQuotationStatus(QuotationDTO dto)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return await objBillingDAL.UpdateQuotationStatus(dto);
        }

        public async Task<bool> LinkQuotationToLedger(int quotationId, int ledgerId, int companyId)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return await objBillingDAL.LinkQuotationToLedger(quotationId, ledgerId, companyId);
        }

        public async Task LinkQuotationsToInvoice(IEnumerable<int> quotationIds, int invoiceId, int companyId)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            await objBillingDAL.LinkQuotationsToInvoice(companyId, invoiceId, quotationIds);
        }

        public List<QuotationDTO> QuotationsList(QuotationDTO dto)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            var _data = objBillingDAL.QuotationsList(dto);

            //  var list = new List<QuotationDTO>();

            return _data;
        }

        public List<QuotationDTO> QuotationsListByContractId(int contractId, int companyId)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return objBillingDAL.QuotationsListByContractId(contractId, companyId);
        }

        public DataSet GetQuotationItems(int quotationId)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return objBillingDAL.GetQuotationItems(quotationId);
        }

        public List<QuotationTaxDTO> GetQuotationTaxes(int quotationId)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return objBillingDAL.GetQuotationTaxes(quotationId);
        }

        public List<InvoiceTaxDTO> GetInvoiceTaxes(int invoiceId)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return objBillingDAL.GetInvoiceTaxes(invoiceId);
        }
        public async Task<QuotationDTO> QuotationById(int quotationId, int companyId)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return await objBillingDAL.QuotationById(quotationId, companyId);
        }
        public List<BillingDTO> GetBilList(string from, string to, int companyId, int clientId, int ledgerSiteId, int statusId, short InvoiceType)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return objBillingDAL.GetBilList(from, to, companyId, clientId, ledgerSiteId, statusId, InvoiceType);
        }
        public List<BillingItemDTO> BillItems(int billId)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return objBillingDAL.BillItems(billId);
        }
        public DataSet PrintBill(int billId)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return objBillingDAL.PrintBill(billId);
        }
        public DataSet PrintContractBill(int billId)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return objBillingDAL.PrintContractBill(billId);
        }
        public List<BillingDTO> CheckForBilling(int ledgerId, string from, string to, string workOrderNumber)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return objBillingDAL.CheckForBilling(ledgerId, from, to, workOrderNumber);
        }
        public async Task<RentBillDto> GetBreakageForBill(BillingDTO bdto, LoggedInUserInfo userInfo)
        {
            //  BillingDAL objBillingDAL = new BillingDAL();
            //  return objBillingDAL.GetBreakageForBill(ledgerId, invoiceId, ledgerSiteId, from, to, finyearId);

            var config = new Config();
            Product objProduct = new Product();
            BillingDAL objBillingDAL = new BillingDAL();
            var company = new Company(bdto.CompanyId).GetDetails();

            var allProducts = await objProduct.GetAll(bdto.CompanyId);
            var taxCategories = await objBillingDAL.GetAllTaxCategories();
            var clietObj = new Ledger(bdto.LedgerId);
            var client = new Ledger(bdto.LedgerId).GetDetails();
            var configs = config.GetBillingConfig(bdto.CompanyId);

            var taxConfig = configs.Where(o => o.Key == "applyTax").FirstOrDefault();
            var billData = new RentBillDto();
            if (taxConfig == null)
            {
                bdto.ApplyTax = true;
            }
            billData.ApplyTax = bdto.ApplyTax;

            billData.BillingItems = (await objBillingDAL.GetBreakageForBill(bdto.CompanyId, bdto.LedgerId,
                bdto.InvoiceId, bdto.LedgerSiteId, bdto.From, bdto.To, userInfo.FinYearId)).ToList();

            if (billData.BillingItems.Count > 0)
            {
                billData.BreakageDamageDetails = (await objBillingDAL.GetBreakageDamageDetailsForSeparateBill(bdto.CompanyId, bdto.LedgerId, bdto.InvoiceId,
                    bdto.LedgerSiteId
               )).ToList();
            }
            var rates = clietObj.GetProductRates(bdto.LedgerId, bdto.LedgerSiteId, bdto.CompanyId);
            billData.Challans = new List<BillChallanDto>();

            foreach (var bItem in billData.BillingItems)
            {
                var p = allProducts.Where(o => o.ProductId == bItem.ProductId).FirstOrDefault();
                if (p != null)
                {
                    bItem.Rate = p.BrekageRate;
                    if (rates != null)
                    {
                        var rate = rates.Where(o => o.ProductId == bItem.ProductId).FirstOrDefault();
                        if (rate != null)
                        {
                            bItem.Rate = rate.DamageRate;
                        }
                    }
                    //bItem.Amount = bItem.Rate * bItem.Quantity;
                    if (billData.BreakageDamageDetails != null)
                    {
                        bItem.Amount = Convert.ToDouble(billData.BreakageDamageDetails.Where(o => o.ProductId == bItem.ProductId).Sum(o => o.Cost));
                    }

                    var tax = taxCategories.Where(o => o.TaxCategoryId == p.TaxCategoryId).FirstOrDefault();
                    if (tax != null)
                    {
                        if (client.StateId == company.StateId)
                        {
                            bItem.SGSTRate = tax.SGST;
                            bItem.CGSTRate = tax.CGST;
                        }
                        else
                            bItem.IGSTRate = tax.IGST;
                        if (!bdto.ApplyTax)
                        {
                            bItem.SGSTRate = 0;
                            bItem.CGSTRate = 0;
                            bItem.IGSTRate = 0;
                        }

                        bItem.IGST = (bItem.IGSTRate * bItem.Amount) / 100;
                        bItem.SGST = (bItem.SGSTRate * bItem.Amount) / 100;
                        bItem.CGST = (bItem.CGSTRate * bItem.Amount) / 100;
                    }
                }
            }
            billData.Challans = (from b in billData.BillingItems
                                 group b by b.ChallanId into g
                                 select new BillChallanDto
                                 {
                                     ChallanId = g.Key,
                                     ChallanNumber = g.First().GRN,
                                     Type = 2
                                 }).ToList();

            return billData;


        }
        public List<BillingItemDTO> BillingItemsTax(int invoiceId)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return objBillingDAL.BillingItemsTax(invoiceId);
        }

        public async Task<bool> CancelInvoice(int invoiceId, int modifiedBy, DateTime modifiedDate, int compnayId)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return await objBillingDAL.ChangeInvoieStatus(invoiceId, InvoiceStatus.Cancelled, modifiedBy, modifiedDate, compnayId);
        }
        public async Task<bool> SendForApproval(int invoiceId, int modifiedBy, DateTime modifiedDate, int compnayId)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return await objBillingDAL.ChangeInvoieStatus(invoiceId, InvoiceStatus.SendForApprval, modifiedBy, modifiedDate, compnayId);
        }
        public async Task<bool> Approve(int invoiceId, int modifiedBy, DateTime modifiedDate, int compnayId)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return await objBillingDAL.ChangeInvoieStatus(invoiceId, InvoiceStatus.Approved, modifiedBy, modifiedDate, compnayId);
        }
        public async Task<bool> MarkSettle(int invoiceId, int modifiedBy, DateTime modifiedDate, int compnayId)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return await objBillingDAL.ChangeInvoieStatus(invoiceId, InvoiceStatus.Settled, modifiedBy, modifiedDate, compnayId);
        }
        public DataSet DueBills(int finYearId)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return objBillingDAL.DueBills(finYearId);

        }
        public DataSet DueBillsSummary(int ledgerSiteId, int companyId, int ledgerId, DateTime from, DateTime to, int finYearId)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return objBillingDAL.DueBillsSummary(ledgerSiteId, companyId, ledgerId, from, to, finYearId);

        }
        public DataSet GetLossItems(int invoiceId)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return objBillingDAL.GetLossItems(invoiceId);
        }

        public DataSet GetBreakageItems(int parentInvoiceId)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return objBillingDAL.GetBreakageItems(parentInvoiceId);
        }
        public DataSet SelInvoiceHeader(int invoiceId, int companyId)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return objBillingDAL.SelInvoiceHeader(invoiceId, companyId);
        }

        public BillingDTO GetBillingInfo(int invoiceId)

        {
            BillingDAL objBillingDAL = new BillingDAL();
            return objBillingDAL.GetBillingInfo(invoiceId);
        }

        #region EwayBill
        public async Task<bool> AddEwayBill(EwayBillDTO dto)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return await objBillingDAL.AddEwayBill(dto);
        }
        public List<EwayBillDTO> GetAllEwayBills(EwayBillFilterDto filter)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return objBillingDAL.GetAllEwayBills(filter);
        }
        public async Task<EwayBillDTO> GetEwayBill(int ewayBillId, int companyId)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return await objBillingDAL.GetEwayBill(ewayBillId, companyId);
        }
        public bool UpdateEwayBillInfo(EwayBillDTO dto)
        {

            BillingDAL objBillingDAL = new BillingDAL();
            return objBillingDAL.UpdateEwayBillInfoInBill(dto);
        }
        public bool UpdateEwayBillPortalInfo(EwayBillDTO dto)
        {

            BillingDAL objBillingDAL = new BillingDAL();
            return objBillingDAL.UpdateEwayBillPortalInfo(dto);
        }
        public DataSet PrintEwayBill(EwayBillDTO dto)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return objBillingDAL.PrintEwayBill(dto);
        }
        public bool CancelEwayBIll(CancelEwayBillDto dto)
        {
            try
            {
                BillingDAL objBillingDAL = new BillingDAL();
                return objBillingDAL.CancelEwayBill(dto);
            }
            catch (Exception ex)
            {
                throw new UDFException(ex.Message, ErrorCodes.ERROR_WHILE_CANCEL_BILL_IN_DB);
            }
        }
        public async Task<IEnumerable<EwayBillDTO>> GetEwayBillByDocNumber(EwayBillDTO dto)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return await objBillingDAL.GetEwayBillByDocNumber(dto);
        }
        #endregion

        public async Task<bool> SaveMatLoss(MatLossDTO dto)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return await objBillingDAL.SaveMatLoss(dto);
        }
        public async Task<MatLossDTO> MatLossById(MatLossFilterDTO dto)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return await objBillingDAL.MatLossById(dto);
        }
        public async Task<IEnumerable<MatLossDTO>> MatLossList(MatLossFilterDTO dto)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return await objBillingDAL.MatLossList(dto);
        }
        public async Task<bool> DeleteMatLoss(MatLossDTO dto)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return await objBillingDAL.DeleteMatLoss(dto);
        }

        public async Task<IEnumerable<InvoiceDTO>> GetUnpaidBills(int ledgerId, int companyId, int ledgerSiteId)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return await objBillingDAL.GetUnpaidBills(ledgerId, companyId, ledgerSiteId);
        }
        public async Task<IEnumerable<InvoiceDTO>> GetBillsByIds(string billIds, int companyId)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return await objBillingDAL.GetBillsByIds(billIds, companyId);

        }
        public async Task<int> SettleBill(InvoiceDTO dto)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return await objBillingDAL.SettleBill(dto);
        }

        public async Task<QuotationDTO> QuotationByNumber(QuotationDTO dto)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return await objBillingDAL.QuotationByNumber(dto);
        }

        public DataSet getAdjustedItemsToPrintOnBill(int invoiceId, int companyId)
        {
            BillingDAL objBillingDAL = new BillingDAL();

            return objBillingDAL.getAdjustedItemsToPrintOnBill(invoiceId, companyId);
        }
        public async Task<IEnumerable<BreakageDamageDetailDTO>> GetBreakageDamageDetailsForBill(int invoiceId, int companyId)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return await objBillingDAL.GetInvoiceBreakageDamageComponentsForEdit(invoiceId, companyId);
        }
    }


    public class RentCalculator
    {
        /// <summary>
        /// Calculate rent periods based on transactions and opening quantity
        /// </summary>
        /// <param name="transactions">List of transactions (Sent/Return)</param>
        /// <param name="billingMonth">Billing month for calculation</param>
        /// <param name="openingQuantity">Opening quantity rented out at month start</param>
        /// <returns>List of rent periods with quantities and durations</returns>
        public async Task<List<BillingItemDTO>> CalculateRentPeriods(
            List<BillingItemDTO> transactions,
            BillingDTO bdto,
            int openingQuantity = 0)
        {
            int companyId = bdto.CompanyId;
            var config = new Config();
            var configs = config.GetBillingConfig(companyId);
            //  bool chargSentDate = true;
            bool chargeReceivingDate = bdto.ChargeReturnDay == 1;
            bool billNegativeQty = false;
            bool hideBomComponents = false;
            bool hideZeroAmountsItems = false;
            string dayscalctype = "";
            byte dayscalctype_days = 30;

            if (configs != null)
            {
                var billnegativeConfig = configs.Where(o => o.SubCategory.ToLower() == "billing" &&
                o.Key.ToLower() == "billnegativeqty").FirstOrDefault();
                if (billnegativeConfig != null && !String.IsNullOrEmpty(billnegativeConfig.Value))
                {
                    // billNegativeQty = Convert.ToBoolean(billnegativeConfig.Value);
                    if (!bool.TryParse(billnegativeConfig.Value, out billNegativeQty))
                    {
                        throw new Exception("Invalid setting 'bill negative quantity'. Please fix under Settings > Billing");
                    }
                }
                var configHideBomComponents = configs.Where(o => o.SubCategory.ToLower() == "billing"
                && o.Key.ToLower() == "hidezeroamountitem").FirstOrDefault();
                if (configHideBomComponents != null && !String.IsNullOrEmpty(configHideBomComponents.Value))
                {
                    // hideBomComponents = Convert.ToBoolean(configHideBomComponents.Value);
                    if (!bool.TryParse(configHideBomComponents.Value, out hideBomComponents))
                    {
                        throw new Exception("Invalid setting 'Hide bom components'. Please fix under Settings > Billing");
                    }
                }
                var confighideZeroAmountsItems = configs.Where(o => o.SubCategory.ToLower() == "billing"
                && o.Key.ToLower() == "hidebomcomponents").FirstOrDefault();
                if (confighideZeroAmountsItems != null && !String.IsNullOrEmpty(confighideZeroAmountsItems.Value))
                {
                    if (!bool.TryParse(confighideZeroAmountsItems.Value, out hideZeroAmountsItems))
                    {
                        throw new Exception("Invalid setting 'Hide Zero(0) amount items'. Please fix under Settings > Billing");
                    }
                    // hideZeroAmountsItems = Convert.ToBoolean(confighideZeroAmountsItems.Value);

                }
                var configdayscalctype = configs.Where(o => o.SubCategory.ToLower() == "billing" && o.Key.ToLower() == "dayscalctype").FirstOrDefault();
                if (configdayscalctype != null && !String.IsNullOrEmpty(configdayscalctype.Value))
                {
                    dayscalctype = Convert.ToString(configdayscalctype.Value);
                }

                var configdayscalctype_days = configs.Where(o => o.SubCategory.ToLower() == "billing" && o.Key.ToLower() == "dayscalctype_days").FirstOrDefault();
                if (configdayscalctype_days != null && !String.IsNullOrEmpty(configdayscalctype_days.Value)
                     && !String.IsNullOrEmpty(dayscalctype))
                {
                    // dayscalctype_days = Convert.ToByte(configdayscalctype_days.Value);
                    if (!byte.TryParse(configdayscalctype_days.Value, out dayscalctype_days))
                    {
                        throw new Exception("Invalid setting 'Month Days Setup'. Please fix under Settings > Billing");
                    }
                    if (dayscalctype_days <= 0)
                    {
                        throw new Exception("Invalid setting 'Month Days Setup'. Please fix under Settings > Billing");
                    }
                }
            }
            else
            {
                throw new Exception("Invalid bill setting . Please fix under Settings > Billing and save");
            }



            if (transactions == null || transactions.Count == 0)
            {
                throw new UDFException("Nothing to bill", ErrorCodes.NOTHING_TO_BILL);
            }
            var freshData = new List<BillingItemDTO>();
            var ledger = new Ledger();
            //var ledgerId = items.First().LedgerId;
            var rates = ledger.GetProductRates(bdto.LedgerId, bdto.LedgerSiteId, companyId);

            var applyUnit2RateConfig = configs.Where(o => o.SubCategory.ToLower() == "billing"
              && o.Key.ToLower() == "applyunit2rate").FirstOrDefault();

            bool applyUnit2Rate = false;
            if (applyUnit2RateConfig != null)
            {
                applyUnit2Rate = applyUnit2RateConfig.Value == "true" || applyUnit2RateConfig.Value == "1";
            }

            var allPeriods = new List<BillingItemDTO>();
            //var itemGroups = transactions.GroupBy(t => t.ProductId);
            var itemGroups1 = (from item in transactions

                               group item by new { item.From, item.ProductId, item.ItemCategory } into g

                               select new BillingItemDTO
                               {
                                   ProductId = g.Key.ProductId,
                                   From = g.Key.From,
                                   Rate = g.First().Rate,

                                   TranType = g.First().TranType,
                                   Id = g.First().Id,
                                   //Rate = g.Key.Rate,
                                   ItemCategory = g.Key.ItemCategory,
                                   Quantity = g.Sum(o => o.Quantity),



                                   ClosingBalance = g.Sum(o => o.ClosingBalance),
                                   QtyCalculation = String.Join(" ",
                                                  transactions.Where(o => o.From == g.Key.From &&
                                                  o.ProductId == g.Key.ProductId).Select(o =>
                                                  o.Quantity > 0 ? " + " + o.Quantity.ToString() : o.Quantity.ToString()
                                                  )),
                                   ExcessQty = g.Sum(o => o.ExcessQty),
                                   Product = g.First().Product,
                                   Item = g.First().Item,
                                   Freight = transactions.First().Freight,
                                   FreightTax = transactions.First().FreightTax
                                   //   Id = items.IndexOf(g.Key) + 1,
                                   //GuId = Guid.NewGuid().ToString()
                               }
                            ).GroupBy(t => t.ProductId);


            foreach (var itemGroup in itemGroups1)
            {
                int item = itemGroup.Key;
                var bItem = itemGroup.First();
                var sortedTransactions = itemGroup.OrderBy(t => t.From).ToList();
                //foreach (var s in sortedTransactions)
                //{
                //    s.To = bdto.To;
                //    var d = Convert.ToDateTime(s.From);


                //    int days = (s.To - d).Days + 1;

                //    if (days > Utils.DaysInMonth(d))
                //    {
                //        var nextMonthEntry = new BillingItemDTO();
                //        nextMonthEntry.InjectFrom(s);


                //        s.To = Utils.LastDaysOfMonth(d);
                //        s.Days = (Convert.ToDateTime(s.To) - Convert.ToDateTime(s.From)).Days;
                //        s.Days++;

                //        nextMonthEntry.From = s.To.AddDays(1);
                //        var currentIndex = sortedTransactions.IndexOf(s);
                //        sortedTransactions.Insert(currentIndex + 1, nextMonthEntry);
                //    }

                //}
                var result = new List<BillingItemDTO>();

                foreach (var s in sortedTransactions)
                {
                    if (s.To.Year == 1)
                    {
                        s.To = bdto.To;
                    }
                    var itemName = s.Item;
                    if (s.ProductId == 3389)
                    {
                        var q = s.Quantity;
                    }

                    var d = Convert.ToDateTime(s.From);

                    int days = (s.To - d).Days + 1;
                    if (days > Utils.DaysInMonth(d))
                    {
                        // Inline recursive lambda
                        Action<BillingItemDTO, DateTime> processSplit = null;
                        processSplit = (currentItem, currentDate) =>
                        {
                            int daysInMonth = Utils.DaysInMonth(currentDate);
                            int daysRemaining = (currentItem.To - currentDate).Days + 1;

                            if (daysRemaining <= daysInMonth)
                            {
                                result.Add(currentItem);
                                return;
                            }

                            var nextMonthEntry = new BillingItemDTO();
                            nextMonthEntry.InjectFrom(currentItem);

                            currentItem.To = Utils.LastDaysOfMonth(currentDate);
                            currentItem.Days = (Convert.ToDateTime(currentItem.To) - Convert.ToDateTime(currentItem.From)).Days;

                            nextMonthEntry.From = currentItem.To.AddDays(1);

                            result.Add(currentItem);
                            processSplit(nextMonthEntry, Convert.ToDateTime(nextMonthEntry.From));
                        };

                        processSplit(s, d);
                    }
                    else
                    {
                        result.Add(s);
                    }
                }

                sortedTransactions = result; // Replace original list with modified version
                //}
                //foreach (var s in uniqueTxn)
                //{
                double qtyToBill = 0;
                var rate = rates.Find(o => o.ProductId == bItem.ProductId);

                bItem.Rate = 0;
                if (rate != null)
                {
                    bItem.Rate = rate.RentRate;

                    if (rate.ApplyUnit2Rate)
                    {
                        bItem.Rate = rate.UnitSizeRate;
                    }
                }
                var isSetWith = transactions.Where(o => o.ProductId == bItem.ProductId && o.GroupItemId > 0).FirstOrDefault();
                if (isSetWith != null)
                {
                    bItem.Rate = 0;
                    continue;
                }

                // FIFO queue for active rentals
                var activeRentals = new Queue<ActiveRental>();

                DateTime monthStart = bdto.From;
                DateTime monthEnd = bdto.To;

                // Add opening quantity as rental from month start
                if (openingQuantity > 0)
                {
                    activeRentals.Enqueue(new ActiveRental
                    {
                        StartDate = monthStart,
                        Quantity = openingQuantity,
                        DailyRate = (decimal)bItem.Rate
                    });
                }

                // Add sent transactions
                foreach (var trans in sortedTransactions.Where(t => t.TranType == 1 || t.TranType == 3))
                {
                    activeRentals.Enqueue(new ActiveRental
                    {
                        StartDate = trans.From,
                        EndDate = trans.To,
                        Quantity = (int)trans.Quantity,
                        DailyRate = (decimal)bItem.Rate
                    });
                }

                // Process returns using FIFO
                foreach (var trans in sortedTransactions.Where(t => t.TranType == 2))
                {
                    int returnQty = Math.Abs((int)trans.Quantity);

                    while (returnQty > 0 && activeRentals.Count > 0)
                    {
                        var rental = activeRentals.Peek();
                        //var actualQty = rental.Quantity;

                        rental.Quantity = Math.Abs(rental.Quantity);
                        //double rentRate = bItem.Rate;
                        int returned = Math.Min(returnQty, Math.Abs(rental.Quantity));
                        string calculation = rental.Quantity.ToString() + " - " + returnQty.ToString();
                        int days = (trans.From - rental.StartDate).Days + 1;
                        decimal amount = 0;
                        decimal _rentalRate = rental.DailyRate;
                        if (days > 0 && returned > 0)
                        {
                            amount = _rentalRate * days * returned;
                            if (bdto.RateCalcType == 2 || bdto.RateCalcType == 3)
                            {
                                var d = Convert.ToDateTime(trans.From);
                                if (days < Utils.DaysInMonth(d) && bdto.RateCalcType == 2)
                                {
                                    if (dayscalctype == "fixed")
                                    {
                                        _rentalRate = Convert.ToDecimal(Math.Round(bItem.Rate / dayscalctype_days, 2, MidpointRounding.AwayFromZero));
                                    }
                                    else
                                        _rentalRate = Convert.ToDecimal(Math.Round(bItem.Rate / Utils.DaysInMonth(d), 2, MidpointRounding.AwayFromZero));
                                }
                                amount = _rentalRate * days * returned;
                                if (days == Utils.DaysInMonth(rental.StartDate))
                                {
                                    amount = _rentalRate * returned;
                                    days = 1;
                                }
                            }

                            var cb = rental.Quantity - returned;

                            allPeriods.Add(new BillingItemDTO
                            {
                                Product = bItem.Item,
                                ProductId = bItem.ProductId,
                                ClosingBalance = returned,
                                Closingbalance = returned,
                                LedgerId = bItem.LedgerId,
                                LedgerSiteId = bItem.LedgerSiteId,
                                Item = bItem.Item,
                                Quantity = returned,
                                From = rental.StartDate,
                                To = trans.From,
                                Days = days,

                                Amount = (double)amount,
                                Rate = (double)_rentalRate,
                                QtyCalculation = calculation
                            });

                        }
                        rental.Quantity -= returned;
                        returnQty -= returned;

                        if (rental.Quantity == 0)
                            activeRentals.Dequeue();
                    }
                }

                // Handle remaining active rentals at month end
                foreach (var rental in activeRentals)
                {
                    int days = (monthEnd - rental.StartDate).Days + 1;
                    if (rental.EndDate.Year > 1)
                    {
                        days = (rental.EndDate - rental.StartDate).Days + 1;
                    }
                    decimal amount = 0;
                    decimal _rentalRate = rental.DailyRate;
                    if (bdto.RateCalcType == 2 || bdto.RateCalcType == 3)
                    {
                        var d = Convert.ToDateTime(rental.StartDate);
                        if (days < Utils.DaysInMonth(d) && bdto.RateCalcType == 2)
                        {
                            if (dayscalctype == "fixed")
                            {
                                _rentalRate = Convert.ToDecimal(Math.Round(bItem.Rate / dayscalctype_days, 2, MidpointRounding.AwayFromZero));
                            }
                            else
                                _rentalRate = Convert.ToDecimal(Math.Round(bItem.Rate / Utils.DaysInMonth(d), 2, MidpointRounding.AwayFromZero));
                        }
                        amount = _rentalRate * days * rental.Quantity;
                        if (days == Utils.DaysInMonth(rental.StartDate))
                        {
                            amount = _rentalRate * rental.Quantity;
                            days = 1;
                        }

                    }
                    string calculation = rental.Quantity.ToString();
                    if (days > 0 && rental.Quantity > 0)
                    {
                        allPeriods.Add(new BillingItemDTO
                        {
                            Product = bItem.Item,
                            ProductId = bItem.ProductId,
                            ClosingBalance = rental.Quantity,
                            Closingbalance = rental.Quantity,
                            LedgerId = bItem.LedgerId,
                            LedgerSiteId = bItem.LedgerSiteId,
                            Item = bItem.Item,
                            Quantity = rental.Quantity,
                            From = rental.StartDate,
                            To = rental.EndDate.Year > 1 ? rental.EndDate : monthEnd,
                            Amount = (double)amount,
                            Days = days,
                            Rate = (double)_rentalRate,
                            QtyCalculation = calculation
                        });
                    }
                }


                // var lastEntry = allPeriods.Last();
                // if (lastEntry != null)
                // {
                //     lastEntry.Closingbalance = lastEntry.ClosingBalance = transactions.Where(o => o.ProductId == bItem.ProductId
                //&& o.From <= bItem.From
                //).Sum(o => o.Quantity);
                // }
            }
            //var cb = (from item in transactions

            //          group item by new { item.ProductId } into g

            //          select new BillingItemDTO
            //          {
            //              ProductId = g.Key.ProductId,
            //              Freight = transactions.First().Freight,
            //              FreightTax = transactions.First().FreightTax
            //              //   Id = items.IndexOf(g.Key) + 1,
            //              //GuId = Guid.NewGuid().ToString()
            //          }
            //                ).GroupBy(t => t.ProductId);


            //var latestPerProduct = allPeriods
            //                      .GroupBy(r => r.Product)
            //                      .Select(g => g.OrderByDescending(r => r.From).First())
            //                      .ToList();

            return allPeriods;
        }

        /// <summary>
        /// Calculate total rent amount
        /// </summary>
        public decimal CalculateTotalRent(List<BillingItemDTO> periods)
        {
            return Convert.ToDecimal(periods.Sum(p => p.Amount));
        }

        /// <summary>
        /// Get consolidated periods (combines same date ranges)
        /// </summary>
        public List<RentalResult> GetConsolidatedResults(List<RentPeriod> periods)
        {
            return periods
                .GroupBy(p => new { p.FromDate, p.ToDate, p.Item })
                .Select(g => new RentalResult
                {
                    Item = g.Key.Item,
                    Quantity = g.Sum(p => p.Quantity),
                    From = g.Key.FromDate,
                    To = g.Key.ToDate,
                    Days = g.First().Days,
                    RentAmount = g.Sum(p => p.RentAmount)
                })
                .OrderBy(r => r.From)
                .ThenBy(r => r.To)
                .ToList();
        }

        private decimal GetDailyRate(List<BillingItemDTO> transactions, int productId)
        {
            var transaction = transactions.FirstOrDefault(t => t.ProductId == productId);
            return (decimal)(transaction?.Rate ?? 0);
        }
    }
    public class RentalTransaction
    {
        public string Item { get; set; }
        public string Mode { get; set; }  // "Sent" or "Return"
        public int Quantity { get; set; }
        public decimal Rate { get; set; }  // Daily rent rate
        public DateTime Date { get; set; }
    }

    public class RentPeriod
    {
        public string Item { get; set; }
        public int Quantity { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int Days { get; set; }
        public decimal DailyRate { get; set; }
        public decimal RentAmount => Quantity * DailyRate * Days;
    }

    public class ActiveRental
    {
        public DateTime StartDate { get; set; }
        public int Quantity { get; set; }
        public decimal DailyRate { get; set; }
        public DateTime EndDate { get; set; }

    }

    public class RentalResult
    {
        public string Item { get; set; }
        public int Quantity { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int Days { get; set; }
        public decimal RentAmount { get; set; }
    }


}
