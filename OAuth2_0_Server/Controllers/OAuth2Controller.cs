using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OAuth2_0_Server.Controllers
{
    public class OAuth2Controller : Controller
    {
        public async Task<IActionResult> Authorize(
            string response_type,
            string client_id,
            string redirect_uri,
            string scope,
            string state) 
        {
            QueryBuilder query = new QueryBuilder();
            query.Add("redirect_uri", redirect_uri);
            query.Add("state", state);

            return await Task.FromResult(View(model: query.ToString()));
        }

        [HttpPost]
        public async Task<IActionResult> Authorize(string username,
            string response_type,
            string redirect_uri,
            string state)
        {
            string code = "OAUTH20_CODEFLOW_AUTHCODE";

            QueryBuilder query = new QueryBuilder();
            query.Add("code", code);
            query.Add("state", state);

            return await Task.FromResult(Redirect($"{redirect_uri}{query.ToString()}"));
        }

        public async Task<IActionResult> Token(
            string grant_type,
            string code,
            string redirect_uri,
            string client_id
            )
        {
            if (code == "OAUTH20_CODEFLOW_AUTHCODE") 
            {
                var bytes = Encoding.UTF8.GetBytes(CustomJwtBearerOptions.Secret);
                var key = new SymmetricSecurityKey(bytes);
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    CustomJwtBearerOptions.Issuer, 
                    CustomJwtBearerOptions.Audience, 
                    new List<Claim>()
                {
                    new Claim(JwtRegisteredClaimNames.Sub, "some_id"),
                    new Claim("O", "Auth")
                }, DateTime.Now, DateTime.Now.AddHours(1), credentials);

                var tokenJson = new JwtSecurityTokenHandler().WriteToken(token);

                var responseObject = new {
                    access_token = tokenJson,
                    token_type = "Bearer",
                    raw_claim = "oauth2"
                };
                var responseJson = JsonConvert.SerializeObject(responseObject);

                byte[] _bytes = Encoding.UTF8.GetBytes(responseJson);
                await Response.Body.WriteAsync(_bytes, 0, _bytes.Length);

                return Redirect(redirect_uri);
            }
            return BadRequest();
        }

    }
}
