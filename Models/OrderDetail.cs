using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Tran_Zachary_HW5.Models
{
	public class OrderDetail
	{
		[Key]
		public Int32 OrderDetailID { get; set; }

		[Required]
        [Range(1, 1000, ErrorMessage = "The quantity must be a positive number.")]
        public int Quantity { get; set; }

		public decimal Price { get; set; }
		public decimal ExtendedPrice { get; set; }
        public Order? Order { get; set; }
        public Product? Product { get; set; }
    }
}

