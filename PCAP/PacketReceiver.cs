using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Common.DTOs;
using PcapDotNet.Core;
using PcapDotNet.Packets;

namespace PCAP
{
    public class PacketReceiver
    {
        private readonly PacketDevice _device;
        private bool _stopReceiver;
        private Thread _workingThread;

        public event Action<PacketDto> PacketReceived;

        public PacketReceiver(string deviceName)
        {
            IList<LivePacketDevice> allDevices = LivePacketDevice.AllLocalMachine;

            _device = allDevices.First(x => x.Name == deviceName);
        }

        public void Start()
        {
            _workingThread = new Thread(StartReceiving);
            _workingThread.Start();
        }

        private void StartReceiving()
        {
            _stopReceiver = false;

            using (PacketCommunicator communicator =
                _device.Open(65536, // portion of the packet to capture
                    // 65536 guarantees that the whole packet will be captured on all the link layers
                    PacketDeviceOpenAttributes.Promiscuous, // promiscuous mode
                    1000)) // read timeout
            {
                Console.WriteLine("Listening on " + _device.Description + "...");

                using (BerkeleyPacketFilter filter = communicator.CreateFilter("tcp"))
                {
                    // Set the filter
                    communicator.SetFilter(filter);
                }

                do
                {
                    Packet packet;
                    PacketCommunicatorReceiveResult result = communicator.ReceivePacket(out packet);
                    switch (result)
                    {
                        case PacketCommunicatorReceiveResult.Timeout:
                            // Timeout elapsed
                            continue;
                        case PacketCommunicatorReceiveResult.Ok:
                        {
                            var packetDtoBuilder = new PacketDtoBuilder();
                            var packetDto = packetDtoBuilder.CreateDto(packet);

                            PacketReceived?.Invoke(packetDto);
                            break;
                        }
                        default:
                            throw new InvalidOperationException("The result " + result + " shoudl never be reached here");
                    }
                } while (!_stopReceiver);
            }
        }

        public void Stop()
        {
            _stopReceiver = true;
            _workingThread.Join();
        }
    }
}
