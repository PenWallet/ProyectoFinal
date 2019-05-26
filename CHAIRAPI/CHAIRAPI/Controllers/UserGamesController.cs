using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CHAIRAPI.Utils;
using CHAIRAPI_DAL.Handlers;
using CHAIRAPI_Entities.Persistent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CHAIRAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class UserGamesController : ControllerBase
    {
        /// <summary>
        /// POST Method is used to create new relationship between users and games
        /// </summary>
        /// <param name="relationship">The relationship that will be stored in the database</param>
        [HttpPost]
        public IActionResult Post([FromBody] UserGames relationship)
        {
            string accept = Request.Headers["Content-Type"].ToString();
            if (accept != "application/json" && accept != "*/*")
                return StatusCode(415);
            else if (Utilities.checkUserGamesHasRequiredFieldsToPostOrPut(relationship))
            {
                if (Utilities.checkUsrClaimValidity(User, relationship.user))
                {
                    //Save to the database and collect status message (specified in the handler)
                    int saveStatus = UserGamesHandler.saveNewRelationship(relationship);

                    //Return the status code depending on what happened when saving the user
                    if (saveStatus == 1)
                        return StatusCode(201); //Created
                    else if (saveStatus == 0)
                        return StatusCode(404); //Not Found
                    else if (saveStatus == -1)
                        return StatusCode(409); //Conflict
                    else
                        return StatusCode(500); //Internal Server Error
                }
                else
                    return StatusCode(401); //Unauthorized
            }
            else
                return StatusCode(400); //Bad Request
        }

        /// <summary>
        /// PUT Method is used to update a relationship
        /// </summary>
        /// <param name="relationship">The relationship to update</param>
        [HttpPut]
        public IActionResult Put([FromBody] UserGames relationship)
        {
            string accept = Request.Headers["Content-Type"].ToString();
            if (accept != "application/json" && accept != "*/*")
                return StatusCode(415);
            else if (Utilities.checkUserGamesHasRequiredFieldsToPostOrPut(relationship))
            {
                if (Utilities.checkUsrClaimValidity(User, relationship.user))
                {
                    //Save to the database and collect status message (specified in the handler)
                    int updateStatus = UserGamesHandler.updateRelationship(relationship);

                    //Return the status code depending on what happened when saving the user
                    if (updateStatus == 1)
                        return StatusCode(204); //Created
                    else if (updateStatus == 0)
                        return StatusCode(404); //Not Found
                    else
                        return StatusCode(500); //Internal Server Error
                }
                else
                    return StatusCode(401); //Unauthorized
            }
            else
                return StatusCode(400); //Bad Request
        }

        /// <summary>
        /// GET Method to get a game given his nickname
        /// </summary>
        /// <param name="nickname">The user to look for</param>
        /// <param name="game">The game to look for</param>
        [HttpGet("{nickname}/plays/{game}")]
        public IActionResult Get(string nickname, string game)
        {
            string accept = Request.Headers["Accept"].ToString();
            if (accept != "application/json" && accept != "*/*")
                return StatusCode(406); //Not Acceptable
            else if (Utilities.checkUsrClaimValidity(User, nickname))
            {
                UserGames relationship = UserGamesHandler.searchRelationshipByUserAndGame(nickname, game);

                if (relationship != null)
                    return Ok(relationship);
                else
                    return StatusCode(404); //Not Found
            }
            else
                return StatusCode(401); //Unauthorized
            
        }

        /// <summary>
        /// GET Method to get all games an user plays given his nickname
        /// </summary>
        /// <param name="user">The user to look for</param>
        [HttpGet("{nickname}")]
        public IActionResult Get(string nickname)
        {
            string accept = Request.Headers["Accept"].ToString();
            if (accept != "application/json" && accept != "*/*")
                return StatusCode(406); //Not Acceptable
            else if (Utilities.checkUsrClaimValidity(User, nickname))
            {
                List<UserGames> list = UserGamesHandler.searchRelationshipsByUser(nickname);

                if (list != null)
                    return Ok(list);
                else
                    return StatusCode(404); //Not Found
            }
            else
                return StatusCode(401); //Unauthorized

        }

        /// <summary>
        /// PATCH Method is used here to change the playing status to playing given a name and a game
        /// </summary>
        /// <param name="nickname">The user's nickname</param>
        [HttpPatch("{user}/playing/{game}")]
        public IActionResult Playing(string user, string game)
        {
            if (Utilities.checkUsrClaimValidity(User, user))
            {
                int updateStatus = UserGamesHandler.updatePlayingStatus(user, game, true, 0);
                if (updateStatus == 1)
                    return StatusCode(204); //No Content
                else if (updateStatus == 0)
                    return StatusCode(404); //Not Found
                else
                    return StatusCode(500); //Internal Server Error
            }
            else
                return StatusCode(401); //Unauthorized

        }

        /// <summary>
        /// PATCH Method is used here to change the playing status to playing given a name and a game and to add to the total
        /// of time the user has played that game
        /// </summary>
        /// <param name="nickname">The user's nickname</param>
        [HttpPatch("{user}/notplaying/{game}")]
        public IActionResult NotPlaying(string user, string game, [FromQuery(Name = "s")] string secondsToAdd)
        {
            if (Utilities.checkUsrClaimValidity(User, user))
            {
                int.TryParse(secondsToAdd, out int secondsToAddInt);

                int updateStatus = UserGamesHandler.updatePlayingStatus(user, game, false, secondsToAddInt);
                if (updateStatus >= 1)
                    return StatusCode(204); //No Content
                else if (updateStatus == 0)
                    return StatusCode(404); //Not Found
                else
                    return StatusCode(500); //Internal Server Error
            }
            else
                return StatusCode(401); //Unauthorized

        }

    }
}