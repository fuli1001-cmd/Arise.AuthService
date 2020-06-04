using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arise.DDD.API.Paging
{
	public class PagedList<T> : List<T>
	{
		public PagingInfo PagingInfo { get; private set; }

		public PagedList(List<T> items, int count, PagingParameters pagingParameters)
		{
			PagingInfo = new PagingInfo(pagingParameters.PageNumber, (int)Math.Ceiling(count / (double)pagingParameters.PageSize), pagingParameters.PageSize, count);
			AddRange(items);
		}

		public static async Task<PagedList<T>> ToPagedListAsync(IQueryable<T> source, PagingParameters pagingParameters)
		{
			var count = await source.CountAsync();
			var items = await source.Skip((pagingParameters.PageNumber - 1) * pagingParameters.PageSize).Take(pagingParameters.PageSize).ToListAsync();

			return new PagedList<T>(items, count, pagingParameters);
		}
	}
}
