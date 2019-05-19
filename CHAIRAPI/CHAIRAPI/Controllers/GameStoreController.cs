using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class GameStoreController : ControllerBase
    {
        /// <summary>
        /// GET Method to get a game given its name
        /// </summary>
        /// <param name="name">The name to look for</param>
        [HttpGet("{game}/{nickname}")]
        public IActionResult Get(string game, string nickname)
        {
            string accept = Request.Headers["Accept"].ToString();
            if (accept != "application/json" && accept != "*/*")
                return StatusCode(406); //Not Acceptable
            else
            {
                if (Utilities.checkUsrClaimValidity(User, nickname))
                {
                    GameStore gameStore = GameStoreHandler.searchGameByNameAndUser(game, nickname);

                    if (gameStore != null)
                        return Ok(gameStore);
                    else
                        return StatusCode(404); //Not Found
                }
                else
                    return StatusCode(401); //Unauthorized
            }
        }
    }
}