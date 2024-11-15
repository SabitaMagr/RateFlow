using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Assignment.Models
{
    public class LoginModel
    {        
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string MobileNumber { get; set; }
        public string Status { get; set; }
        public string Email { get; set; }
    }
    public class LoginCredModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
