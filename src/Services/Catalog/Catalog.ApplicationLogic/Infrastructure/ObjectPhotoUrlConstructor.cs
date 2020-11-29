using Catalog.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web;

namespace Catalog.ApplicationLogic.Infrastructure
{
    public interface IPhotoUrlConstructor
    {
        Expression<Func<ObjectPhoto, string>> ConstructPhotos { get; }

        string Construct(ObjectPhoto objectPhoto); 
        
        string Construct(TagPhoto objectPhoto);
    }

    public class PhotoUrlConstructor : IPhotoUrlConstructor
    {

        private string _currentDomain;

        public PhotoUrlConstructor(CurrentDomainGetter domainGetter)
        {
            _currentDomain = domainGetter.GetDomain8Schema();
        }

        public string Construct(ObjectPhoto objectPhoto)
        {
            return $"{_currentDomain}/Resources/Photo/Object/{HttpUtility.ParseQueryString(objectPhoto.AdditionalInformation)["Name"]}";
        }

        public Expression<Func<ObjectPhoto, string>> ConstructPhotos =>
            (op) =>
             $"{_currentDomain}/Resources/Photo/Object/{HttpUtility.ParseQueryString(op.AdditionalInformation)["Name"]}";

        public string Construct(TagPhoto tagPhoto)
        {
            if (tagPhoto is null)
                return null;
            return $"{_currentDomain}/Resources/Photo/Tag/{HttpUtility.ParseQueryString(tagPhoto.AdditionalInformation)["Name"]}";
        }
    }
}
