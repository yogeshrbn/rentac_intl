using BAL.DAL;
using BAL.DTO;
using Omu.ValueInjecter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BAL.Objects
{
    public class RBNClient
    {

        public RBNClient() { }

        public RBNClientDTO GetInfo(int clientId)
        {
            var dal = new RBNClientDAL();
            return dal.GetInfo(clientId);
        }

        public int UpdateInfo(RBNClientDTO client)
        {
            var dal = new RBNClientDAL();
            return dal.UpdateInfo(client);
        }

        public int Create(UserRegisterDTO dto)
        {
            var dal = new RBNClientDAL();
            var clientdto = new RBNClientDTO();
            clientdto.Name = dto.Company;
            clientdto.SpocName = dto.Name;
            clientdto.Email = dto.Email;
            clientdto.Mobile = dto.Mobile;
            int clientId = dal.Register(clientdto);
            var cDal = new CompanyDAL();
            var company = new CompanyDTO();
            company.Address1 = "";
            company.Name = clientdto.Name;
            company.RbnClientId = clientId;
            int companyId = cDal.Save(company);

            if (companyId > 0)
            {
                clientdto.DefaultCompanyId = companyId;
                dal.UpdateInfo(clientdto);

                var user = new UserDTO();
                user.Active = true;
                user.AllowSwitchCompany = true;
                user.FullName = clientdto.SpocName;
                user.Email = clientdto.Email;
                user.Phone = clientdto.Mobile;
                user.UserName = clientdto.Email;
                user.DefaultCompanyId = companyId;
                user.Companies = companyId.ToString();
                user.RbnClientId = clientId;
                user.RoleId = 1;
                user.Password = Guid.NewGuid().ToString().Substring(1, 6);
                var userDal = new User();
                var userResult = userDal.CreateUser(user);

                var clientPackage = new ClientPackageDTO();

                clientPackage.RBNClientID = clientId;
                clientPackage.CompanyId = companyId;
                clientPackage.Amount = 0;
                clientPackage.PackageId = 99;
                clientPackage.MonthlyYearly = "monthly";
                clientPackage.MonthlyPrice = clientPackage.Amount = 0;
                clientPackage.PurchasedBy = clientId;
                clientPackage.PurchasedDate = DateTime.Now;
                clientPackage.ValidTill = DateTime.Now.AddDays(7);
                clientPackage.payment_id = Guid.NewGuid().ToString();
                var rentacPackageService = new RentacPackageService();

                rentacPackageService.PurchasePackage(clientPackage);


            }
            else
            {
                //objSql.Rollback();
            }
            return clientId;
        }

        //public int UpdateTaxInfo(RBNClientDTO client)
        //{
        //    var dal = new RBNClientDAL();
        //    return dal.UpdateTaxInfo(client);
        //}
    }
}
