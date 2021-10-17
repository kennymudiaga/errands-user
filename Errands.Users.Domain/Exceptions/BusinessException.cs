using System;

namespace Errands.Users.Domain.Exceptions
{
    public class BusinessException : ApplicationException
    {
        public BusinessException(string message)
            : base(message) { }
        public BusinessException(string message, object logData)
            : this(message)
        {
            LogData = logData;
        }
        public object LogData { get; }
    }
}
