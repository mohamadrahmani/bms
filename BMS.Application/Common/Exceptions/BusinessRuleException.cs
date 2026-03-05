using System;

namespace BMS.Application.Common.Exceptions;



/// <summary>
/// خطای نقض قوانین تجاری (Business Rules)
/// </summary>
public sealed class BusinessRuleException : Exception
{
    public BusinessRuleException(string message)
        : base(message)
    {
    }
}
