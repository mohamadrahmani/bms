using System.Threading.Channels;
using BMS.Application.Interfaces;
using BMS.Application.Models;
using BMS.Application.Interfaces;
using BMS.Application.Models;

namespace BMS.Infrastructure.Historian
{
    public class ChannelHistorianWriter : IHistorianWriter
    {
        private readonly Channel<DataPointDeltaModel> _channel;

        public ChannelHistorianWriter(
            Channel<DataPointDeltaModel> channel)
        {
            _channel = channel;
        }

        public void Enqueue(DataPointDeltaModel delta)
        {
            _channel.Writer.TryWrite(delta);
        }
    }
}