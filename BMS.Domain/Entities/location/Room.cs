using System;

namespace BMS.Domain.Entities.Location
{
    public class Room : BaseEntity<Guid>
    {
        public Guid FloorId { get; private set; }
        public Guid WardId { get; private set; }
        public string Name { get; private set; } = default!;
        public string RoomNumber { get; private set; } = default!;
        /// نوع اتاق
        /// مثال:
        /// Patient
        public string? Type { get; private set; }

        /// مساحت اتاق (متر مربع)
        public double Area { get; private set; }

        private Room() { } // EF Core

        public Room(
            Guid floorId,
            Guid wardId,
            string name,
            string roomNumber,
            string? type,
            double area)
        {
            if (floorId == Guid.Empty)
                throw new ArgumentException("FloorId is required");

            if (wardId == Guid.Empty)
                throw new ArgumentException("WardId is required");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Room name is required");

            if (string.IsNullOrWhiteSpace(roomNumber))
                throw new ArgumentException("Room number is required");

            if (area < 0)
                throw new ArgumentException("Area cannot be negative");

            FloorId = floorId;
            WardId = wardId;
            Name = name.Trim();
            RoomNumber = roomNumber.Trim();
            Type = type?.Trim();
            Area = area;
        }

        public void Update(
            Guid floorId,
    Guid wardId,
            string name,
            string roomNumber,
            string? type,
            double area)
        {
            if (floorId == Guid.Empty)
                throw new ArgumentException("FloorId is required");

            if (wardId == Guid.Empty)
                throw new ArgumentException("WardId is required");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Room name is required");

            if (string.IsNullOrWhiteSpace(roomNumber))
                throw new ArgumentException("Room number is required");

            if (area < 0)
                throw new ArgumentException("Area cannot be negative");
            FloorId = floorId;
            WardId = wardId;
            Name = name.Trim();
            RoomNumber = roomNumber.Trim();
            Type = type?.Trim();
            Area = area;

            SetUpdated();
        }

    }
}
