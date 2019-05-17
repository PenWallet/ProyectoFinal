using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CHAIRAPI.Utils;
using CHAIRAPI_DAL.Handlers;
using CHAIRAPI_Entidades.Complex;
using CHAIRAPI_Entidades.Persistent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CHAIRAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class UserGamesInfo : ControllerBase
    {
        /// <summary>
        /// GET Method to get all games a player plays
        /// </summary>
        [HttpGet("{user}")]
        public IActionResult GetPlayingUsers(string user)
        {
            string accept = Request.Headers["Accept"].ToString();
            if (accept != "application/json" && accept != "*/*")
                return StatusCode(406); //Not Acceptable
            else
            {
                List<GameBeingPlayed> games = AdminHandler.getGamesBeingPlayed();

                if (games != null)
                    return Ok(games);
                else
                    return StatusCode(500);
            }
        }
    }
}