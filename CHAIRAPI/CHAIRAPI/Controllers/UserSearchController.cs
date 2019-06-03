using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CHAIRAPI.Responses;
using CHAIRAPI.Utils;
using CHAIRAPI_DAL.Handlers;
using CHAIRAPI_Entities.Complex;
using CHAIRAPI_Entities.Persistent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CHAIRAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class UserSearchController : ControllerBase
    {
        /// <summary>
        /// GET Method used to get the users whose nicknames match the 'search' parameter
        /// </summary>
        /// <param name="s">The search parameter</param>
        [HttpGet]
        public IActionResult Get([FromQuery(Name = "s")] string search)
        {
            string nickname = User.Claims.ToList().Single(x => x.Type == "usr").Value;
            List<UserSearch> users = UserSearchHandler.searchUsersFromString(search, nickname);

            if (users != null)
                return Ok(users);
            else
                return StatusCode(500); //Internal Server Error
        }
    }
}