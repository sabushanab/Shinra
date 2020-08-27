using System;

namespace Shinra.Messages
{
    public class FailureMessage 
    {
        public Exception Exception { get; private set; }
        public FailureMessage(Exception exception) 
        {
            Exception = exception;
        }
    }
}
