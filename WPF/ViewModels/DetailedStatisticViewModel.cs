using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common.Enums;

namespace WPF.ViewModels
{
    public class DetailedStatisticViewModel
    {
        public ushort Port { get; set; }
        public string IP { get; set; }
        public ulong Bytes { get; set; }
        public HttpMethod? HttpMethod { get; set; }
        public HttpStatusCode? HttpStatusCode { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
