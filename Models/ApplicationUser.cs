using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser(string email)
        {
            UserName = email;
            Email = email;
            DateCreated = DateTime.UtcNow;
            LocationOfImage = "";
            About = "The user has not written anything about themeselves yet.";
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string About { get; set; }
        public int Age { get; set; }
        public Gender Gender { get; set; }
        public DateTime DateCreated { get; set; }
        public string LocationOfImage { get; set; }


        #region Methods

        public static ApplicationUser UpdateUsersInformation(ApplicationUser user, UserInfoToUpdateRequestModel info)
        {
            user.FirstName = info.FirstName ?? user.FirstName ?? "";
            user.LastName = info.LastName ?? user.LastName ?? "";
            user.DisplayName = info.DisplayName;
            user.About = info.About ?? user.About;

            return user;
        }

        public static string CreateJWTAuthenticationToken(string email, string secret)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(secret);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, email)
                }),

                Expires = DateTime.UtcNow.AddDays(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }

        #endregion

    }

    public class PublicUserInfo
    {

        public PublicUserInfo(string userId, string displayName, string about, int age,
            Gender gender, DateTime joined, string avartarUrl)
        {
            UserId = userId;
            DisplayName = displayName;
            About = about;
            Age = age;
            Gender = gender;
            DateJoined = joined;
            AvatarFilePath = avartarUrl;
        }

        public string UserId { get; set; }

        public string DisplayName { get; set; }

        public string About { get; set; }

        public int Age { get; set; }

        public Gender Gender { get; set; }

        public DateTime DateJoined { get; set; }

        public string AvatarFilePath { get; set; }

    }

    public class UserInfoToUpdateRequestModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string About { get; set; }
    }
}
