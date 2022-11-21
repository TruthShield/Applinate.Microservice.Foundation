namespace Applinate.Internals
{
    public static class RequestExecutorManager
    {
        public static void Register()
        {
            ServiceProvider.Register<IRequestExecutor>(() => new RequestExecutor());
        }
    }
}