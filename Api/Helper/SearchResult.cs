using System;
using System.Collections.Generic;
using System.Web.Optimization;

namespace FarmaAPI.Helper
{
    public class SearchResult<T>
    {
        public int total_count { get; set; }
        public bool incomplete_results { get; set; }
        public List<T> items { get; set; }
    }
}