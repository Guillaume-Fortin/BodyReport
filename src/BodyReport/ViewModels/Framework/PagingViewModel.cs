using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.ViewModels.Framework
{
    public class PagingViewModel
    {
        public int CurrentPage { get; set; }
        public int TotalRecords { get; set; }
        public int PageSize { get; set; }
        public string ControlerName { get; set; }
        public string ActionName { get; set; }
        public Dictionary<string, string> RouteValues { get; set; } = null;
    }
}
