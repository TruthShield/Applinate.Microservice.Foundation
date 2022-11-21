namespace Applinate.Internals
{
    public static class RequestContextHelper
    {
        public static void SetCurrentRequestContext(this RequestContext ctx) =>
            RequestContext.Current = ctx;

        internal static void SetCurrentRequestContextAsClient() =>
            RequestContext.Current = RequestContext.Current with 
            { 
                ServiceType = ServiceType.Client 
            };
    }
}