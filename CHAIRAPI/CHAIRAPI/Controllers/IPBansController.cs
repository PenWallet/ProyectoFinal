using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CHAIRAPI.Utils;
using CHAIRAPI_DAL.Handlers;
using CHAIRAPI_Entities.Persistent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CHAIRAPI.Controllers
{
    [Route("admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrator")]
    public class IPBansController : ControllerBase
    {
        /// <summary>
        /// POST Method is used to create new IP Bans
        /// </summary>
        /// <param name="ipBan">The IPBan to be stored in the database</param>
        [HttpPost]
        public IActionResult Post([FromBody] IPBan ipBan)
        {
            string accept = Request.Headers["Content-Type"].ToString();
            if (accept != "application/json" && accept != "*/*")
                return StatusCode(415);
            else
            {
                //Save to the database and collect status message (specified in the handler)
                int saveStatus = IPBansHandler.saveNewIPBan(ipBan);

                //Return the status code depending on what happened when saving the user
                if (saveStatus == 1)
                    return StatusCode(201); //Created
                else if (saveStatus == 0)
                    return StatusCode(409); //Conflict
                else
                    return StatusCode(500); //Internal Server Error
            }
        }

        /// <summary>
        /// GET Method to get an IPBan given an IP
        /// </summary>
        /// <param name="IP">The IP to look for</param>
        [HttpGet("{IP}")]
        public IActionResult Get(string IP)
        {
            string accept = Request.Headers["Accept"].ToString();
            if (accept != "application/json" && accept != "*/*")
                return StatusCode(406); //Not Acceptable
            else
            {
                IPBan ipBan = IPBansHandler.searchIPBanByIP(IP);

                if (ipBan != null)
                    return Ok(ipBan);
                else
                    return StatusCode(404);
            }
        }
    }
}