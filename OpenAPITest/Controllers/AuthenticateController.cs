using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OpenAPITest.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        [HttpPost]
        public IActionResult Authenticate()
        {
            try
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("thisisasecretkey@123"));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var jwtSecurityToken = new JwtSecurityToken(
                                            issuer: "ABCXYZ",
                                            audience: "http://localhost:7088",
                                            claims: new List<Claim>(),
                                            expires: DateTime.Now.AddMinutes(10),
                                            signingCredentials: signinCredentials
                                        );

                var key = System.Text.Encoding.ASCII.GetBytes("64A63153-11C1-4919-9133-EFAF99A9B456");

                var JWToken = new JwtSecurityToken(
                                            issuer: "true", 
                                            audience: "true", 
                                            claims: new List<Claim>(),
                                            notBefore: new DateTimeOffset(DateTime.Now).DateTime, 
                                            expires: new DateTimeOffset(DateTime.UtcNow.AddDays(1)).DateTime, 
                                            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), 
                                            SecurityAlgorithms.HmacSha256));

                //UserToken.Token = new JwtSecurityTokenHandler().WriteToken(JWToken);

                return Ok(new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken));
            }
            catch
            {
                return BadRequest ("An error occurred in generating the token");
            }
            return Unauthorized();
        }
    }
}


