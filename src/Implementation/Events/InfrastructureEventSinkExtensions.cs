// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    public static class InfrastructureEventSinkExtensions
    {
        private static readonly AsyncLocal<ContextChangeInfrastructureEventSink> _Sink1 = new AsyncLocal<ContextChangeInfrastructureEventSink>();
        private static readonly ContextChangeInfrastructureEventSink _Sink2 = new ContextChangeInfrastructureEventSink();

        /// <summary>
        /// Context changes occurring on every thread and every calling scope.
        /// </summary>
        /// <param name="_">The .</param>
        /// <returns>ContextChangeInfrastructureEventSink.</returns>
        /// <remarks>Generally used for logging purposes</remarks>
        public static ContextChangeInfrastructureEventSink AnyContextChange(this InfrastructureEventSinkFor _) => _Sink2;

        /// <summary>
        /// Context changes occurring only with the calling scope (thread) of the registration.
        /// </summary>
        /// <param name="_">The .</param>
        /// <returns>ContextChangeInfrastructureEventSink.</returns>
        /// <remarks>used for request-specific information</remarks>
        public static ContextChangeInfrastructureEventSink ScopedContextChange(this InfrastructureEventSinkFor _)
        {
            if (_Sink1.Value == null)
            {
                _Sink1.Value = new ContextChangeInfrastructureEventSink();
            }

            return _Sink1.Value;
        }
    }
}