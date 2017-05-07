using System;
using System.Globalization;
using System.Linq;
using System.Net;
using Common.DTOs;
using Common.Enums;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Http;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Transport;

namespace PCAP
{
    public class PacketDtoBuilder
    {
        public PacketDto CreateDto(Packet packet)
        {
            IpV4Datagram ip = packet.Ethernet.IpV4;

            UdpDatagram udp = ip.Udp;

            var model = new PacketDto
            {
                TotalBytes = (ulong)packet.Length,
                SourceIP = ip.Source.ToString(),
                Port = udp.SourcePort,
            };

            HttpDatagram http = packet.Ethernet.Ip.Tcp.Http;

            if (http != null)
            {
                if (http.IsRequest)
                {
                    var requestDatagram = (HttpRequestDatagram) http;

                    if (requestDatagram.Method != null)
                    {
                        var methodString = requestDatagram.Method.Method;
                        var methods = Enum.GetValues(typeof (HttpMethod)).Cast<HttpMethod>();

                        var currentMethod =
                            methods.FirstOrDefault(
                                x => x.ToString().ToUpperInvariant() == methodString.ToUpperInvariant());

                        if (currentMethod != HttpMethod.Unknown)
                        {
                            model.HttpMethod = currentMethod;
                        }
                    }
                }
                else
                {
                    var responseDatagram = (HttpResponseDatagram) http;

                    if (responseDatagram.StatusCode.HasValue)
                    {
                        var statusCode = responseDatagram.StatusCode.Value;
                        model.HttpStatusCode = (HttpStatusCode)statusCode;
                    }
                }
            }

            return model;
        }
    }
}
