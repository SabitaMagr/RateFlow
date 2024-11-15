
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Assignment.Models;
using Assignment.Data;


namespace Assignment.Framework
{
    public class WebWorkContext: IWorkContext
    {
        #region Const


        private readonly IAuthenticationService _authenticationService;
        private const string UserCookieName = "User";

        #endregion

        #region Fields

        private readonly HttpContext _httpContext;


        private User _cachedUser;

        #endregion

        public WebWorkContext(HttpContext httpContext, IAuthenticationService authenticationService)
        {
            this._httpContext = httpContext;
            this._authenticationService = authenticationService;

        }

        #region Utilities

        //protected virtual HttpCookie GetCustomerCookie()
        //{
        //    if (_httpContext == null || _httpContext.Request == null)
        //        return null;

        //    return _httpContext.Request.Cookies[UserCookieName];
        //}

        protected virtual void SetCustomerCookie(Guid customerGuid)
        {
            if (_httpContext != null && _httpContext.Response != null)
            {
                //var cookie = new HttpCookie(UserCookieName);
                //cookie.HttpOnly = true;
                //cookie.Value = customerGuid.ToString();
                //if (customerGuid == Guid.Empty)
                //{
                //    cookie.Expires = DateTime.Now.AddMonths(-1);
                //}
                //else
                //{
                //    int cookieExpires = 24 * 365; //TODO make configurable
                //    cookie.Expires = DateTime.Now.AddHours(cookieExpires);
                //}

                //_httpContext.Response.Cookies.Remove(UserCookieName);
                //_httpContext.Response.Cookies.Add(cookie);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current customer
        /// </summary>
        public virtual User CurrentUserinformation
        {
            get
            {
                if (_cachedUser != null)
                    return _cachedUser;

                User user = null;


                //registered user
                if (user == null)
                {
                    //user = _authenticationService.GetAuthenticatedCustomer();
                }


                //On first load no userGuid found which cause exception , Must resolved this issue which only cause sometime - aaku
                SetCustomerCookie(user.UserGuid);
                _cachedUser = user;


                return _cachedUser;
            }
            set
            {
                SetCustomerCookie(value.UserGuid);
                _cachedUser = value;
            }
        }



        #endregion
    }
}
