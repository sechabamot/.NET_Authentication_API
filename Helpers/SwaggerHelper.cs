using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationApp.Helpers
{
    public static class SwaggerHelper
    {
        /// <summary>
        /// Generates the swagger jwt security scheme object
        /// </summary>
        /// <returns>The swagger jwt security scheme</returns>
        public static OpenApiSecurityScheme GetSwaggerTokenSecurityScheme()
        {
            var scheme = new OpenApiSecurityScheme
            {
                Name = "JWT Authentication",
                Description = "Enter JWT Bearer token **_only_**",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Id = "bearer",
                    Type = ReferenceType.SecurityScheme
                }
            };
            return scheme;
        }

        /// <summary>
        /// Generates the swagger security scheme object
        /// </summary>
        /// <returns>The swagger security scheme</returns>
        public static OpenApiSecurityRequirement GetSwaggerJwtSecurityRequirement()
        {
            var req = new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme()
                    {
                        Reference = new OpenApiReference() {Type = ReferenceType.SecurityScheme, Id = "Bearer"}
                    },
                    new[] {"readAccess", "writeAccess"}
                }
            };
            return req;
        }



    }

    public class SecurityRequirementsOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Get Authorize attribute
            var attributes = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                                    .Union(context.MethodInfo.GetCustomAttributes(true))
                                    .OfType<AuthorizeAttribute>();

            if (attributes != null && attributes.Count() > 0)
            {
                var attr = attributes.ToList()[0];

                // Add what should be show inside the security section
                IList<string> securityInfos = new List<string>();
                securityInfos.Add($"{nameof(AuthorizeAttribute.Policy)}:{attr.Policy}");
                securityInfos.Add($"{nameof(AuthorizeAttribute.Roles)}:{attr.Roles}");
                securityInfos.Add($"{nameof(AuthorizeAttribute.AuthenticationSchemes)}:{attr.AuthenticationSchemes}");

                switch (attr.AuthenticationSchemes)
                {

                    case var p when p == JwtBearerDefaults.AuthenticationScheme: // = JwtBearerDefaults.AuthenticationScheme
                    default:
                        operation.Security = new List<OpenApiSecurityRequirement>()
                        {
                            new OpenApiSecurityRequirement()
                            {
                                {
                                    new OpenApiSecurityScheme
                                    {
                                        Reference = new OpenApiReference
                                        {
                                            Id = "bearer", // Must fit the defined Id of SecurityDefinition in global configuration
                                            Type = ReferenceType.SecurityScheme
                                        }
                                    },
                                    securityInfos
                                }
                            }
                        };
                        break;
                }
            }
            else
            {
                operation.Security.Clear();
            }
        }
    }
}
