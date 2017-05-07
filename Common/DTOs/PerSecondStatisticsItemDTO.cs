using System;
using System.Net;
using Common.Enums;

namespace Common.DTOs
{
    public class PerSecondStatisticsItemDTO
    {
        public ulong ReceivedBytes { get; set; }
        public ulong SentBytes { get; set; }

        public DateTime Time { get; set; }
    }
}