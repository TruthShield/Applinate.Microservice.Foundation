// Copyright (c) TruthShield, LLC. All rights reserved.

using Microsoft.Extensions.DependencyModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Applinate
{
    public static class TypeRegistry
    {
        private static readonly Lazy<Type[]> _MyClasses = new Lazy<Type[]>(() => Types.Where(x => x.IsClass).ToArray());
        private static readonly Lazy<Type[]> _MyInitializers = new Lazy<Type[]>(GetAllInitializers);
        private static readonly Lazy<Type[]> _MyServiceFactories = new Lazy<Type[]>(GetServiceFactories);
        private static readonly Lazy<Type[]> _MyTypes = new Lazy<Type[]>(GetTypes);

        private static bool _LoadFromDisk = true;
        public static Type[] Classes => _MyClasses.Value;

        public static bool LoadFromDisk
        {
            get
            {
                return _LoadFromDisk;
            }
            set
            {
                if (!_MyTypes.IsValueCreated)
                {
                    _LoadFromDisk = value;
                }

                if (value == _LoadFromDisk)
                {
                    return;
                }

                throw new InvalidOperationException("types have alredy been loaded, can not change loading strategy post-load");
            }
        }

        public static Type[] Types => _MyTypes.Value;

        internal static Type[] Initializers => _MyInitializers.Value;

        internal static Type[] ServiceFactories => _MyServiceFactories.Value;

        public static Boolean IsNotAnonymousType(Type type)
        {
            var hasCompilerGeneratedAttribute = type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Any();
            var nameContainsAnonymousType = type.FullName?.Contains("AnonymousType", StringComparison.OrdinalIgnoreCase) ?? false;
            var isAnonymousType = hasCompilerGeneratedAttribute && nameContainsAnonymousType;

            return !isAnonymousType;
        }

        private static Type[] GetAllInitializers() =>
            (from x in TypeRegistry.Types
             where x.IsClass && x.IsAssignableTo(typeof(IInitialize))
             let ordinal = x.GetCustomAttribute<InitializationPriorityAttribute>()?.Ordinal ?? int.MaxValue
             orderby ordinal ascending
             select x)
            .ToArray();

        private static Assembly[] GetDirectAssemblies()
        {
            var returnAssemblies = new List<Assembly>();
            var loadedAssemblies = new HashSet<string>(StringComparer.Ordinal);
            var assembliesToCheck = new Queue<Assembly>();
            var runtimeLibraries = DependencyContext.Default.RuntimeLibraries;

            var libs =
                runtimeLibraries.Where(x => IsServiceAssembly(x.Name)).Select(x => new AssemblyName(x.Name))
                .Union(
                runtimeLibraries
                .SelectMany(x => x.Dependencies).Where(x => IsServiceAssembly(x.Name))
                .Select(x => new AssemblyName(x.Name)))
                .Distinct()
                .Select(x => Assembly.Load(x))
                .ToArray();

            foreach (var a in libs)
            {
                if (a is null)
                {
                    continue;
                }

                returnAssemblies.Add(a);
                loadedAssemblies.Add(a.FullName ?? String.Empty);
                assembliesToCheck.Enqueue(a);
            }

            assembliesToCheck.Enqueue(Assembly.GetEntryAssembly() ?? throw ExceptionFactory.UnexpectedNull());

            while (assembliesToCheck.Any())
            {
                var assemblyToCheck = assembliesToCheck.Dequeue();

                foreach (var reference in assemblyToCheck.GetReferencedAssemblies())
                {
                    if (IsServiceAssembly(reference) && !loadedAssemblies.Contains(reference.FullName))
                    {
                        var assembly = Assembly.Load(reference);
                        assembliesToCheck.Enqueue(assembly);
                        loadedAssemblies.Add(reference.FullName);
                        returnAssemblies.Add(assembly);
                    }
                }
            }

            return returnAssemblies.ToArray();
        }

        private static Assembly[] GetServiceAssemblies() =>
            Directory
            .GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
            .Select(x => AssemblyName.GetAssemblyName(x))
            .Where(IsServiceAssembly)
            .Distinct()
            .Select(x => Assembly.Load(x))
            .ToArray();

        private static Type[] GetServiceFactories() =>
            (from x in TypeRegistry.Types
            where
            x.IsClass && x.IsAssignableTo(typeof(IInstanceRegistry)) &&
            x != typeof(EmptyInstanceRegistry)
            select x)
                .ToArray();

        private static Type[] GetTypes() =>
           (_LoadFromDisk ? GetServiceAssemblies() : GetDirectAssemblies())
            .SelectMany(x => x.GetTypes()).Where(IsNotAnonymousType)
            .Distinct()
            .ToArray();

        private static bool IsServiceAssembly(AssemblyName a) =>
            IsServiceAssembly(a?.Name ?? String.Empty);

        private static bool IsServiceAssembly(string name) =>
            name.IndexOf("Applinate", StringComparison.OrdinalIgnoreCase) >= 0 ||
            name.IndexOf(".Integrate.", StringComparison.OrdinalIgnoreCase) >= 0 ||
            name.IndexOf(".Integration.", StringComparison.OrdinalIgnoreCase) >= 0 ||
            name.IndexOf(".Calculate.", StringComparison.OrdinalIgnoreCase) >= 0 ||
            name.IndexOf(".Calculation.", StringComparison.OrdinalIgnoreCase) >= 0 ||
            name.IndexOf(".Orchestrate.", StringComparison.OrdinalIgnoreCase) >= 0 ||
            name.IndexOf(".Orchestration.", StringComparison.OrdinalIgnoreCase) >= 0;
    }
}