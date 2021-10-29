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
        ApplicationUser Read(string userId);
        IQueryable<ApplicationUser> Read();
        Task<string> Update(string userId, UserInfoToUpdate userInfo);
        Task<string> Delete(string userId);
        Task<string> Authenticate(string email, string password);

    }
}
