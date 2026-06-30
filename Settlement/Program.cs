using BAL.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Settlement
{
    public class Program
    {
        static void Main(string[] args)
        {
            var paymentService = new PaymentService();
            paymentService.SettlePayments(1, 1007);
        }
    }
}
