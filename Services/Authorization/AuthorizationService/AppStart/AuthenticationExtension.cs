using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;
using Unity;
using AuthorizationService.AppStart;
using Microsoft.OpenApi.Models;
using AuthorizationService.Identity;
using System;
using Serilog;
using System.Text;
using System.Threading.Tasks;
using CommonLibrary;
using Newtonsoft.Json;

namespace AuthorizationService.AppStart
{
    public static class AuthenticationExtension
    {

        public static AuthenticationBuilder AddCustomAuthentication(this IServiceCollection builder)
        {
            return builder.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = "http://localhost:20000";
                options.RequireHttpsMetadata = false;

                options.Audience = "ProfileManagement";
                options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
                {
                    OnChallenge = async cc =>
                    {
                        cc.Response.StatusCode = 401;
                        var message = new ErrorMessage
                        {
                            ErrorCode = "NOT.AUTHORIZED",
                            Message = "You are not authorized to make this request",
                            StatusCode = System.Net.HttpStatusCode.Unauthorized
                        };
                        var serialized = JsonConvert.SerializeObject(message);
                        var asBytes = Encoding.UTF8.GetBytes(serialized);
                        cc.Response.ContentType = "application/json";
                        await cc.Response.Body.WriteAsync(asBytes);
                        cc.HandleResponse();
                    }
                };
            });
        }
    }
}
