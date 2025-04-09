using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace Tran_Zachary_HW5.Models
{
	public class ProductViewModel
	{
		public Product? Product { get; set; }
		public List<int> SelectedSupplierIds { get; set; } = new List<int>();
		public MultiSelectList? Suppliers { get; set; }

	}
}

