using CommonLibrary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AdministrationGateway.Controllers
{
    [Route("Identity/connect")]
    public class TokenController : ControllerBase
    {
        IConfiguration _configuration;
        HttpResponseMessageConverter _responseMessageConverter;
        public TokenController(IConfiguration configuration)
        {
            _configuration = configuration;
            _responseMessageConverter = new HttpResponseMessageConverter();
        }

        [HttpPost]
        [Route("token")]
        public async Task<IActionResult> Token([FromBody]LoginDto loginDto)
        {
            HttpClient client = new HttpClient();
            var clientId = _configuration["IdentityServerConfigs:client_id"];
            var clientSecret = _configuration["IdentityServerConfigs:client_secret"];
            var accessTokenUrl = _configuration["IdentityServerConfigs:AccessTokenUrl"];
            var dic = new Dictionary<string, string>();
            dic.Add("client_id", clientId);
            dic.Add("client_secret", clientSecret);
            if (loginDto != null)
            {
                dic.Add("grant_type", loginDto.grant_type);
                dic.Add("username", loginDto.username);
                dic.Add("password", loginDto.password);
            }

            var request = new HttpRequestMessage(HttpMethod.Post, accessTokenUrl)
            {
                Content = new FormUrlEncodedContent(dic)
            };

            try
            {
                var response = await client.SendAsync(request);
                await _responseMessageConverter.ConvertAndCopyResponse(HttpContext, response);
                return new EmptyResult();
            }
            catch
            {
                var message = new ErrorMessage
                {
                    ErrorCode = "ACCESS.TOKEN.INTERNAL.ERROR",
                    Message = "there were an error while trying to create access token"
                };
                return StatusCode(500, message);
            }
        }
    }
}
