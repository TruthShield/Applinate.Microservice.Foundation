// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    using System.Diagnostics;
    using System.Reflection;

    internal static class RequestInterceptorHelper<TRequest, TResponse>
        where TRequest : class, IReturn<TResponse>
        where TResponse : class, IHaveResponseStatus
    {
        private static readonly object _SyncLock = new();

        private static InterceptorRecord<TRequest, TResponse>[]? _ProxyTypes;

        public static async Task<TResponse> Execute(IRequestHandler<TRequest, TResponse> command, TRequest request, CancellationToken cancellationToken)
        {
            var interceptorTypes = GetProxyTypes();

            if (!interceptorTypes.Any())
            {
                return await command.ExecuteAsync(request, cancellationToken).ConfigureAwait(false);
            }

            var cmd = new InterceptorBase<TRequest, TResponse>(command.ExecuteAsync);

            foreach (var p in interceptorTypes)
            {
                cmd = p.GetInterceptor(cmd.ExecuteAsync);
            }

            var result = await cmd.ExecuteAsync(request, cancellationToken).ConfigureAwait(false);

            return result;
        }

        [STAThread]
        static InterceptorRecord<TRequest, TResponse>[] GetProxyTypes()
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
                where t.IsAssignableTo(typeof(RequestInterceptorBase<TRequest, TResponse>)) &&
                t != typeof(RequestInterceptorBase<TRequest, TResponse>)
                let att = t.GetCustomAttribute<InterceptAttribute>()
                let ordinal = att?.Ordinal ?? int.MaxValue
                select new ProxyInterceptorRecord<TRequest, TResponse>(ordinal, t)
                    as InterceptorRecord<TRequest, TResponse>;

                var factoryQry =
                    (from t in TypeRegistry.Types
                     where t.IsAssignableTo(typeof(InterceptorFactoryBase)) &&
                     t != typeof(InterceptorFactoryBase)
                     let att = t.GetCustomAttribute<InterceptAttribute>()
                     let ordinal = att?.Ordinal ?? int.MaxValue
                     select new FactoryInterceptorRecord<TRequest, TResponse>(ordinal, t) 
                        as InterceptorRecord<TRequest, TResponse>).ToArray();

                _ProxyTypes = proxyQry.Union(factoryQry).OrderByDescending(x => x.Ordinal).ToArray();

                return _ProxyTypes;
            }
        }

        private class FactoryInterceptorRecord<TRequest2, TResponse2> : InterceptorRecord<TRequest2, TResponse2>
            where TRequest2 : class, IReturn<TResponse2>
            where TResponse2 : class, IHaveResponseStatus
        {
            public FactoryInterceptorRecord(int ordinal, Type type) : base(ordinal, type)
            {
            }

            public override InterceptorBase<TRequest2, TResponse2> GetInterceptor(ExecuteDelegate<TRequest2, TResponse2> core)
            {
                return new InterceptorBase<TRequest2, TResponse2>(async (a, r) => await Execute(core, a, r).ConfigureAwait(false));
            }

            private Task<TResponse2?> Execute(ExecuteDelegate<TRequest2, TResponse2> core, TRequest2 a, CancellationToken r)
            {
                var instance = Activator.CreateInstance(Type) as InterceptorFactoryBase;
                return instance?.ExecuteAsync(core, a, r) ?? Task.FromResult<TResponse2>(default);
            }
        }

        private abstract class InterceptorRecord<TRequest2, TResponse2>
             where TRequest2 : class, IReturn<TResponse2>
             where TResponse2 : class, IHaveResponseStatus
        {
            protected InterceptorRecord(int ordinal, Type type)
            {
                Ordinal = ordinal;
                Type = type;
            }

            public int Ordinal { get; }
            public Type Type { get; }

            [DebuggerHidden]
            public abstract InterceptorBase<TRequest2, TResponse2>? GetInterceptor(ExecuteDelegate<TRequest2, TResponse2> core);
        }

        private class ProxyInterceptorRecord<TRequest2, TResponse2> : InterceptorRecord<TRequest2, TResponse2>
            where TRequest2 : class, IReturn<TResponse2>
            where TResponse2 : class, IHaveResponseStatus
        {
            public ProxyInterceptorRecord(int ordinal, Type type) : base(ordinal, type) { }

            [DebuggerHidden]
            public override InterceptorBase<TRequest2, TResponse2>? GetInterceptor(ExecuteDelegate<TRequest2, TResponse2> core) => 
                Activator.CreateInstance(Type, new[] { core }) as InterceptorBase<TRequest2, TResponse2>;
        }
    }
}