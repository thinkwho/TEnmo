using System;
using System.Collections.Generic;
using TenmoClient.Models;

namespace TenmoClient
{
    class Program
    {   
        private static readonly BalanceService balanceService = new BalanceService();
        private static readonly TransferService transferService = new TransferService();
        private static readonly ConsoleService consoleService = new ConsoleService();
        private static readonly AuthService authService = new AuthService();

        static void Main(string[] args)
        {
            Run();
        }
        private static void Run()
        {
            while(true)
            {
                int loginRegister = -1;
                while (loginRegister != 1 && loginRegister != 2)
                {
                    Console.WriteLine("Welcome to TEnmo!");
                    Console.WriteLine("1: Login");
                    Console.WriteLine("2: Register");
                    Console.Write("Please choose an option: ");

                    if (!int.TryParse(Console.ReadLine(), out loginRegister))
                    {
                        Console.WriteLine("Invalid input. Please enter only a number.");
                    }
                    else if (loginRegister == 1)
                    {
                        while (!UserService.IsLoggedIn()) //will keep looping until user is logged in
                        {
                            LoginUser loginUser = consoleService.PromptForLogin();
                            ApiUser user = authService.Login(loginUser);
                            if (user != null)
                            {
                                UserService.SetLogin(user);
                            }
                        }
                    }
                    else if (loginRegister == 2)
                    {
                        bool isRegistered = false;
                        while (!isRegistered) //will keep looping until user is registered
                        {
                            LoginUser registerUser = consoleService.PromptForLogin();
                            isRegistered = authService.Register(registerUser);
                            if (isRegistered)
                            {
                                Console.WriteLine("");
                                Console.WriteLine("Registration successful. You can now log in.");
                                loginRegister = -1; //reset outer loop to allow choice for login
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid selection.");
                    }
                }

                MenuSelection();
            }
        }

        private static void MenuSelection()
        {
            
            int menuSelection = -1;
            while (menuSelection != 0)
            {
                Console.WriteLine("");
                Console.WriteLine("Welcome to TEnmo! Please make a selection: ");
                Console.WriteLine("1: View your current balance");
                Console.WriteLine("2: View your past transfers");
                Console.WriteLine("3: View your pending requests");
                Console.WriteLine("4: Send TE bucks");
                Console.WriteLine("5: Request TE bucks");
                Console.WriteLine("6: Log in as different user");
                Console.WriteLine("0: Exit");
                Console.WriteLine("---------");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out menuSelection))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (menuSelection == 1)
                {
                    Console.WriteLine("Your current balance is {0:C2}", balanceService.GetAccountBalance(UserService.GetUserId()));
                }
                else if (menuSelection == 2)
                {
                   
                    transferService.PrintTransferHistoryList(UserService.GetUserId());

                    Prompts.ViewPastTransfersPrompt();

                }
                else if (menuSelection == 3)
                {

                    transferService.PrintPendingRequests(UserService.GetUserId());

                    Console.WriteLine("Please enter transfer ID to approve/reject (0 to cancel): ");

                    int transferId = int.Parse(Console.ReadLine());

                    if(transferId != 0) //AND VALID USER ID
                    {
                        Console.WriteLine("1: Approve");
                        Console.WriteLine("2: Reject");
                        Console.WriteLine("0: Don't approve or reject");
                        Console.WriteLine("----------");
                        Console.WriteLine("Please choose an option: ");

                        int choice = int.Parse(Console.ReadLine());

                        if(choice == 1)
                        {

                        }
                        else if(choice == 2)
                        {

                        }                      
                    }
                   
                }
                else if (menuSelection == 4)
                {                
                    Console.WriteLine();

                    Console.WriteLine("IDs  | Usernames");

                    transferService.PrintAllUsersAndIds();
                    Prompts.SendTeBucksPrompt();
                }
                else if (menuSelection == 5)
                {
                    transferService.PrintAllUsersAndIds();

                    Console.WriteLine("Please input User Id of who you would like to request from: ");
                    int requestId = int.Parse(Console.ReadLine());

                    Console.WriteLine("Please input amount of TE bucks you would like to request: ");
                    decimal amountToRequest = decimal.Parse(Console.ReadLine());

                    Transfer TEnmoRequest = new Transfer(1, 1, UserService.GetUserId(), requestId, amountToRequest);

                    transferService.AddTransfer(TEnmoRequest);

                    Console.WriteLine("Request Sent!");
                }
                else if (menuSelection == 6)
                {
                    Console.WriteLine("");
                    UserService.SetLogin(new ApiUser()); //wipe out previous login info
                    Console.Clear();
                    menuSelection = 0;
                }
                else
                {
                    Console.WriteLine("Goodbye!");
                    Environment.Exit(0);
                }
            }
        }
    }
    
}
