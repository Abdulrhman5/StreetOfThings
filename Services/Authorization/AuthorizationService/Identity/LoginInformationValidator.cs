using ApplicationLogic.AppUserCommands;
using CommonLibrary;
using IdentityServer4.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AuthorizationService.Identity
{
    public class LoginInformationValidator
    {

        /// <summary>
        /// The context should have LoginInfo in the body
        /// LoginInfo: Lon=99.99&Lat=99.99&Imei=xxx-xxx
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public (bool IsSuccess, LoginInformationDto? LoginInformation) ValidateLoginInfo(ResourceOwnerPasswordValidationContext context)
        {
            var loginInfo = context?.Request?.Raw?["loginInfo"];

            if(loginInfo == null)
            {
                return (false, null);
            }
            var query = HttpUtility.ParseQueryString(loginInfo);
            var lon = query.Get("lon");
            double dLon;
            if (lon.IsNullOrEmpty())
            {
                return (false, null);
            }

            if (!double.TryParse(lon, out dLon))
            {
                return (false, null);
            }

            var lat = query.Get("lat");
            double dLat;
            if (lat.IsNullOrEmpty())
            {
                return (false, null);
            }

            if (!double.TryParse(lat, out dLat))
            {
                return (false, null);
            }

            // if dLon is not in this range -180 <= dLon <= +180
            // or of dLat is not in this range -90 <= dLat <= +90
            // return error
            if (Math.Abs(dLon) > 180 || Math.Abs(dLat) > 90)
            {
                return (false, null);
            }

            var imei = query.Get("Imei");
            if (imei.IsNullOrEmpty())
            {
                return (false, null);
            }

            return (true, new LoginInformationDto
            {
                Imei = imei,
                Location = (dLat, dLon)
            });
        }
    }
}
