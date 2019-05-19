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
    [Authorize(Roles = "Administrator")]
    public class AdminController : ControllerBase
    {
        /// <summary>
        /// PATCH Method is used here to set the user's online status to true
        /// </summary>
        /// <param name="nickname">The user's nickname</param>
        [HttpPatch("ban")]
        public IActionResult Ban([FromBody]User user)
        {
            if (Utilities.checkUserHasRequiredFieldsToBan(user))
            {
                int updateStatus = UsersHandler.banUser(user);

                if (updateStatus == 1)
                    return StatusCode(204); //No Content
                else if (updateStatus == 0)
                    return StatusCode(404); //Not Found
                else
                    return StatusCode(500); //Internal Server Error
            }
            else
                return StatusCode(400); //Bad Request
            

        }

        /// <summary>
        /// PATCH Method is used here to set the user's online status to true
        /// </summary>
        /// <param name="nickname">The user's nickname</param>
        [HttpPatch("pardon/{nickname}")]
        public IActionResult Pardon(string nickname)
        {
            int updateStatus = UsersHandler.pardonUser(nickname);

            if (updateStatus == 1)
                return StatusCode(204); //No Content
            else if (updateStatus == 0)
                return StatusCode(404); //Not Found
            else
                return StatusCode(500); //Internal Server Error
        }

        /// <summary>
        /// POST Method is used to add new games to the database
        /// </summary>
        /// <param name="game">The game to be stored in the database</param>
        [HttpPost("games")]
        public IActionResult Post([FromBody] Game game)
        {
            string accept = Request.Headers["Content-Type"].ToString();
            if (accept != "application/json" && accept != "*/*")
                return StatusCode(415);
            else
            {
                //Save to the database and collect status message (specified in the handler)
                int saveStatus = GamesHandler.saveNewGame(game);

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
        /// PUT Method is used to update a game
        /// </summary>
        /// <param name="game">The game to update</param>
        [HttpPut("games")]
        public IActionResult Put([FromBody] Game game)
        {
            int updateStatus = GamesHandler.updateGame(game);

            if (updateStatus == 1)
                return StatusCode(204); //No Content
            else if (updateStatus == 0)
                return StatusCode(404); //Not Found
            else
                return StatusCode(500); //Internal Server Error
        }

        /// <summary>
        /// GET Method to get all games in the database
        /// </summary>
        [HttpGet("playingusers")]
        public IActionResult GetPlayingUsers()
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