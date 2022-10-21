# Applinate Architecture Overview

The aim of `Applinate` is to provide a complete microservice development platform anyone can use with many of the core building blocks already done for you.

Structurally, `Applinate` uses a similar approach as the popular [MediatR](https://github.com/jbogard/MediatR) package, but with more focus on helping you be more productive with on microservice development.  

`Applinate` provides a more prescriptive service taxonomy, more tooling, and conventions to separate your plumbing code from your business code.  

Also, `Applinate` aims to provide you with a complete library of reusable building blocks.  You should have less to figure out, less to code, and drastically reduce your cost and time to develop new products.

And without locking you in to any particular technology.  For example, you can use the `Applinate.Encryption` nuget package to use our encryption libraries, or plug in whatever encryption library you want just by implementing the encryption interface.

## Service Taxonomy

The `Applinate` architecture separates your concerns into distinct service types used to encapsulate the volatility of different aspects of your system.

We starts with a traditional separation of concerns between presentation, logic, and storage. (No sense reinventing the wheel, right?)

Each layer may have one or more services.

On top of our service conventions, the bulk of `Applinate` consists of reusable tools that any service can use, such as encryption, compression, messaging (pub-sub), caching, etc.

### Presentation Tier

`Applinate` does not deal with the presentation concerns directly.

You can use any front-end technology on top of the `Applinate` architecture.

### Logic Tier

We separate logic into two distinct types for facilitating microservice development: `orchestration` services and `work` services.

#### Orchestration Services

Orchestration services handle incoming requests and orchestrate all sub-services to fulfill your use cases.

> Note: The majority of your business logic will likely be in orchestration services.

Orchestration services are purposfully stateless.

#### Work Services

Work services encapsulate reusable blocks of domain-specific logic.  They are also designed to be stateless.

### Storage Layer

We have extended the storage layer to include any external systems to your platform.  This includes external APIs and databases.

#### Integration Services

The integration services encapsulate the volatility of integration with external systems.

### Tools

The majority of the `Applinate` code base is reusable tools for every service type.

#### Encryption

```csharp
public class X{}
```

