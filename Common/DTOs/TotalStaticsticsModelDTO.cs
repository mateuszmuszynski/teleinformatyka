using System.Collections.Generic;
using System.Net;
using Common.Enums;

namespace Common.DTOs
{
    public class TotalStaticsticsModelDTO
    {
        public ulong TotalSentPackets { get; set; }
        public ulong TotalReceivedPackets { get; set; }
        public ulong TotalSentBytes { get; set; }
        public ulong TotalReceivedBytes { get; set; }
        public Dictionary<HttpStatusCode, ulong> HttpStatusCodesCount { get; set; }
        public Dictionary<HttpMethod, ulong> MethodsCount { get; set; }

        public TotalStaticsticsModelDTO()
        {
            HttpStatusCodesCount = new Dictionary<HttpStatusCode, ulong>();
            MethodsCount = new Dictionary<HttpMethod, ulong>();
        }
    }
}
