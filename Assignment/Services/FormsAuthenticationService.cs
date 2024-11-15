using Assignment.Models;
using Assignment.Data;
using System.Security.Claims;
using System.Text;


namespace Assignment.Services
{
    public class FormsAuthenticationService : IAuthenticationService
    {
        #region Fields

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TimeSpan _expirationTimeSpan;
        private readonly AssignmentCoreEntity _objectEntity;

        private User _cachedUser;

        #endregion

        #region Ctor

        public FormsAuthenticationService(IHttpContextAccessor httpContextAccessor, AssignmentCoreEntity objectEntity)
        {
            _httpContextAccessor = httpContextAccessor;
            _expirationTimeSpan = TimeSpan.FromMinutes(30); // set your session expiration time
            _objectEntity = objectEntity;
        }

        #endregion

        #region Utilities

        protected virtual User GetAuthenticatedCustomerFromTicket(string ticket)
        {
            if (string.IsNullOrEmpty(ticket))
                throw new ArgumentNullException("ticket");

            var userString = Encoding.UTF8.GetString(Convert.FromBase64String(ticket));
            var user = userString.Split('#');
            var currentUser = new User();

            if (user.Length > 0)
            {
                currentUser.UserName = user[0];
                currentUser.Password = user[1];

                // Handle any additional properties or logic here if needed
            }

            return currentUser;
        }

        #endregion

        #region Methods

        //public async Task SignInAsync(User user, bool createPersistentCookie)
        //{
        //    var now = DateTime.UtcNow;
        //    var userdata = $"{user.User_id}#{user.company_code}#{user.branch_code}#{user.startFiscalYear}#{user.endfiscalyear}#{user.branch_name}#{user.company_name}#{user.login_code}#{user.LOGIN_EDESC}";

        //    var claims = new[] {
        //        new Claim(ClaimTypes.Name, user.login_code),
        //        new Claim("UserData", Convert.ToBase64String(Encoding.UTF8.GetBytes(userdata)))
        //    };

        //    var claimsIdentity = new ClaimsIdentity(claims, "FormsAuthentication");

        //    //var authProperties = new AuthenticationProperties
        //    //{
        //    //    IsPersistent = createPersistentCookie,
        //    //    ExpiresUtc = now.Add(_expirationTimeSpan)
        //    //};

        //    ////await _httpContextAccessor.HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity), authProperties);

        //    _cachedUser = user;
        //}

        public async Task SignOutAsync()
        {
            _cachedUser = null;
            //await _httpContextAccessor.HttpContext.SignOutAsync("Cookies");
        }

        public User GetAuthenticatedCustomer()
        {
            if (_cachedUser != null)
                return _cachedUser;

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null || httpContext.User == null || !httpContext.User.Identity.IsAuthenticated)
                return null;

            var userData = httpContext.User.Claims.FirstOrDefault(c => c.Type == "UserData")?.Value;
            if (userData == null)
                return null;

            var decodedUserData = Encoding.UTF8.GetString(Convert.FromBase64String(userData));
            var user = GetAuthenticatedCustomerFromTicket(decodedUserData);

            _cachedUser = user;
            return _cachedUser;
        }

        #endregion
    }
}
