using System.Collections.Generic;

namespace Bangazon.Models.OrderViewModels
{
    public class OrderDetailViewModel
    {
        public Order Order { get; set; }
        public int OrderId { get; set; }

        public string PaymentTypeId { get; set; }

        public string PaymentType { get; set; }
        public IEnumerable<OrderLineItem> LineItems { get; set; }

        public double OrderTotalCost { get; set; }
    }
}