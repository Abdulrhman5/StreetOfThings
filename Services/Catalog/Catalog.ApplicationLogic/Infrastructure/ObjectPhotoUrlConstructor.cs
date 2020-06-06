using Catalog.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web;

namespace Catalog.ApplicationLogic.Infrastructure
{
    public interface IObjectPhotoUrlConstructor
    {
        Expression<Func<ObjectPhoto, string>> ConstructPhotos { get; }

        string Construct(ObjectPhoto objectPhoto);
    }

    public class ObjectPhotoUrlConstructor : IObjectPhotoUrlConstructor
    {

        private string _currentDomain;

        public ObjectPhotoUrlConstructor(CurrentDomainGetter domainGetter)
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
