using BAL.DAL;
using BAL.DTO;
using Omu.ValueInjecter;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Objects
{
    public class RentacPackageService
    {
        RentacPackageDAL paymentDal = new RentacPackageDAL();
        public List<PackageDTO> GetAllPackages()
        {
            var packages = paymentDal.GetActivePackages();

            var packageDTO = new List<PackageDTO>();
            packageDTO = packages.Select(o => new PackageDTO().InjectFrom(o)).Cast<PackageDTO>().ToList();
            return packageDTO;
        }
        public RentacPackage PackageById(int packageId)
        {
            return paymentDal.PackageById(packageId);
        }
        public int PurchasePackage(ClientPackageDTO clientPackageDTO)
        {
            var clientPackage = new ClientPackage();
            clientPackage.InjectFrom(clientPackageDTO);
            return paymentDal.PurchasePackage(clientPackage);
        }
        public ClientPackageDTO ClientPackageSel(int rbClientId)
        {
            var clientPackage = paymentDal.ClientPackageSel(rbClientId);
            var clientPackageDTO = new ClientPackageDTO();
            if (clientPackage != null)
            {
                if (clientPackageDTO.PackageId == 99)
                {
                    clientPackageDTO.IsDemo = true;
                }
                clientPackageDTO.InjectFrom(clientPackage);
            }
            return clientPackageDTO;
        }
    }
}
