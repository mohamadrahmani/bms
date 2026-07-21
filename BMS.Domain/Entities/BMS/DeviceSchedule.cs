using System;
using BMS.Domain.Entities; // فرض بر این است که BaseEntity در اینجا قرار دارد

namespace BMS.Domain.Entities.BMS
{
    public class DeviceSchedule : BaseEntity<Guid>
    {
        // Property های خاص این جدول
        public Guid DeviceId { get; set; }
        public int RegisterIndex { get; set; }
        public bool IsActive { get; set; }
        public int StartDay { get; set; }
        public TimeSpan? StartTime { get; set; }
        public int EndDay { get; set; }
        public TimeSpan? EndTime { get; set; }

        //// Navigation Property (برای ارتباط با موجودیت Device)
        //// اگر کلاس Device دارید، بهتر است برای رعایت اصول Clean این رابطه را برقرار کنید
        //public virtual Device Device { get; set; }

        // سازنده برای مقداردهی اولیه در صورت نیاز
        public DeviceSchedule()
        {
        }

        // متدی برای آپدیت کردن رکورد
        public void Update(int registerIndex, bool isActive, int startDay, TimeSpan? startTime, int endDay, TimeSpan? endTime, Guid deviceId)
        {
            RegisterIndex = registerIndex;
            IsActive = isActive;
            StartDay = startDay;
            StartTime = startTime;
            EndDay = endDay;
            EndTime = endTime;
            DeviceId = deviceId;

            // فراخوانی متد موجود در BaseEntity برای ثبت زمان ویرایش
            SetUpdated();
        }
    }
}
