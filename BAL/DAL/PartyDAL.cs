using BAL.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DAL
{
    public class PartyDAL
    {
        public List<GRNDTO> PartyReturns(string partyMobileNo, DateTime from, DateTime to)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@partyMobile", DbType.String, ParameterDirection.Input, 0, partyMobileNo);
            objSql.AddParameter("@from", DbType.DateTime, ParameterDirection.Input, 0, from);
            objSql.AddParameter("@to", DbType.DateTime, ParameterDirection.Input, 0, to);

            return objSql.ContructList<GRNDTO>(objSql.ExecuteDataSet(PARTY_RETURND_MATERIAL));
        }
        public List<WorkOrderItemDTO> PartyReceived(string partyMobileNo, DateTime from, DateTime to)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@partyMobile", DbType.String, ParameterDirection.Input, 0, partyMobileNo);
            objSql.AddParameter("@from", DbType.DateTime, ParameterDirection.Input, 0, from);
            objSql.AddParameter("@to", DbType.DateTime, ParameterDirection.Input, 0, to);

            return objSql.ContructList<WorkOrderItemDTO>(objSql.ExecuteDataSet(PARTY_RECEIVED));
        }
        public List<PartyStockBalanceDTO> PartyStockBalance( string partyPhone, int companyId)
        {
            SQL objSql = new SQL();

            objSql.AddParameter("@partyPhone", DbType.String, ParameterDirection.Input, 0, partyPhone);

            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
     
           
            return objSql.ContructList<PartyStockBalanceDTO>(objSql.ExecuteDataSet(PARTY_STOCK_BALANCE));

            
        }
        const string PARTY_RECEIVED = "p_partyReceiveMaterial_sel";
        const string PARTY_RETURND_MATERIAL = "p_partyReturnedMaterial_sel";
        const string PARTY_STOCK_BALANCE = "p_partystockBalance";

    }
}
