using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.ApplicationCore.Services
{
    public class PagingArguments
    {
        int _startObject = 0;
        int _size = 10;
        private int _total = 0;

        public virtual int StartObject
        {
            get
            {
                return _startObject;
            }
            set
            {
                if (value < 0)
                {
                    _startObject = 0;
                }
                else
                {
                    _startObject = value;
                }
            }
        }

        public virtual int Size
        {
            get
            {
                return _size;
            }
            set
            {
                if (value <= 0 || value > 20)
                {
                    _size = 10;
                }
                else
                {
                    _size = value;
                }
            }
        }

        public virtual int Total
        {
            get => _total;
            set => _total = value;
        }

        public virtual int CurrentPage
        {
            get
            {
                var currentPage = StartObject / Size + 1;
                if (currentPage > TotalPages)
                {
                    return TotalPages;
                }

                return currentPage;
            }
            set
            {
                StartObject = (value - 1) * Size;
            }
        }

        public virtual int TotalPages
        {
            get => (int)Math.Ceiling((decimal)Total / Size);
        }
    }
}
