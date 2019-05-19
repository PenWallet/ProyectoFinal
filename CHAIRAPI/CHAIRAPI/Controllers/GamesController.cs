using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class GamesController : ControllerBase
    {
        /// <summary>
        /// GET Method to get a game given its name
        /// </summary>
        /// <param name="name">The name to look for</param>
        [HttpGet("{name}")]
        public IActionResult Get(string name)
        {
            string accept = Request.Headers["Accept"].ToString();
            if (accept != "application/json" && accept != "*/*")
                return StatusCode(406); //Not Acceptable
            else
            {
                Game game = GamesHandler.searchGameByName(name);

                if (game != null)
                    return Ok(game);
                else
                    return StatusCode(404);
            }
        }

        /// <summary>
        /// GET Method to get all games in the database
        /// </summary>
        [HttpGet]
        public IActionResult GetAll()
        {
            string accept = Request.Headers["Accept"].ToString();
            if (accept != "application/json" && accept != "*/*")
                return StatusCode(406); //Not Acceptable
            else
            {
                List<Game> games = GamesHandler.getAllGames();

                if (games != null)
                    return Ok(games);
                else
                    return StatusCode(500);
            }
        }
    }
}