using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon.Models.OrderViewModels
{
    public class OrderEditViewModel
    {
        public int OrderId { get; set; }
        public Order order { get; set; }
        public int PaymentTypeId { get; set; }

        public List<SelectListItem> paymentTypes { get; set; }


    }
}
