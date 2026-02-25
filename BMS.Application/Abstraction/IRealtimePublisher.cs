using BMS.Application.Models;

namespace BMS.Application.Interfaces
{
    public interface IRealtimePublisher
    {
        void Enqueue(DataPointDeltaModel delta);
    }
}
