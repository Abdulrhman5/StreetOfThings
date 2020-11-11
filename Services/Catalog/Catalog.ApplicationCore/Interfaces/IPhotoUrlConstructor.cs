using Catalog.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Catalog.ApplicationCore.Interfaces
{
    public interface IPhotoUrlConstructor
    {
        string Construct(Photo photo);

    }
}
