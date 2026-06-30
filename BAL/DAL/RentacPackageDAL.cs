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
    public class RentacPackageDAL
    {
        public IEnumerable<RentacPackage> GetActivePackages()
        {
            SQL objSql = new SQL();


            var ds = objSql.ExecuteDataSet(ALL_PACKAGES);

            var packages = (from d in ds.Tables[0].AsEnumerable()
                            select new RentacPackage
                            {
                                Name = d.Field<String>("Name"),

                                PackageId = d.Field<int>("PackageId"),
                                Challans = d.Field<int>("Challans"),
                                Users = d.Field<int>("Users"),
                                Companies = d.Field<int>("Companies"),
                                MonthlyPrice = d.Field<decimal>("MonthlyPrice"),
                                YearlyPrice = d.Field<decimal>("YearlyPrice"),
                                Total = d.Field<decimal>("Total"),
                                GST = d.Field<decimal>("GST")


                            }
                       );
            objSql.Dispose();
            return packages;

        }
        public int PurchasePackage(ClientPackage clientPackage)
        {
            SQL objSql = new SQL();

            objSql.AddParameter("@rbnClientId", DbType.Int32, ParameterDirection.Input, 0, clientPackage.RBNClientID);
            objSql.AddParameter("@PackageId", DbType.Int32, ParameterDirection.Input, 0, clientPackage.PackageId);
            objSql.AddParameter("@monthlyYearly", DbType.String, ParameterDirection.Input, 0, clientPackage.MonthlyYearly);
            objSql.AddParameter("@amount", DbType.Double, ParameterDirection.Input, 0, clientPackage.Amount);
            objSql.AddParameter("@PurchaseDate", DbType.DateTime, ParameterDirection.Input, 0, clientPackage.PurchasedDate);
            objSql.AddParameter("@validTill", DbType.DateTime, ParameterDirection.Input, 0, clientPackage.ValidTill);
            objSql.AddParameter("@purchasedBy", DbType.Int32, ParameterDirection.Input, 0, clientPackage.PurchasedBy);
            objSql.AddParameter("@payment_id", DbType.String, ParameterDirection.Input, 0, clientPackage.payment_id);


            var retValue = objSql.ExecuteNonQuery(CLIENT_PACKAGE_INS);

            objSql.Dispose();
            return retValue;

        }
        public RentacPackage PackageById(int packageId)
        {
            SQL objSql = new SQL();

            objSql.AddParameter("@PackageId", DbType.Int32, ParameterDirection.Input, 0, packageId);
            var ds = objSql.ExecuteDataSet(BY_ID);

            var packages = (from d in ds.Tables[0].AsEnumerable()
                            select new RentacPackage
                            {
                                Name = d.Field<String>("Name"),

                                PackageId = d.Field<int>("PackageId"),
                                Challans = d.Field<int>("Challans"),
                                Users = d.Field<int>("Users"),
                                Companies = d.Field<int>("Companies"),
                                MonthlyPrice = d.Field<decimal>("MonthlyPrice"),
                                YearlyPrice = d.Field<decimal>("YearlyPrice")

                            }
                       ).FirstOrDefault();
            objSql.Dispose();
            return packages;

        }
        public ClientPackage ClientPackageSel(int rbClientId)
        {
            SQL objSql = new SQL();

            objSql.AddParameter("@rbnClientId", DbType.Int32, ParameterDirection.Input, 0, rbClientId);
            var ds = objSql.ExecuteDataSet(CLIENT_ACTIVEPACKAGE);

            var package = (from d in ds.Tables[0].AsEnumerable()
                           select new ClientPackage
                           {

                               PackageId = d.Field<int>("PackageId"),
                               Challans = d.Field<int>("Challans"),
                               Users = d.Field<int>("Users"),
                               Companies = d.Field<int>("Companies"),
                               MonthlyYearly = d.Field<string>("MonthlyYearly"),
                               PurchasedDate = d.Field<DateTime>("PurchaseDate"),
                               ValidTill = d.Field<DateTime>("ValidTill")

                           }
                       ).FirstOrDefault();
            objSql.Dispose();
            if (package != null)
            {
                package.RemainingDays = (int)(package.ValidTill - DateTime.Now).TotalDays;
            }
            return package;

        }
        #region procs
        static string ALL_PACKAGES = "p_lookupPackages_sel";
        static string BY_ID = "p_packageById";
        static string CLIENT_PACKAGE_INS = "p_clientPackage_ins";
        static string CLIENT_ACTIVEPACKAGE = "p_clientPackage_sel";
        #endregion
    }
}
