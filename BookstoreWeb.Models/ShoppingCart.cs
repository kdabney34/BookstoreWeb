using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreWeb.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; } // [Key] is understood by EFC if this line is seen. automatically assigns it as the tables primary key
        public int ProductID { get; set; }
        [ForeignKey("ProductID")]
        [ValidateNever] // Model types are bound to their respective validations. disable them with this annotation
        public Product Product { get; set; }
        [Range(1, 1000, ErrorMessage = "Please enter a value between 1 and 1000")]
        public int Count { get; set; }
        public string ApplicationUserID { get; set; }
        [ForeignKey("ApplicationUserID")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
    }
}
