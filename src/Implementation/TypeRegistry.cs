// Copyright (c) TruthShield, LLC. All rights reserved.

using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Applinate
{
    [DebuggerStepThrough]
    public static class TypeRegistry
    {
        private static readonly object _InitializedLock = new object();
        private static bool _LoadFromDisk = true;
        private static Type[]? _Types;
        public static bool LoadFromDisk { get { return _LoadFromDisk; }
            set { 
            
                if(_Types is null)
                {
                    _LoadFromDisk = value;
                }

                if(value == _LoadFromDisk)
                {
                    return;
                }

                throw new InvalidOperationException("types have alredy been loaded, can not change loading strategy post-load");            
            }
        }

        [STAThread]
        public static IEnumerable<Type> GetTypes()
        {
            if (_Types is not null)
            {
                return _Types;
            }

            lock (_InitializedLock)
            {
                if (_Types is not null)
                {
                    return _Types;
                }

                var serviceAssemblies = _LoadFromDisk? GetServiceAssemblies() : GetDirectAssemblies();

                _Types = serviceAssemblies.SelectMany(x => x.GetTypes()).Where(IsNotAnonymousType).Distinct().ToArray();

                return _Types;
            }
        }

        public static Boolean IsNotAnonymousType(Type type)
        {
            Boolean hasCompilerGeneratedAttribute = type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Any();
            Boolean nameContainsAnonymousType = type.FullName?.Contains("AnonymousType", StringComparison.OrdinalIgnoreCase) ?? false;
            Boolean isAnonymousType = hasCompilerGeneratedAttribute && nameContainsAnonymousType;

            return !isAnonymousType;
        }

        private static Assembly[] GetDirectAssemblies()
        {
            var returnAssemblies = new List<Assembly>();
            var loadedAssemblies = new HashSet<string>(StringComparer.Ordinal);
            var assembliesToCheck = new Queue<Assembly>();

            var runtimeLibraries = Microsoft.Extensions.DependencyModel.DependencyContext.Default.RuntimeLibraries;
            var libs =
                runtimeLibraries.Where(x => IsServiceAssembly(x.Name)).Select(x => new AssemblyName(x.Name))
                .Union(
                runtimeLibraries
                .SelectMany(x => x.Dependencies).Where(x => IsServiceAssembly(x.Name))
                .Select(x => new AssemblyName(x.Name)))
                .Distinct()
                .Select(x => Assembly.Load(x))
                .ToArray();

            

            foreach(var a in libs)
            {
                if(a is null)
                {
                    continue;
                }

                returnAssemblies.Add(a);
                loadedAssemblies.Add(a.FullName ?? String.Empty);
                assembliesToCheck.Enqueue(a);
            }

            assembliesToCheck.Enqueue(Assembly.GetEntryAssembly());

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
            Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
                .Select(x => AssemblyName.GetAssemblyName(x))
                .Where(IsServiceAssembly)
                .Distinct()
                .Select(x => Assembly.Load(x))
                .ToArray();

        static bool IsServiceAssembly(AssemblyName a) => 
            IsServiceAssembly(a?.Name ?? String.Empty);

        private static bool IsServiceAssembly(string name) => 
            name.IndexOf(".Integrate.", StringComparison.OrdinalIgnoreCase) >= 0 ||
            name.IndexOf("Applinate", StringComparison.OrdinalIgnoreCase) >= 0 ||
            name.IndexOf(".Calculate.", StringComparison.OrdinalIgnoreCase) >= 0 ||
            name.IndexOf(".Orchestrate.", StringComparison.OrdinalIgnoreCase) >= 0;
    }
}