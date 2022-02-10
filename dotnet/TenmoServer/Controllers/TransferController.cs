using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;
using TenmoServer.DAO;
using Microsoft.AspNetCore.Authorization;

namespace TenmoServer.Controllers
{
    [Route("/")]
    [ApiController]
    [Authorize]
    public class TransferController : ControllerBase
    {
        private IUserDao userDao;

        public TransferController(IUserDao _userDao)
        {
            userDao = _userDao;
        }

        /////////////////////////////////////////--DISPLAY--/////////////////////////////////////////

        
        [HttpGet("list")]
        [AllowAnonymous]
        public List<User> DisplayListForTransfer()
        {
            return userDao.GetUsersForTransfer();
        }

        [HttpGet("{id}/transfer/history")]
        public List<string> DisplayTransferHistory(int id)
        {
            return userDao.DisplayTransferHistory(id);
        }

        [HttpGet("transfer/transferdetails/{id}")]
        public List<string> DisplayTransferDetails(int id)
        {
            return userDao.SelectTransferIdToView(id);
        }

        [HttpGet("transfer/pendingrequestslist/{id}")]
        public List<string> DisplayPendingRequests(int id)
        {
            return userDao.DisplayPendingRequests(id);
        }

        /////////////////////////////////////////--ACTIONS--/////////////////////////////////////////
        
        [HttpPut("transfer")]
        public int TransferFunds(Transfer transfer)
        {
            int result = userDao.TransferFunds(transfer);
            return result;
        }

        [HttpPost("transfer")]
        public int AddSendersTransfer(Transfer transfer)
        {
            int result = userDao.AddSuccessfulTransfer(transfer);
            return result;
        }

        [HttpGet("listOfUserIds")]
        public List<int> GetUserIds()
        {
            return userDao.GetUsersIds();
        }

        [HttpGet("listOfTransferIds")]
        public List<int> GetTransferIds()
        {
            return userDao.GetTransferIds();
        }
    }
}
