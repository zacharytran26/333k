using System;
using System.ComponentModel.DataAnnotations;
namespace Tran_Zachary_HW5.Models
{
    public enum ProductType
    {
        Hot,
        Cold,
        Packaged,
        Drink,
        Other
    }

    public class Product
    {
        [Key] // This will make ProductID an auto-generated primary key
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        public string? Name { get; set; }

        [Display(Name = "Desciption (optional)")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [DataType(DataType.Currency)] // This will ensure the display of currency format (e.g., $ and 2 decimal places)
        [Range(0.01, 10000.00, ErrorMessage = "Price must be between $0.01 and $10,000")]
        public decimal Price { get; set; }

        [Required]
        public ProductType Type { get; set; }

        public List<Supplier>? Suppliers { get; set; }
        public List<OrderDetail>? OrderDetails { get; set; }
    }

   
}

