using System.Collections.Generic;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface IUserDao
    {
        User GetUser(string username);
        User AddUser(string username, string password);
        List<User> GetUsers();

        //////////////////////////////////////////////////////////////--CREATED--////////////////////////////////////////////////////////////////////////

        decimal GetAccountBalance(int loginUserId);

        List<User> GetUsersForTransfer();

        public List<int> GetUsersIds();

        public List<int> GetTransferIds();

        List<string> DisplayTransferHistory(int loginUserId);

        List<string> DisplayPendingRequests(int id);

        int TransferFunds(Transfer transfer);

        public int AddSuccessfulTransfer(Transfer transfer);

        public List<string> SelectTransferIdToView(int transferId);

    }
}
