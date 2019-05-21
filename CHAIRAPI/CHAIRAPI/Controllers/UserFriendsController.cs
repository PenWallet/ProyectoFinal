using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CHAIRAPI.Utils;
using CHAIRAPI_DAL.Handlers;
using CHAIRAPI_Entidades.Complex;
using CHAIRAPI_Entities.Persistent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CHAIRAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class UserFriendsController : ControllerBase
    {
        /// <summary>
        /// POST Method is used to create a new friendship between users and store it in the database
        /// </summary>
        /// <param name="relationship">The relationship that will be stored in the database</param>
        [HttpPost("{user1}/befriends/{user2}")]
        public IActionResult Post(string user1, string user2)
        {
            if (Utilities.checkUsrClaimValidity(User, user1, user2))
            {
                //Save to the database and collect status message (specified in the handler)
                int saveStatus = UserFriendsHandler.saveNewRelationship(user1, user2);

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

        /// <summary>
        /// PATCH Method is used to update the relationship status to accepted by setting the 'acceptedRequestDate' to the current time
        /// </summary>
        /// <param name="user1">One of the users who accepts the relationship</param>
        /// <param name="user2">One of the users who accepts the relationship</param>
        [HttpPatch("{user1}/accept/{user2}")]
        public IActionResult Patch(string user1, string user2)
        {
            string accept = Request.Headers["Content-Type"].ToString();
            if (accept != "application/json" && accept != "*/*")
                return StatusCode(415);
            else if (Utilities.checkUsrClaimValidity(User, user1, user2))
            {
                //Save to the database and collect status message (specified in the handler)
                int updateStatus = UserFriendsHandler.acceptRelationship(user1, user2);

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

        /// <summary>
        /// GET Method to get a relationship given the two related users
        /// </summary>
        /// <param name="user1">One of the users from the relationship</param>
        /// <param name="user2">One of the users from the relationship</param>
        [HttpGet("{user1}/isfriends/{user2}")]
        public IActionResult Get(string user1, string user2)
        {
            string accept = Request.Headers["Accept"].ToString();
            if (accept != "application/json" && accept != "*/*")
                return StatusCode(406); //Not Acceptable
            else if (Utilities.checkUsrClaimValidity(User, user1, user2))
            {
                UserFriends relationship = UserFriendsHandler.searchRelationshipByUsers(user1, user2);

                if (relationship != null)
                    return Ok(relationship);
                else
                    return StatusCode(404); //Not Found
            }
            else
                return StatusCode(401); //Unauthorized

        }

        /// <summary>
        /// GET Method to get all relationships of a given user
        /// </summary>
        /// <param name="nickname">The nickname of the user</param>
        [HttpGet("{nickname}")]
        public IActionResult Get(string nickname)
        {
            string accept = Request.Headers["Accept"].ToString();
            if (accept != "application/json" && accept != "*/*")
                return StatusCode(406); //Not Acceptable
            else if (Utilities.checkUsrClaimValidity(User, nickname))
            {
                List<UserForFriendList> list = UserForFriendListHandler.searchFriends(nickname);

                if (list != null)
                    return Ok(list);
                else
                    return StatusCode(404); //Not Found
            }
            else
                return StatusCode(401); //Unauthorized

        }

        /// <summary>
        /// DELETE Method is used to break a friendship :(
        /// </summary>
        /// <param name="user1">One of the users from the relationship</param>
        /// <param name="user2">One of the users from the relationship</param>
        [HttpDelete("{user1}/breakwith/{user2}")]
        public IActionResult Delete(string user1, string user2)
        {
            if (Utilities.checkUsrClaimValidity(User, user1, user2))
            {
                int deleteStatus = UserFriendsHandler.deleteRelationship(user1, user2);

                if (deleteStatus == 1)
                    return StatusCode(204); //No Content
                else if (deleteStatus == 0)
                    return StatusCode(404); //Not Found
                else
                    return StatusCode(500); //Internal Server Error
            }
            else
                return StatusCode(401); //Unauthorized
            
        }
    }
}