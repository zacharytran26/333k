using System;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tran_Zachary_HW5.Models
{
	public class Supplier
	{
		[Key]
		public Int32 SupplierID { get; set; }

		[Display(Name = "Supplier Name")]
		public String SupplierName { get; set; }

		public String Email { get; set; }

		[Display(Name = "Phone Number")]
		public String PhoneNumber { get; set; }

		public List<Product>? Products { get; set; }


	}

}

