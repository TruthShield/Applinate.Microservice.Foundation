// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    public sealed class ContextChangeInfrastructureEventSink
    {
        public event EventHandler<RequestContextChange>? Changed;

        internal void Fire(RequestContextChange change)
        {
            Changed?.Invoke(null, change);
        }
    }
}