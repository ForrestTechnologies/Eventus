using System;

namespace Eventus.Exceptions
{
    public class AggregateMethodException : Exception
    {
        public AggregateMethodException(string message): base(message)
        {}
    }
}