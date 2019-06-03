using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CHAIRAPI.Responses;
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
    public class UserProfileController : ControllerBase
    {
        /// <summary>
        /// GET Method from UsersControllers to get the basic profile information of an user TODO: Cambiar este método si se implementan los perfiles privados
        /// </summary>
        /// <param name="nickname">The user's nickname</param>
        [HttpGet("{nickname}")]
        public IActionResult Get(string nickname)
        {
            UserProfile user = UserProfileHandler.searchUserProfileInformation(nickname);

            if (user != null)
                return Ok(user);
            else
                return StatusCode(404); //Not Found
        }
    }
}