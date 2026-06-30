using BAL.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentacBillingJob
{
    public class Program
    {
        static void Main(string[] args)
        {
            var subscription = new SubscriptionService();
            subscription.GenrateBill(DateTime.Now);
        }
    }
}
