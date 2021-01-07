using System;

namespace NLog.Target.Datadog
{
    public class CannotSendLogEventException : Exception
    {
        public CannotSendLogEventException(int retries) :
            base($"Could not send payload to Datadog, retries: {retries}")
        {
        }
    }
}