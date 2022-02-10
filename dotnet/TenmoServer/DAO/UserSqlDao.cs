using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TenmoServer.Models;
using TenmoServer.Security;
using TenmoServer.Security.Models;

namespace TenmoServer.DAO
{
    public class UserSqlDao : IUserDao
    {
        private readonly string connectionString;
        const decimal startingBalance = 1000;

        public UserSqlDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public User GetUser(string username)
        {
            User returnUser = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT user_id, username, password_hash, salt FROM users WHERE username = @username", conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        returnUser = GetUserFromReader(reader);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return returnUser;
        }
        public List<User> GetUsers()
        {
            List<User> returnUsers = new List<User>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT user_id, username, password_hash, salt FROM users", conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        User u = GetUserFromReader(reader);
                        returnUsers.Add(u);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return returnUsers;
        }

        public User AddUser(string username, string password)
        {
            IPasswordHasher passwordHasher = new PasswordHasher();
            PasswordHash hash = passwordHasher.ComputeHash(password);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("INSERT INTO users (username, password_hash, salt) VALUES (@username, @password_hash, @salt)", conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password_hash", hash.Password);
                    cmd.Parameters.AddWithValue("@salt", hash.Salt);
                    cmd.ExecuteNonQuery();

                    cmd = new SqlCommand("SELECT @@IDENTITY", conn);
                    int userId = Convert.ToInt32(cmd.ExecuteScalar());

                    cmd = new SqlCommand("INSERT INTO accounts (user_id, balance) VALUES (@userid, @startBalance)", conn);
                    cmd.Parameters.AddWithValue("@userid", userId);
                    cmd.Parameters.AddWithValue("@startBalance", startingBalance);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return GetUser(username);
        }

        public User GetUserFromReader(SqlDataReader reader)
        {
            User u = new User()
            {
                UserId = Convert.ToInt32(reader["user_id"]),
                Username = Convert.ToString(reader["username"]),
                PasswordHash = Convert.ToString(reader["password_hash"]),
                Salt = Convert.ToString(reader["salt"]),
            };

            return u;
        }


        /////////////////////////////////////////////////////////--WE CREATED--///////////////////////////////////////////////////////////////////////

        public decimal GetAccountBalance(int id)
        {
            decimal loginUserBalance = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT balance " +
                                                    "FROM accounts A " +
                                                    "JOIN users U ON A.user_id = U.user_id " +
                                                    "WHERE U.user_id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {

                        loginUserBalance = Convert.ToDecimal(reader["balance"]);

                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return loginUserBalance;
        }

        public List<string> DisplayTransferHistory(int loginUserId)
        {

            List<string> displayTransferHistory = new List<string>();
            
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand displayToCmd = new SqlCommand("SELECT T.transfer_id, U.username, T.amount " +
                                                            "FROM users U " +
                                                            "JOIN accounts A ON U.user_id = A.user_id " +
                                                            "JOIN transfers T ON A.account_id = T.account_to " + 
                                                            "WHERE T.account_from = " +
                                                            "(SELECT account_id " +
                                                            "FROM accounts A " +
                                                            "WHERE A.user_id = @loginUserId)", conn);

                    displayToCmd.Parameters.AddWithValue("@loginUserId", loginUserId);
                    SqlDataReader displayToReader = displayToCmd.ExecuteReader();
                    while (displayToReader.Read())
                    {
                        string transferId = Convert.ToString(displayToReader["transfer_id"]);
                        string username = Convert.ToString(displayToReader["username"]);
                        string amount = Convert.ToString(displayToReader["amount"]);
                        string complete = (transferId + " | To: " + username + " | " + amount);
                        displayTransferHistory.Add(complete);
                    }
                    displayToReader.Close();


                    SqlCommand displayFromCmd = new SqlCommand("SELECT T.transfer_id, U.username, T.amount " +
                                                                "FROM users U " +
                                                                "JOIN accounts A ON U.user_id = A.user_id " +
                                                                "JOIN transfers T ON A.account_id = T.account_from " +
                                                                "WHERE T.account_to = " +
                                                                "(SELECT account_id " +
                                                                "FROM accounts A " +
                                                                "WHERE A.user_id = @loginUserId)", conn);

                    displayFromCmd.Parameters.AddWithValue("@loginUserId", loginUserId);
                    SqlDataReader displayFromReader = displayFromCmd.ExecuteReader();
                    while (displayFromReader.Read())
                    {
                        string transferId = Convert.ToString(displayFromReader["transfer_id"]);
                        string username = Convert.ToString(displayFromReader["username"]);
                        string amount = Convert.ToString(displayFromReader["amount"]);
                        string complete = (transferId + " | From: " + username + " | " + amount);
                        displayTransferHistory.Add(complete);
                    }
                    displayFromReader.Close();

                    /////--Request From List--/////

                    

                }
            }
            catch (SqlException)
            {
                throw;
            }

            return displayTransferHistory;

        }
 
        public List<string> DisplayPendingRequests(int id)
        {
            List<string> displayPendingRequests = new List<string>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand displayPendingRequestsCmd = new SqlCommand("SELECT T.transfer_id, U.username, T.amount " +
                                                                          "FROM users U " +
                                                                          "JOIN accounts A ON U.user_id = A.user_id " +
                                                                          "JOIN transfers T ON A.account_id = T.account_to " +
                                                                          "WHERE transfer_type_id = 1 " +
                                                                          "AND T.account_from = " +
                                                                          "(SELECT account_id " +
                                                                          "FROM accounts A " +
                                                                          "WHERE A.user_id = @loginUserId)", conn);

                    displayPendingRequestsCmd.Parameters.AddWithValue("@loginUserId", id);
                    SqlDataReader displayPendingRequestsReader = displayPendingRequestsCmd.ExecuteReader();

                    while (displayPendingRequestsReader.Read())
                    {
                        string transferId = Convert.ToString(displayPendingRequestsReader["transfer_id"]);
                        string username = Convert.ToString(displayPendingRequestsReader["username"]);
                        string amount = Convert.ToString(displayPendingRequestsReader["amount"]);
                        string complete = (transferId + " | To: " + username + " | " + amount);
                        displayPendingRequests.Add(complete);
                    }

                }
            }
            catch (SqlException)
            {
                throw;
            }

            return displayPendingRequests;

        }

        public List<User> GetUsersForTransfer()
        {
            List<User> returnUsersForTransfer = new List<User>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT user_id, username FROM users", conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        User u = new User();
                        u.UserId = Convert.ToInt32(reader["user_id"]);
                        u.Username = Convert.ToString(reader["username"]);
                        returnUsersForTransfer.Add(u);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return returnUsersForTransfer;
        }
        
        public int TransferFunds(Transfer transfer)
        {

            int rowsAffected = 0;
            
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {

                    conn.Open();

                    //send money
                    SqlCommand sendCmd = new SqlCommand("UPDATE accounts " +
                                                        "SET accounts.balance -= @moneyToSend " +
                                                        "FROM accounts A " +
                                                        "JOIN users U ON A.user_id = U.user_id " +
                                                        "WHERE U.user_id = @senderId", conn);
                    sendCmd.Parameters.AddWithValue("@moneyToSend", transfer.Amount);
                    sendCmd.Parameters.AddWithValue("@senderId", transfer.Account_From);
                    rowsAffected += sendCmd.ExecuteNonQuery();

                    //receive money
                    SqlCommand receiveCmd = new SqlCommand("UPDATE accounts " +
                                                            "SET accounts.balance += @moneyToReceive " +
                                                            "FROM accounts A " +
                                                            "JOIN users U ON A.user_id = U.user_id " +
                                                            "WHERE U.user_id = @recipientId", conn);
                    receiveCmd.Parameters.AddWithValue("@moneyToReceive", transfer.Amount);
                    receiveCmd.Parameters.AddWithValue("@recipientId", transfer.Account_To);
                    rowsAffected += receiveCmd.ExecuteNonQuery();
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return rowsAffected;

        }

        public int AddSuccessfulTransfer(Transfer transfer)
        {
            int rowsAffected = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {

                    conn.Open();

                    SqlCommand insertSendersTransferCmd = new SqlCommand("INSERT INTO transfers " +
                                                                          "(transfer_type_id, transfer_status_id, " +
                                                                          "account_from, account_to, amount) " +
                                                                          "VALUES(@transfer_type_id, @transfer_status_id, " +
                                                                          "(SELECT account_id FROM accounts WHERE user_id = @account_from), " +
                                                                          "(SELECT account_id FROM accounts WHERE user_id = @account_to), @amount)", conn);
                    insertSendersTransferCmd.Parameters.AddWithValue("@transfer_type_id", 2);
                    insertSendersTransferCmd.Parameters.AddWithValue("@transfer_status_id", transfer.Transfer_Status_Id);
                    insertSendersTransferCmd.Parameters.AddWithValue("@account_from", transfer.Account_From);
                    insertSendersTransferCmd.Parameters.AddWithValue("@account_to", transfer.Account_To);
                    insertSendersTransferCmd.Parameters.AddWithValue("@amount", transfer.Amount);
                    rowsAffected += insertSendersTransferCmd.ExecuteNonQuery();
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return rowsAffected;
        }
        public List<string> SelectTransferIdToView(int transferId)
        {

            List<string> transferDetails = new List<string>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {

                    conn.Open();
                    
                    SqlCommand getFromCmd = new SqlCommand("SELECT username FROM users U " +
                                                           "JOIN accounts A ON U.user_id = A.user_id " +
                                                           "JOIN transfers T ON A.account_id = T.account_from " +
                                                           "WHERE T.transfer_id = @transferId", conn);
                    getFromCmd.Parameters.AddWithValue("@transferId", transferId);
                    SqlDataReader getFromReader = getFromCmd.ExecuteReader();
                    if (getFromReader.Read())
                    {
                        transferDetails.Add("From: " + Convert.ToString(getFromReader["username"]));
                    }
                    getFromReader.Close();


                    SqlCommand getToCmd = new SqlCommand("SELECT username FROM users U " +
                                                         "JOIN accounts A ON U.user_id = A.user_id " +
                                                         "JOIN transfers T ON A.account_id = T.account_to " +
                                                         "WHERE T.transfer_id = @transferId", conn);
                    getToCmd.Parameters.AddWithValue("@transferId", transferId);
                    SqlDataReader getToReader = getToCmd.ExecuteReader();
                    if (getToReader.Read())
                    {
                        transferDetails.Add("To: " + Convert.ToString(getToReader["username"]));
                    }
                    getToReader.Close();


                    SqlCommand getTransferTypeDescCmd = new SqlCommand("SELECT transfer_type_desc FROM transfer_types TT " +
                                                                       "JOIN transfers T ON TT.transfer_type_id = T.transfer_type_id " +
                                                                       "WHERE T.transfer_id = @transferId", conn);
                    getTransferTypeDescCmd.Parameters.AddWithValue("@transferId", transferId);
                    SqlDataReader getTransferTypeDescReader = getTransferTypeDescCmd.ExecuteReader();
                    if (getTransferTypeDescReader.Read())
                    {
                        transferDetails.Add(Convert.ToString("Type: " + getTransferTypeDescReader["transfer_type_desc"]));
                    }
                    getTransferTypeDescReader.Close();


                    SqlCommand getTransferStatusDescCmd = new SqlCommand("SELECT transfer_status_desc FROM transfer_statuses TS " +
                                                                         "JOIN transfers T ON TS.transfer_status_id = T.transfer_type_id " +
                                                                         "WHERE T.transfer_id = @transferId", conn);
                    getTransferStatusDescCmd.Parameters.AddWithValue("@transferId", transferId);
                    SqlDataReader getTransferStatusDescReader = getTransferStatusDescCmd.ExecuteReader();
                    if (getTransferStatusDescReader.Read())
                    {
                        transferDetails.Add("Status: " + Convert.ToString(getTransferStatusDescReader["transfer_status_desc"]));
                    }
                    getTransferStatusDescReader.Close();


                    SqlCommand getAmountCmd = new SqlCommand("SELECT amount FROM transfers WHERE transfer_id = @transferId", conn);
                    getAmountCmd.Parameters.AddWithValue("@transferId", transferId);
                    SqlDataReader getAmountReader = getAmountCmd.ExecuteReader();
                    if (getAmountReader.Read())
                    {
                        transferDetails.Add("Amount: " + Convert.ToString(getAmountReader["amount"]));
                    }
                    getAmountReader.Close();

                }
            }
            catch (SqlException)
            {
                throw;
            }

            return transferDetails;

        }
        public List<int> GetUsersIds()
        {
            List<int> usersIds = new List<int>();

            try
            {

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT user_id FROM users" ,conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["user_id"]);
                        usersIds.Add(id);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return usersIds;
        }

        public List<int> GetTransferIds()
        {
            List<int> transferIds = new List<int>();

            try
            {

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT transfer_id FROM transfers", conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["transfer_id"]);
                        transferIds.Add(id);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return transferIds;
        }
    }
}
