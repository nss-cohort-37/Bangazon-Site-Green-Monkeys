using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon.Models.PaymentTypeViewModels
{
    public class PaymentTypeViewModel
    {
        public IEnumerable<PaymentType> PaymentTypes { get; set; }
    }
}
