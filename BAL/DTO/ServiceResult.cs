using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }

        public static ServiceResult<T> SuccessResult(T data, string message = "")
        {
            return new ServiceResult<T> { Success = true, Data = data, Message = message };
        }

        public static ServiceResult<T> Failure(string message)
            => new ServiceResult<T> { Success = false, Message = message };
    }
}
