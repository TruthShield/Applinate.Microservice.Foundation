// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    public enum ServiceType
    {
        None,
        /// <summary>
        /// This service orchestrates other services to fufill a use case
        /// </summary>
        Orchestration,
        /// <summary>
        /// This service encapsulates reusable domain logic in Tool, Integrator, and Worker services
        /// </summary>
        Calculation,
        /// <summary>
        /// This is a service used to integrate with external systems such as databases and apis
        /// </summary>
        Integration,
        /// <summary>
        /// This service is a domain-agnostic tool used by any other service
        /// </summary>
        Tool,
        /// <summary>
        /// This is a client used to send requests to an orchestration service
        /// </summary>
        Client
    }
}