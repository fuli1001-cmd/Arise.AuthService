using Arise.DDD.API.Paging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Arise.DDD.API.Response
{
    public class PagedResponseWrapper : ResponseWrapper
    {
        public PagingInfo PagingInfo { get; set; }

        private PagedResponseWrapper(object data, PagingInfo pagingInfo) : base(data)
        {
            PagingInfo = pagingInfo;
        }

        public static PagedResponseWrapper CreateOkPagedResponseWrapper<T>(PagedList<T> pagedList)
        {
            return new PagedResponseWrapper(pagedList, pagedList.PagingInfo);
        }
    }
}
