using System;
using System.Collections.Generic;
using System.Text;
using TenmoClient.Models;
using RestSharp;
using RestSharp.Authenticators;

namespace TenmoClient
{
    public class BalanceService
    {

        private string API_BASE_URL = "https://localhost:44315/";
        private RestClient authClient = new RestClient();

        /////////////////////////////////////--AUTHENTICATE--/////////////////////////////////////

        private void AuthenticateUser()
        {
            if (UserService.IsLoggedIn())
            {
                authClient.Authenticator = new JwtAuthenticator(UserService.GetToken());
            }
        }

        ////////////////////////////////////////--ACTION--////////////////////////////////////////

        public decimal GetAccountBalance(int id)
        {
            AuthenticateUser();
            
            string url = API_BASE_URL;

            if (id != 0)
            {
                url += $"{id}/balance";
            }

            RestRequest request = new RestRequest(url);
            IRestResponse<decimal> response = authClient.Get<decimal>(request);

            return response.Data;
        }

    }
}
