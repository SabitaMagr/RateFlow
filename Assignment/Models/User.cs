using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Assignment.Models
{
    public class User
    {
        public User()
        {
            UserGuid = Guid.NewGuid();
        }
        public Guid UserGuid { get; set; }
        [Required(ErrorMessage = "Please enter username")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Please enter password")]
        [DataType(DataType.Password)]
        [Display(Name = "password")]
        public string Password { get; set; }
        
    }
    
}
