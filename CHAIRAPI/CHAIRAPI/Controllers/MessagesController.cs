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
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        /// <summary>
        /// POST Method is used to insert messages into the database
        /// </summary>
        /// <param name="message">The message that will be stored in the database</param>
        [HttpPost]
        public IActionResult Post([FromBody] Message message)
        {
            string accept = Request.Headers["Content-Type"].ToString();
            if (accept != "application/json" && accept != "*/*")
                return StatusCode(415);
            else
            {
                //Save to the database and collect status message (specified in the handler)
                bool savedSuccessfully = MessagesHandler.saveNewMessage(message);

                //Return the status code depending on what happened when saving the user
                if (savedSuccessfully)
                    return StatusCode(201); //Created
                else
                    return StatusCode(500); //Internal Server Error
            }
        }

        /// <summary>
        /// GET Method is used to get all the messages in a conversation
        /// </summary>
        /// <param name="user1">The nickname of one of the users</param>
        /// <param name="user2">The nickname of one of the users</param>
        /// <returns></returns>
        [HttpGet("{user1}/with/{user2}")]
        public IActionResult GetConversation(string user1, string user2)
        {
            string accept = Request.Headers["Accept"].ToString();
            if (accept != "application/json" && accept != "*/*")
                return StatusCode(406); //Not Acceptable
            else if (Utilities.checkUsrClaimValidity(User, user1, user2))
            {
                List<Message> list = MessagesHandler.getConversationBetweenTwoUsers(user1, user2);

                if (list != null)
                    return Ok(list);
                else
                    return StatusCode(404); //Not Found
            }
            else
                return StatusCode(401); //Unauthorized
        }
    }
}