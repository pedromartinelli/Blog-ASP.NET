using Blog.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Blog.Extensions
{
    public static class RoleClaimsExtension
    {
        public static IEnumerable<Claim> GetClaims(this User user)
        {
            var result = new List<Claim>
            {
                new(ClaimTypes.Email, user.Email),
            };

            result.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role.Slug)));

            return result;
        }

        public static string GetEmailClaim(this HttpContext httpContext)
            => httpContext.User.Claims.Single(x => x.Type == ClaimTypes.Email).Value;
    }
}
