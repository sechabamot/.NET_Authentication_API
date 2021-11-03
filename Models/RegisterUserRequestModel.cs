using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationApp.Models
{
    public class RegisterUserRequestModel
    {
        [Required]
        [EmailAddress]
        [SwaggerSchema("A valid email of the user")]
        public string Email { get; set; }

        [Required]
        [MinLength(8), MaxLength(16)]
        [SwaggerSchema("Password the user will use to access their account")]
        public string Password { get; set; }

    }
}
