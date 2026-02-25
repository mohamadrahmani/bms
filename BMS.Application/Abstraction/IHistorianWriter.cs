using BMS.Application.Models;

namespace BMS.Application.Interfaces
{
    public interface IHistorianWriter
    {
        void Enqueue(DataPointDeltaModel delta);
    }
}
