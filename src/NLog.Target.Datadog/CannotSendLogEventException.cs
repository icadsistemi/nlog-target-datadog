using System;

namespace NLog.Target.Datadog
{
    public class CannotSendLogEventException : Exception
    {
        public CannotSendLogEventException() : base($"Could not send payload to Datadog") {  }
    }
}