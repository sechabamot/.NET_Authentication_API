using AuthenticationApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationApp.Data
{
    public static class Constants
    {
        public static UserRole[] UserRoles = { UserRole.Master, UserRole.Admin, UserRole.User };
    }
}
