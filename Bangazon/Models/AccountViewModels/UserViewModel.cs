using Bangazon.Models.OrderViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon.Models.AccountViewModels
{
    public class UserViewModel
    {

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string StreetAddress { get; set; }

        public string PhoneNumber { get; set; }

        //public PaymentType PaymentType { get; set; }


        public virtual ICollection<PaymentType> PaymentTypes { get; set; }

        //public Order Order { get; set; }
        public virtual ICollection<Order> Orders { get; set; }

        //public IEnumerable<PaymentType> PaymentTypes { get; set; }
        //public Order Order { get; set; }
        //public IEnumerable<OrderLineItem> LineItems { get; set; }
    }
}
