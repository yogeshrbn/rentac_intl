using BAL.Common;
using BAL.DTO;
using BAL.Exceptions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace BAL.DAL
{
    internal class GRNDAL
    {

        internal bool Add(GRNDTO obj, SQL sql = null)
        {
            SQL objSql = sql;

            try
            {

                if (sql == null)
                {
                    objSql = new SQL();
                    objSql.BeginTransaction();
                }

                if (obj.Items.Count > 0)
                {
                    objSql.NewCommand();

                    objSql.AddParameter("@JobNumber", DbType.String, ParameterDirection.Input, 0, obj.JobNumber);

                    objSql.AddParameter("@Receiver", DbType.String, ParameterDirection.Input, 0, obj.Receiver);
                    objSql.AddParameter("@Sender", DbType.String, ParameterDirection.Input, 0, obj.Sender);
                    objSql.AddParameter("@LedgerId", DbType.String, ParameterDirection.Input, 0, obj.LedgerId);

                    objSql.AddParameter("@ReceivingDate", DbType.DateTime, ParameterDirection.Input, 0, obj.ReceivingDate);
                    objSql.AddParameter("@RentStopDate", DbType.DateTime, ParameterDirection.Input, 0, obj.RentStopDate);

                    objSql.AddParameter("@GRN", DbType.String, ParameterDirection.Input, 0, obj.GRN);
                    objSql.AddParameter("@FinYearId", DbType.Int16, ParameterDirection.Input, 0, obj.FinYearId);
                    objSql.AddParameter("@Remarks", DbType.String, ParameterDirection.Input, 0, obj.Remarks);
                    objSql.AddParameter("@freight", DbType.Double, ParameterDirection.Input, 0, obj.Freight);
                    objSql.AddParameter("@otherChargeAmount", DbType.Double, ParameterDirection.Input, 0, obj.TotalOtherCharges);
                    objSql.AddParameter("@driver", DbType.String, ParameterDirection.Input, 0, obj.Driver);
                    objSql.AddParameter("@vehicleNo", DbType.String, ParameterDirection.Input, 0, obj.VehicleNo);
                    objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, obj.CompanyId);
                    objSql.AddParameter("@tnc", DbType.String, ParameterDirection.Input, 0, obj.Tnc);
                    objSql.AddParameter("@warehouseId", DbType.Int32, ParameterDirection.Input, 0, obj.WarehouseId);
                    objSql.AddParameter("@ApproximateValue", DbType.Decimal, ParameterDirection.Input, 0, obj.ApproximateValue);
                    objSql.AddParameter("@Weight", DbType.Decimal, ParameterDirection.Input, 0, obj.Weight);
                    objSql.AddParameter("@LRNumber", DbType.String, ParameterDirection.Input, 0, obj.LRNumber ?? (object)DBNull.Value);
                    objSql.AddParameter("@CRNumber", DbType.String, ParameterDirection.Input, 0, obj.CRNumber ?? (object)DBNull.Value);
                    objSql.AddParameter("@GRNumber", DbType.String, ParameterDirection.Input, 0, obj.GRNumber ?? (object)DBNull.Value);
                    objSql.AddParameter("@PONumber", DbType.String, ParameterDirection.Input, 0, obj.PONumber ?? (object)DBNull.Value);
                    objSql.AddParameter("@shipFrom", DbType.String, ParameterDirection.Input, 0, obj.ShipFrom ?? (object)DBNull.Value);
                    objSql.AddParameter("@adjType", DbType.Byte, ParameterDirection.Input, 0, obj.AdjType > 0 ? obj.AdjType : (byte)1);


                    if (obj.WorkOrderId > 0)
                    {
                        objSql.AddParameter("@WorkOrderId", DbType.Int32, ParameterDirection.Input, 0, obj.WorkOrderId);
                    }
                    if (obj.LedgerSiteId > 0)
                    {
                        objSql.AddParameter("@LedgerSiteId", DbType.Int32, ParameterDirection.Input, 0, obj.LedgerSiteId);
                    }
                    int grnId = obj.GRNId;
                    if (obj.GRNId > 0)
                    {
                        objSql.AddParameter("@GRNID", DbType.Int32, ParameterDirection.Input, 0, obj.GRNId);

                        Convert.ToInt32(objSql.ExecuteScalar(UPDATE));
                    }
                    else
                    {
                        if (obj.ChallanType == 0)
                        {
                            obj.ChallanType = 2;
                        }
                        objSql.AddParameter("@TransactionId", DbType.Int32, ParameterDirection.Input, 0, obj.TransactionId);
                        objSql.AddParameter("@challanType", DbType.Int16, ParameterDirection.Input, 0, obj.ChallanType);
                        objSql.AddParameter("@JobCardId", DbType.Int32, ParameterDirection.Input, 0, obj.JobCardId);

                        obj.GRNId = grnId = Convert.ToInt32(objSql.ExecuteScalar(ADD));
                    }

                    objSql.NewCommand();
                    objSql.AddParameter("@grnId", DbType.Int32, ParameterDirection.Input, 0, grnId);
                    objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, obj.CompanyId);

                    objSql.ExecuteNonQuery(GRN_ITEMS_DEL_ALL);


                    foreach (WorkOrderItemDTO item in obj.Items)
                    {
                        if (item.Deleted == 1)
                        {
                            continue;
                        }
                        //if (item.Deleted == 1)
                        //{
                        //    objSql.NewCommand();
                        //    objSql.AddParameter("@grnItemId", DbType.Int32, ParameterDirection.Input, 0, item.GRNItemId);
                        //    objSql.ExecuteNonQuery(GRN_ITEMS_DEL);
                        //    continue;
                        //}
                        if (item.ProductId == 0)
                        {
                            continue;
                        }
                        objSql.NewCommand();
                        objSql.AddParameter("@GrnId", DbType.Int32, ParameterDirection.Input, 0, grnId);

                        objSql.AddParameter("@ProductId", DbType.Int32, ParameterDirection.Input, 0, item.ProductId);
                        objSql.AddParameter("@Quantity", DbType.Double, ParameterDirection.Input, 0, item.Quantity);
                        objSql.AddParameter("@ExcessQty", DbType.Double, ParameterDirection.Input, 0, item.ExcessQty);
                        objSql.AddParameter("@ShortQty", DbType.Double, ParameterDirection.Input, 0, item.ShortQty);
                        objSql.AddParameter("@receivingQty", DbType.Double, ParameterDirection.Input, 0, item.ReceivingQty);

                        objSql.AddParameter("@Breakage", DbType.Double, ParameterDirection.Input, 0, item.Breakage);
                        objSql.AddParameter("@BreakageRate", DbType.Double, ParameterDirection.Input, 0, item.BreakageRate);
                        //objSql.AddParameter("@DamageComponent", DbType.String, ParameterDirection.Input, 0, string.IsNullOrEmpty(item.DamageComponent) ? (object)DBNull.Value : item.DamageComponent);
                        objSql.AddParameter("@Remarks", DbType.String, ParameterDirection.Input, 0, item.Remarks);
                        objSql.AddParameter("@ChargeReturnedDate", DbType.Boolean, ParameterDirection.Input, 0, item.ChargeReturnedDate);
                        objSql.AddParameter("@ConsiderFullReceive", DbType.Boolean, ParameterDirection.Input, 0, item.ConsiderFullReceive);
                        objSql.AddParameter("@rate", DbType.Double, ParameterDirection.Input, 0, item.Rate);
                        objSql.AddParameter("@scrap", DbType.Double, ParameterDirection.Input, 0, item.Scrap);


                        //if (item.GRNItemId > 0)
                        //{
                        //    objSql.AddParameter("@GRNItemId", DbType.Double, ParameterDirection.Input, 0, item.GRNItemId);
                        //    objSql.ExecuteNonQuery(UPDATE_ITEMS);
                        //}
                        //else
                        //{
                        objSql.AddParameter("@GroupItemId", DbType.Int32, ParameterDirection.Input, 0, item.GroupItemId);

                        //Product sizeId will be added on insert as update is based-up on the primary key of the table.
                        if (item.ProductSizeId > 0)
                        {
                            objSql.AddParameter("@ProductSizeId", DbType.Int32, ParameterDirection.Input, 0, item.ProductSizeId);
                        }
                        objSql.ExecuteNonQuery(ADD_ITEMS);
                        int grnItemId = ResolveGrnItemIdAfterItemInsert(objSql, grnId, item);
                        InsertDamageComponentRows(objSql, grnItemId, item.ProductId, item.DamageComponent);
                        // }

                    }

                    if (obj.OtherCharges != null)
                    {
                        foreach (var charge in obj.OtherCharges)
                        {
                            objSql.NewCommand();
                            charge.CompanyId = obj.CompanyId;
                            objSql.AddParameter("@ChargeId", DbType.Int32, ParameterDirection.Input, 0, charge.ChargeId);
                            objSql.AddParameter("@Amount", DbType.Double, ParameterDirection.Input, 0, charge.Amount);
                            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, charge.CompanyId);

                            if (charge.GRNChargeId > 0)
                            {
                                objSql.AddParameter("@GRNChargeId", DbType.Int32, ParameterDirection.Input, 0, charge.GRNChargeId);
                                objSql.ExecuteNonQuery(GRN_CHARGE_UPD);
                            }
                            else
                            {
                                objSql.AddParameter("@grnId", DbType.Int32, ParameterDirection.Input, 0, obj.GRNId);
                                objSql.ExecuteNonQuery(GRN_CHARGE_ADD);
                            }
                        }
                    }
                }
                //var productIds = obj.Items.Select(o => o.ProductId.ToString()).ToList();
                //string products = String.Join(",", productIds);
                ////delete products in case of update
                //objSql.NewCommand();
                //objSql.AddParameter("@grnId", DbType.Int32, ParameterDirection.Input, 0, obj.GRNId);
                //objSql.AddParameter("@productIds", DbType.String, ParameterDirection.Input, 0, products);
                //objSql.ExecuteNonQuery(GRN_ITEMS_DEL);

                if (sql == null)
                    objSql.Commit();

                //objSql.BeginTransaction();
                //objSql.NewCommand();
                //var inventoryDal = new InventoryDAL();
                //inventoryDal.StockInsUpd(obj.RbnClientId, obj.FinYearId, obj.CompanyId, obj.ReceivingDate);
                //objSql.Commit();
                return true;
            }
            catch (Exception ex)
            {
                if (sql == null)
                    objSql.Rollback();
                throw ex;
            }
        }


        internal async Task<GRNDTO> GrnById(int grnId, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@GrnId", DbType.Int32, ParameterDirection.Input, 0, grnId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);

            return await objSql.QueryFirstAsync<GRNDTO>(GRN_BY_ID);
        }
        internal List<GRNDTO> GetItemsByGrnId(int grnId, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@GrnId", DbType.Int32, ParameterDirection.Input, 0, grnId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);

            var list = objSql.ContructList<GRNDTO>(objSql.ExecuteDataSet(GET_ITEMS_BYGRNID));
            objSql = new SQL();
            MergeDamageComponentsFromDetailTable(objSql, grnId, list);
            return list;
        }
        internal DataSet GRNHeader(int grnId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@GrnId", DbType.Int32, ParameterDirection.Input, 0, grnId);
            return objSql.ExecuteDataSet(GRN_HEADER);
        }

        /// <summary>
        /// Gets the other charges on a issue challan
        /// </summary>
        /// <param name="workOrderId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<GRNChageDTO>> GetOtherChages(int grnId, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@grnId", DbType.Int32, ParameterDirection.Input, 0, grnId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);

            return await objSql.QueryAsync<GRNChageDTO>(GRN_CHARGE_SEL);
        }
        public async Task<int> DeleteChallan(int companyId, int grnId, LoggedInUserInfo user)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@grnId", DbType.Int32, ParameterDirection.Input, 0, grnId);
            objSql.AddParameter("@companyid", DbType.Int32, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@deletedBy", DbType.Int32, ParameterDirection.Input, 0, user.UserId);
            objSql.AddParameter("@deletedon", DbType.DateTime, ParameterDirection.Input, 0, DateTime.Now);

            return await objSql.ExecuteNonQueryAsync(DELETE_CHALLAN);
        }
        public bool UpdateEwayBIllNo(int grnId, int companyId, string ewayBillNo)
        {
            try
            {
                SQL objSql = new SQL();
                objSql.AddParameter("@grnId", DbType.Int32, ParameterDirection.Input, 0, grnId);
                objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);

                objSql.AddParameter("@ewayBillNo", DbType.String, ParameterDirection.Input, 0, ewayBillNo);
                return objSql.ExecuteNonQuery("p_grn_updEwayBillInfo") > 0;
            }

            catch (Exception ex)
            {
                throw new UDFException("Could not update ewaybill no in challan", ErrorCodes.ERROR_WHILE_UPDATE_EWAYBILL_IN_CHALLAN, ex);
            }
        }
        public async Task<int> InwardConfirm(GRNDTO dto)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@grnId", DbType.Int32, ParameterDirection.Input, 0, dto.GRNId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
            objSql.AddParameter("@modifiedBy", DbType.Int32, ParameterDirection.Input, 0, dto.ModifiedBy);
            objSql.AddParameter("@modifiedOn", DbType.DateTime, ParameterDirection.Input, 0, dto.ModifiedOn);
            var result = await objSql.ExecuteScalarAsync(GRN_INWARD_CONFIRM);
            return Convert.ToInt32(result);
        }
        public async Task<string> LastChallanNumber(int companyId, int finYearId, int ChallanType)
        {
            try
            {
                SQL objSql = new SQL();
                objSql.AddParameter("@type", DbType.Int32, ParameterDirection.Input, 0, ChallanType);
                objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);

                objSql.AddParameter("@finYearId", DbType.String, ParameterDirection.Input, 0, finYearId);
                var str = Convert.ToString(await objSql.ExecuteScalarAsync("p_getLastReturnChallanNumber"));
                return String.IsNullOrEmpty(str) ? "" : Convert.ToString(str);
            }

            catch (Exception ex)
            {
                throw new Exception("Could not get last challan Number", ex);
            }

        }

        public async Task<string> GetNextReceivingChallanNumberPreview(int companyId, int finYearId, int challanType)
        {
            try
            {
                SQL objSql = new SQL();
                objSql.AddParameter("@finYearId", DbType.Int32, ParameterDirection.Input, 0, finYearId);
                objSql.AddParameter("@type", DbType.Int32, ParameterDirection.Input, 0, challanType);
                objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
                var str = Convert.ToString(await objSql.ExecuteScalarAsync(PREVIEW_NEXT_RECEIVING_CHALLAN_NUMBER));
                return string.IsNullOrEmpty(str) ? string.Empty : str;
            }
            catch (Exception ex)
            {
                throw new Exception("Could not get next receiving challan number preview", ex);
            }
        }

        #region Procedures
        const string PREVIEW_NEXT_RECEIVING_CHALLAN_NUMBER = "p_previewNextReceivingChallanNumberV2";
        const string ADD = "p_GRN_insV1";
        const string UPDATE = "p_GRN_upd";

        const string ADD_ITEMS = "p_GRNItems_ins";
        const string UPDATE_ITEMS = "p_GRNItems_upd";
        const string GRN_ITEMS_DEL = "p_grnItems_del";
        const string GRN_ITEMS_DEL_ALL = "p_grnItems_delAll";
        const string GET_ITEMS_BYGRNID = "p_ItemsReceived_ByGRNID";
        const string GRN_HEADER = "p_GRNHeader";
        const string GRN_CHARGE_SEL = "p_GRNCharge_sel";
        const string GRN_CHARGE_UPD = "p_GRNCharge_upd";
        const string GRN_CHARGE_ADD = "p_GRNCharge_ins";
        const string DELETE_CHALLAN = "p_grn_delete";
        const string GRN_BY_ID = "p_grn_byId";
        const string GRN_INWARD_CONFIRM = "p_grn_markInwardConfirm";
        const string DAMAGE_COMPONENTS_INS = "p_GRNItemDamageComponents_ins";
        const string DAMAGE_COMPONENTS_SEL_BY_GRN = "p_GRNItemDamageComponents_selByGrnId";
        const string GRN_ITEMS_LAST_ID_FOR_LINE = "p_GRNItems_getLastItemIdForLine";

        #endregion

        private sealed class DamageDetailRow
        {
            public int ProductId;
            public int PieceNo;
            public string Name;
            public decimal Quantity;
            public decimal Rate;
            public decimal Cost;
        }

        private static int ResolveGrnItemIdAfterItemInsert(SQL objSql, int grnId, WorkOrderItemDTO item)
        {
            objSql.NewCommand();
            objSql.AddParameter("@GrnId", DbType.Int32, ParameterDirection.Input, 0, grnId);
            objSql.AddParameter("@ProductId", DbType.Int32, ParameterDirection.Input, 0, item.ProductId);
            var sizeId = item.ProductSizeId > 0 ? item.ProductSizeId : 0;
            objSql.AddParameter("@ProductSizeId", DbType.Int32, ParameterDirection.Input, 0, sizeId);
            object o = objSql.ExecuteScalar(GRN_ITEMS_LAST_ID_FOR_LINE);
            if (o == null || o == DBNull.Value)
                return 0;
            return Convert.ToInt32(o);
        }

        private static bool DamageJsonIsPieceWiseLegacy(JArray arr)
        {
            if (arr == null || arr.Count == 0)
                return false;
            foreach (JToken t in arr)
            {
                if (t is JObject jo && (jo["components"] != null || jo["Components"] != null))
                    return true;
            }
            return false;
        }

        private static void InsertDamageComponentRows(SQL objSql, int grnItemId, int productId, string damageJson)
        {
            if (grnItemId <= 0 || string.IsNullOrWhiteSpace(damageJson))
                return;
            JArray arr;
            try
            {
                arr = JArray.Parse(damageJson.Trim());
            }
            catch
            {
                return;
            }
            if (DamageJsonIsPieceWiseLegacy(arr))
            {
                foreach (JToken piece in arr)
                {
                    int pieceNo = 0;
                    var pn = piece["pieceNo"] ?? piece["PieceNo"];
                    if (pn != null)
                        int.TryParse(pn.ToString(), out pieceNo);
                    var comps = piece["components"] as JArray ?? piece["Components"] as JArray;
                    if (comps == null)
                        continue;
                    foreach (JToken c in comps)
                    {
                        var nameTok = c["name"] ?? c["Name"];
                        string name = nameTok != null ? nameTok.ToString().Trim() : string.Empty;
                        if (name.Length == 0)
                            continue;
                        decimal cost = 0;
                        var costTok = c["cost"] ?? c["Cost"];
                        if (costTok != null)
                            decimal.TryParse(costTok.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out cost);
                        objSql.NewCommand();
                        objSql.AddParameter("@GRNItemId", DbType.Int32, ParameterDirection.Input, 0, grnItemId);
                        objSql.AddParameter("@ProductId", DbType.Int32, ParameterDirection.Input, 0, productId);
                        objSql.AddParameter("@PieceNo", DbType.Int32, ParameterDirection.Input, 0, pieceNo);
                        objSql.AddParameter("@ComponentName", DbType.String, ParameterDirection.Input, 200, name);
                        objSql.AddParameter("@Cost", DbType.Decimal, ParameterDirection.Input, 0, cost);
                        objSql.AddParameter("@Quantity", DbType.Decimal, ParameterDirection.Input, 0, 0m);
                        objSql.AddParameter("@Rate", DbType.Decimal, ParameterDirection.Input, 0, 0m);
                        objSql.ExecuteNonQuery(DAMAGE_COMPONENTS_INS);
                    }
                }
                return;
            }

            foreach (JToken t in arr)
            {
                if (!(t is JObject c))
                    continue;
                var nameTok = c["name"] ?? c["Name"];
                string name = nameTok != null ? nameTok.ToString().Trim() : string.Empty;
                if (name.Length == 0)
                    continue;
                decimal quantity = 0;
                var qtyTok = c["quantity"] ?? c["Quantity"];
                if (qtyTok != null)
                    decimal.TryParse(qtyTok.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out quantity);
                decimal rate = 0;
                var rateTok = c["rate"] ?? c["Rate"];
                if (rateTok != null)
                    decimal.TryParse(rateTok.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out rate);
                decimal cost = 0;
                var costTok = c["cost"] ?? c["Cost"];
                if (costTok != null)
                    decimal.TryParse(costTok.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out cost);
                if (cost == 0 && quantity != 0 && rate != 0)
                    cost = Math.Round(quantity * rate, 2, MidpointRounding.AwayFromZero);
                objSql.NewCommand();
                objSql.AddParameter("@GRNItemId", DbType.Int32, ParameterDirection.Input, 0, grnItemId);
                objSql.AddParameter("@ProductId", DbType.Int32, ParameterDirection.Input, 0, productId);
                objSql.AddParameter("@PieceNo", DbType.Int32, ParameterDirection.Input, 0, 0);
                objSql.AddParameter("@ComponentName", DbType.String, ParameterDirection.Input, 200, name);
                objSql.AddParameter("@Cost", DbType.Decimal, ParameterDirection.Input, 0, cost);
                objSql.AddParameter("@Quantity", DbType.Decimal, ParameterDirection.Input, 0, quantity);
                objSql.AddParameter("@Rate", DbType.Decimal, ParameterDirection.Input, 0, rate);
                objSql.ExecuteNonQuery(DAMAGE_COMPONENTS_INS);
            }
        }

        private static void MergeDamageComponentsFromDetailTable(SQL objSql, int grnId, List<GRNDTO> items)
        {
            if (items == null || items.Count == 0)
                return;
            objSql.NewCommand();
            objSql.AddParameter("@GrnId", DbType.Int32, ParameterDirection.Input, 0, grnId);
            DataSet ds;
            try
            {
                ds = objSql.ExecuteDataSet(DAMAGE_COMPONENTS_SEL_BY_GRN);
            }
            catch
            {
                return;
            }
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return;
            var table = ds.Tables[0];
            var byItem = new Dictionary<int, List<DamageDetailRow>>();
            var byProduct = new Dictionary<int, List<DamageDetailRow>>();
            foreach (DataRow r in table.Rows)
            {
                int gid = Convert.ToInt32(r["GRNItemId"]);
                int productId = r.Table.Columns.Contains("ProductId") && r["ProductId"] != DBNull.Value ? Convert.ToInt32(r["ProductId"]) : 0;
                var detail = new DamageDetailRow
                {
                    ProductId = productId,
                    PieceNo = Convert.ToInt32(r["PieceNo"]),
                    Name = Convert.ToString(r["ComponentName"]),
                    Quantity = r.Table.Columns.Contains("Quantity") && r["Quantity"] != DBNull.Value ? Convert.ToDecimal(r["Quantity"]) : 0m,
                    Rate = r.Table.Columns.Contains("Rate") && r["Rate"] != DBNull.Value ? Convert.ToDecimal(r["Rate"]) : 0m,
                    Cost = r["Cost"] != DBNull.Value ? Convert.ToDecimal(r["Cost"]) : 0m
                };
                if (!byItem.ContainsKey(gid))
                    byItem[gid] = new List<DamageDetailRow>();
                byItem[gid].Add(detail);
                if (productId > 0)
                {
                    if (!byProduct.ContainsKey(productId))
                        byProduct[productId] = new List<DamageDetailRow>();
                    byProduct[productId].Add(detail);
                }
            }
            foreach (var line in items)
            {
                if (!byItem.ContainsKey(line.GRNItemId))
                {
                    // Some legacy GRN list SP variants may not return a reliable GRNItemId.
                    // Fallback to ProductId so editgrn can still prefill damage details.
                    if (line.ProductId > 0 && byProduct.ContainsKey(line.ProductId))
                    {
                        line.DamageComponent = SerializeDamagePiecesForApi(byProduct[line.ProductId]);
                    }
                    continue;
                }
                line.DamageComponent = SerializeDamagePiecesForApi(byItem[line.GRNItemId]);
            }
        }

        private static string SerializeDamagePiecesForApi(List<DamageDetailRow> rows)
        {
            var ja = new JArray();
            foreach (var r in rows)
                ja.Add(new JObject
                {
                    ["name"] = r.Name,
                    ["quantity"] = r.Quantity,
                    ["rate"] = r.Rate,
                    ["cost"] = r.Cost
                });
            return ja.ToString(Newtonsoft.Json.Formatting.None);
        }

    }
}