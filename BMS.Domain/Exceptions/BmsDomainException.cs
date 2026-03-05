using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMS.Domain.Exceptions
{
    public sealed class BmsDomainException : DomainException
    {
        public BmsDomainException(string message) : base(message)
        {
        }
    }
}
