using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Common.DTOs;
using Common.Enums;
using PCAP;

namespace Services
{
    public class PacketService
    {
        private PacketReceiver _packetReceiver;

        private TotalStaticsticsModelDTO _totalStaticstics;
        private List<PerSecondStatisticsItemDTO> _perSecondStatisticsItems;

        private readonly object _lockObject = new object();
        private TimeSpan _timespan;
        private DeviceDto _currentDevice;

        public PacketService()
        {
            InitializeStatistics();
            _timespan = new TimeSpan(0, 0, 1, 0, 0);
        }

        public void ResetStatistics()
        {
            InitializeStatistics();
        }

        public List<DeviceDto> GetAllDevices()
        {
            var provider = new DeviceProvider();
            return provider.GetAllDevices();
        }

        public TotalStaticsticsModelDTO GetTotalStatistics()
        {
            return _totalStaticstics;
        }

        public List<PerSecondStatisticsItemDTO> GetPerSecondStatistics()
        {
            return _perSecondStatisticsItems;
        }

        public void StartGatheringStatistics(DeviceDto device, TimeSpan detailedStatisticsTimespan)
        {
            _currentDevice = device;
            _timespan = detailedStatisticsTimespan;
            _packetReceiver = new PacketReceiver(device.Name);
            _packetReceiver.Start();
            _packetReceiver.PacketReceived += PacketReceiverOnPacketReceived;
        }

        public void StopGatheringStatistics()
        {
            _packetReceiver.Stop();
        }

        private void PacketReceiverOnPacketReceived(PacketDto packetDto)
        {
            lock (_lockObject)
            {
                

                if (packetDto.SourceIP == _currentDevice.IPAddress)
                {
                    _totalStaticstics.TotalSentBytes += packetDto.TotalBytes;
                    _totalStaticstics.TotalSentPackets++;
                }
                else
                {
                    _totalStaticstics.TotalReceivedBytes += packetDto.TotalBytes;
                    _totalStaticstics.TotalReceivedPackets++;
                }
                
                if (packetDto.HttpMethod != null)
                {
                    _totalStaticstics.MethodsCount[packetDto.HttpMethod.Value]++;
                }

                if (packetDto.HttpStatusCode != null)
                {
                    if (_totalStaticstics.HttpStatusCodesCount.ContainsKey(packetDto.HttpStatusCode.Value))
                    {
                        _totalStaticstics.HttpStatusCodesCount[packetDto.HttpStatusCode.Value]++;
                    }
                    else
                    {
                        _totalStaticstics.HttpStatusCodesCount.Add(packetDto.HttpStatusCode.Value, 1);
                    }
                }

                var now = DateTime.UtcNow;

                _perSecondStatisticsItems.RemoveAll(x => x.Time < now.AddSeconds(-60));

                var currentSecondStatistics = _perSecondStatisticsItems.FirstOrDefault(x => (now - x.Time).TotalSeconds < 1 );

                if (currentSecondStatistics != null)
                {
                    if (packetDto.SourceIP == _currentDevice.IPAddress)
                    {
                        currentSecondStatistics.SentBytes += packetDto.TotalBytes;
                    }
                    else
                    {
                        currentSecondStatistics.ReceivedBytes += packetDto.TotalBytes;
                    }
                }
                else
                {
                    var newItem = new PerSecondStatisticsItemDTO
                    {
                        Time = now,
                    };

                    if (packetDto.SourceIP == _currentDevice.IPAddress)
                    {
                        newItem.SentBytes += packetDto.TotalBytes;
                    }
                    else
                    {
                        newItem.ReceivedBytes += packetDto.TotalBytes;
                    }

                    _perSecondStatisticsItems.Add(newItem);
                }
            }
        }

        private void InitializeStatistics()
        {
            _totalStaticstics = new TotalStaticsticsModelDTO();
            _perSecondStatisticsItems = new List<PerSecondStatisticsItemDTO>();

            var allHttpMethods = Enum.GetValues(typeof (HttpMethod)).Cast<HttpMethod>();

            foreach (var httpMethod in allHttpMethods.Where(x => x != HttpMethod.Unknown))
            {
                _totalStaticstics.MethodsCount.Add(httpMethod, 0);
            }
        }
    }
}