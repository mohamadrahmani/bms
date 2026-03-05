using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMS.Domain.Exceptions;

public sealed class UserDomainException : DomainException
{
    public UserDomainException(string message)
        : base(message)
    {
    }
}
