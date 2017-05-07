using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WPF.ViewModels
{
    public class HttpStatusCodeStatisticsViewModel
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public ulong Requests { get; set; }
    }
}
