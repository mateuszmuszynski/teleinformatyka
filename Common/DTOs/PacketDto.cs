using System.Net;
using Common.Enums;

namespace Common.DTOs
{
    public class PacketDto
    {
        public HttpMethod? HttpMethod { get; set; }
        public ulong TotalBytes { get; set; }
        public HttpStatusCode? HttpStatusCode { get; set; }
        public string SourceIP { get; set; }
        public ushort Port { get; set; }
    }
}