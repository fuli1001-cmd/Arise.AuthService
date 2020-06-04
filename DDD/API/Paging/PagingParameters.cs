using System;
using System.Collections.Generic;
using System.Text;

namespace Arise.DDD.API.Paging
{
	public class PagingParameters
	{
		const int maxPageSize = 50;

		private int _pageNumber = 1;
		private int _pageSize = 10;

		public int PageNumber
		{
			get
			{
				return _pageNumber;
			}
			set
			{
				if (value < 1)
					_pageNumber = 1;
				else
					_pageNumber = value;
			}
		}

		public int PageSize
		{
			get
			{
				return _pageSize;
			}
			set
			{
				if (value > maxPageSize)
					_pageSize = maxPageSize;
				else if (value < 1)
					_pageSize = 1;
				else
					_pageSize = value;
			}
		}
	}
}
