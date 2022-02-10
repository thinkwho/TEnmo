using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.DAO;
using TenmoServer.Models;
using Microsoft.AspNetCore.Authorization;

namespace TenmoServer.Controllers
{
    [Route("/")]
    [ApiController]
    [Authorize]
    public class BalanceController : ControllerBase
    {
        private IUserDao userDao;

        public BalanceController(IUserDao _userDao)
        {
            userDao = _userDao;
        }
        
        [HttpGet("{id}/balance")]
        public ActionResult<decimal> GetAccountBalance(int id)
        {
            decimal accountBalance = userDao.GetAccountBalance(id);
            return Ok(accountBalance);
        }
    }
}
