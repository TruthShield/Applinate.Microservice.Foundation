// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate.Test
{
    /// <summary>
    /// Class TestBase.
    /// </summary>
    public class TestBase
    {
        private static readonly object _SyncLock = new object();

        public TestBase(ServiceType serviceType = ServiceType.Client, bool useEncryption = true)
        {
            lock (_SyncLock)
            {
                TestHelper.OnlyLoadReferencedAssemblies();

                InitializationProvider.Initialize(true);

                TestHelper.SetRequestContext(serviceType);

                ServiceProvider.RegisterSingleton<IRequestHandler, DefaultRequestExecutor>();
            }
        }
    }
}