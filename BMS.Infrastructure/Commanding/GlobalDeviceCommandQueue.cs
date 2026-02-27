using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BMS.Application.Abstraction;
using BMS.Domain.Entities;
using System.Threading.Channels;

namespace BMS.Infrastructure.Commanding
{

    public class GlobalDeviceCommandQueue : IDeviceCommandQueue
    {
        private readonly Channel<DeviceCommand> _channel;

        public ChannelReader<DeviceCommand> Reader => _channel.Reader;

        public GlobalDeviceCommandQueue()
        {
            _channel = Channel.CreateBounded<DeviceCommand>(
                new BoundedChannelOptions(5000)
                {
                    FullMode = BoundedChannelFullMode.Wait,
                    SingleReader = false,
                    SingleWriter = false
                });
        }

        public ValueTask EnqueueAsync(DeviceCommand command)
        {
            return _channel.Writer.WriteAsync(command);
        }
    }
}
