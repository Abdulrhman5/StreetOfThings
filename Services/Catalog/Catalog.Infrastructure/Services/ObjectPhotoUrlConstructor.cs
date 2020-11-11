using Catalog.ApplicationCore.Entities;
using Catalog.ApplicationCore.Interfaces;
using Catalog.Infrastructure;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Web;

namespace Catalog.ApplicationLogic.Infrastructure
{
    public class PhotoUrlConstructor : IPhotoUrlConstructor
    {

        private string _currentDomain;

        public PhotoUrlConstructor(CurrentDomainGetter domainGetter)
        {
            _currentDomain = domainGetter.GetDomain8Schema();
        }


        public string Construct(Photo photo)
        {
            string type = "";
            if(photo is ObjectPhoto)
            {
                type = "Object";
            }
            else if(photo is TagPhoto)
            {
                // This is intendent as Resource controller now is reading from Object/
                // TODO: Refactor this  
                type = "Object";
            }
            return $"{_currentDomain}/Resources/Photo/{type}/{HttpUtility.ParseQueryString(photo.AdditionalInformation)["Name"]}";
        }

    }
}
