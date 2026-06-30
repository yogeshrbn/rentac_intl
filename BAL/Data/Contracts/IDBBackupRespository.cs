using BAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Data.Contracts
{
    public interface IDBBackupRespository
    {
        Task<int> Create(BackupLog log);
        Task<int> UpdateProgress(BackupLog log);
        Task<IEnumerable<BackupLog>> List(BackupLog log);

    }
}
