using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Models
{
    public class Transfer
    {
        public readonly int Transfer_Id;
        public int Transfer_Type_Id { get; set; }
        public int Transfer_Status_Id { get; set; }

        public int Account_From { get; set; }
        public int Account_To { get; set; }
        public decimal Amount { get; set; }

        public Transfer()
        {

        }
        
        public Transfer(int transferTypeId, int transferStatusId, int senderId, int recipientId, decimal transferAmount)
        {
            Transfer_Type_Id = transferTypeId;
            Transfer_Status_Id = transferStatusId;
            Account_From = senderId;
            Account_To = recipientId;
            Amount = transferAmount;
        }

        

    }

    
}
