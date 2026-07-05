using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BAL.DTO;
using System.Data;
using BAL.Exceptions;
using BAL.Objects;
using System.Threading.Tasks;
using BAL.Common;
using BAL.Enums;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
namespace BAL.DAL
{
    internal class WorkorderDAL
    {
        WorkOrderDTO _dataObject;
        internal int Save(WorkOrderDTO dataOBject, SQL sql = null)
        {
            SQL objSql = sql;
            if (sql == null)
            {
                objSql = new SQL();
            }
            Int32 _workOrderId = 0;
            try
            {
                var sites = this.GetSites(dataOBject.WorkOrderId).FirstOrDefault();
                //if (sites != null)
                //{
                //    dataOBject.Sites[0].SiteId = sites.SiteId;
                //}
                _dataObject = dataOBject;

                objSql.BeginTransaction();

                objSql.NewCommand();

                var siteObj = dataOBject.Sites[0];
                if (dataOBject.OtherCharges != null)
                {
                    dataOBject.TotalOtherCharges = dataOBject.OtherCharges.Sum(o => o.Amount);
                }
                dataOBject.Freight = siteObj.Freight;
                if (siteObj.AppliedTaxes != null && siteObj.AppliedTaxes.Count > 0)
                {
                    ApplyHeaderTaxTotalsFromAppliedTaxes(dataOBject, siteObj.AppliedTaxes);
                }
                else
                {
                    dataOBject.TotalTax = dataOBject.IGSTAmount + dataOBject.CGSTAmount + dataOBject.SGSTAmount;
                }
                dataOBject.SubTotal = siteObj.SubTotal;
                dataOBject.Total = dataOBject.SubTotal + siteObj.Freight + dataOBject.TotalTax + dataOBject.TotalOtherCharges;

                #region SaveChallan
                objSql.AddParameter("@Number", DbType.String, ParameterDirection.Input, 50, dataOBject.Number);
                objSql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 50, dataOBject.LedgerId);
                objSql.AddParameter("@WorkOrderDate", DbType.DateTime, ParameterDirection.Input, 50, dataOBject.WorkOrderDate);
                objSql.AddParameter("@SubTotal", DbType.Double, ParameterDirection.Input, 50, dataOBject.SubTotal);
                objSql.AddParameter("@TotalTax", DbType.Double, ParameterDirection.Input, 50, dataOBject.TotalTax);
                objSql.AddParameter("@Total", DbType.Double, ParameterDirection.Input, 50, dataOBject.Total);
                objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 50, dataOBject.CompanyId);
                //    objSql.AddParameter("@Site", DbType.String, ParameterDirection.Input, 50, dataOBject.Site);
                objSql.AddParameter("@ClientAmount", DbType.Double, ParameterDirection.Input, 50, dataOBject.ClientAmount);
                objSql.AddParameter("@CreatedBy", DbType.Int32, ParameterDirection.Input, 50, dataOBject.CreatedBy);
                objSql.AddParameter("@RbnClientId", DbType.Int32, ParameterDirection.Input, 50, dataOBject.RbnClientId);
                objSql.AddParameter("@FinYearId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.FinYearId);

                objSql.AddParameter("@IGST", DbType.Double, ParameterDirection.Input, 0, dataOBject.IGSTAmount);
                objSql.AddParameter("@SGST", DbType.Double, ParameterDirection.Input, 0, dataOBject.SGSTAmount);
                objSql.AddParameter("@CGST", DbType.Double, ParameterDirection.Input, 0, dataOBject.CGSTAmount);

                objSql.AddParameter("@IGSTRate", DbType.Double, ParameterDirection.Input, 0, dataOBject.IGSTRate);
                objSql.AddParameter("@SGSTRate", DbType.Double, ParameterDirection.Input, 0, dataOBject.SGSTRate);
                objSql.AddParameter("@CGSTRate", DbType.Double, ParameterDirection.Input, 0, dataOBject.CGSTRate);
                objSql.AddParameter("@remarks", DbType.String, ParameterDirection.Input, 0, dataOBject.Remarks);
                objSql.AddParameter("@tnc", DbType.String, ParameterDirection.Input, 0, dataOBject.Tnc);
                objSql.AddParameter("@sezDescription", DbType.String, ParameterDirection.Input, 0, dataOBject.SezDescription ?? (object)DBNull.Value);
                objSql.AddParameter("@shipFrom", DbType.String, ParameterDirection.Input, 0, dataOBject.ShipFrom ?? (object)DBNull.Value);
                objSql.AddParameter("@refNo", DbType.String, ParameterDirection.Input, 0, dataOBject.RefNo);
                objSql.AddParameter("@freight", DbType.Double, ParameterDirection.Input, 0, dataOBject.Freight);
                objSql.AddParameter("@otherChargeAmount", DbType.Double, ParameterDirection.Input, 0, dataOBject.TotalOtherCharges);
                objSql.AddParameter("@warehouseId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.WarehouseId);
                objSql.AddParameter("@adjType", DbType.Byte, ParameterDirection.Input, 0, dataOBject.AdjType > 0 ? dataOBject.AdjType : (byte)1);

                if (dataOBject.RentStartDate.Year > 1900)
                    objSql.AddParameter("@rentStartDate", DbType.DateTime, ParameterDirection.Input, 0, dataOBject.RentStartDate);

                if (dataOBject.PODate.Year > 2000)
                {
                    objSql.AddParameter("@poDate", DbType.Date, ParameterDirection.Input, 0, dataOBject.PODate);

                }
                if (!String.IsNullOrEmpty(dataOBject.PONumber))
                {
                    objSql.AddParameter("@poNumber", DbType.String, ParameterDirection.Input, 0, dataOBject.PONumber);

                }

                if (dataOBject.ParentWorkOrderId > 0)
                {
                    objSql.AddParameter("@ParentWorkOrderId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.ParentWorkOrderId);
                }
                if (!String.IsNullOrEmpty(dataOBject.SitePic))
                {
                    //objSql.AddParameter("@SitePic", DbType.String, ParameterDirection.Input, 50, dataOBject.SitePic);
                }
                //if adding a complete new challan for rent or adding new billable in an existing site.
                if (dataOBject.WorkOrderId == 0 || dataOBject.ChallanType == Enums.ChallanType.WORKORDER)
                {
                    objSql.AddParameter("@TransactionId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.TransactionId);
                    objSql.AddParameter("@Type", DbType.Byte, ParameterDirection.Input, 0, dataOBject.ChallanType);
                    objSql.AddParameter("@jobCardId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.JobCardId);

                    if (dataOBject.ChallanType == Enums.ChallanType.WORKORDER)
                    {
                        objSql.AddParameter("@workOrderNumber", DbType.String, ParameterDirection.Input, 0, dataOBject.WorkOrderNumber);
                        objSql.AddParameter("@planStartDate", DbType.DateTime, ParameterDirection.Input, 0, dataOBject.PlanStartDate);
                        objSql.AddParameter("@planEndDate", DbType.DateTime, ParameterDirection.Input, 0, dataOBject.PlanEndDate);
                        objSql.AddParameter("@endDate", DbType.DateTime, ParameterDirection.Input, 0, dataOBject.EndDate);
                        objSql.AddParameter("@bomId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.BOMId);
                        objSql.AddParameter("@productId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.ProductId);
                        objSql.AddParameter("@quantity", DbType.Int16, ParameterDirection.Input, 0, dataOBject.Quantity);
                    }
                    dataOBject.WorkOrderId = _workOrderId = Convert.ToInt32(objSql.ExecuteScalar(SAVE_WORKORDER));
                }
                else
                { //update rent-challan (workOrder)
                    objSql.AddParameter("@WorkOrderId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.WorkOrderId);
                    Convert.ToInt32(objSql.ExecuteScalar(UPDATE_WORKORDER));
                    _workOrderId = dataOBject.WorkOrderId;

                }

                if (dataOBject.Operations != null && dataOBject.Operations.Count > 0)
                {
                    foreach (var op in dataOBject.Operations)
                    {
                        objSql.NewCommand();
                        op.CompanyId = dataOBject.CompanyId;
                        op.WorkOrderId = dataOBject.WorkOrderId;
                        op.CreatedBy = dataOBject.CreatedBy;
                        op.CreatedOn = dataOBject.CreatedOn;
                        op.StatusId = 1;

                        op.GuId = Guid.NewGuid().ToString();
                        SaveOperation(op);
                    }
                }
                bool success = false;
                //if (dataOBject.ChallanType == Enums.ChallanType.WORKORDER)
                //{
                //    success = true;
                //}
                //else
                var site = dataOBject.Sites[0];
                site.IGSTRate = dataOBject.IGSTRate;
                site.SGSTRate = dataOBject.SGSTRate;
                site.CGSTRate = dataOBject.CGSTRate;

                success = CreateSite(dataOBject.Sites, dataOBject.WorkOrderId, objSql);

                //Adds other charges to a workOrder if exists.
                if (dataOBject.OtherCharges != null)
                {
                    AddOtherCharges(_workOrderId, dataOBject.OtherCharges, objSql);
                }
                if (sql == null)
                {
                    objSql.Commit();
                }
                if (success)
                {
                    //objSql.BeginTransaction();
                    //objSql.NewCommand();
                    //var inventoryDal = new InventoryDAL();
                    //inventoryDal.StockInsUpd(dataOBject.RbnClientId, dataOBject.FinYearId, dataOBject.CompanyId, dataOBject.WorkOrderDate);
                    //objSql.Commit();
                }
                else
                {
                    _workOrderId = 0;
                    if (sql == null)
                    {
                        objSql.Rollback();
                    }
                }
            }
            catch (Exception ex)
            {
                _workOrderId = 0;
                if (sql == null)
                {
                    objSql.Rollback();
                }
                throw ex;
            }
            #endregion


            return _workOrderId;
        }

        internal int SaveOperation(WorkOrderOperationDto opdto)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, opdto.CompanyId);

            objSql.AddParameter("@workOrderId", DbType.Int32, ParameterDirection.Input, 0, opdto.WorkOrderId);
            objSql.AddParameter("@operationId", DbType.Int32, ParameterDirection.Input, 0, opdto.OperationId);
            objSql.AddParameter("@createdOn", DbType.DateTime, ParameterDirection.Input, 0, opdto.CreatedOn);
            objSql.AddParameter("@createdBy", DbType.Int32, ParameterDirection.Input, 0, opdto.CompanyId);
            objSql.AddParameter("@quantity", DbType.Double, ParameterDirection.Input, 0, opdto.Quantity);
            objSql.AddParameter("@guId", DbType.String, ParameterDirection.Input, 0, opdto.GuId);
            objSql.AddParameter("@statusId", DbType.Int16, ParameterDirection.Input, 0, opdto.StatusId);


            return Convert.ToInt32(objSql.ExecuteScalar(WORKORDER_OPERATION_ADD));
        }

        internal Boolean UpdateWorkOrder(WorkOrderDTO dataOBject)
        {
            SQL objSql = new SQL();
            return UpdateWorkOrder(dataOBject, objSql);
        }

        internal Boolean UpdateWorkOrder(WorkOrderDTO dataOBject, SQL objSql)
        {
            objSql.NewCommand();
            objSql.AddParameter("@Number", DbType.String, ParameterDirection.Input, 0, dataOBject.Number);
            objSql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.LedgerId);
            objSql.AddParameter("@WorkOrderDate", DbType.DateTime, ParameterDirection.Input, 0, dataOBject.WorkOrderDate);
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.CompanyId);
            objSql.AddParameter("@ClientAmount", DbType.Double, ParameterDirection.Input, 0, dataOBject.ClientAmount);
            objSql.AddParameter("@WorkOrderId", DbType.Int32, ParameterDirection.Input, 0, dataOBject.WorkOrderId);

            return objSql.ExecuteNonQuery(UPDATE_WORKORDER) > 0;
        }

        internal bool AddOtherCharges(int workOrderId, List<WorkOrderChageDTO> charges, SQL sql = null)
        {
            SQL objSql = sql;
            if (sql == null)
            {
                objSql = new SQL();
                objSql.BeginTransaction();
            }
            try
            {
                foreach (WorkOrderChageDTO chDto in charges)
                {
                    objSql.NewCommand();
                    chDto.WorkOrderId = workOrderId;
                    AddWorkOrderChage(chDto, objSql);
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
        internal bool CreateSite(List<SiteDTO> sites, int _workOrderId, SQL objSql = null)
        {
            bool localTransaction = false;
            if (objSql == null)
            {
                objSql = new SQL();
                objSql.BeginTransaction();
                localTransaction = true;
            }
            try
            {
                foreach (SiteDTO site in sites)
                {
                    site.WorkOrderId = _workOrderId;
                    objSql.NewCommand();
                    int siteId = AddSite(site, objSql);
                    if (site.Items.Count > 0)
                    {

                        try
                        {
                            objSql.NewCommand();
                            objSql.AddParameter("@siteId", DbType.Int32, ParameterDirection.Input, 0, siteId);
                            objSql.ExecuteNonQuery(DELETE_SITEITEMS);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Error while updating challan items" + ex.Message);
                        }

                        objSql.NewCommand();
                        objSql.AddParameter("@SiteId", DbType.Int32, ParameterDirection.Input, 0, siteId);
                        objSql.ExecuteNonQuery(WORKORDER_TAX_DEL_BY_SITE);

                        foreach (WorkOrderItemDTO item in site.Items)
                        {
                            item.WorkOrder = new WorkOrderDTO(_workOrderId);
                            item.SiteId = siteId;
                            if (item.PurchaseQty == 0)
                            {
                                continue;
                            }

                            ApplyLegacyGstAmountsToItem(item, site);

                            objSql.NewCommand();
                            WorkOrderItemDAL itemDAL = new WorkOrderItemDAL();
                            int workOrderItemId = itemDAL.Save(item, objSql);
                            item.WorkOrderItemId = workOrderItemId;

                            if (item.LineTaxes != null)
                            {
                                foreach (WorkOrderTaxDTO lineTax in item.LineTaxes)
                                {
                                    lineTax.WorkOrderItemId = workOrderItemId;
                                    lineTax.SiteId = siteId;
                                    lineTax.ProductId = item.ProductId;
                                    objSql.NewCommand();
                                    AddWorkOrderTax(lineTax, objSql);
                                }
                            }
                        }

                    }

                    site.SiteId = siteId;
                }
                if (localTransaction)
                {
                    objSql.Commit();
                }
                return true;
            }
            catch (Exception ex)
            {
                if (localTransaction)
                {
                    objSql.Rollback();
                }
                throw ex;
                //   return false;
            }
        }

        internal bool AddItems(SiteDTO site, BAL.Enums.SiteItemType itemType)
        {
            SQL objSql = new SQL();
            try
            {
                objSql.BeginTransaction();

                foreach (WorkOrderItemDTO item in site.Items)
                {
                    item.SiteItemType = itemType;
                    item.SiteId = site.SiteId;
                    objSql.NewCommand();
                    WorkOrderItemDAL itemDAL = new WorkOrderItemDAL();
                    itemDAL.Save(item, objSql);
                }
                objSql.Commit();
                return true;
            }
            catch (Exception ex)
            {
                objSql.Rollback();
                return false;
            }
        }

        internal Int32 AddSite(SiteDTO siteDto, SQL objSql)
        {

            if (siteDto.WorkOrderId > 0)
                objSql.AddParameter("@WorkOrderId", DbType.Int32, ParameterDirection.Input, 0, siteDto.WorkOrderId);

            objSql.AddParameter("@JobNumber", DbType.String, ParameterDirection.Input, 0, siteDto.JobNumber);
            objSql.AddParameter("@ChallanNumber", DbType.String, ParameterDirection.Input, 0, siteDto.ChallanNumber);
            objSql.AddParameter("@Site", DbType.String, ParameterDirection.Input, 0, siteDto.Site);
            objSql.AddParameter("@ShaftSize", DbType.String, ParameterDirection.Input, 0, siteDto.ShaftSize);
            objSql.AddParameter("@ShaftHeight", DbType.String, ParameterDirection.Input, 0, siteDto.ShaftHeight);
            objSql.AddParameter("@SiteEng", DbType.String, ParameterDirection.Input, 0, siteDto.SiteEng);
            objSql.AddParameter("@StartDate", DbType.DateTime, ParameterDirection.Input, 0, siteDto.StartDate);
            objSql.AddParameter("@Duration", DbType.Int16, ParameterDirection.Input, 0, siteDto.Duration);
            objSql.AddParameter("@Doc1", DbType.String, ParameterDirection.Input, 0, siteDto.Doc1);
            objSql.AddParameter("@Doc2", DbType.String, ParameterDirection.Input, 0, siteDto.Doc2);
            objSql.AddParameter("@Doc3", DbType.String, ParameterDirection.Input, 0, siteDto.Doc3);
            objSql.AddParameter("@SubTotal", DbType.Double, ParameterDirection.Input, 0, siteDto.SubTotal);
            objSql.AddParameter("@TaxAmount", DbType.Double, ParameterDirection.Input, 0, siteDto.TaxAmount);
            objSql.AddParameter("@Freight", DbType.Double, ParameterDirection.Input, 0, siteDto.Freight);
            objSql.AddParameter("@FreightTax", DbType.Double, ParameterDirection.Input, 0, siteDto.FreightTax);
            objSql.AddParameter("@Total", DbType.Double, ParameterDirection.Input, 0, siteDto.Total);
            objSql.AddParameter("@CreatedBy", DbType.Int32, ParameterDirection.Input, 0, siteDto.CreatedBy);
            objSql.AddParameter("@Vehicle", DbType.String, ParameterDirection.Input, 0, siteDto.Vehicle);
            objSql.AddParameter("@State", DbType.String, ParameterDirection.Input, 0, siteDto.State);
            objSql.AddParameter("@Driver", DbType.String, ParameterDirection.Input, 0, siteDto.Driver);
            objSql.AddParameter("@rentStartDate", DbType.DateTime, ParameterDirection.Input, 0, siteDto.RentStartDate);
            objSql.AddParameter("@weight", DbType.Double, ParameterDirection.Input, 0, siteDto.Weight);
            objSql.AddParameter("@ApproximateValue", DbType.Double, ParameterDirection.Input, 0, siteDto.ApproximateValue);
            objSql.AddParameter("@LRNumber", DbType.String, ParameterDirection.Input, 0, siteDto.LRNumber ?? (object)DBNull.Value);
            objSql.AddParameter("@CRNumber", DbType.String, ParameterDirection.Input, 0, siteDto.CRNumber ?? (object)DBNull.Value);
            objSql.AddParameter("@GRNumber", DbType.String, ParameterDirection.Input, 0, siteDto.GRNumber ?? (object)DBNull.Value);
            if (siteDto.RecoveryDate.Year > 2000)
                objSql.AddParameter("@recoveryDate", DbType.DateTime, ParameterDirection.Input, 0, siteDto.RecoveryDate);

            if (siteDto.ParentSiteId > 0)
                objSql.AddParameter("@ParentSiteId", DbType.Int32, ParameterDirection.Input, 0, siteDto.ParentSiteId);

            if (siteDto.LedgerSiteId > 0)
                objSql.AddParameter("@LedgerSiteId", DbType.Int32, ParameterDirection.Input, 0, siteDto.LedgerSiteId);
            if (siteDto.DriverId > 0)
                objSql.AddParameter("@DriverId", DbType.Int32, ParameterDirection.Input, 0, siteDto.DriverId);
            if (siteDto.VehicleId > 0)
                objSql.AddParameter("@VehicleId", DbType.Int32, ParameterDirection.Input, 0, siteDto.VehicleId);

            objSql.AddParameter("@TransporterId", DbType.Int32, ParameterDirection.Input, 0, siteDto.TransporterId);
            objSql.AddParameter("@TeamId", DbType.Int32, ParameterDirection.Input, 0, siteDto.TeamId);

            if (siteDto.SiteId > 0)
            {
                objSql.AddParameter("@remarks", DbType.String, ParameterDirection.Input, 0, siteDto.Remarks);
                objSql.AddParameter("@tnc", DbType.String, ParameterDirection.Input, 0, siteDto.Tnc);
                objSql.AddParameter("@SiteId", DbType.Int32, ParameterDirection.Input, 0, siteDto.SiteId);
                Convert.ToInt16(objSql.ExecuteScalar(UPDATE_SITE));
            }
            else
                siteDto.SiteId = Convert.ToInt32(objSql.ExecuteScalar(ADD_SITE));

            return siteDto.SiteId;
        }

        internal List<SiteDTO> GetAll(string workOrderNumber, string jobNumber, string site, string client, bool closed, int companyId, string siteEng)
        {
            SQL objSql = new SQL();
            if (!String.IsNullOrEmpty(workOrderNumber))
            {
                objSql.AddParameter("@Number", DbType.String, ParameterDirection.Input, 0, workOrderNumber);
            }
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
            objSql.AddParameter("@closed", DbType.Boolean, ParameterDirection.Input, 0, closed);
            objSql.AddParameter("@companyId", DbType.Int16, ParameterDirection.Input, 0, companyId);
            return objSql.ContructList<SiteDTO>(objSql.ExecuteDataSet(WORK_ORDERLIST));
        }

        internal List<SiteDTO> GetByCompany(int companyId, int finYearId, string code)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@companyId", DbType.Int16, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@finYearId", DbType.Int16, ParameterDirection.Input, 0, finYearId);
            objSql.AddParameter("@code", DbType.String, ParameterDirection.Input, 0, code);
            return objSql.ContructList<SiteDTO>(objSql.ExecuteDataSet(WORK_ORDERS_COMPANY));
        }

        internal WorkOrderDTO GetById(int workOrderId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@WorkOrderId", DbType.Int32, ParameterDirection.Input, 0, workOrderId);
            return objSql.ContructList<WorkOrderDTO>(objSql.ExecuteDataSet(WORK_SELECT)).FirstOrDefault();
        }
        internal List<WorkOrderItemDTO> GetWorkOrderItems(int workOrderId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@WorkOrderId", DbType.Int32, ParameterDirection.Input, 0, workOrderId);
            return objSql.ContructList<WorkOrderItemDTO>(objSql.ExecuteDataSet(WORKORDER_ITEMS));
        }
        internal List<SiteDTO> GetSites(int workOrderId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@WorkOrderId", DbType.Int32, ParameterDirection.Input, 0, workOrderId);
            return objSql.ContructList<SiteDTO>(objSql.ExecuteDataSet(GET_SITES));
        }
        internal List<WorkOrderItemDTO> GetSiteItems(int siteId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@SiteId", DbType.Int32, ParameterDirection.Input, 0, siteId);
            return objSql.ContructList<WorkOrderItemDTO>(objSql.ExecuteDataSet(GET_SITE_ITEMS));
        }

        void AddWorkOrderTax(WorkOrderTaxDTO taxDto, SQL objSql)
        {
            objSql.AddParameter("@WorkOrderItemId", DbType.Int32, ParameterDirection.Input, 0, taxDto.WorkOrderItemId);
            objSql.AddParameter("@SiteId", DbType.Int32, ParameterDirection.Input, 0, taxDto.SiteId);
            objSql.AddParameter("@ProductId", DbType.Int32, ParameterDirection.Input, 0, taxDto.ProductId);
            objSql.AddParameter("@TaxCategoryId", DbType.Int32, ParameterDirection.Input, 0, taxDto.TaxCategoryId);
            objSql.AddParameter("@TaxId", DbType.Int32, ParameterDirection.Input, 0, taxDto.TaxId);
            objSql.AddParameter("@TaxName", DbType.String, ParameterDirection.Input, 100, taxDto.TaxName ?? string.Empty);
            objSql.AddParameter("@TaxCode", DbType.String, ParameterDirection.Input, 20, (object)taxDto.TaxCode ?? DBNull.Value);
            objSql.AddParameter("@Rate", DbType.Decimal, ParameterDirection.Input, 0, taxDto.Rate);
            objSql.AddParameter("@RateType", DbType.String, ParameterDirection.Input, 20, taxDto.RateType ?? "Percentage");
            objSql.AddParameter("@Amount", DbType.Decimal, ParameterDirection.Input, 0, taxDto.Amount);
            objSql.ExecuteNonQuery(ADD_TAX);
        }

        void ApplyLegacyGstAmountsToItem(WorkOrderItemDTO item, SiteDTO site)
        {
            if (item.LineTaxes != null && item.LineTaxes.Count > 0)
            {
                item.IGSTAmount = (double)item.LineTaxes
                    .Where(t => IsTaxCode(t.TaxCode, "IGST"))
                    .Sum(t => t.Amount);
                item.CGSTAmount = (double)item.LineTaxes
                    .Where(t => IsTaxCode(t.TaxCode, "CGST"))
                    .Sum(t => t.Amount);
                item.SGSTAmount = (double)item.LineTaxes
                    .Where(t => IsTaxCode(t.TaxCode, "SGST"))
                    .Sum(t => t.Amount);
                return;
            }

            item.IGSTAmount = (site.IGSTRate * item.SubTotal) / 100;
            item.SGSTAmount = (site.SGSTRate * item.SubTotal) / 100;
            item.CGSTAmount = (site.CGSTRate * item.SubTotal) / 100;
        }

        void ApplyHeaderTaxTotalsFromAppliedTaxes(WorkOrderDTO workOrder, List<WorkOrderTaxDTO> appliedTaxes)
        {
            workOrder.IGSTAmount = (double)appliedTaxes
                .Where(t => IsTaxCode(t.TaxCode, "IGST"))
                .Sum(t => t.Amount);
            workOrder.CGSTAmount = (double)appliedTaxes
                .Where(t => IsTaxCode(t.TaxCode, "CGST"))
                .Sum(t => t.Amount);
            workOrder.SGSTAmount = (double)appliedTaxes
                .Where(t => IsTaxCode(t.TaxCode, "SGST"))
                .Sum(t => t.Amount);
            workOrder.TotalTax = workOrder.IGSTAmount + workOrder.CGSTAmount + workOrder.SGSTAmount;

            var igst = appliedTaxes.FirstOrDefault(t => IsTaxCode(t.TaxCode, "IGST"));
            var cgst = appliedTaxes.FirstOrDefault(t => IsTaxCode(t.TaxCode, "CGST"));
            var sgst = appliedTaxes.FirstOrDefault(t => IsTaxCode(t.TaxCode, "SGST"));

            workOrder.IGSTRate = igst != null ? (double)igst.Rate : 0;
            workOrder.CGSTRate = cgst != null ? (double)cgst.Rate : 0;
            workOrder.SGSTRate = sgst != null ? (double)sgst.Rate : 0;
        }

        static bool IsTaxCode(string taxCode, string expectedCode)
        {
            return !string.IsNullOrWhiteSpace(taxCode)
                && taxCode.Equals(expectedCode, StringComparison.OrdinalIgnoreCase);
        }

        internal List<WorkOrderTaxDTO> GetSiteTaxes(int siteId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@SiteId", DbType.Int32, ParameterDirection.Input, 0, siteId);
            return objSql.ContructList<WorkOrderTaxDTO>(objSql.ExecuteDataSet(GET_TAX));
        }
        internal bool UpdateSiteInfo(SiteDTO siteDto)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@JobNumber", DbType.String, ParameterDirection.Input, 0, siteDto.JobNumber);
            //  objSql.AddParameter("@ChallanNumber", DbType.String, ParameterDirection.Input, 0, siteDto.ChallanNumber);
            objSql.AddParameter("@Site", DbType.String, ParameterDirection.Input, 0, siteDto.Site);
            objSql.AddParameter("@ShaftSize", DbType.String, ParameterDirection.Input, 0, siteDto.ShaftSize);
            objSql.AddParameter("@ShaftHeight", DbType.String, ParameterDirection.Input, 0, siteDto.ShaftHeight);
            objSql.AddParameter("@SiteEng", DbType.String, ParameterDirection.Input, 0, siteDto.SiteEng);
            objSql.AddParameter("@StartDate", DbType.Date, ParameterDirection.Input, 0, siteDto.StartDate);
            objSql.AddParameter("@Duration", DbType.Int16, ParameterDirection.Input, 0, siteDto.Duration);
            objSql.AddParameter("@WorkOrderNumber", DbType.String, ParameterDirection.Input, 0, siteDto.WorkOrderNumber);

            objSql.AddParameter("@SiteId", DbType.Int32, ParameterDirection.Input, 0, siteDto.SiteId);
            objSql.AddParameter("@State", DbType.String, ParameterDirection.Input, 0, siteDto.State);
            objSql.AddParameter("@Vehicle", DbType.String, ParameterDirection.Input, 0, siteDto.Vehicle);
            objSql.AddParameter("@Driver", DbType.String, ParameterDirection.Input, 0, siteDto.Driver);

            return objSql.ExecuteNonQuery(UPDATE_SITE_BASIC_INFO) > 0;
        }

        /// <summary>Maps UI status to <c>@listStatusFilter</c> on register procs: 0 All, 1 Active, 2 Deleted.</summary>
        internal static byte ParseListStatusFilter(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter)) return 0;
            var t = filter.Trim();
            if (t.Equals("Active", StringComparison.OrdinalIgnoreCase)) return 1;
            if (t.Equals("Deleted", StringComparison.OrdinalIgnoreCase)) return 2;
            return 0;
        }

        public async Task<IEnumerable<WorkOrderItemDTO>> ItemIssued(string from, string to, int challanType, int ledgerId, int ledgerSiteId, int companyId, string challanNo, string listStatusFilter)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@From", DbType.Date, ParameterDirection.Input, 0, from);
            objSql.AddParameter("@To", DbType.Date, ParameterDirection.Input, 0, to);
            if (ledgerId > 0)
                objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@ledgerSiteId", DbType.Int32, ParameterDirection.Input, 0, ledgerSiteId);
            objSql.AddParameter("@challanType", DbType.Byte, ParameterDirection.Input, 0, challanType);
            if (!string.IsNullOrEmpty(challanNo))
                objSql.AddParameter("@challanNo", DbType.String, ParameterDirection.Input, 0, challanNo);
            objSql.AddParameter("@status", DbType.Byte, ParameterDirection.Input, 0, ParseListStatusFilter(listStatusFilter));

            return await objSql.QueryAsync<WorkOrderItemDTO>(ITEM_ISSUED_REGISTER);
        }

        public async Task<IEnumerable<WorkOrderItemDTO>> DeliveryChallans(string from, string to, int challanType, int ledgerId, int ledgerSiteId, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@From", DbType.Date, ParameterDirection.Input, 0, from);
            objSql.AddParameter("@To", DbType.Date, ParameterDirection.Input, 0, to);
            if (ledgerId > 0)
                objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@ledgerSiteId", DbType.Int32, ParameterDirection.Input, 0, ledgerSiteId);
            objSql.AddParameter("@challanType", DbType.Byte, ParameterDirection.Input, 0, challanType);

            return await objSql.QueryAsync<WorkOrderItemDTO>(PARTY_DELIVERY_CHALLANS);
        }

        public async Task<IEnumerable<GRNDTO>> ReturnChallans(string from, string to, int challanType, int ledgerId, int ledgerSiteId, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@From", DbType.Date, ParameterDirection.Input, 0, from);
            objSql.AddParameter("@To", DbType.Date, ParameterDirection.Input, 0, to);
            if (ledgerId > 0)
                objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@ledgerSiteId", DbType.Int32, ParameterDirection.Input, 0, ledgerSiteId);
            objSql.AddParameter("@challanType", DbType.Byte, ParameterDirection.Input, 0, challanType);


            return await objSql.QueryAsync<GRNDTO>(PARTY_RETURN_CHALLANS);

        }

        public async Task<IEnumerable<GRNDTO>> PartyBreakageReport(string from, string to, int challanType, int ledgerId, int ledgerSiteId, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@From", DbType.Date, ParameterDirection.Input, 0, from);
            objSql.AddParameter("@To", DbType.Date, ParameterDirection.Input, 0, to);
            if (ledgerId > 0)
                objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@ledgerSiteId", DbType.Int32, ParameterDirection.Input, 0, ledgerSiteId);
            objSql.AddParameter("@challanType", DbType.Byte, ParameterDirection.Input, 0, challanType);

            return await objSql.QueryAsync<GRNDTO>(PARTY_BREAKAGE_REPORT);
        }

        public async Task<IEnumerable<GRNDTO>> PartyLostReport(string from, string to, int challanType, int ledgerId, int ledgerSiteId, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@From", DbType.Date, ParameterDirection.Input, 0, from);
            objSql.AddParameter("@To", DbType.Date, ParameterDirection.Input, 0, to);
            if (ledgerId > 0)
                objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@ledgerSiteId", DbType.Int32, ParameterDirection.Input, 0, ledgerSiteId);
            objSql.AddParameter("@challanType", DbType.Byte, ParameterDirection.Input, 0, challanType);

            return await objSql.QueryAsync<GRNDTO>(PARTY_LOST_REPORT);
        }

        public async Task<IEnumerable<GRNDTO>> PartyExcessReport(string from, string to, int challanType, int ledgerId, int ledgerSiteId, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@From", DbType.Date, ParameterDirection.Input, 0, from);
            objSql.AddParameter("@To", DbType.Date, ParameterDirection.Input, 0, to);
            if (ledgerId > 0)
                objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@ledgerSiteId", DbType.Int32, ParameterDirection.Input, 0, ledgerSiteId);
            objSql.AddParameter("@challanType", DbType.Byte, ParameterDirection.Input, 0, challanType);

            return await objSql.QueryAsync<GRNDTO>(PARTY_EXCESS_REPORT);
        }

        public async Task<IEnumerable<WorkOrderDTO>> WorkOrders(WorkOrderDTO dto)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@From", DbType.Date, ParameterDirection.Input, 0, dto.WorkOrderDate);
            objSql.AddParameter("@To", DbType.Date, ParameterDirection.Input, 0, dto.EndDate);

            objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, dto.LedgerId);
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
            objSql.AddParameter("@ledgerSiteId", DbType.Int32, ParameterDirection.Input, 0, dto.LedgerSiteId);
            objSql.AddParameter("@challanType", DbType.Byte, ParameterDirection.Input, 0, dto.ChallanType);

            return await objSql.QueryAsync<WorkOrderDTO>(WORK_ORDERS_LIST);
        }

        public DataSet ItemReceived_Report(int grnId)
        {
            SQL objSql = new SQL();
            //objSql.AddParameter("@From", DbType.Date, ParameterDirection.Input, 0, from);
            //objSql.AddParameter("@To", DbType.Date, ParameterDirection.Input, 0, to);
            //if (ledgerId > 0)
            //    objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);
            objSql.AddParameter("@grnId", DbType.Int32, ParameterDirection.Input, 0, grnId);

            return objSql.ExecuteDataSet(ITEM_RECEIVED_REGISTER_REPORT);
        }
        public List<GRNDTO> ItemReceived(string from, string to, short challanType, int ledgerId, int ledgerSiteId, int companyId, string challanNo, string listStatusFilter)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@From", DbType.Date, ParameterDirection.Input, 0, from);
            objSql.AddParameter("@To", DbType.Date, ParameterDirection.Input, 0, to);
            if (ledgerId > 0)
                objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@ledgerSiteId", DbType.Int32, ParameterDirection.Input, 0, ledgerSiteId);
            objSql.AddParameter("@challanType", DbType.Byte, ParameterDirection.Input, 0, challanType);
            if (!String.IsNullOrEmpty(challanNo))
                objSql.AddParameter("@challanNo", DbType.String, ParameterDirection.Input, 0, challanNo);
            objSql.AddParameter("@status", DbType.Byte, ParameterDirection.Input, 0, ParseListStatusFilter(listStatusFilter));

            return objSql.ContructList<GRNDTO>(objSql.ExecuteDataSet(ITEM_RECEIVED_REGISTER));
        }
        public List<SiteDTO> GetClientSites(int ledgerId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);
            return objSql.ContructList<SiteDTO>(objSql.ExecuteDataSet(GET_CLIENT_SITES));
        }
        public DataSet ItemIssuedForPrint(int workOrderId, int companyId)
        {
            SQL objSql = new SQL();

            objSql.AddParameter("@workOrderId", DbType.Int32, ParameterDirection.Input, 0, workOrderId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);

            return objSql.ExecuteDataSet(ITEM_ISSUED_FOR_PRINT);
        }
        public List<SiteDTO> GetWorkOrderBalance(int ledgerId, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);


            return objSql.ContructList<SiteDTO>(objSql.ExecuteDataSet(WORK_ORDER_BALANCE));
        }
        public List<SiteDTO> WorkOrderDueDateReminder(int ledgerId, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            return objSql.ContructList<SiteDTO>(objSql.ExecuteDataSet(WORK_DUE_DATE_REMINDER));
        }
        public List<SiteDTO> WorkOrderOverDuesReminder(int ledgerId, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);

            return objSql.ContructList<SiteDTO>(objSql.ExecuteDataSet(WORK_ORDER_OVER_DUE_REMINDER));
        }
        internal DataSet GetChallanReportHeader(int workOrderId, int challanHeaderType = 0)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@workOrderId", DbType.Int32, ParameterDirection.Input, 0, workOrderId);
            string strCallanHeaderType = "";
            switch (challanHeaderType)
            {
                case 1:
                    strCallanHeaderType = "Original Copy";
                    break;
                case 2:
                    strCallanHeaderType = "Duplicate Copy";
                    break;
                case 3:
                    strCallanHeaderType = "Transport Copy";
                    break;
            }
            objSql.AddParameter("@ChallanHeaderType", DbType.String, ParameterDirection.Input, 0, strCallanHeaderType);


            return objSql.ExecuteDataSet(CHALLAN_REPORT_HEADER);
        }

        /// <summary>
        /// Adds updates other chages for a challan
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        internal bool AddWorkOrderChage(WorkOrderChageDTO dto, SQL sql = null)
        {
            SQL objSql = new SQL();
            if (sql != null)
            {
                objSql = sql;
            }
            objSql.AddParameter("@ChargeId", DbType.Int32, ParameterDirection.Input, 0, dto.ChargeId);
            objSql.AddParameter("@Amount", DbType.Double, ParameterDirection.Input, 0, dto.Amount);
            if (dto.WorkOrderChargeId > 0)
            {
                objSql.AddParameter("@WorkOrderChargeId", DbType.Int32, ParameterDirection.Input, 0, dto.WorkOrderChargeId);
                return objSql.ExecuteNonQuery(UPDATE_OTHER_CHARGE) > 0;
            }
            else
            {
                objSql.AddParameter("@WorkOrderId", DbType.Int32, ParameterDirection.Input, 0, dto.WorkOrderId);
                return objSql.ExecuteNonQuery(ADD_OTHER_CHARGE) > 0;
            }


        }
        /// <summary>
        /// Gets the other charges on a issue challan
        /// </summary>
        /// <param name="workOrderId"></param>
        /// <returns></returns>
        public List<WorkOrderChageDTO> GetOtherChages(int workOrderId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@WorkOrderId", DbType.Int32, ParameterDirection.Input, 0, workOrderId);
            return objSql.ContructList<WorkOrderChageDTO>(objSql.ExecuteDataSet(WORK_ORDER_CHARGE_SEL));
        }
        /// <summary>
        /// Gets the other charges on a issue challan
        /// </summary>
        /// <param name="workOrderId"></param>
        /// <returns></returns>
        public List<InvoiceChargeDTO> GetSiteOtherCharges(int ledgerSiteId, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@ledgerSiteId", DbType.Int32, ParameterDirection.Input, 0, ledgerSiteId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);

            return objSql.ContructList<InvoiceChargeDTO>(objSql.ExecuteDataSet(SITE_ORDER_CHARGE_SEL));
        }

        /// <summary>
        /// Adds a document to the cahllan
        /// </summary>
        /// <returns></returns>
        public ChallanDocumentDTO AddChallanDocument(ChallanDocumentDTO dto)
        {
            SQL objSql = new SQL();
            if (!String.IsNullOrEmpty(dto.Title))
            {
                objSql.AddParameter("@Title", DbType.String, ParameterDirection.Input, 0, dto.Title);
            }
            objSql.AddParameter("@UploadedBy", DbType.Int32, ParameterDirection.Input, 0, dto.UploadedBy);

            objSql.AddParameter("@WorkOrderId", DbType.Int32, ParameterDirection.Input, 0, dto.WorkOrderId);
            objSql.AddParameter("@FilePath", DbType.String, ParameterDirection.Input, 0, dto.FilePath);
            if (dto.FileType > 0)
            {
                objSql.AddParameter("@FileType", DbType.Int16, ParameterDirection.Input, 0, dto.FileType);
            }
            return objSql.ContructList<ChallanDocumentDTO>(objSql.ExecuteDataSet(CHALLAN_DOCUMENT_ADD)).FirstOrDefault();
        }

        /// <summary>
        /// Gets all documents uploaded for a challan
        /// </summary>
        /// <returns></returns>
        public List<ChallanDocumentDTO> GetChallanDocuments(int workOrderId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@WorkOrderId", DbType.Int32, ParameterDirection.Input, 0, workOrderId);
            return objSql.ContructList<ChallanDocumentDTO>(objSql.ExecuteDataSet(CHALLAN_DOCUMENT_SEL));
        }

        /// <summary>
        /// Deletes a challan document
        /// </summary>
        /// <returns></returns>
        public Boolean DeleteChallanDocument(int challanDocumentId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@challanDocumentId", DbType.Int32, ParameterDirection.Input, 0, challanDocumentId);
            return objSql.ExecuteNonQuery(CHALLAN_DOCUMENT_DEL) > 0;
        }
        public int DeleteChallanItem(int workOrderItemId)
        {
            SQL objSql = new SQL();
            return DeleteChallanItem(workOrderItemId, objSql);
        }

        internal int DeleteChallanItem(int workOrderItemId, SQL objSql)
        {
            objSql.NewCommand();
            objSql.AddParameter("@workOrderItemId", DbType.Int32, ParameterDirection.Input, 0, workOrderItemId);

            return objSql.ExecuteNonQuery(DELETE_CHALLAN_ITEM);
        }
        public DataSet PendingChallanAcknoledgements(int finYearId, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@finYearId", DbType.Int32, ParameterDirection.Input, 0, finYearId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);

            return objSql.ExecuteDataSet(PENDING_CHALLAN_ACKNOWLEDGMENTS);

        }
        public async Task<bool> TransferChallan(WorkOrder dto, GRN objGRN)
        {
            SQL objSql = new SQL();
            try
            {
                objSql.BeginTransaction();

                GRNDAL grnDal = new GRNDAL();
                objGRN.ChallanType = 12;
                var result = grnDal.Add(objGRN, objSql);
                if (!result)
                {
                    throw new Exception("Items received from site failed.");
                }
                if (objGRN.GRNId == 0)
                {
                    throw new Exception("Items received from site failed.");
                }
                objSql.NewCommand();

                objSql.AddParameter("@ChallanNumber", DbType.String, ParameterDirection.Input, 0, dto.Number);
                objSql.AddParameter("@CreatedBy", DbType.Int32, ParameterDirection.Input, 0, dto.CreatedBy);
                objSql.AddParameter("@Vehicle", DbType.String, ParameterDirection.Input, 0, dto.Vehicle);
                objSql.AddParameter("@LedgerSiteId", DbType.Int32, ParameterDirection.Input, 0, dto.LedgerSiteId);
                objSql.AddParameter("@Driver", DbType.String, ParameterDirection.Input, 0, dto.Driver);
                objSql.AddParameter("@grnId", DbType.Int32, ParameterDirection.Input, 0, objGRN.GRNId);
                objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                objSql.AddParameter("@finYearId", DbType.Int32, ParameterDirection.Input, 0, dto.FinYearId);
                objSql.AddParameter("@weight", DbType.Decimal, ParameterDirection.Input, 0, dto.Weight);


                var worderId = Convert.ToInt32(await objSql.ExecuteScalarAsync(TRANSFER_CHALLAN));

                if (worderId == 0)
                {
                    throw new Exception("Could not create workOrder");
                }

                if (worderId > 0 && dto.OtherCharges != null)
                {
                    AddOtherCharges(worderId, dto.OtherCharges, objSql);
                }

                //objSql.NewCommand();
                //objSql.AddParameter("@GRNId", DbType.Int32, ParameterDirection.Input, 0, objGRN.GRNId);
                //objSql.AddParameter("@WorkOrderId", DbType.Int32, ParameterDirection.Input, 0, worderId);
                //objSql.ExecuteNonQuery(LINK_TRANSFER_GRN);

                objSql.Commit();
                return true;
            }
            catch (Exception ex)
            {
                objSql.Rollback();
                throw ex;
            }


        }

        /// <summary>Load material transfer challan for edit (one row per work order line).</summary>
        internal async Task<MaterialTransferDto> MatTransferById(int workOrderId, int companyId)
        {
            var matTransferObj = new MaterialTransferDto();
            var wo = GetById(workOrderId);
            if (wo == null || wo.CompanyId != companyId)
            {
                throw new Exception("Challan not found.");
            }
            matTransferObj.WorkOrder = wo;

            var sites = GetSites(workOrderId);
            var site = sites != null ? sites.FirstOrDefault() : null;
            if (site == null)
            {
                throw new Exception("Challan site not found.");
            }

            matTransferObj.WorkOrder.OtherCharges = GetOtherChages(workOrderId);

            SQL sql = new SQL();
            sql.AddParameter("@workOrderId", DbType.Int32, ParameterDirection.Input, 0, workOrderId);
            DataSet grnLookup = sql.ExecuteDataSet(TRANSFER_GRN_LOOKUP);
            if (grnLookup == null || grnLookup.Tables.Count == 0 || grnLookup.Tables[0].Rows.Count == 0)
            {
                throw new Exception("Could not resolve transfer GRN for this challan. If this is an older transfer, save once from the database after running the TransferWorkOrderId migration.");
            }
            DataRow gr = grnLookup.Tables[0].Rows[0];
            int grnId = Convert.ToInt32(gr["GRNId"]);
            int sourceLedgerId = Convert.ToInt32(gr["SourceLedgerId"]);
            int sourceLedgerSiteId = Convert.ToInt32(gr["SourceLedgerSiteId"]);
            string grnRemarks = gr["Remarks"] == DBNull.Value ? "" : Convert.ToString(gr["Remarks"]);
            DateTime transferDate = Convert.ToDateTime(gr["TransferDate"]);

            var items = GetWorkOrderItems(workOrderId);
            matTransferObj.WorkOrder.Items = items;
            matTransferObj.WorkOrder.GrnId = grnId;
            if (grnId > 0)
            {
                var grnDAL = new GRNDAL();
                matTransferObj.GRN = await grnDAL.GrnById(grnId, companyId);
                matTransferObj.GRN.GrnItems = grnDAL.GetItemsByGrnId(grnId, companyId);
            }
            //var table = new DataTable();
            // table.Columns.Add("WorkOrderId", typeof(int));
            // table.Columns.Add("SiteId", typeof(int));
            // table.Columns.Add("GRNId", typeof(int));
            // table.Columns.Add("SourceLedgerId", typeof(int));
            // table.Columns.Add("SourceLedgerSiteId", typeof(int));
            // table.Columns.Add("DestLedgerId", typeof(int));
            // table.Columns.Add("DestLedgerSiteId", typeof(int));
            // table.Columns.Add("ReceivingDate", typeof(DateTime));
            // table.Columns.Add("ChallanNumber", typeof(string));
            // table.Columns.Add("Remarks", typeof(string));
            // table.Columns.Add("Freight", typeof(double));
            // table.Columns.Add("Weight", typeof(decimal));
            // table.Columns.Add("Driver", typeof(string));
            // table.Columns.Add("Vehicle", typeof(string));
            // table.Columns.Add("WorkOrderItemId", typeof(int));
            // table.Columns.Add("ProductId", typeof(int));
            // table.Columns.Add("ProductSizeId", typeof(int));
            // table.Columns.Add("Quantity", typeof(double));
            // table.Columns.Add("Rate", typeof(double));
            // table.Columns.Add("Item", typeof(string));

            // foreach (var it in items)
            // {
            //     if (it.Deleted == 1) continue;
            //     DataRow row = table.NewRow();
            //     row["WorkOrderId"] = workOrderId;
            //     row["SiteId"] = site.SiteId;
            //     row["GRNId"] = grnId;
            //     row["SourceLedgerId"] = sourceLedgerId;
            //     row["SourceLedgerSiteId"] = sourceLedgerSiteId;
            //     row["DestLedgerId"] = wo.LedgerId;
            //     row["DestLedgerSiteId"] = site.LedgerSiteId;
            //     row["ReceivingDate"] = transferDate;
            //     row["ChallanNumber"] = wo.Number ?? (object)DBNull.Value;
            //     row["Remarks"] = string.IsNullOrEmpty(grnRemarks) ? (object)DBNull.Value : grnRemarks;
            //     row["Freight"] = site.Freight;
            //     row["Weight"] = wo.Weight;
            //     row["Driver"] = string.IsNullOrEmpty(wo.Driver) ? (object)DBNull.Value : wo.Driver;
            //     row["Vehicle"] = string.IsNullOrEmpty(wo.Vehicle) ? (object)DBNull.Value : wo.Vehicle;
            //     row["WorkOrderItemId"] = it.WorkOrderItemId;
            //     row["ProductId"] = it.ProductId;
            //     row["ProductSizeId"] = it.ProductSizeId > 0 ? it.ProductSizeId : 0;
            //     row["Quantity"] = it.PurchaseQty;
            //     row["Rate"] = it.Rate;
            //     row["Item"] = string.IsNullOrEmpty(it.Product) ? (object)DBNull.Value : it.Product;
            //     table.Rows.Add(row);
            // }

            //if (table.Rows.Count == 0)
            //{
            //    throw new Exception("This transfer challan has no line items.");
            //}

            //var ds = new DataSet();
            //ds.Tables.Add(table);
            return matTransferObj;
        }

        public async Task<bool> UpdateTransferChallan(WorkOrder dto, GRN objGRN)
        {
            if (dto.WorkOrderId <= 0 || objGRN.GRNId <= 0)
            {
                throw new Exception("Invalid transfer challan identifiers.");
            }
            var existingHeader = GetById(dto.WorkOrderId);
            if (existingHeader == null || existingHeader.CompanyId != dto.CompanyId)
            {
                throw new Exception("Challan not found.");
            }

            var snapshot = GetWorkOrderItems(dto.WorkOrderId);
            var payloadKeys = new HashSet<string>(
                objGRN.Items.Where(i => i.ProductId > 0 && i.Quantity > 0)
                    .Select(i => i.ProductId + "_" + (i.ProductSizeId > 0 ? i.ProductSizeId.ToString() : "0")));

            SQL objSql = new SQL();
            try
            {
                objSql.BeginTransaction();

                objGRN.ChallanType = 12;
                objGRN.CompanyId = dto.CompanyId;
                objGRN.FinYearId = dto.FinYearId;
                GRNDAL grnDal = new GRNDAL();
                if (!grnDal.Add(objGRN, objSql))
                {
                    throw new Exception("Could not update transfer items (GRN).");
                }

                var sites = GetSites(dto.WorkOrderId);
                var site = sites.FirstOrDefault();
                if (site == null)
                {
                    throw new Exception("Site not found for challan.");
                }
                site.LedgerSiteId = dto.LedgerSiteId;
                site.StartDate = dto.RentStartDate.Year > 1900 ? dto.RentStartDate : dto.WorkOrderDate;
                site.RentStartDate = site.StartDate;
                site.Freight = dto.Freight;
                site.Vehicle = dto.Vehicle ?? "";
                site.Driver = dto.Driver ?? "";
                site.Weight = dto.Weight;
                site.JobNumber = dto.Number;
                site.ChallanNumber = dto.Number;
                site.WorkOrderId = dto.WorkOrderId;
                objSql.NewCommand();
                AddSite(site, objSql);

                var header = new WorkOrderDTO(dto.WorkOrderId)
                {
                    Number = dto.Number,
                    LedgerId = dto.LedgerId,
                    WorkOrderDate = dto.WorkOrderDate,
                    CompanyId = dto.CompanyId,
                    ClientAmount = existingHeader.ClientAmount
                };
                UpdateWorkOrder(header, objSql);

                foreach (var ex in snapshot)
                {
                    string key = ex.ProductId + "_" + (ex.ProductSizeId > 0 ? ex.ProductSizeId.ToString() : "0");
                    if (!payloadKeys.Contains(key))
                    {
                        DeleteChallanItem(ex.WorkOrderItemId, objSql);
                    }
                }

                foreach (var grnItem in objGRN.Items.Where(i => i.ProductId > 0 && i.Quantity > 0))
                {
                    string key = grnItem.ProductId + "_" + (grnItem.ProductSizeId > 0 ? grnItem.ProductSizeId.ToString() : "0");
                    var ex = snapshot.FirstOrDefault(e =>
                        e.ProductId == grnItem.ProductId &&
                        e.ProductSizeId == grnItem.ProductSizeId);
                    var line = new WorkOrderItemDTO
                    {
                        WorkOrderId = dto.WorkOrderId,
                        SiteId = site.SiteId,
                        ProductId = grnItem.ProductId,
                        ProductSizeId = grnItem.ProductSizeId,
                        PurchaseQty = grnItem.Quantity,
                        Quantity = grnItem.Quantity,
                        SiteItemType = SiteItemType.DELIVERED,
                        Rate = ex != null ? ex.Rate : (grnItem.Rate > 0 ? grnItem.Rate : 0)
                    };
                    line.SubTotal = line.Rate * line.PurchaseQty;
                    if (ex != null && payloadKeys.Contains(key))
                    {
                        line.WorkOrderItemId = ex.WorkOrderItemId;
                    }
                    objSql.NewCommand();
                    WorkOrderItemDAL itemDAL = new WorkOrderItemDAL();
                    itemDAL.Save(line, objSql);
                }

                if (dto.OtherCharges != null)
                {
                    foreach (var ch in dto.OtherCharges)
                    {
                        ch.WorkOrderId = dto.WorkOrderId;
                        objSql.NewCommand();
                        AddWorkOrderChage(ch, objSql);
                    }
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

        public bool ItemAdjustment(WorkOrderDTO wo, GRNDTO grn)
        {
            SQL sql = new SQL();
            try
            {
                int workOrderId = this.Save(wo, sql);
                if ((workOrderId > 0 || wo.WorkOrderId > 0) && grn.Items.Count > 0)
                {
                    GRNDAL grnDal = new GRNDAL();
                    if (wo.WorkOrderId > 0)
                    {
                        workOrderId = wo.WorkOrderId;
                    }
                    grn.WorkOrderId = workOrderId;
                    var existing = grn.Items.Where(o => o.GRNId > 0).FirstOrDefault();
                    if (existing != null)
                    {
                        grn.GRNId = existing.GRNId;
                    }
                    if (!grnDal.Add(grn, sql))
                    {
                        throw new Exception("Could not save receive list");
                    }
                }
                sql.Commit();
                return true;

            }
            catch (Exception ex)
            {
                sql.Rollback();
                throw ex;
            }
        }
        public DataSet MatAdjustList(string from, string to, int ledgerId, int ledgerSiteId, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@From", DbType.Date, ParameterDirection.Input, 0, from);
            objSql.AddParameter("@To", DbType.Date, ParameterDirection.Input, 0, to);
            if (ledgerId > 0)
                objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@ledgerSiteId", DbType.Int32, ParameterDirection.Input, 0, ledgerSiteId);
            return objSql.ExecuteDataSet(MAT_ADJ_LIST);
        }
        public DataSet MatAdjustById(int workOrderId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@workOrderId", DbType.Int32, ParameterDirection.Input, 0, workOrderId);
            return objSql.ExecuteDataSet(MAT_ADJ_DETAILS_BY_ID);
        }

        public bool UpdateEwayBIllNo(int workorderId, string ewayBillNo)
        {
            try
            {
                SQL objSql = new SQL();
                objSql.AddParameter("@workOrderid", DbType.Int32, ParameterDirection.Input, 0, workorderId);
                objSql.AddParameter("@ewayBillNo", DbType.String, ParameterDirection.Input, 0, ewayBillNo);
                return objSql.ExecuteNonQuery(p_workOrder_updEwayBillInfo) > 0;
            }

            catch (Exception ex)
            {
                throw new UDFException("Could not update ewaybill no in challan", ErrorCodes.ERROR_WHILE_UPDATE_EWAYBILL_IN_CHALLAN, ex);
            }
        }
        public async Task<int> UpdateStatus(WorkOrderDTO dto)
        {
            var objSql = new SQL();
            objSql.AddParameter("@workOrderId", DbType.Int32, ParameterDirection.Input, 0, dto.WorkOrderId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
            objSql.AddParameter("@modifiedBy", DbType.Int32, ParameterDirection.Input, 0, dto.ModifiedBy);
            objSql.AddParameter("@modifiedOn", DbType.DateTime, ParameterDirection.Input, 0, dto.ModifiedOn);
            var result = await objSql.ExecuteScalarAsync(WORKORDER_UPDATE_STATUS);
            return Convert.ToInt32(result);
        }
        public async Task<IEnumerable<WorkOrderOperationDto>> GetOperations(int workorderId, int companyId)
        {
            try
            {
                SQL objSql = new SQL();
                objSql.AddParameter("@workOrderId", DbType.Int32, ParameterDirection.Input, 0, workorderId);
                objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
                return await objSql.QueryAsync<WorkOrderOperationDto>(WORKORDER_OPERATIONS);
            }

            catch (Exception ex)
            {
                throw new UDFException("Could not get work order operations", ErrorCodes.ERROR_WHILE_UPDATE_EWAYBILL_IN_CHALLAN, ex);
            }
        }
        public async Task<WorkOrderDTO> JobWoById(int workOrderId, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@WorkOrderId", DbType.Int32, ParameterDirection.Input, 0, workOrderId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);

            return await objSql.QueryFirstAsync<WorkOrderDTO>(JOB_WORKORDER_SELBYID);
        }
        public async Task<IEnumerable<WorkOrderItemDTO>> JobCardChallanItems(int jobCardId, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@jobCardId", DbType.Int32, ParameterDirection.Input, 0, jobCardId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);

            return await objSql.QueryAsync<WorkOrderItemDTO>(JOB_WORKORDER_ITEMS_BY_JOBCARD_ID);
        }
        public async Task<IEnumerable<WorkOrderItemDTO>> ContractDelChallanItems(int contractId, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@contractId", DbType.Int32, ParameterDirection.Input, 0, contractId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);

            return await objSql.QueryAsync<WorkOrderItemDTO>(WORKORDER_ITEMS_BY_CONTRACT_ID);
        }
        public async Task<IEnumerable<WorkOrderItemDTO>> JobCardReturnChallanItems(int jobCardId, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@jobCardId", DbType.Int32, ParameterDirection.Input, 0, jobCardId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);

            return await objSql.QueryAsync<WorkOrderItemDTO>(JOB_WORKORDER_RETURN_ITEMS_BY_JOBCARD_ID);
        }

        public async Task<IEnumerable<WorkOrderItemDTO>> ContractReturnChallanItems(int contractId, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@contractId", DbType.Int32, ParameterDirection.Input, 0, contractId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);

            return await objSql.QueryAsync<WorkOrderItemDTO>(WORKORDER_RETURN_ITEMS_BY_CONTRACT_ID);
        }
        public async Task<int> DeleteChallan(int workOrderId, int siteId, LoggedInUserInfo user)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@siteId", DbType.Int32, ParameterDirection.Input, 0, siteId);
            objSql.AddParameter("@workOrderId", DbType.Int32, ParameterDirection.Input, 0, workOrderId);
            objSql.AddParameter("@deletedBy", DbType.Int32, ParameterDirection.Input, 0, user.UserId);
            objSql.AddParameter("@deletedon", DbType.DateTime, ParameterDirection.Input, 0, DateTime.Now);

            return await objSql.ExecuteNonQueryAsync(DELETE_SITE);
        }
        public async Task<bool> ChangeChallanParty(List<ChallanChangePartyDto> data, LoggedInUserInfo user)
        {
            SQL objSql = new SQL();
            try
            {
                objSql.BeginTransaction();
                foreach (var item in data)
                {
                    objSql.AddParameter("@workOrderId", DbType.Int32, ParameterDirection.Input, 0, item.WorkOrderId);
                    objSql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 0, item.LedgerId);

                    objSql.AddParameter("@LedgerSiteId", DbType.Int32, ParameterDirection.Input, 0, item.LedgerSiteId);
                    objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, user.DefaultCompanyId);
                    objSql.AddParameter("@modifiedBy", DbType.Int32, ParameterDirection.Input, 0, user.UserId);
                    objSql.AddParameter("@modifiedOn", DbType.DateTime, ParameterDirection.Input, 0, DateTime.Now);
                    var r = await objSql.ExecuteNonQueryAsync("p_DeliveryChallanParty_upd");
                    if (r <= 0)
                    {
                        throw new Exception("Information not updated for DC:" + item.ChallanNumber);
                    }
                }
                objSql.NewCommand();
                return true;
            }
            catch (Exception ex)
            {
                objSql.Rollback();
                throw ex;
            }
        }
        public async Task<string> GetLastChallanNumber(WorkOrderDTO dto)
        {
            SQL objSql = new SQL();
            try
            {

                objSql.AddParameter("@finYearId", DbType.Int32, ParameterDirection.Input, 0, dto.FinYearId);
                objSql.AddParameter("@type", DbType.Int32, ParameterDirection.Input, 0, dto.ChallanType);
                objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                var r = await objSql.ExecuteScalarAsync(GET_LAST_NUMBER);


                return Convert.ToString(r);
            }
            catch (Exception ex)
            {
                objSql.Rollback();
                throw ex;
            }
        }

        public async Task<string> GetNextChallanNumberPreview(WorkOrderDTO dto)
        {
            SQL objSql = new SQL();
            try
            {
                objSql.AddParameter("@finYearId", DbType.Int32, ParameterDirection.Input, 0, dto.FinYearId);
                objSql.AddParameter("@type", DbType.Int32, ParameterDirection.Input, 0, dto.ChallanType);
                objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                var r = await objSql.ExecuteScalarAsync(PREVIEW_NEXT_CHALLAN_NUMBER);
                return Convert.ToString(r);
            }
            catch (Exception ex)
            {
                objSql.Rollback();
                throw ex;
            }
        }

        #region procedures
        const string SAVE_WORKORDER = "p_WorkOrder_insV1";
        const string UPDATE_WORKORDER = "p_WorkOrder_upd";
        const string WORK_ORDERLIST = "p_WorkOrder_List";
        const string WORK_ORDERS_COMPANY = "p_WorkOrders_Company";
        const string WORK_SELECT = "p_WorkOrder_sel";
        const string WORKORDER_ITEMS = "p_WorkOrderItems_sel";
        const string WORKORDER_ITEMS_DEL = "p_workOrderItems_del";
        const string DELETE_SITEITEMS = "p_siteItems_del";
        const string ADD_SITE = "p_Site_ins";
        const string UPDATE_SITE = "p_Site_upd";
        const string UPDATE_SITE_BASIC_INFO = "p_Site_basicInfo_upd";

        const string GET_SITES = "p_SiteInfo_sel";
        const string GET_SITE_ITEMS = "p_SiteItems_sel";
        const string ADD_TAX = "p_WorkOrderTax_ins";
        const string GET_TAX = "p_WorkOrderTax_sel";
        const string WORKORDER_TAX_DEL_BY_SITE = "p_WorkOrderTax_delBySite";
        const string ITEM_ISSUED_REGISTER = "p_ItemsIssuedRegister";
        const string PARTY_DELIVERY_CHALLANS = "p_partyDeliveryChallans";
        const string PARTY_RETURN_CHALLANS = "p_partyReturnChallans";
        const string PARTY_BREAKAGE_REPORT = "p_partyBreakageReport";
        const string PARTY_LOST_REPORT = "p_partyLostReport";
        const string PARTY_EXCESS_REPORT = "p_partyExcessReport";
        const string ITEM_ISSUED_FOR_PRINT = "p_ItemsIssued_rpt";
        const string ITEM_RECEIVED_REGISTER_REPORT = "p_ItemsReceived_rpt";
        const string ITEM_RECEIVED_REGISTER = "p_ItemsReceived_register";

        const string GET_CLIENT_SITES = "p_ClientSiteInfo_sel";
        const string WORK_ORDER_BALANCE = "p_WorkOrderBalance";
        const string WORK_DUE_DATE_REMINDER = "p_WorkOrder_DueDates_Reminder";
        const string DELETE_CHALLAN_ITEM = "p_deleteChallanItem";

        const string WORK_ORDER_OVER_DUE_REMINDER = "P_WorkORder_OverDues_Reminder";
        const string CHALLAN_REPORT_HEADER = "p_ChallanReportHeader";

        const string ADD_OTHER_CHARGE = "p_WorkOrderCharge_ins";
        const string UPDATE_OTHER_CHARGE = "p_WorkOrderCharge_upd";
        const string WORK_ORDER_CHARGE_SEL = "p_WorkOrderCharge_sel";
        const string SITE_ORDER_CHARGE_SEL = "p_LedgerSiteCharges";
        const string CHALLAN_DOCUMENT_ADD = "p_ChallanDocument_ins";
        const string CHALLAN_DOCUMENT_SEL = "p_ChallanDocument_sel";
        const string CHALLAN_DOCUMENT_DEL = "p_ChallanDocument_del";
        const string PENDING_CHALLAN_ACKNOWLEDGMENTS = "p_pendingChallanAcknoledgements_sel";
        const string TRANSFER_CHALLAN = "p_TransferChallan_ins";
        const string LINK_TRANSFER_GRN = "p_GRN_LinkTransferWorkOrder";
        const string TRANSFER_GRN_LOOKUP = "p_TransferGrn_lookupByWorkOrderId";
        const string MAT_ADJ_LIST = "p_ItemsAdjust_ListV1";
        const string MAT_ADJ_DETAILS_BY_ID = "p_ItemsAdjust_ById";
        const string p_workOrder_updEwayBillInfo = "p_workOrder_updEwayBillInfo";

        const string WORKORDER_OPERATION_ADD = "p_workOrderOperation_ins";
        const string WORK_ORDERS_LIST = "p_workOrders_list";

        const string WORKORDER_OPERATIONS = "p_workOrder_operations_sel";
        const string JOB_WORKORDER_SELBYID = "p_jobWorkOrder_selById";
        const string JOB_WORKORDER_ITEMS_BY_JOBCARD_ID = "p_jobCardChallans_sel";
        const string WORKORDER_ITEMS_BY_CONTRACT_ID = "p_ContractDelChallans_sel";

        const string JOB_WORKORDER_RETURN_ITEMS_BY_JOBCARD_ID = "p_jobCardReturnChallans_sel";
        const string WORKORDER_RETURN_ITEMS_BY_CONTRACT_ID = "p_ContractReturnChallans_sel";

        const string DELETE_SITE = "p_siteInfo_delete";
        const string GET_LAST_NUMBER = "p_getNextChallanNumberV2";
        const string PREVIEW_NEXT_CHALLAN_NUMBER = "p_previewNextChallanNumberV2";
        const string WORKORDER_UPDATE_STATUS = "p_workOrder_markDispatched";


        #endregion

    }
}
