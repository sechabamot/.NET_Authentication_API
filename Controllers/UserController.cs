using AuthenticationApp.Interfaces;
using AuthenticationApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationApp.Controllers
{
    [Route("api")]
    [ApiController]
    [SwaggerTag("Create, read, update and delete users")]
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
        [SwaggerResponse(201, "Authentication Successful.")]
        [SwaggerResponse(401, "Unauthorised.")]
        [SwaggerOperation(
            Description = "Checks if the user is authenticated on the platform."
        )]
        public ActionResult Authenticated()
        {
            return Ok();
        }

        [HttpPost]
        [Route("[controller]/[action]")]
        [SwaggerResponse(201, "Authentication Successful.")]
        [SwaggerResponse(400, "Bad request, check your input.")]
        [SwaggerResponse(401, "Wrong emai or password.")]
        [SwaggerResponse(500, "Internal servor error.")]
        [SwaggerOperation(
            Description = "Authenticate the user using an email and password. Returns a JWT token used to authenticate the user."
        )]
        public async Task<ActionResult> Authenticate([FromBody] AuthenicateRequestModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string securityToken = await _userService
                        .Authenticate(model.Email, model.Password);

                    if (string.IsNullOrEmpty(securityToken))
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
        [Route("[controller]/[action]")]
        [SwaggerResponse(201, "Registration Successful.")]
        [SwaggerResponse(400, "Bad request, registration info.")]
        [SwaggerResponse(500, "Internal servor error.")]
        [SwaggerResponse(409, "Cannot register user.")]
        [SwaggerOperation(
            Description = "Registers a new user without an assigned role onto the identity store."
        )]
        public async Task<ActionResult> Register([FromBody, 
            SwaggerRequestBody("The user registration info", Required = true)] RegisterUserRequestModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    
                    string result = await _userService.Create(model.Email, model.Password);
                    if (string.IsNullOrEmpty(result)) return Ok();

                    return Conflict(result);
                }
                catch (Exception exception)
                {
                    string action = "Create user using email and password";
                    _problemRecorder.RecordProblem(_controllerName, action, exception);

                    return StatusCode(500);
                }
            }
            return BadRequest("Missing parameters");

        }

        [Authorize]
        [HttpGet]
        [Route("[controller]")]
        [SwaggerResponse(500, "Internal servor error.")]
        [SwaggerResponse(404, "Could not find user.")]
        [SwaggerResponse(401, "Unauthorised.")]
        [SwaggerOperation(
            Description = "Return the public user info for the user with the provided email."
        )]
        public ActionResult Return([Required] string email)
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                try
                {
                    ApplicationUser user = _userService.Read(email);
                    if (user == null)
                    {
                        return NotFound("User does not exist");
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
        [Route("[controller]/All")]
        [SwaggerResponse(500, "Internal servor error.")]
        [SwaggerResponse(204, "No users.")]
        [SwaggerResponse(401, "Unauthorised.")]
        [SwaggerOperation(
            Description = "Return the internal user info for all the user on the data store."
        )]
        public ActionResult Return()
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
        [Route("[controller]/[action]/User")]
        [SwaggerResponse(500, "Internal servor error.")]
        [SwaggerResponse(404, "Could not find user.")]
        [SwaggerResponse(401, "Unauthorised.")]
        [SwaggerResponse(409, "Cannot update user.")]
        [SwaggerOperation(
            Description = "Update user info for the user with the provided email."
        )]
        public async Task<ActionResult> Update([Required] string email, [FromBody] UserInfoToUpdateRequestModel userInfo)
        {
            if (!string.IsNullOrEmpty(email) && ModelState.IsValid)
            {
                try
                {
                    string result = await _userService.Update(email, userInfo);
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
        [SwaggerResponse(500, "Internal servor error.")]
        [SwaggerResponse(404, "Could not find user.")]
        [SwaggerResponse(401, "Unauthorised.")]
        [SwaggerResponse(409, "Cannot delete user.")]
        [SwaggerOperation(
            Description = "Delete user with the provided email."
        )]
        public async Task<ActionResult> Delete([Required] string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                try
                {
                    string result = await _userService.Delete(email);
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
