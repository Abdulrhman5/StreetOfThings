using CommonLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MobileApiGateway
{
    public class ResponseProcessor
    {
        public async Task<FetchingResult<T>> Process<T>(HttpResponseMessage responseMessage)
        {
            var jsonString = await responseMessage.Content.ReadAsStringAsync();
            if (responseMessage.IsSuccessStatusCode)
            {
                var objectified = JsonConvert.DeserializeObject<T>(jsonString);
                return new FetchingResult<T>(objectified);
            }
            else
            {
                try
                {
                    var errorObjectified = JsonConvert.DeserializeObject<ErrorMessage>(jsonString);
                    return new FetchingResult<T>(errorObjectified);
                }
                catch (Exception e)
                {
                    return new FetchingResult<T>(new ErrorMessage
                    {
                        Message = "Internal server error",
                        ErrorCode = "INTERNAL.SERVER.ERROR",
                        StatusCode = System.Net.HttpStatusCode.InternalServerError
                    });
                }
            }
        }
    }
}
