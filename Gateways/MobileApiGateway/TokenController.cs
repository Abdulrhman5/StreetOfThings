using CommonLibrary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MobileApiGateway
{
    [Route("Identity/connect")]
    public class TokenController : ControllerBase
    {
        IConfiguration _configuration;
        public TokenController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("token")]
        public async Task<ErrorMessage> Token([FromBody]LoginDto loginDto)
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
                dic.Add("loginInfo", loginDto.LoginInfo);
            }

            var request = new HttpRequestMessage(HttpMethod.Post,accessTokenUrl)
            {
                Content = new FormUrlEncodedContent(dic)
            };

            try
            {
                var response = await client.SendAsync(request);
                await CopyProxyHttpResponse(HttpContext, response);
                return null;
            }
            catch
            {
                var message = new ErrorMessage
                {
                    ErrorCode = "ACCESS.TOKEN.INTERNAL.ERROR",
                    Message = "there were an error while trying to create access token"
                };
                return message;
            }
        }

        public async Task CopyProxyHttpResponse(HttpContext context, HttpResponseMessage responseMessage)
        {
            if (responseMessage == null)
            {
                throw new ArgumentNullException(nameof(responseMessage));
            }

            var response = context.Response;

            response.StatusCode = (int)responseMessage.StatusCode;
            foreach (var header in responseMessage.Headers)
            {
                response.Headers[header.Key] = header.Value.ToArray();
            }

            foreach (var header in responseMessage.Content.Headers)
            {
                response.Headers[header.Key] = header.Value.ToArray();
            }

            // SendAsync removes chunking from the response. This removes the header so it doesn't expect a chunked response.
            response.Headers.Remove("transfer-encoding");

            using (var responseStream = await responseMessage.Content.ReadAsStreamAsync())
            {
                await responseStream.CopyToAsync(response.Body, 81920, context.RequestAborted);
            }
        }
    }
}
