using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CHAIRAPI.Utils;
using CHAIRAPI_DAL.Handlers;
using CHAIRAPI_Entidades.Persistent;
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
        /// <param name="ipBan">The information that will be stored in the database about the user</param>
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

        /// <summary>
        /// PUT Method is used to update a ban
        /// </summary>
        /// <param name="ipBan">The IPBan to update</param>
        [HttpPut]
        public IActionResult Put([FromBody] IPBan ipBan)
        {
            int updateStatus = IPBansHandler.updateIPBan(ipBan);

            if (updateStatus == 1)
                return StatusCode(204); //No Content
            else if (updateStatus == 0)
                return StatusCode(404); //Not Found
            else
                return StatusCode(500); //Internal Server Error
        }

        /// <summary>
        /// DELETE Method is used to pardon a ban
        /// </summary>
        /// <param name="ip">The IP to delete</param>
        [HttpDelete("{IP}")]
        public IActionResult Delete(string ip)
        {
            int deleteStatus = IPBansHandler.deleteIPBan(ip);

            if (deleteStatus == 1)
                return StatusCode(204); //No Content
            else if (deleteStatus == 0)
                return StatusCode(404); //Not Found
            else
                return StatusCode(500); //Internal Server Error
        }
    }
}