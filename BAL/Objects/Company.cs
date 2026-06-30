using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;
using BAL.DAL;
using BAL.Exceptions;
using Razorpay.Api.Errors;
using ErrorCodes = BAL.Exceptions.ErrorCodes;
namespace BAL.Objects
{
    public class Company : CompanyDTO
    {
        public Company(int companyId)
            : base(companyId)
        {

        }
        public Company() { }
        public int Save()
        {
            CompanyDAL dal = new CompanyDAL();
            return dal.Save(this);

        }

        public static List<CompanyDTO> GetAll(int rbnClientId, string query = "")
        {
            return CompanyDAL.GetAll(rbnClientId, query);
        }

        public static string GetRentacApiKeyByCompanyId(int companyId)
        {
            return CompanyDAL.GetRentacApiKeyByCompanyId(companyId);
        }

        public CompanyDTO GetDetails()
        {
            CompanyDAL dal = new CompanyDAL();
            return dal.GetDetails(this.CompanyId);
        }
        public bool DeActivate(bool activate)
        {
            CompanyDAL dal = new CompanyDAL();
            return dal.ActivateDeActivate(activate, this.CompanyId);

        }
        public bool UpdateGSTDetails(CompanyDTO dto)
        {
            if (String.IsNullOrEmpty(dto.GSTNo))
            {
                throw new UDFException("Incorrect GST Number", ErrorCodes.INVALID_GST);
            }
            if (String.IsNullOrEmpty(dto.LegalName))
            {
                throw new UDFException("Incorrect LegalName", ErrorCodes.INVALID_LEGALNAME);
            }
            if (String.IsNullOrEmpty(dto.TradeName))
            {
                throw new UDFException("Incorrect Trade Name", ErrorCodes.INVALID_TRADE_NAME);
            }
            if (String.IsNullOrEmpty(dto.GSTRegistrationDate))
            {
                throw new UDFException("Incorrect GST Registration Date", ErrorCodes.INVALID_GST_REG_DATE);
            }
            CompanyDAL dal = new CompanyDAL();
            return dal.UpdateGSTDetails(dto);
        }
        public bool UpdateEInvoiceEnabled(CompanyDTO dto)
        {
            if (dto.EInvoiceStartDate.Year < 2000)
            {
                throw new UDFException("Incorrect Start Date", ErrorCodes.INVALID_EINVOICE_STARTDATE);
            }

            CompanyDAL dal = new CompanyDAL();
            return dal.UpdateEInvoiceEnabled(dto);
        }

        public bool UpdateIRPCredentials(CompanyDTO dto)
        {
            if (String.IsNullOrEmpty(dto.IRPUserName))
            {
                throw new UDFException("Incorrect IRPUserName", ErrorCodes.INVALID_IRP_USERNAME);
            }
            if (String.IsNullOrEmpty(dto.IRPPassword))
            {
                throw new UDFException("Incorrect IRPPassword", ErrorCodes.INVALID_IRP_USERPASSWORD);
            }

            CompanyDAL dal = new CompanyDAL();
            return dal.UpdateIPRUserCrentials(dto);
        }
        public bool UpdateIPRToken(CompanyDTO dto)
        {
            CompanyDAL dal = new CompanyDAL();
            return dal.UpdateIPRToken(dto);
        }

        public bool UpdateEwayBillCreds(CompanyDTO dto)

        {
            CompanyDAL dal = new CompanyDAL();
            return dal.UpdateEwayBillCreds(dto);
        }

        public CompanyDTO ApiKeyExists(string apiKey)
        {
            CompanyDAL dal = new CompanyDAL();
            return dal.ApiKeyExists(apiKey);
        }
    }
}
