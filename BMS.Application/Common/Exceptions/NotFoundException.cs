using System;

namespace BMS.Application.Common.Exceptions;

/// <summary>
/// خطای عدم یافتن موجودیت مورد نظر
/// </summary>
public sealed class NotFoundException : Exception
{
    public NotFoundException(string message)
        : base(message)
    {
    }
}
