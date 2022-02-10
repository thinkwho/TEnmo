using System;
using System.Collections.Generic;
using System.Text;
using TenmoClient.Models;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Deserializers;
using RestSharp.Serialization;
using RestSharp.Serializers;

namespace TenmoClient
{
    public class TransferService
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

        ////////////////////////////////////////--PRINT--////////////////////////////////////////

        public void PrintAllUsersAndIds()
        {
            RestRequest request = new RestRequest(API_BASE_URL + "list");
            IRestResponse<List<User>> response = authClient.Get<List<User>>(request);
            foreach (User user in response.Data)
            {
                Console.WriteLine($"{user.UserId} | {user.Username}");
            }
        }

        public void PrintTransferHistoryList(int id)
        {
            AuthenticateUser();
            
            string url = API_BASE_URL;
            if (id != 0)
            {
                url += $"{id}/transfer/history";
            }
            RestRequest request = new RestRequest(url);
            IRestResponse<List<string>> response = authClient.Get<List<string>>(request);
            foreach(string line in response.Data)
            {
                Console.WriteLine(line);
            }
        }

        public void PrintTransferDetails(int id)
        {
            AuthenticateUser();
            
            string url = API_BASE_URL;
            if (id != 0)
            {
                url += $"transfer/transferdetails/{id}";
            }

            RestRequest request = new RestRequest(url);
            IRestResponse<List<string>> response = authClient.Get<List<string>>(request);
            foreach (string line in response.Data)
            {
                Console.WriteLine(line);
            }
        }
        
        public void PrintPendingRequests(int id)
        {
            AuthenticateUser();

            string url = API_BASE_URL;
            if (id != 0)
            {
                url += $"transfer/pendingrequestslist/{id}";
            }

            RestRequest request = new RestRequest(url);
            IRestResponse<List<string>> response = authClient.Get<List<string>>(request);
            foreach (string line in response.Data)
            {
                Console.WriteLine(line);
            }
        }

        ////////////////////////////////////////--ACTION--////////////////////////////////////////

        public Transfer ExecuteTransfer(Transfer transfer)
        {
            AuthenticateUser();
            
            RestRequest request = new RestRequest(API_BASE_URL + "transfer");
            request.AddJsonBody(transfer);

            IRestResponse<Transfer> response = authClient.Put<Transfer>(request);
            return response.Data;
        }
        
        public int AddTransfer(Transfer transfer)
        {
            AuthenticateUser();
            
            RestRequest request = new RestRequest(API_BASE_URL + "transfer");
            request.AddJsonBody(transfer);

            IRestResponse<int> response = authClient.Post<int>(request);
            return response.Data;
        }

        public List<int> validIdsCheck()
        {
            AuthenticateUser();
            RestRequest request = new RestRequest(API_BASE_URL + "listOfUserIds");
            
            IRestResponse<List<int>> response = authClient.Get<List<int>>(request);
            return response.Data;
        }

        public List<int> ValidTransfersCheck()
        {
            AuthenticateUser();
            RestRequest request = new RestRequest(API_BASE_URL + "listOfTransferIds");

            IRestResponse<List<int>> response = authClient.Get<List<int>>(request);
            return response.Data;
        }
    }
}
