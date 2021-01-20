using System;

namespace NLog.Target.Datadog
{
    internal static class ExceptionExtensions
    {
        public static Exception FlattenToActualException(this Exception exception)
        {
            if (!(exception is AggregateException aggregateException))
                return exception;

            var flattenException = aggregateException.Flatten();
            return flattenException.InnerExceptions.Count == 1 ? 
                flattenException.InnerExceptions[0] : 
                flattenException;
        }
    }
}