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
    public class UserGamesInfo : ControllerBase
    {
        /// <summary>
        /// GET Method to get all games a user plays, along with all the information about each game and which friends play them
        /// </summary>
        /// <param name="nickname">The user's nickname</param>
        [HttpGet("{nickname}")]
        public IActionResult GetMyGames(string nickname)
        {
            string accept = Request.Headers["Accept"].ToString();
            if (accept != "application/json" && accept != "*/*")
                return StatusCode(406); //Not Acceptable
            else if (Utilities.checkUsrClaimValidity(User, nickname))
            {
                List<UserGamesWithGameAndFriends> games = UserGamesInfoHandler.searchAllMyGamesAndFriendsWhoPlayThem(nickname);

                if (games != null)
                    return Ok(games);
                else
                    return StatusCode(500);
            }
            else
                return StatusCode(401); //Unauthorized
        }
    }
}