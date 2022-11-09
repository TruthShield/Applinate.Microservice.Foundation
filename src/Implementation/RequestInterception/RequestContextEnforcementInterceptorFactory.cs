// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Commands.Interceptors
{
    using Applinate;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    [Intercept(-2000000000)]
    internal class RequestContextEnforcementInterceptorFactory : InterceptorFactoryBase
    {
        private static readonly IReadOnlyDictionary<ServiceType, ServiceType[]> _Allowed =
            new ReadOnlyDictionary<ServiceType, ServiceType[]>(
                new Dictionary<ServiceType, ServiceType[]>()
                {
                    { ServiceType.Client, new[]{ ServiceType.Orchestration, ServiceType.Tool } },
                    { ServiceType.Orchestration, new[]{ ServiceType.Calculation, ServiceType.Integration, ServiceType.Tool } },
                    { ServiceType.Calculation, new[]{ ServiceType.Integration, ServiceType.Tool} },
                    { ServiceType.Integration, new[]{ ServiceType.Tool} }
                });

        public override Task<TResponse> ExecuteAsync<TRequest, TResponse>(
            ExecuteDelegate<TRequest, TResponse> next,
            TRequest request,
            CancellationToken cancellationToken)
        {
            var currentServiceType = RequestContext.Current?.ServiceType ?? ServiceType.None;

            if (currentServiceType == ServiceType.None)
            {
                throw ExceptionFactory.CommandContextUnknown();
            }

            var commandType = typeof(TRequest).GetCustomAttribute<ServiceRequestAttribute>()?.CommandType ?? ServiceType.None;

            if (_Allowed[currentServiceType].Contains(commandType))
            {
                return next(request, cancellationToken);
            }

            var accepted = string.Join(", ", _Allowed[currentServiceType].Select(x => x.ToString()).ToArray());


            throw ExceptionFactory.InvalidCallingContext(currentServiceType, commandType, accepted);
        }


    }
}