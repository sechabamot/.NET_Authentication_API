using AuthenticationApp.Data;
using AuthenticationApp.Interfaces;
using AuthenticationApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationApp.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly string _secrete;
        private ApplicationDbContext _dbContext;
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;

        public UserService(ApplicationDbContext dbContext, 
            IConfiguration configuration, 
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager)
        {
            _secrete = configuration["Application:Secret"];
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<string> Create(string email, string password)
        {
            if (await _userManager.FindByEmailAsync(email) != null)
            {
                return "Sorry, email already taken";
            }

            IdentityResult result = await _userManager.CreateAsync(new ApplicationUser(email), password);
            if(result.Succeeded == true)
            {
                //TODO: Send user a welcome + confirmation email.
                return string.Empty;
            }

            return "Something went wrong and it's not your fault";
        }
        public ApplicationUser Read(string email)
        {
            return _userManager.Users.FirstOrDefault(u => u.Email.Equals(email));
        }
        public IQueryable<ApplicationUser> Read()
        {
            return _userManager.Users;
        }
        public async Task<string> Update(string email, UserInfoToUpdateRequestModel info)
        {
            var user =  await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                IdentityResult update = await _userManager.UpdateAsync(ApplicationUser.UpdateUsersInformation(user, info));
                if (update.Succeeded)
                {
                    return string.Empty;
                }
                return "Something went wrong and it's not your fault";
            }
            return "Cannot update a user that does not exist";
        }
        public async Task<string> Delete(string email)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(email);
            if (user != null )
            {
                IdentityResult delete = await _userManager.DeleteAsync(user);
                if (delete.Succeeded)
                {
                    return string.Empty;
                }

                return "Something went wrong and it's not your fault";
            }
            return "Cannot delete a user that does not exist";
        }
        public async Task<string> Authenticate(string email, string password) 
        {

            SignInResult signIn = await _signInManager.PasswordSignInAsync(email, password, false, false);
            if (signIn.Succeeded)
            {
                return ApplicationUser.CreateJWTAuthenticationToken(email, _secrete);
            }

            return string.Empty;
        }

    }
}
