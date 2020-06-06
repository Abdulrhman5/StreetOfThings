using Catalog.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web;

namespace Catalog.ApplicationLogic.Infrastructure
{
    public class ObjectPhotoUrlConstructor
    {

        private string _currentDomain;

        private ObjectPhotoUrlConstructor(CurrentDomainGetter domainGetter)
        {
            _currentDomain = domainGetter.GetDomain8Schema();
        }

        public string Construct(ObjectPhoto objectPhoto)
        {
            return $"{_currentDomain}/Resources/Photos/Object/{HttpUtility.ParseQueryString(objectPhoto.AdditionalInformation)["Name"]}";
        }


        public Expression<Func<ObjectPhoto, string>> ConstructPhotos => 
            (op) =>
             $"{_currentDomain}/Resources/Photos/Object/{HttpUtility.ParseQueryString(op.AdditionalInformation)["Name"]}";
    }
}
