using BAL.Data.Contracts;
using BAL.Data.Repository;
using BAL.DTO;
using BAL.Exceptions;
using Omu.ValueInjecter;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BAL.DAL
{
    public class ProductDAL
    {
        IProductTaxClassificationContract _productTaxClassificationContract;
        public ProductDAL()
        {
            _productTaxClassificationContract = new ProductTaxClassificationRepository();
        }
        public async Task<bool> Save(ProductDTO product)
        {
            SQL objSql = new SQL();
            try
            {

                objSql.AddParameter("@Name", DbType.String, ParameterDirection.Input, 0, product.Name);
                objSql.AddParameter("@Code", DbType.String, ParameterDirection.Input, 0, product.Code);
                objSql.AddParameter("@SACCode", DbType.String, ParameterDirection.Input, 0, product.SACCode);


                objSql.AddParameter("@ProductType", DbType.Int16, ParameterDirection.Input, 0, product.ProductType);
                objSql.AddParameter("@Category", DbType.Int16, ParameterDirection.Input, 0, product.Category);
                objSql.AddParameter("@UOM", DbType.Int16, ParameterDirection.Input, 0, product.UOM);
                objSql.AddParameter("@Size", DbType.String, ParameterDirection.Input, 0, product.Size);
                objSql.AddParameter("@Weight", DbType.Double, ParameterDirection.Input, 0, product.Weight);

                objSql.AddParameter("@Description", DbType.String, ParameterDirection.Input, 0, product.Description);
                objSql.AddParameter("@SortOrder", DbType.Int16, ParameterDirection.Input, 0, product.SortOrder);
                objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, product.CompanyId);
                objSql.AddParameter("@openingBalance", DbType.Int32, ParameterDirection.Input, 0, product.OpeningBalance);

                objSql.AddParameter("@mrp", DbType.Double, ParameterDirection.Input, 0, product.MRP);
                objSql.AddParameter("@salePrice", DbType.Double, ParameterDirection.Input, 0, product.SalePrice);
                objSql.AddParameter("@saleAccount", DbType.Int32, ParameterDirection.Input, 0, product.SaleAccount);
                objSql.AddParameter("@costPrice", DbType.Double, ParameterDirection.Input, 0, product.CostPrice);
                objSql.AddParameter("@purchaseAccount", DbType.Int32, ParameterDirection.Input, 0, product.PurchaseAccount);
                objSql.AddParameter("@taxCategoryId", DbType.Int16, ParameterDirection.Input, 0, product.TaxCategoryId);
                objSql.AddParameter("@HSNCode", DbType.String, ParameterDirection.Input, 0, product.HSNCode);
                objSql.AddParameter("@lossRate", DbType.Double, ParameterDirection.Input, 0, product.LossRate);
                objSql.AddParameter("@breakageRate", DbType.Double, ParameterDirection.Input, 0, product.BrekageRate);
                objSql.AddParameter("@rentRate", DbType.Double, ParameterDirection.Input, 0, product.RentRate);
                objSql.AddParameter("@unitSizeRate", DbType.Double, ParameterDirection.Input, 0, product.UnitSizeRate);
                objSql.AddParameter("@weightRate", DbType.Double, ParameterDirection.Input, 0, product.WeightRate);
                objSql.AddParameter("@itemGroupId", DbType.Int32, ParameterDirection.Input, 0, product.ItemGroupId);
                objSql.AddParameter("@image1", DbType.String, ParameterDirection.Input, 0, product.Image1);
                objSql.AddParameter("@PurchaseUnit", DbType.Int32, ParameterDirection.Input, 0, product.PurchaseUnitId);
                objSql.AddParameter("@uom2", DbType.Int32, ParameterDirection.Input, 0, product.UOM2);
                objSql.AddParameter("@ApplyUnit2Rate", DbType.Byte, ParameterDirection.Input, 0, product.ApplyUnit2Rate);


                bool result = true;
                if (product.ProductId == 0)
                {

                    objSql.AddParameter("@StoreId", DbType.Int32, ParameterDirection.Input, 0, product.StoreId);
                    objSql.AddParameter("@RbnClientId", DbType.Int16, ParameterDirection.Input, 0, product.RbnClientId);
                    product.ProductId = Convert.ToInt32(objSql.ExecuteScalar(ADD));
                    result = product.ProductId > 0;
                }
                else
                {
                    objSql.AddParameter("@ProductId", DbType.Int32, ParameterDirection.Input, 0, product.ProductId);
                    result = objSql.ExecuteNonQuery(UPDATE) == 1;
                }

                //if (result && product.ProductId > 0 && product.CompanyId > 0)
                //{
                //    try
                //    {
                //        UpdateProductUom2(product.ProductId, product.CompanyId, product.UOM2);
                //        UpdateProductApplyUnit2Rate(product.ProductId, product.CompanyId, product.ApplyUnit2Rate);
                //    //    UpdateProductPurchaseUnit(product.ProductId, product.CompanyId, product.PurchaseUnitId);
                //    }
                //    catch (SqlException)
                //    {
                //        // Column not deployed yet, or permission issue — do not fail whole save.
                //    }
                //}

                //update tax classifications if product is created or saved
                if (result && product.TaxClassiFications != null)
                {
                    foreach (var tc in product.TaxClassiFications)
                    {
                        var upsertDto = new SaveProductTaxClassificationDto();
                        upsertDto.InjectFrom(tc);
                        upsertDto.CompanyId = product.CompanyId;
                        upsertDto.ProductId = product.ProductId;
                        await this._productTaxClassificationContract.UpsertAsync(upsertDto);
                    }
                }

                var inventory = new InventoryDAL();
                var i = await inventory.UpdateItemBalance(product.FinYearId, product.CompanyId, product.ProductId);
                if (i == 0)
                {
                    throw new Exception("Could not update item stock. Please try again.");
                }

                return result;
            }
            catch (SqlException ex)
            {

                throw new UDFException(ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                objSql.Dispose();
            }
        }


        public async Task<List<ProductDTO>> GetAll(int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            //var ds = objSql.ExecuteDataSet(GETALL);
            return (await objSql.QueryAsync<ProductDTO>(GETALL)).ToList();
        }
        public async Task<ProductDTO> GetInfo(int productId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@productId", DbType.Int32, ParameterDirection.Input, 0, productId);
            var ds = objSql.ExecuteDataSet(GETINFO);
            var produtDto = objSql.ContructList<ProductDTO>(ds).FirstOrDefault();
            if (produtDto != null)
            {
                produtDto.UOM2 = await GetProductUom2Async(productId);
                produtDto.ApplyUnit2Rate = await GetProductApplyUnit2RateAsync(productId);
                produtDto.PurchaseUnitId = await GetProductPurchaseUnitAsync(productId);
                produtDto.TaxClassiFications = await _productTaxClassificationContract.GetByProductAsync(produtDto.CompanyId,
                    produtDto.ProductId
                    );
            }
            return produtDto;
        }
        public List<ProductDTO> Search(string query, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@Query", DbType.String, ParameterDirection.Input, 0, query);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            return objSql.ContructList<ProductDTO>(objSql.ExecuteDataSet(SEARCH));
        }

        public void AddRate(RentRateDTO obj)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@ProductId", DbType.String, ParameterDirection.Input, 0, obj.ProductId);
            objSql.AddParameter("@UOM", DbType.Int32, ParameterDirection.Input, 0, obj.UOM);
            objSql.AddParameter("@Rate", DbType.Double, ParameterDirection.Input, 0, obj.Rate);
            objSql.AddParameter("@MRP", DbType.Double, ParameterDirection.Input, 0, obj.MRP);
            objSql.AddParameter("@LossRate", DbType.Double, ParameterDirection.Input, 0, obj.LossRate);

            objSql.AddParameter("@EffectiveDate", DbType.Date, ParameterDirection.Input, 0, obj.EffectiveDate);
            objSql.AddParameter("@Active", DbType.Boolean, ParameterDirection.Input, 0, obj.Active);
            objSql.ExecuteDataSet(ADD_RATE);
        }
        public List<RentRateDTO> GetRates(int productId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@ProductId", DbType.String, ParameterDirection.Input, 0, productId);
            return objSql.ContructList<RentRateDTO>(objSql.ExecuteDataSet(GET_RATES));
        }
        public bool ActivateRate(bool active, int rentRateId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@Active", DbType.Boolean, ParameterDirection.Input, 0, active);
            objSql.AddParameter("@RentRateId", DbType.Int32, ParameterDirection.Input, 0, rentRateId);
            return objSql.ExecuteNonQuery(ACTIVATE_RATE) == 1;
        }

        public Boolean AddUOMSize(UOMSizeDTO dto)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@UOMId", DbType.Int32, ParameterDirection.Input, 0, dto.UOMId);
            objSql.AddParameter("@Size", DbType.String, ParameterDirection.Input, 0, dto.Size);
            return objSql.ExecuteNonQuery(ADDSIZE) == 1;
        }
        public List<UOMSizeDTO> GetALLSize()
        {
            SQL objSql = new SQL();
            return objSql.ContructList<UOMSizeDTO>(objSql.ExecuteDataSet(GET_ALLSIZE));

        }

        public Boolean ActviateSize(bool active, Int32 uomSizeId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@UomSizeId", DbType.Int32, ParameterDirection.Input, 0, uomSizeId);
            objSql.AddParameter("@Active", DbType.Boolean, ParameterDirection.Input, 0, active);
            return objSql.ExecuteNonQuery(ACTIVATE_SIZE) == 1;
        }
        public bool AddProductSize(List<ProductSizeDTO> list, int createdBy, int finYear, int companyId)
        {
            SQL objSql = new SQL();
            objSql.BeginTransaction();
            try
            {
                List<StockTransactionDTO> lstStock = new List<StockTransactionDTO>();
                foreach (ProductSizeDTO pS in list)
                {

                    objSql.NewCommand();
                    objSql.AddParameter("@ProductId", DbType.Int32, ParameterDirection.Input, 0, pS.ProductId);
                    objSql.AddParameter("@Size", DbType.String, ParameterDirection.Input, 0, pS.Size);
                    objSql.AddParameter("@Code", DbType.String, ParameterDirection.Input, 0, pS.Code);

                    if (pS.ProductSizeId > 0)
                    {
                        objSql.AddParameter("@ProductSizeId", DbType.Int32, ParameterDirection.Input, 0, pS.ProductSizeId);

                        objSql.ExecuteScalar(PRODUCT_SIZE_UPD);
                    }
                    else
                    {
                        pS.ProductSizeId = Convert.ToInt32(objSql.ExecuteScalar(PRODUCT_SIZE_INS));
                    }

                    StockTransactionDTO stockDTO = new StockTransactionDTO();
                    stockDTO.ProductId = pS.ProductId;
                    stockDTO.ProductSizeId = pS.ProductSizeId;
                    stockDTO.Quantity = pS.OpeningBalance;
                    stockDTO.PostingType = Convert.ToInt16(BAL.Enums.StockPostingType.OB);
                    stockDTO.PostedBy = createdBy;
                    stockDTO.FinYear = finYear;
                    stockDTO.CompanyId = companyId;
                    stockDTO.Remarks = "Opening balance";
                    // posting date will not be passed here as op will always be the first entry of stock in a fin-year
                    lstStock.Add(stockDTO);
                }
                InventoryDAL iDal = new InventoryDAL();
                iDal.PostStock(lstStock, objSql);
                objSql.Commit();
                return true;
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
            finally
            {
                objSql.Dispose();
            }




        }
        public async Task<bool> DeleteProductId(int productId, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@productId", DbType.Int32, ParameterDirection.Input, 0, productId);


            return (await objSql.ExecuteNonQueryAsync(DELETE_PRODUCT)) > 0;

        }
        public List<ProductSizeDTO> ProductSizeList(int productId, int finYearId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@ProductId", DbType.Int32, ParameterDirection.Input, 0, productId);
            objSql.AddParameter("@finYearId", DbType.Int16, ParameterDirection.Input, 0, finYearId);

            return objSql.ContructList<ProductSizeDTO>(objSql.ExecuteDataSet(PRODUCT_SIZE_SEL));
        }
        /// <summary>
        /// Gets all products with its all sizes
        /// </summary>
        /// <param name="companyId">Company to select the product of</param>
        /// <returns></returns>
        public List<ProductSizeDTO> ProductSizeListByCompany(int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            return objSql.ContructList<ProductSizeDTO>(objSql.ExecuteDataSet(ALL_PRODUCTS_SIZES));
        }

        #region BOM

        public async Task<bool> SaveBom(BOMDTO bom)
        {
            SQL objSql = new SQL();
            try
            {
                objSql.BeginTransaction();
                objSql.NewCommand();
                objSql.AddParameter("@productId", DbType.Int32, ParameterDirection.Input, 0, bom.ProductId);
                objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, bom.CompanyId);
                objSql.AddParameter("@quantity", DbType.Byte, ParameterDirection.Input, 0, bom.Quantity);
                objSql.AddParameter("@description", DbType.String, ParameterDirection.Input, 0, bom.Description);

                objSql.AddParameter("@modifiedOn", DbType.DateTime, ParameterDirection.Input, 0, bom.ModifiedOn);
                objSql.AddParameter("@modifiedBy", DbType.Int32, ParameterDirection.Input, 0, bom.ModifiedBy);
                if (bom.BomId > 0)
                {
                    objSql.AddParameter("@BomId", DbType.Int32, ParameterDirection.Input, 0, bom.BomId);
                    var x = await objSql.ExecuteNonQueryAsync(UPDATE_BOM);
                    if (x > 0)
                    {
                        objSql.NewCommand();
                        var products = String.Join(",", bom.Details.Select(o => o.ProductId));
                        objSql.AddParameter("@BomId", DbType.Int32, ParameterDirection.Input, 0, bom.BomId);
                        objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, bom.CompanyId);
                        objSql.AddParameter("@productIdsToKeep", DbType.String, ParameterDirection.Input, 0, products);
                        var bomDeleted = await objSql.ExecuteNonQueryAsync(DELETE_BOM_DETAILS);

                    }
                    else
                    {
                        throw new Exception("Coudl not update BOM");
                    }
                }
                else
                {
                    objSql.AddParameter("@CreatedOn", DbType.DateTime, ParameterDirection.Input, 0, bom.CreatedOn);
                    objSql.AddParameter("@CreatedBy", DbType.Int32, ParameterDirection.Input, 0, bom.CreatedBy);
                    objSql.AddParameter("@guId", DbType.String, ParameterDirection.Input, 0, bom.GuId);
                    bom.BomId = Convert.ToInt32(await objSql.ExecuteScalarAsync(ADD_BOM));
                }

                foreach (var d in bom.Details)
                {
                    d.GuId = Guid.NewGuid().ToString();
                    d.Amount = d.Quantity * d.Rate;
                    objSql.NewCommand();
                    objSql.AddParameter("@bomId", DbType.Int32, ParameterDirection.Input, 0, bom.BomId);

                    objSql.AddParameter("@productId", DbType.Int32, ParameterDirection.Input, 0, d.ProductId);
                    objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, bom.CompanyId);
                    objSql.AddParameter("@quantity", DbType.Double, ParameterDirection.Input, 0, d.Quantity);

                    objSql.AddParameter("@rate", DbType.Double, ParameterDirection.Input, 0, d.Rate);
                    objSql.AddParameter("@amount", DbType.Double, ParameterDirection.Input, 0, d.Amount);


                    objSql.AddParameter("@CreatedOn", DbType.DateTime, ParameterDirection.Input, 0, bom.CreatedOn);
                    objSql.AddParameter("@CreatedBy", DbType.Int32, ParameterDirection.Input, 0, bom.CreatedBy);
                    objSql.AddParameter("@guId", DbType.String, ParameterDirection.Input, 0, d.GuId);
                    objSql.AddParameter("@modifiedOn", DbType.DateTime, ParameterDirection.Input, 0, bom.ModifiedOn);
                    objSql.AddParameter("@modifiedBy", DbType.Int32, ParameterDirection.Input, 0, bom.ModifiedBy);

                    var x = await objSql.ExecuteNonQueryAsync(ADD_BOM_DETAILS);


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
        public async Task<IEnumerable<BOMDTO>> BOMList(int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            return await objSql.QueryAsync<BOMDTO>(BOM_LIST);
        }
        public async Task<BOMDTO> BOMById(int bomId, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@bomId", DbType.Int32, ParameterDirection.Input, 0, bomId);

            var bom = await objSql.QueryFirstAsync<BOMDTO>(BOM_BY_ID);
            bom.Details = (await objSql.QueryAsync<BomDetails>(BOMDETAILS_BY_BOMID)).ToList();

            return bom;

        }
        public async Task<BOMDTO> BOMByProductId(int productId, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@productId", DbType.Int32, ParameterDirection.Input, 0, productId);

            var bom = new BOMDTO();
            bom.Details = (await objSql.QueryAsync<BomDetails>(BOM_BY_PRODUCT_ID)).ToList();

            return bom;

        }
        public async Task<bool> DeleteBOM(int bomId, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@bomId", DbType.Int32, ParameterDirection.Input, 0, bomId);


            return (await objSql.ExecuteNonQueryAsync(BOM_DELETE)) > 0;

        }
        #endregion


        public async Task<int> SaveGroup(ItemGroupDTO dto)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@groupName", DbType.String, ParameterDirection.Input, 0, dto.GroupName);
            objSql.AddParameter("@groupCode", DbType.String, ParameterDirection.Input, 0, dto.GroupCode);
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);

            if (dto.ItemGroupId == 0)
            {
                objSql.AddParameter("@createdBy", DbType.Int32, ParameterDirection.Input, 0, dto.CreatedBy);
                objSql.AddParameter("@createdOn", DbType.DateTime, ParameterDirection.Input, 0, dto.CreatedOn);
                objSql.AddParameter("@guid", DbType.String, ParameterDirection.Input, 0, dto.GuId);
                return await objSql.ExecuteNonQueryAsync(ITEM_GROUP_INS);
            }
            else
            {
                objSql.AddParameter("@modifiedBy", DbType.Int32, ParameterDirection.Input, 0, dto.ModifiedBy);
                objSql.AddParameter("@modifiedOn", DbType.DateTime, ParameterDirection.Input, 0, dto.ModifiedOn);
                objSql.AddParameter("@itemGroupId", DbType.Int32, ParameterDirection.Input, 0, dto.ItemGroupId);
                return await objSql.ExecuteNonQueryAsync(ITEM_GROUP_UPD);
            }

        }
        public async Task<IEnumerable<ItemGroupDTO>> ListItemGroup(ItemGroupDTO dto)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
            return await objSql.QueryAsync<ItemGroupDTO>(ITEM_GROUP_LIST);
        }
        public async Task<ItemGroupDTO> ItemGroupById(ItemGroupDTO dto)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
            objSql.AddParameter("@itemGroupid", DbType.Int32, ParameterDirection.Input, 0, dto.ItemGroupId);

            return await objSql.QueryFirstAsync<ItemGroupDTO>(ITEM_GROUP_BYID);
        }
        public async Task<int> DeleteItemGroup(ItemGroupDTO dto)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
            objSql.AddParameter("@itemGroupid", DbType.Int32, ParameterDirection.Input, 0, dto.ItemGroupId);
            objSql.AddParameter("@deletedOn", DbType.DateTime, ParameterDirection.Input, 0, dto.ModifiedOn);
            objSql.AddParameter("@deletedBy", DbType.Int32, ParameterDirection.Input, 0, dto.ModifiedBy);

            return await objSql.ExecuteNonQueryAsync(ITEM_GROUP_DEL);
        }


        //private static void UpdateProductUom2(int productId, int companyId, short? uom2)
        //{
        //    var cs = ConfigurationManager.ConnectionStrings["sqlCon"]?.ConnectionString;
        //    if (string.IsNullOrEmpty(cs))
        //    {
        //        return;
        //    }

        //    using (var cn = new SqlConnection(cs))
        //    using (var cmd = new SqlCommand(
        //        "UPDATE dbo.Product SET UOM2 = @uom2 WHERE ProductId = @pid AND CompanyId = @cid", cn))
        //    {
        //        cmd.Parameters.Add("@uom2", SqlDbType.SmallInt).Value = (object)uom2 ?? DBNull.Value;
        //        cmd.Parameters.Add("@pid", SqlDbType.Int).Value = productId;
        //        cmd.Parameters.Add("@cid", SqlDbType.Int).Value = companyId;
        //        cn.Open();
        //        cmd.ExecuteNonQuery();
        //    }
        //}

        //private static void UpdateProductApplyUnit2Rate(int productId, int companyId, bool applyUnit2Rate)
        //{
        //    var cs = ConfigurationManager.ConnectionStrings["sqlCon"]?.ConnectionString;
        //    if (string.IsNullOrEmpty(cs))
        //    {
        //        return;
        //    }

        //    using (var cn = new SqlConnection(cs))
        //    using (var cmd = new SqlCommand(
        //        "UPDATE dbo.Product SET ApplyUnit2Rate = @apply WHERE ProductId = @pid AND CompanyId = @cid", cn))
        //    {
        //        cmd.Parameters.Add("@apply", SqlDbType.Bit).Value = applyUnit2Rate;
        //        cmd.Parameters.Add("@pid", SqlDbType.Int).Value = productId;
        //        cmd.Parameters.Add("@cid", SqlDbType.Int).Value = companyId;
        //        cn.Open();
        //        cmd.ExecuteNonQuery();
        //    }
        //}

        /// <summary>Fills <see cref="ProductRateDTO.ApplyUnit2Rate"/> from dbo.Product (same pattern as UOM2 column).</summary>
        public static void MergeApplyUnit2RateIntoProductRates(List<ProductRateDTO> rates, int companyId)
        {
            if (rates == null || rates.Count == 0)
            {
                return;
            }

            var ids = rates.Select(r => (int)r.ProductId).Distinct().ToList();
            if (ids.Count == 0)
            {
                return;
            }

            var cs = ConfigurationManager.ConnectionStrings["sqlCon"]?.ConnectionString;
            if (string.IsNullOrEmpty(cs))
            {
                return;
            }

            try
            {
                var idList = string.Join(",", ids);
                using (var cn = new SqlConnection(cs))
                using (var cmd = new SqlCommand(
                    "SELECT ProductId, ApplyUnit2Rate FROM dbo.Product WITH (NOLOCK) WHERE CompanyId = @cid AND ProductId IN (" + idList + ")", cn))
                {
                    cmd.Parameters.Add("@cid", SqlDbType.Int).Value = companyId;
                    cn.Open();
                    var map = new Dictionary<int, bool>();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var pid = Convert.ToInt32(dr["ProductId"]);
                            map[pid] = dr["ApplyUnit2Rate"] != DBNull.Value && Convert.ToBoolean(dr["ApplyUnit2Rate"]);
                        }
                    }

                    foreach (var r in rates)
                    {
                        var pid = (int)r.ProductId;
                        if (map.TryGetValue(pid, out var flag))
                        {
                            r.ApplyUnit2Rate = flag;
                        }
                    }
                }
            }
            catch (SqlException)
            {
                // Column not deployed yet or permission issue — billing falls back to company setting only.
            }
        }

        private static async Task<short?> GetProductUom2Async(int productId)
        {
            var cs = ConfigurationManager.ConnectionStrings["sqlCon"]?.ConnectionString;
            if (string.IsNullOrEmpty(cs))
            {
                return null;
            }

            try
            {
                using (var cn = new SqlConnection(cs))
                using (var cmd = new SqlCommand(
                    "SELECT UOM2 FROM dbo.Product WITH (NOLOCK) WHERE ProductId = @pid", cn))
                {
                    cmd.Parameters.Add("@pid", SqlDbType.Int).Value = productId;
                    await cn.OpenAsync();
                    var o = await cmd.ExecuteScalarAsync();
                    if (o == null || o == DBNull.Value)
                    {
                        return null;
                    }

                    return Convert.ToInt16(o);
                }
            }
            catch (SqlException)
            {
                return null;
            }
        }

        private static async Task<bool> GetProductApplyUnit2RateAsync(int productId)
        {
            var cs = ConfigurationManager.ConnectionStrings["sqlCon"]?.ConnectionString;
            if (string.IsNullOrEmpty(cs))
            {
                return false;
            }

            try
            {
                using (var cn = new SqlConnection(cs))
                using (var cmd = new SqlCommand(
                    "SELECT ApplyUnit2Rate FROM dbo.Product WITH (NOLOCK) WHERE ProductId = @pid", cn))
                {
                    cmd.Parameters.Add("@pid", SqlDbType.Int).Value = productId;
                    await cn.OpenAsync();
                    var o = await cmd.ExecuteScalarAsync();
                    if (o == null || o == DBNull.Value)
                    {
                        return false;
                    }

                    return Convert.ToBoolean(o);
                }
            }
            catch (SqlException)
            {
                return false;
            }
        }



        private static async Task<short?> GetProductPurchaseUnitAsync(int productId)
        {
            var cs = ConfigurationManager.ConnectionStrings["sqlCon"]?.ConnectionString;
            if (string.IsNullOrEmpty(cs))
            {
                return null;
            }

            try
            {
                using (var cn = new SqlConnection(cs))
                using (var cmd = new SqlCommand(
                    "SELECT PurchaseUnitId FROM dbo.Product WITH (NOLOCK) WHERE ProductId = @pid", cn))
                {
                    cmd.Parameters.Add("@pid", SqlDbType.Int).Value = productId;
                    await cn.OpenAsync();
                    var o = await cmd.ExecuteScalarAsync();
                    if (o == null || o == DBNull.Value)
                    {
                        return null;
                    }

                    return Convert.ToInt16(o);
                }
            }
            catch (SqlException)
            {
                return null;
            }
        }

        #region procedures
        const string ADD = "p_product_ins";
        const string UPDATE = "p_product_upd";
        const string DELETE_PRODUCT = "p_product_delete";

        const string GETALL = "p_product_selAll";
        const string GETINFO = "p_Product_sel";
        const string SEARCH = "p_Product_search";
        const string ADD_RATE = "p_RentRate_ins";
        const string GET_RATES = "p_RentRate_sel";
        const string ACTIVATE_RATE = "p_RentRate_Activate";
        const string ADDSIZE = "p_UOMSize_ins";
        const string GET_ALLSIZE = "p_UOMSize_selAll";
        const string ACTIVATE_SIZE = "p_ActivateSize";
        const string PRODUCT_SIZE_INS = "p_productSize_ins";
        const string PRODUCT_SIZE_UPD = "p_productSize_upd";
        const string PRODUCT_SIZE_SEL = "p_productSize_sel";
        const string ALL_PRODUCTS_SIZES = "p_AllProduct_sizes";
        const string ADD_BOM = "p_bom_ins";
        const string UPDATE_BOM = "p_bom_upd";

        const string ADD_BOM_DETAILS = "p_bomDetail_ins";
        const string BOM_LIST = "p_bom_list";
        const string BOM_BY_ID = "p_bom_byId";
        const string BOMDETAILS_BY_BOMID = "p_bomDetails";
        const string DELETE_BOM_DETAILS = "p_bomdetails_del";
        const string BOM_BY_PRODUCT_ID = "p_bom_byProductId";
        const string BOM_DELETE = "p_bom_delete";


        const string ITEM_GROUP_INS = "p_itemGroupMaster_ins";
        const string ITEM_GROUP_UPD = "p_itemGroupMaster_upd";
        const string ITEM_GROUP_LIST = "p_itemGroupMaster_list";
        const string ITEM_GROUP_BYID = "p_itemGroupMaster_byId";
        const string ITEM_GROUP_DEL = "p_itemGroupMaster_del";

        #endregion
    }
}
