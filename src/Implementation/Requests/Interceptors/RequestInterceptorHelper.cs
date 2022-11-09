// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    using System.Diagnostics;
    using System.Reflection;

    internal static class RequestInterceptorHelper<TRequest, TResult>
        where TRequest : class, IReturn<TResult>
        where TResult : class, IHaveRequestStatus
    {
        private static readonly object _SyncLock = new();

        private static InterceptorRecord<TRequest, TResult>[]? _ProxyTypes;

        public static async Task<TResult> Execute(IRequestHandler<TRequest, TResult> command, TRequest arg, CancellationToken cancellationToken)
        {
            var interceptorTypes = GetProxyTypes();

            if (!interceptorTypes.Any())
            {
                return await command.ExecuteAsync(arg, cancellationToken).ConfigureAwait(false);
            }

            var cmd = new InterceptorBase<TRequest, TResult>(command.ExecuteAsync);

            foreach (var p in interceptorTypes)
            {
                cmd = p.GetInterceptor(cmd.ExecuteAsync);
            }

            var result = await cmd.ExecuteAsync(arg, cancellationToken).ConfigureAwait(false);

            return result;
        }

        [STAThread]
        static InterceptorRecord<TRequest, TResult>[] GetProxyTypes()
        {
            if (_ProxyTypes is not null)
            {
                return _ProxyTypes;
            }

            lock (_SyncLock)
            {
                if (_ProxyTypes is not null)
                {
                    return _ProxyTypes;
                }

                var proxyQry =
                from t in TypeRegistry.Types
                where t.IsAssignableTo(typeof(RequestInterceptorBase<TRequest, TResult>)) &&
                t != typeof(RequestInterceptorBase<TRequest, TResult>)
                let att = t.GetCustomAttribute<InterceptAttribute>()
                let ordinal = att?.Ordinal ?? int.MaxValue
                select new ProxyInterceptorRecord<TRequest, TResult>(ordinal, t)
                    as InterceptorRecord<TRequest, TResult>;

                var factoryQry =
                    (from t in TypeRegistry.Types
                     where t.IsAssignableTo(typeof(InterceptorFactoryBase)) &&
                     t != typeof(InterceptorFactoryBase)
                     let att = t.GetCustomAttribute<InterceptAttribute>()
                     let ordinal = att?.Ordinal ?? int.MaxValue
                     select new FactoryInterceptorRecord<TRequest, TResult>(ordinal, t) 
                        as InterceptorRecord<TRequest, TResult>).ToArray();

                _ProxyTypes = proxyQry.Union(factoryQry).OrderByDescending(x => x.Ordinal).ToArray();

                return _ProxyTypes;
            }
        }

        private class FactoryInterceptorRecord<TRequest2, TResult2> : InterceptorRecord<TRequest2, TResult2>
            where TRequest2 : class, IReturn<TResult2>
            where TResult2 : class, IHaveRequestStatus
        {
            public FactoryInterceptorRecord(int ordinal, Type type) : base(ordinal, type)
            {
            }

            public override InterceptorBase<TRequest2, TResult2> GetInterceptor(ExecuteDelegate<TRequest2, TResult2> core)
            {
                return new InterceptorBase<TRequest2, TResult2>(async (a, r) => await Execute(core, a, r).ConfigureAwait(false));
            }

            private Task<TResult2?> Execute(ExecuteDelegate<TRequest2, TResult2> core, TRequest2 a, CancellationToken r)
            {
                var instance = Activator.CreateInstance(Type) as InterceptorFactoryBase;
                return instance?.ExecuteAsync(core, a, r) ?? Task.FromResult<TResult2>(default);
            }
        }

        private abstract class InterceptorRecord<TRequest2, TResult2>
             where TRequest2 : class, IReturn<TResult2>
             where TResult2 : class, IHaveRequestStatus
        {
            protected InterceptorRecord(int ordinal, Type type)
            {
                Ordinal = ordinal;
                Type = type;
            }

            public int Ordinal { get; }
            public Type Type { get; }

            [DebuggerHidden]
            public abstract InterceptorBase<TRequest2, TResult2>? GetInterceptor(ExecuteDelegate<TRequest2, TResult2> core);
        }

        private class ProxyInterceptorRecord<TRequest2, TResult2> : InterceptorRecord<TRequest2, TResult2>
            where TRequest2 : class, IReturn<TResult2>
            where TResult2 : class, IHaveRequestStatus
        {
            public ProxyInterceptorRecord(int ordinal, Type type) : base(ordinal, type) { }

            [DebuggerHidden]
            public override InterceptorBase<TRequest2, TResult2>? GetInterceptor(ExecuteDelegate<TRequest2, TResult2> core) => 
                Activator.CreateInstance(Type, new[] { core }) as InterceptorBase<TRequest2, TResult2>;
        }
    }
}