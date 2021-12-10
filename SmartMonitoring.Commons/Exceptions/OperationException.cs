using System;

namespace SmartMonitoring.Commons.Exceptions
{
    public class OperationException : Exception
    {
        public OperationException(string message)
        : base(message)
        {
        }
    }
}
