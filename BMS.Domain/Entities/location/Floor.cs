using System;

namespace BMS.Domain.Entities.Location
{

    public class Floor : BaseEntity<Guid>
    {
        public Guid BuildingId { get; private set; }
        public string Name { get; private set; } = default!;
        public int LevelNumber { get; private set; }
        public string? Description { get; private set; }


        private Floor() { }

        /// <summary>
        /// ایجاد طبقه جدید
        /// </summary>
        public Floor(Guid buildingId, string name, int levelNumber, string? description)
        {
            if (buildingId == Guid.Empty)
                throw new ArgumentException("BuildingId is required");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Floor name is required");

            BuildingId = buildingId;
            Name = name.Trim();
            LevelNumber = levelNumber;
            Description = description?.Trim();
        }

        /// <summary>
        /// بروزرسانی اطلاعات طبقه
        /// </summary>
        public void Update(Guid buildingId, string name, int levelNumber, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Floor name is required");
            BuildingId = buildingId;
            Name = name.Trim();
            LevelNumber = levelNumber;
            Description = description?.Trim();

            SetUpdated();
        }
    }
}
