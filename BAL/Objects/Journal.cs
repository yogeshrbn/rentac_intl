using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;
using BAL.DAL;
namespace BAL.Objects
{
    public class Journal : JournalDTO
    {

        public bool CreateEntry()
        {
            JournalDAL objDal = new JournalDAL();
            return objDal.CreateEntry(this);
        }
        public List<JournalDTO> GetSiteJournal()
        {
            JournalDAL objDal = new JournalDAL();
            return objDal.GetSiteJournal(this.SiteId);
        }
        public List<JournalDTO> GetJournal(string fromDate, string toDate)
        {
            JournalDAL objDal = new JournalDAL();
            return objDal.GetJournal(fromDate, toDate);
        }
    }
}
