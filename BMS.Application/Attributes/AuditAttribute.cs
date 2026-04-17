using BMS.Domain.Entities.Logs;
using System;

namespace BMS.Application.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AuditAttribute : Attribute
    {
        public EventType EventType { get; }

        public string ObjectName { get; }

        public AuditAttribute(EventType eventType, string objectName)
        {
            EventType = eventType;
            ObjectName = objectName;
        }
    }
}
