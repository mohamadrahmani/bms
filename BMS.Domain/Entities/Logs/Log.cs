using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using BMS.Domain.Entities;
using BMS.Domain.Entities.Files;

namespace BMS.Domain.Entities.Logs
{
    public class Log : BaseEntity<Guid>
    {
        public Guid? ParentLogId { get; set; }
        [JsonIgnore]
        public Log? ParentLog { get; set; }
        public ICollection<Log>? Children { get; set; }
        public Guid? UserId { get; set; }
        [NotMapped]
        public string? UserName { get; set; }
        [NotMapped]
        public string? EntityDisplayName { get; set; }
        public EventType? EventType { get; set; }// نوع رویداد ثبت شده در سیستم (افزودن، ویرایش، حذف، ورود، خروج)
        public string? ObjectName { get; set; } // اسم جدولی که تغییر کرده
        public Guid? EntityTypeId { get; set; }
        public string? ObjectId { get; set; } // شناسه ردیفی که تغییر کرده
        [MaxLength(300)]
        public string? ObjectDisplayName { get; set; }
        public string? FieldName { get; set; } // نام فیلد یا ستونی که تغییر کرده
        public string? OldValue { get; set; } // مقدار قدیم
        public string? NewValue { get; set; } // مقدار جدید
        [NotMapped]

        public OperationResult? Result { get; set; } // موفق یا ناموفق بودن عملیات
        public string? ResultMessage { get; set; } // پیام سیستم مربوط به نتیجه عملیات
        public DateTime LogDate { get; set; }
        public string? IpAddress { get; set; }
        // منبع ثبت لاگ (مثلاً API، وب‌اپلیکیشن، سرویس پس‌زمینه و ...)
        public string? Source { get; set; }
        // بدنه درخواست (Request Body)
        public string? RequestBody { get; set; }
        // مدت زمان اجرای درخواست به میلی‌ثانیه
        public long? ExecutionTimeMs { get; set; }
        // اطلاعات مرورگر یا کلاینت
        public string? UserAgent { get; set; }
        // کد وضعیت HTTP
        public int? StatusCode { get; set; }
    }

    public enum EventType
    {
        AddData = 1,
        UpdateData = 2,
        DeleteData = 3,
        Login = 4,
        Logout = 5
    }
    public enum OperationResult
    {
        Success = 0,
        Failed = 1,
        Error = 2
    }

}
