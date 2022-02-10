using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Models
{
    class Prompts
    {

        static BalanceService balanceService = new BalanceService();
        static TransferService transferService = new TransferService();
        
        public static void SendTeBucksPrompt()
        {
            Console.WriteLine("Please input ID of user you would like to send to: ");
            int recipientId = int.Parse(Console.ReadLine());
            int noValidIds = 1;
            foreach (int id in transferService.validIdsCheck())
            {
                if (recipientId == id)
                {
                    noValidIds--;
                    Console.WriteLine("How much money would you like to send?: ");
                    decimal moneyToTransfer = decimal.Parse(Console.ReadLine());
                    if (balanceService.GetAccountBalance(UserService.GetUserId()) >= moneyToTransfer)
                    {
                        Transfer newTransfer = new Transfer(2, 2, UserService.GetUserId(), recipientId, moneyToTransfer);
                        transferService.ExecuteTransfer(newTransfer);
                        transferService.AddTransfer(newTransfer);
                        Console.WriteLine("Success!");
                    }
                    else
                    {
                        Console.WriteLine("Insufficient funds. Please try again.");
                    }
                }
            }
            if (noValidIds == 1)
            {
                Console.WriteLine("Did not input valid ID.");
                SendTeBucksPrompt();
            }
        }
        public static void ViewPastTransfersPrompt()
        {
            Console.WriteLine("Input Transfer ID of a transfer you would like to see details of. To exit, input 0");
            int transferId = int.Parse(Console.ReadLine());
            int noValidTransferIds = 1;
            foreach(int id in transferService.ValidTransfersCheck())
            {
                if (transferId == id)
                {
                   noValidTransferIds--;
                   transferService.PrintTransferDetails(transferId);
                }
            }
            if(noValidTransferIds == 1)
            {
                Console.WriteLine("Please input valid transfer ID.");
                ViewPastTransfersPrompt();
            }
        }
    }
}
