using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DateYoWaifuApp.API.Helpers
{

    // Pagination for generic type, on data
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; set;}
        public int TotalPages { get; set;}
        public int PageSize { get; set;}
        public int TotalCount { get; set;}

        public PagedList(List<T> items, int count, int pgNum, int pgSize) {
            TotalCount = count;
            PageSize = pgSize;
            CurrentPage = pgNum;
            TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            this.AddRange(items);
        }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pgNum, int pgSize) {
            var count = await source.CountAsync();
            var items = await source.Skip((pgNum - 1) * pgSize).Take(pgSize).ToListAsync();
            return new PagedList<T>(items, count, pgNum, pgSize);
        }
        
    }
}