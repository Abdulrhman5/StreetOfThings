using Catalog.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Catalog.ApplicationCore.Interfaces
{
    public interface IPhotoUrlConstructor
    {
        Expression<Func<ObjectPhoto, string>> ConstructPhotos { get; }

        string Construct(ObjectPhoto objectPhoto);

        string Construct(TagPhoto objectPhoto);
    }
}
