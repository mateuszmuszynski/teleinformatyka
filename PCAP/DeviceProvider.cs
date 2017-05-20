using System.Collections.Generic;
using System.Linq;
using Common.DTOs;
using PcapDotNet.Core;

namespace PCAP
{
    public class DeviceProvider
    {
        public List<DeviceDto> GetAllDevices()
        {
        var allDevices = LivePacketDevice.AllLocalMachine;

            return allDevices.Select(x => new DeviceDto
            {
                Name = x.Name,
                Description = x.Description,
                IPAddress = x.Addresses.Last().Address.ToString().Split(' ').Last()
            }).ToList();
        } 
    }
}
