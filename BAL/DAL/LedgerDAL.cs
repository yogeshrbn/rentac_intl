using BAL.Common;
using BAL.DTO;
using BAL.Enums;
using BAL.Exceptions;
using BAL.Objects;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BAL.DAL
{
    public class LedgerDAL
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        public int Save(LedgerDTO _data)
        {
            int ledgerId = 0;
            SQL objSql = new SQL();
            objSql.BeginTransaction();
            objSql.NewCommand();
            try
            {
                objSql.AddParameter("@Name", DbType.String, ParameterDirection.Input, 0, _data.Name);
                objSql.AddParameter("@TradeName", DbType.String, ParameterDirection.Input, 0, _data.TradeName);
                objSql.AddParameter("@UseTradeNameForBilling", DbType.Byte, ParameterDirection.Input, 0, _data.UseTradeNameForBilling);

                objSql.AddParameter("@Code", DbType.String, ParameterDirection.Input, 0, _data.Code);

                objSql.AddParameter("@Address1", DbType.String, ParameterDirection.Input, 0, _data.Address1);
                objSql.AddParameter("@Address2", DbType.String, ParameterDirection.Input, 0, _data.Address2);
                objSql.AddParameter("@Email", DbType.String, ParameterDirection.Input, 0, _data.Email);

                objSql.AddParameter("@Fax", DbType.String, ParameterDirection.Input, 0, _data.Fax);
                objSql.AddParameter("@Phone1", DbType.String, ParameterDirection.Input, 0, _data.Phone1);
                objSql.AddParameter("@Phone2", DbType.String, ParameterDirection.Input, 0, _data.Phone2);
                objSql.AddParameter("@Contact", DbType.String, ParameterDirection.Input, 0, _data.Contact);
                objSql.AddParameter("@City", DbType.String, ParameterDirection.Input, 0, _data.City);
                objSql.AddParameter("@State", DbType.String, ParameterDirection.Input, 0, _data.State);
                objSql.AddParameter("@StateId", DbType.Int32, ParameterDirection.Input, 0, _data.StateId);

                objSql.AddParameter("@ZipCode", DbType.String, ParameterDirection.Input, 0, _data.ZipCode);
                objSql.AddParameter("@Web", DbType.String, ParameterDirection.Input, 0, _data.Web);
                objSql.AddParameter("@TIN", DbType.String, ParameterDirection.Input, 0, _data.TIN);
                objSql.AddParameter("@TAN", DbType.String, ParameterDirection.Input, 0, _data.TAN);
                objSql.AddParameter("@AccGroup", DbType.Int16, ParameterDirection.Input, 0, _data.AccountGroup);
                objSql.AddParameter("@OpeningBal", DbType.Double, ParameterDirection.Input, 0, _data.OpeningBal);
                objSql.AddParameter("@TransType", DbType.Int16, ParameterDirection.Input, 0, _data.TransType);
                objSql.AddParameter("@GSTNumber", DbType.String, ParameterDirection.Input, 0, _data.GSTNo);
                objSql.AddParameter("@AadharNumber", DbType.String, ParameterDirection.Input, 0, _data.AadharCard);
                objSql.AddParameter("@PAN", DbType.String, ParameterDirection.Input, 0, _data.PAN);
                objSql.AddParameter("@ServiceTax", DbType.String, ParameterDirection.Input, 0, _data.ServiceTaxNumber);
                objSql.AddParameter("@ContactPersonName", DbType.String, ParameterDirection.Input, 0, _data.ContactPersonName);
                objSql.AddParameter("@ContactPersonDesignation", DbType.String, ParameterDirection.Input, 0, _data.ContactPersonDesignation);
                objSql.AddParameter("@ContactPersonMobile", DbType.String, ParameterDirection.Input, 0, _data.ContactPersonMobile);
                objSql.AddParameter("@ContactPersonOffPhone", DbType.String, ParameterDirection.Input, 0, _data.ContactPersonOffPhone);
                objSql.AddParameter("@DefaultRate", DbType.Double, ParameterDirection.Input, 0, _data.DefaultRate);


                objSql.AddParameter("@RbnClientId", DbType.Int16, ParameterDirection.Input, 0, _data.RbnClientId);
                objSql.AddParameter("@CompanyId", DbType.Int16, ParameterDirection.Input, 0, _data.CompanyId);
                objSql.AddParameter("@creditDays", DbType.Int16, ParameterDirection.Input, 0, _data.CreditDays);
                objSql.AddParameter("@VAT", DbType.String, ParameterDirection.Input, 0, _data.VAT);



                if (_data.Addresses != null)
                {
                    var shipAddress = _data.Addresses.Where(o => o.AddressTypeId == 2).FirstOrDefault();
                    if (shipAddress != null)
                    {
                        objSql.AddParameter("@shipAddress1", DbType.String, ParameterDirection.Input, 0, shipAddress.Address1);
                        objSql.AddParameter("@shipAddress2", DbType.String, ParameterDirection.Input, 0, shipAddress.Address2);
                        objSql.AddParameter("@shipCity", DbType.String, ParameterDirection.Input, 0, shipAddress.City);
                        objSql.AddParameter("@shipStateId", DbType.Int32, ParameterDirection.Input, 0, shipAddress.StateId);
                        objSql.AddParameter("@shipZipCode", DbType.String, ParameterDirection.Input, 0, shipAddress.ZipCode);

                    }
                }
                if (_data.LedgerId == 0)
                {
                    objSql.AddParameter("@source", DbType.String, ParameterDirection.Input, 0, _data.Source);
                    objSql.AddParameter("@FinYearId", DbType.Int16, ParameterDirection.Input, 0, _data.FinYearId);
                    objSql.AddParameter("@forQuotation", DbType.Int16, ParameterDirection.Input, 0, _data.ForQuotation);

                    _data.LedgerId = ledgerId = Convert.ToInt32(objSql.ExecuteScalar(ADD));

                    //create site if new client created
                    if (_data.LedgerId > 0 && _data.AccountGroup == 17)
                    {
                        var siteDto = new ClientSiteDTO();
                        siteDto.Address1 = _data.Address1;
                        siteDto.Address2 = _data.Address2;
                        siteDto.ZipCode = _data.ZipCode;
                        siteDto.LedgerId = _data.LedgerId;
                        siteDto.City = _data.City;
                        siteDto.StateId = _data.StateId;
                        siteDto.SiteAddress = _data.Address1;
                        if (_data.Addresses != null)
                        {
                            var shipAddress = _data.Addresses.Where(o => o.AddressTypeId == 2).FirstOrDefault();
                            if (shipAddress != null)
                            {
                                if (shipAddress.Address1 != _data.Address1)
                                {
                                    siteDto.Address1 = shipAddress.Address1;
                                    siteDto.Address2 = shipAddress.Address2;
                                    siteDto.ZipCode = shipAddress.ZipCode;
                                    siteDto.LedgerId = _data.LedgerId;
                                    siteDto.City = shipAddress.City;
                                    siteDto.StateId = shipAddress.StateId;
                                    siteDto.SiteAddress = shipAddress.Address1;
                                }
                            }
                        }
                        AddSite(siteDto);

                    }
                }
                else
                {
                    ledgerId = _data.LedgerId;
                    objSql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 0, _data.LedgerId);
                    objSql.ExecuteNonQuery(UPDATE);
                }
                if (ledgerId > 0)
                {
                    //SaveCreditDays(ledgerId, _data.CreditDays);
                    foreach (AddressDTO ad in _data.Addresses)
                    {
                        ad.AddressHolderId = ledgerId;
                        AddressDAL aDal = new AddressDAL();
                        objSql.NewCommand();
                        aDal.Save(objSql, ad);
                    }
                }
                if (_data.OpeningBal > 0 && _data.TransType > 0)
                {
                    objSql.NewCommand();
                    //var finyearList = Config.GetFinYearList();
                    //var finYear = finyearList.Where(o => o.FinYearId == _data.FinYearId).FirstOrDefault();
                    int txnId = CreateTransactions(new LedgerTransactionDTO
                    {
                        LedgerId = _data.LedgerId,
                        TransactionAmount = _data.OpeningBal,
                        TransactionDate = _data.CreationDate,//.ToShortDateString(),
                        Description = "Opening Balance",
                        CreatedBy = _data.CreatedBy,
                        TransactionType = _data.TransType,
                        TransactionMode = TransactionModes.Cash,
                        EntryType = 17,
                        Narration = "Opening Balance",
                        TranRefNumber = _data.LedgerId.ToString(),
                        FinYearId = _data.FinYearId,
                        CompanyId = _data.CompanyId,
                        LedgerSiteId = 0,
                        Invoiceid = 0

                    }, objSql);

                    if (txnId == 0)
                    {
                        throw new Exception("Could not save transaction");
                    }
                }

                objSql.Commit();

            }
            catch (SqlException ex)
            {
                objSql.Rollback();
                throw new UDFException(ex.Message, ex.ErrorCode);
            }

            catch (Exception ex)
            {
                objSql.Rollback();
                throw ex;
            }
            return ledgerId;
        }

        public List<LedgerDTO> GetAll(int companyId, string query, bool active)
        {
            SQL objSql = new SQL();
            if (!String.IsNullOrEmpty(query))
            {
                objSql.AddParameter("@ClientName", DbType.String, ParameterDirection.Input, 0, query);
            }
            objSql.AddParameter("@CompanyId", DbType.Int16, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@Active", DbType.Boolean, ParameterDirection.Input, 0, active);
            return objSql.ContructList<LedgerDTO>(objSql.ExecuteDataSet(GETALL));
        }
        public async Task<IEnumerable<LedgerDTO>> GetAll(LedgerDTO dto)
        {
            using (SQL objSql = new SQL())
            {
                if (!String.IsNullOrEmpty(dto.Name))
                {
                    objSql.AddParameter("@ClientName", DbType.String, ParameterDirection.Input, 0, dto.Name);
                }
                objSql.AddParameter("@CompanyId", DbType.Int16, ParameterDirection.Input, 0, dto.CompanyId);
                objSql.AddParameter("@Active", DbType.Boolean, ParameterDirection.Input, 0, dto.IsActive);
                if (!String.IsNullOrEmpty(dto.Code))
                {
                    objSql.AddParameter("@code", DbType.String, ParameterDirection.Input, 0, dto.Code);
                }
                if (!String.IsNullOrEmpty(dto.Phone1))
                {
                    objSql.AddParameter("@phone", DbType.String, ParameterDirection.Input, 0, dto.Phone1);
                }

                objSql.AddParameter("@accGroup", DbType.Int64, ParameterDirection.Input, 0, dto.AccountGroup);

                return await objSql.QueryAsync<LedgerDTO>(GETALL);
            }

        }
        public async Task<IEnumerable<LedgerDTO>> GetAllByGroups(int companyId, string groupIds)
        {
            SQL objSql = new SQL();

            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@groupIds", DbType.String, ParameterDirection.Input, 0, groupIds);
            return await objSql.QueryAsync<LedgerDTO>(GETALL_BY_GROUP);
        }
        public LedgerDTO GetDetails(int clientId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 0, clientId);
            var dto = objSql.ContructList<LedgerDTO>(objSql.ExecuteDataSet(SELCECTCOMPANY)).FirstOrDefault();
            if (dto != null)
            {
                dto.CreditDays = GetCreditDays(clientId);
            }
            return dto;

        }



        private static int GetCreditDays(int ledgerId)
        {
            using (var con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["sqlCon"].ConnectionString))
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = @"IF COL_LENGTH('Ledger', 'CreditDays') IS NOT NULL
                                    BEGIN
                                        SELECT TOP 1 ISNULL([CreditDays], 0) FROM [Ledger] WHERE [LedgerId] = @ledgerId
                                    END
                                    ELSE
                                    BEGIN
                                        SELECT 0
                                    END";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ledgerId", ledgerId);
                con.Open();
                var value = cmd.ExecuteScalar();
                return value == null || value == DBNull.Value ? 0 : Convert.ToInt32(value);
            }
        }
        public LedgerDAL() { }

        public bool ActivateDeActivate(bool activate, int clientId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 0, clientId);
            objSql.AddParameter("@IsActive", DbType.Boolean, ParameterDirection.Input, 0, activate);
            return objSql.ExecuteNonQuery(ACTIVATE_DEACTIVATE) == 1;

        }
        public List<BillingItemDTO> StockRegister(int ledgerId, int lederSiteId, int companyId, string fromDate, string toDate)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@lederSiteId", DbType.Int32, ParameterDirection.Input, 0, lederSiteId);


            objSql.AddParameter("@FromDate", DbType.Date, ParameterDirection.Input, 0, fromDate);
            objSql.AddParameter("@ToDate", DbType.Date, ParameterDirection.Input, 0, toDate);
            return objSql.ContructList<BillingItemDTO>(objSql.ExecuteDataSet(PARTY_STOCK_REGISTER));

        }
        public DataSet StockRegister_rpt(int ledgerId, int companyId, string fromDate, string toDate)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, companyId);

            objSql.AddParameter("@FromDate", DbType.Date, ParameterDirection.Input, 0, fromDate);
            objSql.AddParameter("@ToDate", DbType.Date, ParameterDirection.Input, 0, toDate);
            return objSql.ExecuteDataSet(PARTY_STOCK_REGISTER);

        }
        public List<BillingItemDTO> PartyOpeningBalance(int ledgerId, string fromDate)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);
            objSql.AddParameter("@FromDate", DbType.Date, ParameterDirection.Input, 0, fromDate);

            return objSql.ContructList<BillingItemDTO>(objSql.ExecuteDataSet(PARTY_OPENING_BALANCE));

        }
        #region Transactions
        public int CreateTransactions(LedgerTransactionDTO dto, SQL sql = null)
        {

            SQL objSql = sql;

            bool localSql = false;
            try
            {
                if (objSql == null)
                {
                    localSql = true;
                    objSql = new SQL();
                    objSql.BeginTransaction();
                    objSql.NewCommand();
                }

                if (dto.TransactionType <= 0)
                {
                    throw new Exception("Incorrect transaction type");
                }


                objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, dto.LedgerId);
                objSql.AddParameter("@ledgerTxnId", DbType.Int32, ParameterDirection.Input, 0, dto.LedgerTransactionId);


                //    objSql.AddParameter("@CrLedgerId", DbType.Int16, ParameterDirection.Input, 0, dto.CrLedgerId);
                objSql.AddParameter("@TransactoinAmount", DbType.Double, ParameterDirection.Input, 0, dto.TransactionAmount);
                objSql.AddParameter("@TransactionDate", DbType.DateTime, ParameterDirection.Input, 0, dto.TransactionDate);
                objSql.AddParameter("@Description", DbType.String, ParameterDirection.Input, 0, dto.Description);
                objSql.AddParameter("@CreatedBy", DbType.Int16, ParameterDirection.Input, 0, dto.CreatedBy);
                objSql.AddParameter("@TransactionType", DbType.Int16, ParameterDirection.Input, 0, dto.TransactionType);
                //added by ram
                objSql.AddParameter("@TransactionMode", DbType.Int16, ParameterDirection.Input, 0, dto.TransactionMode);

                objSql.AddParameter("@EntryType", DbType.Int16, ParameterDirection.Input, 0, dto.EntryType);
                objSql.AddParameter("@Narration", DbType.String, ParameterDirection.Input, 0, dto.Narration);
                objSql.AddParameter("@TranRefNumber", DbType.String, ParameterDirection.Input, 0, dto.TranRefNumber);
                objSql.AddParameter("@FinYearId", DbType.Int16, ParameterDirection.Input, 0, dto.FinYearId);
                objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                objSql.AddParameter("@WorkOrderId", DbType.Int32, ParameterDirection.Input, 0, dto.WorkOrderId);
                objSql.AddParameter("@LedgerSiteId", DbType.Int32, ParameterDirection.Input, 0, dto.LedgerSiteId);
                objSql.AddParameter("@refLedgerId", DbType.Int32, ParameterDirection.Input, 0, dto.RefLedgerId);
                objSql.AddParameter("@creationDate", DbType.DateTime, ParameterDirection.Input, 0, DateTime.Now);
                objSql.AddParameter("@advance", DbType.Byte, ParameterDirection.Input, 0, dto.Advance);
                objSql.AddParameter("@RefTransactionId", DbType.Int32, ParameterDirection.Input, 0, dto.RefTransactionId);
               // dto.TotalTds = dto.TxnDetails.Sum(x => x.TdsAmount);
                objSql.AddParameter("@TDS", DbType.Int32, ParameterDirection.Input, 0, dto.TDS);
                // objSql.AddParameter("@receiptDocPath", DbType.String, ParameterDirection.Input, 0, dto.ReceiptDocumentPath);

                if (dto.TransactionMode == BAL.Enums.TransactionModes.Cheque)
                {
                    if (String.IsNullOrEmpty(dto.ChequeNumber) && !String.IsNullOrEmpty(dto.TranRefNumber))
                    {
                        dto.ChequeNumber = dto.TranRefNumber;
                    }
                }
                //added by ram
                if (!(dto.TransactionMode == BAL.Enums.TransactionModes.Cash))
                {
                  //  objSql.AddParameter("@TDS", DbType.Int32, ParameterDirection.Input, 0, dto.TDS);
                    objSql.AddParameter("@Discount", DbType.Int32, ParameterDirection.Input, 0, dto.Discount);
                    objSql.AddParameter("@ChequeNumber", DbType.String, ParameterDirection.Input, 0, dto.ChequeNumber);
                    objSql.AddParameter("@ChequeDate", DbType.DateTime, ParameterDirection.Input, 0, dto.ChequeDate);
                    objSql.AddParameter("@ChequeBankId", DbType.Int32, ParameterDirection.Input, 0, dto.ChequeBankId);
                    objSql.AddParameter("@ExecutiveName", DbType.String, ParameterDirection.Input, 0, dto.ExecutiveName);
                }

                DataSet ds = objSql.ExecuteDataSet(CREATE_TRANSACTION);
                logger.Info("Transaction Created Successfully.");
                if (ds.Tables.Count > 0 || dto.LedgerTransactionId > 0)
                {
                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            dto.LedgerTransactionId = Convert.ToInt32(ds.Tables[0].Rows[0]["LedgerTransactionId"]);
                            dto.RefLedgerId = Convert.ToInt32(ds.Tables[0].Rows[0]["RefLedgerId"]);
                        }
                    }
                    if (dto.TxnDetails != null)
                    {
                        foreach (var txnDetail in dto.TxnDetails)
                        {
                            objSql.NewCommand();
                            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                            objSql.AddParameter("@ledgerTransactionId", DbType.Int32, ParameterDirection.Input, 0, dto.LedgerTransactionId);
                            objSql.AddParameter("@amount", DbType.Decimal, ParameterDirection.Input, 0, txnDetail.AppliedAmount);
                            objSql.AddParameter("@billType", DbType.String, ParameterDirection.Input, 0, txnDetail.BillType);
                            objSql.AddParameter("@billId", DbType.Int32, ParameterDirection.Input, 0, txnDetail.BillId);
                            objSql.AddParameter("@creationDate", DbType.DateTime, ParameterDirection.Input, 0, dto.CreationDate);
                            objSql.AddParameter("@TdsAmount", DbType.Decimal, ParameterDirection.Input, 0, Convert.ToDecimal(txnDetail.TdsAmount));
                            objSql.ExecuteNonQuery(CREATE_LEDGERTRANSACTION_DETAIL);

                            //objSql.NewCommand();
                            //objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                            //objSql.AddParameter("@LedgerTransactionId", DbType.Int32, ParameterDirection.Input, 0, dto.LedgerTransactionId);
                            //objSql.AddParameter("@BillId", DbType.Int32, ParameterDirection.Input, 0, txnDetail.BillId);
                            //objSql.AddParameter("@TdsAmount", DbType.Decimal, ParameterDirection.Input, 0, Convert.ToDecimal(txnDetail.TdsAmount));
                            //objSql.ExecuteNonQuery(LEDGER_TRANSACTION_DETAIL_SET_TDS);

                            //if payment made against purchase invoice
                            if (dto.EntryType == 9 && dto.TransactionType == 1)
                            {
                                objSql.NewCommand();
                                objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                                objSql.AddParameter("@purchaseId", DbType.Int32, ParameterDirection.Input, 0, txnDetail.BillId);
                                objSql.ExecuteNonQuery(UPDATE_PURCHASE_PAYMENT);
                            }
                            //if payment made against purchase invoice
                            if (dto.EntryType == 8 && dto.TransactionType == 2)
                            {
                                objSql.NewCommand();
                                objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                                objSql.AddParameter("@invoiceId", DbType.Int32, ParameterDirection.Input, 0, txnDetail.BillId);
                                objSql.ExecuteNonQuery(UPDATE_INVOICE_PAYMENT);
                            }
                        }

                        //if (dto.TxnDetails.Count > 0 && dto.LedgerTransactionId > 0)
                        //{
                        //    dto.TotalTds = dto.TxnDetails.Sum(x => x.TdsAmount);
                        //    objSql.NewCommand();
                        //    objSql.AddParameter("@LedgerTransactionId", DbType.Int32, ParameterDirection.Input, 0, dto.LedgerTransactionId);
                        //    objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                        //    objSql.AddParameter("@TotalTds", DbType.Decimal, ParameterDirection.Input, 0, Convert.ToDecimal(dto.TotalTds));
                        //    objSql.ExecuteNonQuery(LEDGER_TRANSACTION_UPD_TOTAL_TDS);
                        //}
                    }

                    objSql.NewCommand();
                    objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, dto.LedgerId);
                    objSql.AddParameter("@FinYearId", DbType.Int16, ParameterDirection.Input, 0, dto.FinYearId);
                    objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                    objSql.ExecuteNonQuery(UPDATE_LEDGER_CLOSING_BALANCE);
                    logger.Info("Ledger closing balance updated Successfully.");
                    if (dto.RefLedgerId > 0)
                    {
                        objSql.NewCommand();
                        objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, dto.RefLedgerId);
                        objSql.AddParameter("@FinYearId", DbType.Int16, ParameterDirection.Input, 0, dto.FinYearId);
                        objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                        objSql.ExecuteNonQuery(UPDATE_LEDGER_CLOSING_BALANCE);
                        logger.Info("Reference Ledger closing balance updated Successfully.");
                    }
                    if (dto.ContractId > 0 && dto.LedgerTransactionId > 0)
                    {
                        objSql.NewCommand();
                        objSql.AddParameter("@LedgerTransactionId", DbType.Int32, ParameterDirection.Input, 0, dto.LedgerTransactionId);
                        objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                        objSql.AddParameter("@ContractId", DbType.Int32, ParameterDirection.Input, 0, dto.ContractId);
                        objSql.ExecuteNonQuery(SET_TRANSACTION_CONTRACT_ID);
                    }
                    if (dto.LedgerTransactionId > 0 && !string.IsNullOrWhiteSpace(dto.ReceiptStagingPath))
                    {
                        TryApplyQuickReceiptStaging(dto, objSql);
                    }

                }
                if (localSql)
                {
                    objSql.Commit();
                }


                return dto.LedgerTransactionId;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.Error(JsonConvert.SerializeObject(dto));
                if (localSql)
                {
                    objSql.Rollback();
                }
                throw ex;
            }

        }

        void TryApplyQuickReceiptStaging(LedgerTransactionDTO dto, SQL objSql)
        {
            var oldPermanent = dto.ReceiptDocumentPath;
            var finalRelative = QuickReceiptDocumentHelper.MoveStagingToPermanent(dto.ReceiptStagingPath.Trim(), dto.LedgerTransactionId, dto.CompanyId);

            objSql.NewCommand();
            objSql.AddParameter("@LedgerTransactionId", DbType.Int32, ParameterDirection.Input, 0, dto.LedgerTransactionId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
            objSql.AddParameter("@path", DbType.String, ParameterDirection.Input, 500, finalRelative);
            objSql.ExecuteNonQuery(LEDGER_TRANSACTION_UPD_RECEIPT_PATH);

            dto.ReceiptDocumentPath = finalRelative;
            dto.ReceiptStagingPath = null;

            if (!string.IsNullOrWhiteSpace(oldPermanent)
                && QuickReceiptDocumentHelper.IsValidPermanentRelativePath(oldPermanent)
                && !string.Equals(oldPermanent, finalRelative, StringComparison.OrdinalIgnoreCase))
            {
                QuickReceiptDocumentHelper.TryDeletePhysicalForWebPath(oldPermanent);
            }
        }

        public Boolean VerifySameDayAmount(LedgerTransactionDTO dto)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@TransactoinAmount", DbType.Double, ParameterDirection.Input, 0, dto.TransactionAmount);
            objSql.AddParameter("@TransactionDate", DbType.DateTime, ParameterDirection.Input, 0, dto.TransactionDate);
            DataSet ds = objSql.ExecuteDataSet(VERIFY_SAMEDAY_AMOUNT);
            Boolean success = false;
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    success = Convert.ToBoolean(ds.Tables[0].Rows[0]["success"]);
                }
            }
            return success;

        }

        public Boolean VerifyChequeNumber(LedgerTransactionDTO dto)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@ChequeNumber", DbType.String, ParameterDirection.Input, 0, dto.ChequeNumber);
            objSql.AddParameter("@ChequeBankId", DbType.Int32, ParameterDirection.Input, 0, dto.ChequeBankId);
            DataSet ds = objSql.ExecuteDataSet(VERIFY_CHEQUE);
            Boolean success = false;
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    success = Convert.ToBoolean(ds.Tables[0].Rows[0]["success"]);
                }
            }
            return success;

        }
        //modified by ram

        public List<LedgerTransactionDTO> GetReceiptRegister(int ledgerId, int ledgerSiteId,
            DateTime from, DateTime to, int entyType, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@crledgerId", DbType.Int16, ParameterDirection.Input, 0, ledgerId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);

            if (ledgerSiteId > 0)
                objSql.AddParameter("@LedgerSiteId", DbType.Int16, ParameterDirection.Input, 0, ledgerSiteId);
            objSql.AddParameter("@From", DbType.Date, ParameterDirection.Input, 0, Convert.ToDateTime(from));
            objSql.AddParameter("@To", DbType.Date, ParameterDirection.Input, 0, Convert.ToDateTime(to));
            objSql.AddParameter("@EntryType", DbType.Int16, ParameterDirection.Input, 0, entyType);
            var list = objSql.ContructList<LedgerTransactionDTO>(objSql.ExecuteDataSet(LEDGER_TRAN_RECEIPT_REGISTER));
            if (list != null && list.Count > 0)
            {
                var paths = QuickReceiptDocumentHelper.GetReceiptPathsByIds(list.Select(x => x.LedgerTransactionId), companyId);
                foreach (var item in list)
                {
                    if (paths.TryGetValue(item.LedgerTransactionId, out var p) && !string.IsNullOrEmpty(p))
                        item.ReceiptDocumentPath = p;
                }
            }
            return list;
        }

        public List<LedgerTransactionDTO> GetContractReceiptPayments(int contractId, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@ContractId", DbType.Int32, ParameterDirection.Input, 0, contractId);
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            return objSql.ContructList<LedgerTransactionDTO>(objSql.ExecuteDataSet(LEDGER_TRAN_CONTRACT_RECEIPTS));
        }

        public List<LedgerTransactionDTO> LedgerTransactionsAll(int ledgerId, string from, string to, int entyType, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@ledgerId", DbType.Int16, ParameterDirection.Input, 0, ledgerId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);

            objSql.AddParameter("@From", DbType.Date, ParameterDirection.Input, 0, from);
            objSql.AddParameter("@To", DbType.Date, ParameterDirection.Input, 0, to);
            objSql.AddParameter("@EntryType", DbType.Int16, ParameterDirection.Input, 0, entyType);
            return objSql.ContructList<LedgerTransactionDTO>(objSql.ExecuteDataSet(LEDGER_TRANSACTOINS_ALL));
        }
        public async Task<IEnumerable<LedgerTransactionDTO>> PartySiteWisePayments(FilterCriteria filter)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, filter.LedgerId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, filter.CompanyId);
            objSql.AddParameter("@LedgerSiteId", DbType.Int32, ParameterDirection.Input, 0, filter.LedgerSiteId);
            objSql.AddParameter("@From", DbType.Date, ParameterDirection.Input, 0, filter.From);
            objSql.AddParameter("@To", DbType.Date, ParameterDirection.Input, 0, filter.To);

            return await objSql.QueryAsync<LedgerTransactionDTO>(PARTY_SITE_WISE_PAYMENTS);
        }
        /// <summary>
        /// Get Ledger Transaction Lookup Data. It will select the TransactionDate and LedgerId. This 
        /// is being used for navigation (next and prev) of transaction by date
        /// </summary>
        /// <param name="ledgerId">LedgerId</param>
        /// <returns>List of TransactionLookupDTO</returns>
        public List<TransactionLookupDTO> GetLedgerTransactionLookup(int ledgerId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@LedgerId", DbType.Int16, ParameterDirection.Input, 0, ledgerId);
            return objSql.ContructList<TransactionLookupDTO>(objSql.ExecuteDataSet(LEDGER_TRANSACTION_LOOKUP));
        }
        /// <summary>
        /// Get Ledger Transaction Details
        /// </summary>
        /// <param name="ledgerId">LedgerId</param>
        /// <param name="date">Date</param>
        /// <returns></returns>
        public List<LedgerTransactionDTO> GetTransactionDetails(int ledgerId, string date)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@LedgerId", DbType.Int16, ParameterDirection.Input, 0, ledgerId);
            objSql.AddParameter("@Date", DbType.Date, ParameterDirection.Input, 0, date);
            return objSql.ContructList<LedgerTransactionDTO>(objSql.ExecuteDataSet(LEDGER_TRAN_DATA_BY_LOOKUP));
        }

        /// <summary>
        /// Delete Ledger Transaction Details
        /// </summary>
        /// <param name="ledgerTransactionId">ledgerTransactionId</param>
        /// <returns></returns>
        public async Task<bool> DeleteLedgerTransaction(int ledgerTransactionId, LoggedInUserInfo user)
        {
            SQL objSql = new SQL();
            var txn = await GetTransaction(ledgerTransactionId, user.DefaultCompanyId);
            try
            {
                objSql.BeginTransaction();
                objSql.NewCommand();

                objSql.AddParameter("@TransactionId", DbType.Int16, ParameterDirection.Input, 0, ledgerTransactionId);
                var x = objSql.ExecuteNonQuery(LEDGER_TRAN_DELETE) > 0;

                objSql.NewCommand();
                objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, txn.LedgerId);
                objSql.AddParameter("@FinYearId", DbType.Int16, ParameterDirection.Input, 0, txn.FinYearId);
                objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, txn.CompanyId);
                objSql.ExecuteNonQuery(UPDATE_LEDGER_CLOSING_BALANCE);

                if (txn.RefLedgerId > 0)
                {
                    objSql.NewCommand();
                    objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, txn.RefLedgerId);
                    objSql.AddParameter("@FinYearId", DbType.Int16, ParameterDirection.Input, 0, txn.FinYearId);
                    objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, txn.CompanyId);
                    objSql.ExecuteNonQuery(UPDATE_LEDGER_CLOSING_BALANCE);
                }
                objSql.Commit();
                if (x && !string.IsNullOrWhiteSpace(txn.ReceiptDocumentPath))
                {
                    QuickReceiptDocumentHelper.TryDeletePhysicalForWebPath(txn.ReceiptDocumentPath);
                }
                return x;
            }
            catch (Exception ex)
            {
                objSql.Rollback();
                throw ex;
            }
        }

        /// <summary>Clears ReceiptDocumentPath for the transaction (company-scoped) and deletes the permanent file if present.</summary>
        public async Task<bool> ClearQuickReceiptDocument(int ledgerTransactionId, LoggedInUserInfo user)
        {
            if (ledgerTransactionId <= 0 || user == null)
                return false;
            var companyId = user.DefaultCompanyId;
            var oldPath = QuickReceiptDocumentHelper.GetReceiptPathForId(ledgerTransactionId, companyId);

            var objSql = new SQL();
            objSql.NewCommand();
            objSql.AddParameter("@LedgerTransactionId", DbType.Int32, ParameterDirection.Input, 0, ledgerTransactionId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            var affected = await objSql.ExecuteNonQueryAsync(LEDGER_TRANSACTION_CLR_RECEIPT_PATH);
            if (affected <= 0)
                return false;

            if (!string.IsNullOrWhiteSpace(oldPath))
                QuickReceiptDocumentHelper.TryDeletePhysicalForWebPath(oldPath);
            return true;
        }

        /// <summary>
        /// Gets bank entry transactions register
        /// </summary>
        /// <param name="bankId">Bank Id</param>
        /// <returns></returns>
        public List<LedgerTransactionDTO> BankEntryRegister(int bankId, int partyId, int ledgerSiteId, string fromDate, string toDate, Int16 entryTypeId, string cheque, int companyId)
        {
            SQL objSql = new SQL();
            if (bankId > 0)
                objSql.AddParameter("@BankId", DbType.Int16, ParameterDirection.Input, 0, bankId);

            if (partyId > 0)
                objSql.AddParameter("@PartyId", DbType.Int16, ParameterDirection.Input, 0, partyId);
            if (!String.IsNullOrEmpty(fromDate))
                objSql.AddParameter("@From", DbType.Date, ParameterDirection.Input, 0, fromDate);
            if (!String.IsNullOrEmpty(toDate))
                objSql.AddParameter("@To", DbType.Date, ParameterDirection.Input, 0, toDate);
            if (entryTypeId > 0)
                objSql.AddParameter("@EntryType", DbType.Int16, ParameterDirection.Input, 0, entryTypeId);
            if (ledgerSiteId > 0)
            {
                objSql.AddParameter("@ledgerSiteId", DbType.Int16, ParameterDirection.Input, 0, ledgerSiteId);
            }
            if (!String.IsNullOrEmpty(cheque))
                objSql.AddParameter("@TranRefNumber", DbType.String, ParameterDirection.Input, 0, cheque);

            objSql.AddParameter("@companyId", DbType.Int16, ParameterDirection.Input, 0, companyId);

            DataSet ds = objSql.ExecuteDataSet(BANK_ENTRY_REGISTER);
            return objSql.ContructList<LedgerTransactionDTO>(ds);
        }
        /// <summary>
        /// Gets bank entry transactions register
        /// </summary>
        /// <param name="bankId">Bank Id</param>
        /// <returns></returns>
        public DataSet BankEntryRegister_rpt(int bankId, int partyId, string fromDate, string toDate, Int16 entryTypeId, string cheque, int companyId)
        {
            SQL objSql = new SQL();
            if (bankId > 0)
                objSql.AddParameter("@BankId", DbType.Int16, ParameterDirection.Input, 0, bankId);

            if (partyId > 0)
                objSql.AddParameter("@PartyId", DbType.Int16, ParameterDirection.Input, 0, partyId);
            if (!String.IsNullOrEmpty(fromDate))
                objSql.AddParameter("@From", DbType.Date, ParameterDirection.Input, 0, fromDate);
            if (!String.IsNullOrEmpty(toDate))
                objSql.AddParameter("@To", DbType.Date, ParameterDirection.Input, 0, toDate);
            if (entryTypeId > 0)
                objSql.AddParameter("@EntryType", DbType.Int16, ParameterDirection.Input, 0, entryTypeId);

            if (!String.IsNullOrEmpty(cheque))
                objSql.AddParameter("@TranRefNumber", DbType.String, ParameterDirection.Input, 0, cheque);
            objSql.AddParameter("@companyId", DbType.Int16, ParameterDirection.Input, 0, companyId);

            DataSet ds = objSql.ExecuteDataSet(BANK_ENTRY_REGISTER);
            return ds;
        }
        /// <summary>
        /// Party Stock balance
        /// </summary>
        /// <param name="ledgerId">LedgerId</param>
        /// <param name="date">Date</param>
        /// <returns></returns>
        public List<InvoiceItemDTO> PartyStockBalance(int ledgerId, int companyId, string fromdate, string toDate, int ledgerSiteId)
        {
            SQL objSql = new SQL();
            if (ledgerId > 0)
                objSql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);

            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@FromDate", DbType.Date, ParameterDirection.Input, 0, fromdate);
            objSql.AddParameter("@EndDate", DbType.Date, ParameterDirection.Input, 0, toDate);

            objSql.AddParameter("@ledgerSiteId", DbType.Int32, ParameterDirection.Input, 0, ledgerSiteId);
            var _ds = objSql.ExecuteDataSet(PARTY_STOCK_BALANCE);
            return objSql.ContructList<InvoiceItemDTO>(_ds);
        }
        /// <summary>
        /// Party Stock balance
        /// </summary>
        /// <param name="ledgerId">LedgerId</param>
        /// <param name="date">Date</param>
        /// <returns></returns>
        public List<InvoiceItemDTO> PartyStockBalanceBySize(int ledgerId, int companyId, string fromdate, string toDate, int ledgerSiteId)
        {
            SQL objSql = new SQL();
            if (ledgerId > 0)
                objSql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);

            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@FromDate", DbType.Date, ParameterDirection.Input, 0, fromdate);
            objSql.AddParameter("@EndDate", DbType.Date, ParameterDirection.Input, 0, toDate);

            objSql.AddParameter("@ledgerSiteId", DbType.Int32, ParameterDirection.Input, 0, ledgerSiteId);

            return objSql.ContructList<InvoiceItemDTO>(objSql.ExecuteDataSet(PARTY_STOCK_BY_SIZE_BALANCE));
        }
        /// <summary>
        /// Party Stock balance
        /// </summary>
        /// <param name="ledgerId">LedgerId</param>
        /// <param name="date">Date</param>
        /// <returns></returns>
        public DataSet PartyStockBalance_REPORT(int ledgerId, int companyId, string fromdate, string toDate)
        {
            SQL objSql = new SQL();
            if (ledgerId > 0)
                objSql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);

            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@FromDate", DbType.Date, ParameterDirection.Input, 0, fromdate);
            objSql.AddParameter("@EndDate", DbType.Date, ParameterDirection.Input, 0, toDate);
            return objSql.ExecuteDataSet(PARTY_STOCK_BALANCE);
        }
        public DataSet PartyStockBalance_DashBoard(int ledgerId, int companyId, string fromdate, string toDate)
        {
            SQL objSql = new SQL();
            if (ledgerId > 0)
                objSql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);

            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@FromDate", DbType.Date, ParameterDirection.Input, 0, fromdate);
            objSql.AddParameter("@EndDate", DbType.Date, ParameterDirection.Input, 0, toDate);
            return objSql.ExecuteDataSet(PARTY_STOCK_BALANCE_DAHSBOARD);
        }
        /// <summary>
        /// Gets account ledger and balance as on date
        /// </summary>
        /// <param name="ledgerId">LedgerId</param>
        /// <param name="date">Date</param>
        /// <returns></returns>
        public List<AccountLedgerDTO> GetAccountLedger(int ledgerId, string fromDate, String toDate, int ledgerSiteId, int companyId, int finyearId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@finYearId", DbType.Int16, ParameterDirection.Input, 0, finyearId);

            objSql.AddParameter("@From", DbType.Date, ParameterDirection.Input, 0, fromDate);
            objSql.AddParameter("@To", DbType.Date, ParameterDirection.Input, 0, toDate);
            if (ledgerSiteId > 0)
            {
                objSql.AddParameter("@LedgerSiteId", DbType.Int16, ParameterDirection.Input, 0, ledgerSiteId);
            }


            return objSql.ContructList<AccountLedgerDTO>(objSql.ExecuteDataSet(ACCOUNT_LEDGER));
        }
        /// <summary>
        /// Gets account ledger and balance as on date
        /// </summary>
        /// <param name="ledgerId">LedgerId</param>
        /// <param name="date">Date</param>
        /// <returns></returns>
        public List<LedgerTransactionDTO> GetLedgerTransactions(int ledgerId, string fromDate, String toDate, int ledgerSiteId, int companyId, int finyearId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@finYearId", DbType.Int16, ParameterDirection.Input, 0, finyearId);

            objSql.AddParameter("@From", DbType.Date, ParameterDirection.Input, 0, fromDate);
            objSql.AddParameter("@To", DbType.Date, ParameterDirection.Input, 0, toDate);
            if (ledgerSiteId > 0)
            {
                objSql.AddParameter("@LedgerSiteId", DbType.Int16, ParameterDirection.Input, 0, ledgerSiteId);
            }


            return objSql.ContructList<LedgerTransactionDTO>(objSql.ExecuteDataSet(ACCOUNT_LEDGER_DETAILS));
        }
        public List<LedgerTransactionDTO> GetAccountGroupLedgerTransactions(int accountGroupId, string fromDate,
            String toDate, int ledgerSiteId, int companyId, int finyearId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@accountGroupId", DbType.Int32, ParameterDirection.Input, 0, accountGroupId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@finYearId", DbType.Int16, ParameterDirection.Input, 0, finyearId);

            objSql.AddParameter("@From", DbType.Date, ParameterDirection.Input, 0, fromDate);
            objSql.AddParameter("@To", DbType.Date, ParameterDirection.Input, 0, toDate);



            return objSql.ContructList<LedgerTransactionDTO>(objSql.ExecuteDataSet(ACCOUNTGROUP_LEDGER_DETAILS));
        }

        /// <summary>
        /// Gets account ledger and balance as on date
        /// </summary>
        /// <param name="ledgerId">LedgerId</param>
        /// <param name="date">Date</param>
        /// <returns></returns>
        public LedgerTransactionDTO GetAccountBalanceForBill(int ledgerId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@LedgerId", DbType.Int16, ParameterDirection.Input, 0, ledgerId);




            return objSql.ContructList<LedgerTransactionDTO>(objSql.ExecuteDataSet(ACCOUNT_BALANCEFORBILL)).FirstOrDefault();
        }

        /// <summary>
        /// Gets account ledger and balance as on date
        /// </summary>
        /// <param name="ledgerId">LedgerId</param>
        /// <param name="date">Date</param>
        /// <returns></returns>
        public DataSet GetAccountLedger_rpt(int ledgerId, string fromDate, String toDate)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@LedgerId", DbType.Int16, ParameterDirection.Input, 0, ledgerId);
            objSql.AddParameter("@From", DbType.Date, ParameterDirection.Input, 0, fromDate);
            objSql.AddParameter("@To", DbType.Date, ParameterDirection.Input, 0, toDate);


            return objSql.ExecuteDataSet(ACCOUNT_LEDGER);
        }
        /// <summary>
        /// Gets account receipts or payments and prints on pdf using rdlc
        /// </summary>
        /// <param name="ledgerId">LedgerId</param>
        /// <param name="date">Date</param>
        /// <returns></returns>
        public DataSet GetReceiptRegisterPRT(int ledgerId, string fromDate, String toDate, int entryType, string receiptNumber)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@LedgerId", DbType.Int16, ParameterDirection.Input, 0, ledgerId);
            if (!String.IsNullOrEmpty(fromDate))
                objSql.AddParameter("@From", DbType.Date, ParameterDirection.Input, 0, fromDate);
            if (!String.IsNullOrEmpty(toDate))
                objSql.AddParameter("@To", DbType.Date, ParameterDirection.Input, 0, toDate);

            objSql.AddParameter("@EntryType", DbType.Int16, ParameterDirection.Input, 0, entryType);
            if (!String.IsNullOrEmpty(receiptNumber))
                objSql.AddParameter("@ReceiptNumber", DbType.Date, ParameterDirection.Input, 0, receiptNumber);
            return objSql.ExecuteDataSet(LEDGER_RECEIPT_REGISTER_RPT);
        }
        /// <summary>
        /// Gets site ledger and balance as on date
        /// </summary>
        /// <param name="ledgerId">LedgerId</param>
        /// <param name="date">Date</param>
        /// <returns></returns>
        public List<LedgerTransactionDTO> GetFixedSiteLedger(int ledgerId, string fromDate, String toDate, int workOrderId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@LedgerId", DbType.Int16, ParameterDirection.Input, 0, ledgerId);
            objSql.AddParameter("@From", DbType.Date, ParameterDirection.Input, 0, fromDate);
            objSql.AddParameter("@To", DbType.Date, ParameterDirection.Input, 0, toDate);
            objSql.AddParameter("@workOrderId", DbType.Int32, ParameterDirection.Input, 0, workOrderId);

            return objSql.ContructList<LedgerTransactionDTO>(objSql.ExecuteDataSet(FIXD_SITE_LEDGER));
        }

        public DataSet GetTransactionById(int ledgerTransactionId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@TransactionId", DbType.Int32, ParameterDirection.Input, 0, ledgerTransactionId);
            return objSql.ExecuteDataSet(TRANSACTION_BY_ID);

        }
        public async Task<LedgerTransactionDTO> GetTransaction(int ledgerTransactionId, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@TransactionId", DbType.Int32, ParameterDirection.Input, 0, ledgerTransactionId);
            var txn = await objSql.QueryFirstAsync<LedgerTransactionDTO>(TRANSACTION_BY_ID);
            objSql.NewCommand();
            objSql.AddParameter("@TransactionId", DbType.Int32, ParameterDirection.Input, 0, ledgerTransactionId);
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            var list = await objSql.QueryAsync<LedgerTransactionDetailDto>(TRANSACTION_DETAIL_BY_ID);
            if (list != null)
            {
                txn.TxnDetails = list.ToList();
            }
            var receiptPath = QuickReceiptDocumentHelper.GetReceiptPathForId(ledgerTransactionId, companyId);
            if (!string.IsNullOrEmpty(receiptPath))
                txn.ReceiptDocumentPath = receiptPath;
            objSql.Dispose();
            return txn;
        }


        public List<ProductRateDTO> ProductRates(int ledgerId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);
            return objSql.ContructList<ProductRateDTO>(objSql.ExecuteDataSet(LEDGER_RATES));

        }
        public List<LedgerTransactionDTO> GetDrCrNotes(int ledgerId, int companyId, int finYearId, int entryTypeId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);
            objSql.AddParameter("@FinYearId", DbType.Int32, ParameterDirection.Input, 0, finYearId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);

            // objSql.AddParameter("@TransactionType", DbType.Int16, ParameterDirection.Input, 0, transactionType);
            objSql.AddParameter("@entryTypeId", DbType.Int16, ParameterDirection.Input, 0, entryTypeId);


            return objSql.ContructList<LedgerTransactionDTO>(objSql.ExecuteDataSet(SELECT_DR_CR_NOTES));

        }
        public DataSet GetPartyBalance(int ledgerId, int finYearId, int companyId, int ledgerSiteId, string from, string to)
        {
            SQL objSql = new SQL();
            if (ledgerId > 0)
                objSql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);

            objSql.AddParameter("@FinYearId", DbType.Int32, ParameterDirection.Input, 0, finYearId);
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@From", DbType.Date, ParameterDirection.Input, 0, from);
            objSql.AddParameter("@To", DbType.Date, ParameterDirection.Input, 0, to);
            if (ledgerSiteId > 0)
            {
                objSql.AddParameter("@LedgerSiteId", DbType.Int32, ParameterDirection.Input, 0, ledgerSiteId);
            }
            return objSql.ExecuteDataSet(PARTY_BALANCE);

        }
        public DataSet UnbilledSites(int finYearId, int companyId)
        {
            SQL objSql = new SQL();

            objSql.AddParameter("@FinYearId", DbType.Int32, ParameterDirection.Input, 0, finYearId);
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            return objSql.ExecuteDataSet(PARTY_UNBILLED_SITES);

        }

        public DataSet ClientWiseItems(int ledgerId, int ledgerSiteId, int companyId, string from,
            string to, string poNumber = "", string balanceType = "rent", byte forBill = 0)
        {
            SQL objSql = new SQL();

            if (ledgerId > 0)
                objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);
            objSql.AddParameter("@ledgerSiteId", DbType.Int32, ParameterDirection.Input, 0, ledgerSiteId);

            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@FromDate", DbType.Date, ParameterDirection.Input, 0, from);
            objSql.AddParameter("@EndDate", DbType.Date, ParameterDirection.Input, 0, to);
            objSql.AddParameter("@balanceType", DbType.String, ParameterDirection.Input, 0, balanceType);
            if (!String.IsNullOrEmpty(poNumber))
                objSql.AddParameter("@poNumber", DbType.String, ParameterDirection.Input, 0, poNumber);
            objSql.AddParameter("@forBill", DbType.Byte, ParameterDirection.Input, 0, forBill);


            return objSql.ExecuteDataSet(CLIENT_WISE_ITEMS);

        }
        public DataSet ItemWiseClients(int companyId, int productId, string from, string to, string balanceType = "rent")
        {
            SQL objSql = new SQL();

            objSql.AddParameter("@ProductId", DbType.Int32, ParameterDirection.Input, 0, productId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);

            objSql.AddParameter("@FromDate", DbType.Date, ParameterDirection.Input, 0, from);
            objSql.AddParameter("@EndDate", DbType.Date, ParameterDirection.Input, 0, to);
            objSql.AddParameter("@balanceType", DbType.String, ParameterDirection.Input, 0, balanceType);

            return objSql.ExecuteDataSet(ITEM_WISE_CLIENTS);

        }
        public DataSet Cashbook(int accountGroupId, int companyId, string from, string to)
        {
            SQL objSql = new SQL();

            objSql.AddParameter("@AccountGroupId", DbType.Int32, ParameterDirection.Input, 0, accountGroupId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@From", DbType.Date, ParameterDirection.Input, 0, from);
            objSql.AddParameter("@To", DbType.Date, ParameterDirection.Input, 0, to);
            return objSql.ExecuteDataSet(CASH_BOOK);

        }

        public LedgerDTO GetByPhone(string phone)
        {
            SQL objSql = new SQL();

            objSql.AddParameter("@phone", DbType.String, ParameterDirection.Input, 0, phone);
            var ds = objSql.ExecuteDataSet(LEDGERBYPHONE);
            if (ds != null && ds.Tables.Count > 0)
            {
                var ldto = (from d in ds.Tables[0].AsEnumerable()

                            select new LedgerDTO
                            {
                                LedgerId = d.Field<int>("LedgerId"),
                                Phone1 = d.Field<string>("ContactPersonMobile")
                            }
                            ).FirstOrDefault();
                return ldto;
            }
            return null;
        }
        public List<InvoiceDTO> BillsByPhone(string phone)
        {
            SQL objSql = new SQL();

            objSql.AddParameter("@partyMobileNo", DbType.String, ParameterDirection.Input, 0, phone);
            var ds = objSql.ExecuteDataSet(PARTY_BILLS_BY_PHONE);
            if (ds != null && ds.Tables.Count > 0)
            {
                var bills = (from d in ds.Tables[0].AsEnumerable()

                             select new InvoiceDTO
                             {
                                 InvoiceDate = d.Field<DateTime>("InvoiceDate"),
                                 Company = d.Field<string>("Company"),
                                 InvoiceNumber = d.Field<string>("InvoiceNumber"),
                                 From = d["From"] == DBNull.Value ? DateTime.MinValue : d.Field<DateTime>("From"),
                                 To = d["To"] == DBNull.Value ? DateTime.MinValue : d.Field<DateTime>("To"),
                                 Balance = d["Balance"] == DBNull.Value ? 0 : d.Field<double>("Balance"),
                                 Total = Convert.ToDouble(d.Field<decimal>("RoundedAmount")),
                                 LedgerId = d.Field<Int32>("LedgerId"),
                                 CompanyId = d.Field<Int32>("CompanyId"),
                                 LedgerName = d.Field<string>("PartyName"),

                             }
                            ).ToList();
                return bills;
            }
            return null;
        }

        #endregion


        #region ProductRates
        public bool AddUpdateProductRates(int ledgerId, List<ProductRateDTO> list)
        {
            SQL objSql = new SQL();

            try
            {
                objSql.BeginTransaction();
                foreach (ProductRateDTO dto in list)
                {

                    objSql.NewCommand();
                    objSql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);
                    objSql.AddParameter("@LedgerSiteId", DbType.Int32, ParameterDirection.Input, 0, dto.LedgerSiteId);

                    objSql.AddParameter("@ProductId", DbType.Int32, ParameterDirection.Input, 0, dto.ProductId);
                    objSql.AddParameter("@Unit", DbType.Int32, ParameterDirection.Input, 0, dto.Unit);
                    objSql.AddParameter("@RentRate", DbType.Double, ParameterDirection.Input, 0, dto.RentRate);
                    objSql.AddParameter("@LossRate", DbType.Double, ParameterDirection.Input, 0, dto.LossRate);
                    objSql.AddParameter("@Damagerate", DbType.Double, ParameterDirection.Input, 0, dto.DamageRate);
                    objSql.AddParameter("@ModifiedBy", DbType.Int32, ParameterDirection.Input, 0, dto.ModifiedBy);
                    objSql.AddParameter("@ProductSizeId", DbType.Int32, ParameterDirection.Input, 0, dto.ProductSizeId);
                    objSql.AddParameter("@OpeningBalance", DbType.Double, ParameterDirection.Input, 0, dto.OpeningBalance);
                    objSql.AddParameter("@UnitSizeRate", DbType.Double, ParameterDirection.Input, 0, dto.UnitSizeRate);

                    objSql.ExecuteNonQuery(ADD_UPDATE_PRODUCT_RATES);
                }
                objSql.Commit();
            }
            catch (Exception ex)
            {
                objSql.Rollback();
                throw ex;
            }
            return true;



        }
        public List<ProductRateDTO> GetProductRates(int ledgerId, int ledgerSiteId, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@ledgerSiteId", DbType.Int32, ParameterDirection.Input, 0, ledgerSiteId);

            return objSql.ContructList<ProductRateDTO>(objSql.ExecuteDataSet(SELECT_PRODUCT_RATES));

        }
        #endregion

        #region Sites
        public bool AddSite(ClientSiteDTO dto)
        {
            SQL objSql = new SQL();
            objSql.BeginTransaction();
            try
            {
                objSql.NewCommand();
                objSql.AddParameter("@SiteAddress", DbType.String, ParameterDirection.Input, 0, dto.SiteAddress);
                objSql.AddParameter("@StateId", DbType.Int32, ParameterDirection.Input, 0, dto.StateId);
                objSql.AddParameter("@taxCategoryId", DbType.Int32, ParameterDirection.Input, 0, dto.TaxCategoryId);
                objSql.AddParameter("@zipCode", DbType.String, ParameterDirection.Input, 0, dto.ZipCode);

                objSql.AddParameter("@SiteGST", DbType.String, ParameterDirection.Input, 0, dto.SiteGST);
                objSql.AddParameter("@ContactPerson", DbType.String, ParameterDirection.Input, 0, dto.ContactPerson);
                objSql.AddParameter("@ContactPersonPhone", DbType.String, ParameterDirection.Input, 0, dto.ContactPersonPhone);
                objSql.AddParameter("@City", DbType.String, ParameterDirection.Input, 0, dto.City);
                objSql.AddParameter("@project", DbType.String, ParameterDirection.Input, 0, dto.Project);
                objSql.AddParameter("@address2", DbType.String, ParameterDirection.Input, 0, dto.Address2);
                objSql.AddParameter("@poNumber", DbType.String, ParameterDirection.Input, 0, dto.PONumber);
                objSql.AddParameter("@printLastBillDetails", DbType.Byte, ParameterDirection.Input, 0, dto.PrintLastBillDetails);
                objSql.AddParameter("@printBalanceMaterial", DbType.Byte, ParameterDirection.Input, 0, dto.PrintBalanceMaterial);
                objSql.AddParameter("@useForBilling", DbType.Byte, ParameterDirection.Input, 0, dto.UseForBilling);
                objSql.AddParameter("@projectOwnerId", DbType.Int32, ParameterDirection.Input, 0, dto.ProjectOwnerId);
                objSql.AddParameter("@billingToSiteId", DbType.Int32, ParameterDirection.Input, 0, dto.BillingToSiteId);
                objSql.AddParameter("@VAT", DbType.String, ParameterDirection.Input, 0, dto.VAT);

                if (dto.PODate != null && dto.PODate.Value.Year > 2000)
                {
                    objSql.AddParameter("@poDate", DbType.Date, ParameterDirection.Input, 0, dto.PODate);
                }
                if (dto.LedgerSiteId == 0)
                {
                    objSql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 0, dto.LedgerId);
                    dto.LedgerSiteId = Convert.ToInt32(objSql.ExecuteScalar(ADD_SITE));
                }
                else
                {
                    objSql.AddParameter("@LedgerSiteId", DbType.Int32, ParameterDirection.Input, 0, dto.LedgerSiteId);
                    objSql.ExecuteNonQuery(UPDATE_SITE);
                }

                if (dto.Taxes != null)
                {
                    AddLedgerSiteTax(dto.LedgerId, dto.LedgerSiteId, dto.Taxes, objSql);
                }

                objSql.Commit();
                return true;
            }
            catch (Exception ex)
            {
                objSql.Rollback();
                throw ex;
            }
        }

        /// <summary>
        /// Persists Document1FileName–Document5FileName on ClientSites (files are stored separately under docs/sites).
        /// </summary>
        public bool UpdateClientSiteDocuments(ClientSiteDTO dto)
        {
            if (dto == null || dto.LedgerSiteId <= 0)
            {
                return false;
            }
            SQL objSql = new SQL();
            objSql.NewCommand();
            objSql.AddParameter("@LedgerSiteId", DbType.Int32, ParameterDirection.Input, 0, dto.LedgerSiteId);
            objSql.AddParameter("@Document1FileName", DbType.String, ParameterDirection.Input, 260, (object)dto.Document1FileName ?? DBNull.Value);
            objSql.AddParameter("@Document2FileName", DbType.String, ParameterDirection.Input, 260, (object)dto.Document2FileName ?? DBNull.Value);
            objSql.AddParameter("@Document3FileName", DbType.String, ParameterDirection.Input, 260, (object)dto.Document3FileName ?? DBNull.Value);
            objSql.AddParameter("@Document4FileName", DbType.String, ParameterDirection.Input, 260, (object)dto.Document4FileName ?? DBNull.Value);
            objSql.AddParameter("@Document5FileName", DbType.String, ParameterDirection.Input, 260, (object)dto.Document5FileName ?? DBNull.Value);
            return objSql.ExecuteNonQuery(UPDATE_CLIENT_SITE_DOCUMENTS) >= 0;
        }

        public List<ClientSiteDTO> GetSites(int ledgerId, byte closed = 0)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);
            objSql.AddParameter("@allSites", DbType.Int16, ParameterDirection.Input, 0, closed);

            var sites = objSql.ContructList<ClientSiteDTO>(objSql.ExecuteDataSet(SEL_SITES));
            MergeClientSiteDocumentFileNames(ledgerId, null, sites, null);
            return sites;

        }
        public ClientSiteDTO GetSiteById(int ledgerSiteId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@ledgerSiteId", DbType.Int32, ParameterDirection.Input, 0, ledgerSiteId);
            var site = objSql.ContructList<ClientSiteDTO>(objSql.ExecuteDataSet(GET_CLIENT_SITE_BY_ID)).FirstOrDefault();
            if (site != null)
            {
                MergeClientSiteDocumentFileNames(null, ledgerSiteId, null, site);
            }
            return site;

        }

        static void MergeClientSiteDocumentFileNames(int? ledgerId, int? ledgerSiteId, List<ClientSiteDTO> sites, ClientSiteDTO single)
        {
            SQL objSql = new SQL();
            objSql.NewCommand();
            if (ledgerSiteId.HasValue && ledgerSiteId.Value > 0)
            {
                objSql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 0, DBNull.Value);
                objSql.AddParameter("@LedgerSiteId", DbType.Int32, ParameterDirection.Input, 0, ledgerSiteId.Value);
            }
            else if (ledgerId.HasValue && ledgerId.Value > 0)
            {
                objSql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId.Value);
                objSql.AddParameter("@LedgerSiteId", DbType.Int32, ParameterDirection.Input, 0, DBNull.Value);
            }
            else
            {
                return;
            }

            var docRows = objSql.ContructList<ClientSiteDTO>(objSql.ExecuteDataSet(CLIENT_SITES_DOCUMENT_NAMES_SEL));
            if (single != null)
            {
                var r = docRows.FirstOrDefault();
                if (r != null)
                {
                    CopySiteDocumentNames(single, r);
                }
                return;
            }
            if (sites == null)
            {
                return;
            }
            foreach (var s in sites)
            {
                var r = docRows.FirstOrDefault(x => x.LedgerSiteId == s.LedgerSiteId);
                if (r != null)
                {
                    CopySiteDocumentNames(s, r);
                }
            }
        }

        static void CopySiteDocumentNames(ClientSiteDTO target, ClientSiteDTO source)
        {
            if (source.LedgerId > 0)
            {
                target.LedgerId = source.LedgerId;
            }
            target.Document1FileName = source.Document1FileName;
            target.Document2FileName = source.Document2FileName;
            target.Document3FileName = source.Document3FileName;
            target.Document4FileName = source.Document4FileName;
            target.Document5FileName = source.Document5FileName;
        }
        public List<LedgerTaxDTO> GetSiteTaxes(int ledgerSiteId)
        {
            SQL objSql = new SQL();
            if (ledgerSiteId > 0)
            {
                objSql.AddParameter("@LedgerSiteId", DbType.Int32, ParameterDirection.Input, 0, ledgerSiteId);
            }
            return objSql.ContructList<LedgerTaxDTO>(objSql.ExecuteDataSet(GET_SITE_TAXES));
        }
        internal bool AddLedgerSiteTax(int ledgerId, int ledgerSiteId, List<LedgerTaxDTO> taxes, SQL sql = null)
        {
            SQL objSql = sql;
            if (objSql == null)
            {
                objSql = new SQL();
                objSql.BeginTransaction();
            }
            try
            {
                foreach (LedgerTaxDTO dto in taxes)
                {
                    objSql.NewCommand();
                    dto.LedgerId = ledgerId;
                    dto.LedgerSiteId = ledgerSiteId;
                    AddLedgerSiteTax(dto, objSql);
                }
                if (sql == null)
                {
                    objSql.Commit();
                }
                return true;
            }
            catch (Exception ex)
            {
                if (sql == null)
                {
                    objSql.Rollback();
                }
                throw ex;
            }
        }
        internal bool AddLedgerSiteTax(LedgerTaxDTO taxDto, SQL sql = null)
        {
            SQL objSql = sql;
            if (objSql == null)
            {
                objSql = new SQL();
                objSql.BeginTransaction();
            }

            objSql.AddParameter("@Rate", DbType.Double, ParameterDirection.Input, 0, taxDto.Rate);

            if (taxDto.LedgerTaxId > 0)
            {
                objSql.AddParameter("@LedgerTaxId", DbType.Int32, ParameterDirection.Input, 0, taxDto.LedgerTaxId);
                return objSql.ExecuteNonQuery(LEDGER_SITE_TAX_UPD) > 0;
            }
            else
            {
                objSql.AddParameter("@TaxId", DbType.Int32, ParameterDirection.Input, 0, taxDto.TaxId);
                objSql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 0, taxDto.LedgerId);
                objSql.AddParameter("@LedgerSiteId", DbType.Int32, ParameterDirection.Input, 0, taxDto.LedgerSiteId);
                objSql.AddParameter("@FinYearId", DbType.Int32, ParameterDirection.Input, 0, taxDto.FinYearId);
                return objSql.ExecuteNonQuery(LEDGER_SITE_TAX_ADD) > 0;
            }

        }

        /// <summary>
        /// Get last invoice of the party in a financial year.
        /// </summary>
        /// <param name="ledgerId"></param>
        /// <param name="finYearId"></param>
        /// <returns></returns>
        internal InvoiceDTO GetLastBill(int ledgerId, int ledgerSiteId, int finYearId, int invoiceId = 0)
        {
            SQL sql = new SQL();
            sql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);
            sql.AddParameter("@FinYearId", DbType.Int32, ParameterDirection.Input, 0, finYearId);
            sql.AddParameter("@invoiceId", DbType.Int32, ParameterDirection.Input, 0, invoiceId);

            if (ledgerSiteId > 0)
            {
                sql.AddParameter("@ledgerSiteId", DbType.Int32, ParameterDirection.Input, 0, ledgerSiteId);
            }
            return sql.ContructList<InvoiceDTO>(sql.ExecuteDataSet(LAST_BILL)).FirstOrDefault();

        }
        internal bool RemoveLedger(int ledgerId)
        {
            SQL sql = new SQL();
            sql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);
            return sql.ExecuteNonQuery(REMOVE_LEDGER) > 0;

        }
        public List<LedgerTransactionDTO> EstimatedRentPerDay(int ledgerId, string fromDate, string toDate, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);

            objSql.AddParameter("@FromDate", DbType.Date, ParameterDirection.Input, 0, fromDate);

            objSql.AddParameter("@EndDate", DbType.Date, ParameterDirection.Input, 0, toDate);
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            return objSql.ContructList<LedgerTransactionDTO>(objSql.ExecuteDataSet(ESTIMATED_RENT_PER_DAY));

        }
        public async Task<IEnumerable<ClientSiteDTO>> AllClientsWithSites(int companyId)
        {
            SQL objSql = new SQL();

            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            return await objSql.QueryAsync<ClientSiteDTO>(p_ledgerSitesWithLedger_sel);

        }
        public async Task<IEnumerable<AccountLedgerDTO>> TrialBalance(FilterCriteria dto)
        {
            SQL objSql = new SQL();

            objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, dto.LedgerId);
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
            objSql.AddParameter("@To", DbType.Date, ParameterDirection.Input, 0, dto.To);

            return await objSql.QueryAsync<AccountLedgerDTO>(TRIAL_BALANNCE);

        }
        internal List<ClientSiteDTO> GetAllClientSites(string jobNumber, string site, string client, string siteEng, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, companyId);

            if (!String.IsNullOrEmpty(jobNumber))
            {
                objSql.AddParameter("@JobNumber", DbType.String, ParameterDirection.Input, 0, jobNumber);
            }
            if (!String.IsNullOrEmpty(site))
            {
                objSql.AddParameter("@site", DbType.String, ParameterDirection.Input, 0, site);
            }
            if (!String.IsNullOrEmpty(client))
            {
                objSql.AddParameter("@client", DbType.String, ParameterDirection.Input, 0, client);
            }
            if (!String.IsNullOrEmpty(siteEng))
            {
                objSql.AddParameter("@SiteEng", DbType.String, ParameterDirection.Input, 0, siteEng);
            }

            return objSql.ContructList<ClientSiteDTO>(objSql.ExecuteDataSet(ALL_CLIENTS_SITES));
        }
        public List<SiteDTO> GetClientJobs(int ledgerId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);


            return objSql.ContructList<SiteDTO>(objSql.ExecuteDataSet(GET_CLIENT_JOBS));
        }
        #endregion
        public async Task<int> CloseSite(ClientSiteDTO dto, LoggedInUserInfo user)
        {
            SQL objSql = new SQL();
            try
            {
                objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, dto.LedgerId);
                objSql.AddParameter("@ledgerSiteId", DbType.Int32, ParameterDirection.Input, 0, dto.LedgerSiteId);
                objSql.AddParameter("@closedDate", DbType.DateTime, ParameterDirection.Input, 0, dto.ClosedOn);
                objSql.AddParameter("@closedBy", DbType.Int32, ParameterDirection.Input, 0, dto.ClosedBy);
                objSql.AddParameter("@remarks", DbType.String, ParameterDirection.Input, 0, dto.ClosedRemarks);

                return await objSql.ExecuteNonQueryAsync(CLOSE_SITE);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> CopyRates(CopyRatesDTO copy,

            LoggedInUserInfo user)
        {
            SQL objSql = new SQL();
            try
            {
                objSql.BeginTransaction();
                foreach (var dto in copy.CopyTo)
                {
                    objSql.NewCommand();

                    objSql.AddParameter("@copyFrom", DbType.Int32, ParameterDirection.Input, 0, copy.CopyFromSiteId);
                    objSql.AddParameter("@copyTo", DbType.Int32, ParameterDirection.Input, 0, dto.LedgerSiteId);
                    objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, dto.LedgerId);
                    objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, user.DefaultCompanyId);
                    objSql.AddParameter("@createdBy", DbType.Int32, ParameterDirection.Input, 0, user.UserId);
                    objSql.AddParameter("@CreatedOn", DbType.DateTime, ParameterDirection.Input, 0, DateTime.Now);

                    var d = await objSql.ExecuteNonQueryAsync(COPY_RATES);
                }
                objSql.Commit();
                return 1;
            }
            catch (Exception ex)
            {
                objSql.Rollback();
                throw ex;
            }
        }
        public async Task<IEnumerable<LedgerTransactionDTO>> GetAdvancereceipts(FilterCriteria filter)
        {
            using (SQL objSql = new SQL())
            {
                objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, filter.LedgerId);
                objSql.AddParameter("@ledgerSiteId", DbType.Int32, ParameterDirection.Input, 0, filter.LedgerSiteId);
                objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, filter.CompanyId);


                return await objSql.QueryAsync<LedgerTransactionDTO>(GET_ADVANCE_RECEIPTS);
            }
        }

        public async Task<decimal> getOpeningBalacneAsOnDate(FilterCriteria filter)
        {
            using (SQL objSql = new SQL())
            {
                objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, filter.LedgerId);
                objSql.AddParameter("@ledgerSiteId", DbType.Int32, ParameterDirection.Input, 0, filter.LedgerSiteId);
                objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, filter.CompanyId);
                objSql.AddParameter("@date", DbType.Date, ParameterDirection.Input, 0, filter.OnDate);


                var cb = await objSql.ExecuteScalarAsync(LEDGER_OPENING_BALANCE);
                return Convert.ToDecimal(cb);
            }
        }
        #region Procedures
        const string CLOSE_SITE = "p_CloseSite";

        const string ADD = "p_Ledger_ins";
        const string UPDATE = "p_ledger_upd";
        const string GETALL = "p_ledger_getAll";
        const string GETALL_BY_GROUP = "p_ledgerBygroup";
        const string SELCECTCOMPANY = "p_ledger_sel";
        const string ACTIVATE_DEACTIVATE = "p_Ledger_ActivateDeactivate";
        const string CHANGE_ADDRESS = "";
        const string CREATE_TRANSACTION = "p_LedgerTransactions_ins";
        const string SET_TRANSACTION_CONTRACT_ID = "p_LedgerTransactions_setContractId";
        const string LEDGER_TRANSACTION_UPD_RECEIPT_PATH = "p_LedgerTransactions_upd_receiptPath";
        const string LEDGER_TRANSACTION_CLR_RECEIPT_PATH = "p_LedgerTransactions_clr_receiptPath";
        const string LEDGER_TRAN_CONTRACT_RECEIPTS = "p_LedgerTransactions_sel_contractReceipts";
        // const string DEL_TRANSACTION = "p_LedgerTransactions_del";
        const string UPDATE_LEDGER_CLOSING_BALANCE = "p_updateledgerBalance";
        const string CREATE_LEDGERTRANSACTION_DETAIL = "p_ledgerTransactionDetail_ins";
        const string LEDGER_TRANSACTION_DETAIL_SET_TDS = "p_ledgerTransactionDetail_setTds";
        const string LEDGER_TRANSACTION_UPD_TOTAL_TDS = "p_LedgerTransactions_updTotalTds";
        const string UPDATE_PURCHASE_PAYMENT = "p_purchaseInvoicePayment_upd";
        const string UPDATE_INVOICE_PAYMENT = "p_InvoicePayment_upd";

        const string VERIFY_CHEQUE = "p_VerifyCheque";
        const string VERIFY_SAMEDAY_AMOUNT = "p_VerifySameDayAmount";
        const string LEDGER_TRAN_RECEIPT_REGISTER = "p_LedgerTransRegister";
        const string PARTY_STOCK_REGISTER = "p_party_Stock_Register";
        const string PARTY_OPENING_BALANCE = "p_party_OpeningBalance";
        const string LEDGER_TRANSACTION_LOOKUP = "p_BankTrans_Lookup";
        const string LEDGER_TRAN_DATA_BY_LOOKUP = "p_LedgerTranByDate";
        const string LEDGER_TRAN_DELETE = "p_LedgerTransactions_Delete";
        const string BANK_ENTRY_REGISTER = "p_BankEntryregister_sel";
        const string PARTY_STOCK_BALANCE = "p_PartyStock_balance";
        const string PARTY_STOCK_BY_SIZE_BALANCE = "p_PartyStockBySize_balance";

        const string PARTY_STOCK_BALANCE_DAHSBOARD = "p_PartyStock_balance_report";
        const string ACCOUNT_LEDGER = "p_Account_ledger";
        const string ACCOUNT_LEDGER_DETAILS = "p_Account_ledgerDetails";
        const string ACCOUNTGROUP_LEDGER_DETAILS = "p_AccountGroup_ledgerDetails";

        const string LEDGER_RECEIPT_REGISTER_RPT = "p_LedgerTransRegisterRpt";
        const string ADD_UPDATE_PRODUCT_RATES = "p_ProductRates_ins";
        const string SELECT_PRODUCT_RATES = "p_ProductRates_sel";
        const string FIXD_SITE_LEDGER = "p_Site_Fixedledger";
        const string ADD_SITE = "p_ClientSites_ins";
        const string UPDATE_SITE = "p_ClientSites_upd";
        const string UPDATE_CLIENT_SITE_DOCUMENTS = "p_ClientSites_UpdateDocuments";
        const string CLIENT_SITES_DOCUMENT_NAMES_SEL = "p_ClientSites_DocumentNames_sel";
        const string SEL_SITES = "p_ClientSites_sel";
        const string GET_CLIENT_SITE_BY_ID = "p_ClientSiteById_sel";
        const string TRANSACTION_BY_ID = "p_LedgerTrans_ById";
        const string TRANSACTION_DETAIL_BY_ID = "p_LedgerTransactionDetail_sel";

        const string LEDGER_RATES = "p_Ledger_Rates";
        const string ESTIMATED_RENT_PER_DAY = "p_PartyWise_RentPerDay";
        const string GET_SITE_TAXES = "p_LedgerSiteTaxes_sel";
        const string LEDGER_SITE_TAX_ADD = "p_LedgerSiteTaxes_ins";
        const string LEDGER_SITE_TAX_UPD = "p_LedgerSiteTaxes_upd";
        const string LAST_BILL = "p_LastInvoice_sel";
        const string REMOVE_LEDGER = "p_RemoveLedger";
        const string SELECT_DR_CR_NOTES = "p_clientDrCrNotes";
        const string PARTY_BALANCE = "p_PartyBalance_sel";
        const string PARTY_UNBILLED_SITES = "p_unbilled_Sites";
        const string CLIENT_WISE_ITEMS = "p_ClientWiseItems";
        const string ITEM_WISE_CLIENTS = "p_ItemWiseClients";
        const string CASH_BOOK = "p_Account_Statement";
        const string ACCOUNT_BALANCEFORBILL = "p_AccountLastBalanceForBill";
        const string LEDGERBYPHONE = "p_getPartyByPhone";
        const string PARTY_BILLS_BY_PHONE = "p_partyBills";
        const string p_ledgerSitesWithLedger_sel = "p_ledgerSitesWithLedger_sel";
        const string TRIAL_BALANNCE = "p_trialbalance";
        const string PROFIT_AND_LOSS_STATEMENT = "p_profitandLossStatement";
        const string LEDGER_TRANSACTOINS_ALL = "p_LedgerTransactions";
        const string ALL_CLIENTS_SITES = "p_CLientSites_all";
        const string GET_CLIENT_JOBS = "p_GetClientJobs";
        const string PARTY_SITE_WISE_PAYMENTS = "p_LedgerSiteWisePayments";
        const string COPY_RATES = "p_copyRates";
        const string GET_ADVANCE_RECEIPTS = "p_getAdvanceReceipts";
        const string LEDGER_OPENING_BALANCE = "p_openingbalanceAsOnDate";

        #endregion
    }
}
