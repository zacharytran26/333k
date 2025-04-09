using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

//TODO: Make this namespace match your project name
namespace Tran_Zachary_HW5.Models
{
    public class AppUser : IdentityUser
    {
        //TODO: Add custom user fields - first name is included as an example
        [Display(Name = "First Name")]
        public String FirstName { get; set; }
        [Display(Name = "Last Name")]
        public String LastName { get; set; }
        [Display(Name = "User Name:")]
        public String FullName
        {
            get { return FirstName + " " + LastName; }
        }
        //public String Email { get; set; }
        //[Display(Name ="Phone Number")]
        //public String PhoneNumber { get; set; }
        public List<AppUser>? AppUsers { get; set; }
        public List<Order>? Orders { get; set; }

    }

}
