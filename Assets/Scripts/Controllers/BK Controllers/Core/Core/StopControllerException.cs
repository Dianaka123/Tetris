using System;

namespace Infra.Controllers.Core
{
    public class StopControllerException : Exception
    {
        public StopControllerException(Exception exception)
        {
            Exception = exception;
        }

        public Exception Exception { get; }
    }
}