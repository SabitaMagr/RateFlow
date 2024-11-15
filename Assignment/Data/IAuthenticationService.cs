using Assignment.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Assignment.Data
{
    public interface IAuthenticationService
    {
        //Task SignInAsync(User user, bool createPersistentCookie);
        Task SignOutAsync();
        User GetAuthenticatedCustomer();
        //IEnumerable<User> SignIn(string UserName, string Password, string company, string branch);
        //void UpdateAuthenticatedCustomer(User user);
    }
}
