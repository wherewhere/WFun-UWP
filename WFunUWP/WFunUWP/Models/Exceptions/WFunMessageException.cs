using System;

namespace WFunUWP.Core.Exceptions
{
    public sealed class WFunMessageException : Exception
    {
        public string MessageStatus { get; }

        public WFunMessageException(string message) : base(message) { }

        public WFunMessageException(string message, Exception innerException) : base(message, innerException) { }
    }
}
