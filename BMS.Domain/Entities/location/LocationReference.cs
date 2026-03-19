using System;

namespace BMS.Domain.Entities.Location
{
    public class LocationReference
    {
        public Guid? SiteId { get; private set; }
        public Guid? BuildingId { get; private set; }
        public Guid? FloorId { get; private set; }
        public Guid? WardId { get; private set; }
        public Guid? RoomId { get; private set; }

        private LocationReference() { } // EF Core

        public LocationReference(
            Guid? siteId,
            Guid? buildingId,
            Guid? floorId,
            Guid? wardId,
            Guid? roomId)
        {
            SiteId = siteId;
            BuildingId = buildingId;
            FloorId = floorId;
            WardId = wardId;
            RoomId = roomId;
        }
        public bool IsEmpty()
        {
            return SiteId == null &&
                   BuildingId == null &&
                   FloorId == null &&
                   WardId == null &&
                   RoomId == null;
        }
    }
}
