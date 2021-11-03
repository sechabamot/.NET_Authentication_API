using AuthenticationApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationApp.Interfaces
{
    public interface IUserService
    {
        Task<string> Create(string email, string password);
        ApplicationUser Read(string email);
        IQueryable<ApplicationUser> Read();
        Task<string> Update(string email, UserInfoToUpdateRequestModel userInfo);
        Task<string> Delete(string email);
        Task<string> Authenticate(string email, string password);

    }
}
