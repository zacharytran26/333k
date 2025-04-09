using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tran_Zachary_HW5.Models
{
    public class Order
    {
        // Correcting constant naming and scope
        public const decimal TaxRate = 0.0825m;

        [Key]
        public int OrderID { get; set; }

        public int OrderNumber { get; set; }

        public DateTime OrderDate { get; set; }
        [Display(Name = " Order Notes")]
        public string OrderNotes { get; set; }

        public decimal OrderSubtotal => OrderDetails?.Sum(od => od.ExtendedPrice) ?? 0;

        // Read-only property for Sales Tax
        public decimal SalesTax => OrderSubtotal * 0.0825m;

        // Read-only property for Order Total
        public decimal OrderTotal => OrderSubtotal + SalesTax;

        // Corrected type for OrderDetails to List<OrderDetail>
        public List<OrderDetail> OrderDetails { get; } = new List<OrderDetail>();
        public AppUser AppUser { get; set; }
    }
}

