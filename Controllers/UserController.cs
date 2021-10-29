using AuthenticationApp.Interfaces;
using AuthenticationApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private string _controllerName = "User Controller";
        private IUserService _userService;
        private readonly IRecordProblems _problemRecorder;

        public UserController(IUserService userService, IRecordProblems problemRecorder)
        {
            _userService = userService;
            _problemRecorder = problemRecorder;
        }

        [Authorize]
        [HttpGet]
        [Route("[controller]/[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult Authenticated()
        {
            return Ok();
        }

        [HttpGet]
        [Route("[controller]/[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Authenticate([Required] string email, [Required] string password)
        {
            if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password))
            {
                try
                {
                    string securityToken = await _userService
                        .Authenticate(email, password);

                    if (string.IsNullOrWhiteSpace(securityToken))
                    {
                        return Unauthorized();
                    }

                    return Ok(securityToken);
                }
                catch (Exception exception)
                {
                    string action = "Signing in the user using email and password";

                    _problemRecorder.RecordProblem(_controllerName, action, exception);
                    return StatusCode(500);
                }
            }
            else
            {
                return BadRequest("Missing parameters");
            }
        }

        [HttpPost]
        [Route("Website/[controller]/[action]")]
        public async Task<ActionResult> Create([Required] string email, [Required] string password)
        {
            if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password))
            {
                try
                {
                    string result = await _userService.Create(email, password);
                    if (string.IsNullOrEmpty(result)) return Ok();

                    return Conflict(result);
                }
                catch (Exception exception)
                {
                    string action = "Failed to create user using email and password";

                    _problemRecorder.RecordProblem(_controllerName, action, exception);

                    return StatusCode(500);
                }
            }
            return BadRequest("Missing parameters");

        }

        [Authorize]
        [HttpGet]
        [Route("[controller]/[action]/Public")]
        public ActionResult Read([Required] string userId)
        {
            if (!string.IsNullOrWhiteSpace(userId))
            {
                try
                {
                    ApplicationUser user = _userService.Read(userId);
                    if (user == null)
                    {
                        return NotFound("User does not exist.");
                    }

                    PublicUserInfo publicUser = new PublicUserInfo(user.Id, user.DisplayName,
                        user.About, user.Age, user.Gender, user.DateCreated, user.LocationOfImage);

                    return Ok(publicUser);
                }
                catch (Exception exception)
                {
                    string action = "Reading individual user info";

                    _problemRecorder.RecordProblem(_controllerName, action, exception);

                    return StatusCode(500);
                }

            }
            return BadRequest();
        }

        [Authorize]
        [HttpGet]
        [Route("[controller]/[Action]/All")]
        public ActionResult Read()
        {
            try
            {

                IQueryable<ApplicationUser> users = _userService.Read();
                if (users.Count() < 0) return NoContent();

                return Ok(users);

            }
            catch (Exception exception)
            {
                string action = "Reading multiple users info";

                _problemRecorder.RecordProblem(_controllerName, action, exception);

                return StatusCode(500);
            }
        }

        [Authorize]
        [HttpPut]
        [Route("[controller]/[action]")]
        public async Task<ActionResult> Update([Required] string userId, [FromBody] UserInfoToUpdate userInfo)
        {
            if (!string.IsNullOrEmpty(userId) && ModelState.IsValid)
            {
                try
                {
                    string result = await _userService.Update(userId, userInfo);
                    if (string.IsNullOrEmpty(result))
                    {
                        return Ok();
                    }

                    return Conflict(result);
                }
                catch (Exception exception)
                {
                    string action = "Updating user info";

                    _problemRecorder.RecordProblem(_controllerName, action, exception);

                    return StatusCode(500);
                }
            }
            return BadRequest();
        }

        [Authorize]
        [HttpDelete]
        [Route("[controller]/[action]")]
        public async Task<ActionResult> Delete([Required] string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                try
                {
                    string result = await _userService.Delete(userId);
                    if (string.IsNullOrEmpty(result))
                    {
                        return Ok();
                    }
                    return Conflict(result);
                }
                catch (Exception exception)
                {
                    string action = "Removing a user";

                    _problemRecorder.RecordProblem(_controllerName, action, exception);
                    return StatusCode(500);

                }
            }
            return BadRequest();
        }


    }
}
