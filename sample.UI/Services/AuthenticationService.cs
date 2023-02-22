﻿using Microsoft.Extensions.Logging;
using System;
using System.Security.Principal;
using System.Threading.Tasks;
using sample.WebServices;
using Uno.Extensions;

namespace sample.Services
{
    public sealed class AuthenticationService 
        : SingletonBase<AuthenticationService>
    {
        private AuthenticationService()
        {
        }

        public event EventHandler LoggedIn;
        public event EventHandler LoggedOut;
        public event EventHandler LoginFailed;

        public async Task<bool> LoginUser(string userName, string password)
        {
            var result = false;
            try
            {
                var api = new IdentityApi(
                    "https://localhost:6001", 
                    await TokenService.Instance.GetAccessTokenAsync());

                var validUser = await api.ValidateUser(userName, password);
                if (validUser)
                {
                    var authUser = await api.GetAuthenticatedUser(userName);
                    if (authUser != null)
                    {
                        this.CurrentPrincipal = authUser;
                        LoggedIn?.Invoke(this, EventArgs.Empty);
                        result = true;
                    }
                }
                else
                {
                    LoginFailed?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (Exception e)
            {
                this.Log().LogError(e.Message);
                LoginFailed?.Invoke(this, EventArgs.Empty);
            }

            return result;
        }

        public void Logout()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            this.CurrentPrincipal = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            LoggedOut?.Invoke(this, EventArgs.Empty);
        }

        public IPrincipal CurrentPrincipal { get; private set; }
    }
}
