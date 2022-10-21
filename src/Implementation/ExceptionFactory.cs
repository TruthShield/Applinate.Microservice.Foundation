// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    internal static class ExceptionFactory
    {
        private static readonly Dictionary<Type, string> _typeToFriendlyName = 
            new Dictionary<Type, string>
            {
                { typeof(string) , "string"  },
                { typeof(object) , "object"  },
                { typeof(bool)   , "bool"    },
                { typeof(byte)   , "byte"    },
                { typeof(char)   , "char"    },
                { typeof(decimal), "decimal" },
                { typeof(double) , "double"  },
                { typeof(short)  , "short"   },
                { typeof(int)    , "int"     },
                { typeof(long)   , "long"    },
                { typeof(sbyte)  , "sbyte"   },
                { typeof(float)  , "float"   },
                { typeof(ushort) , "ushort"  },
                { typeof(uint)   , "uint"    },
                { typeof(ulong)  , "ulong"   },
                { typeof(void)   , "void"    }
            };

        public static Exception CommandContextUnknown() => new InvalidOperationException($@"
>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
The current service context is unknown... services should only be executed from
another client, manager, access, utility, or engine service.
>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

If you are calling from an API, you need to initialize the infrastucture by 
using a client as follows:

new ServiceClient(UserProfileId, AppContextKeys.MyAppContext);


");

        public static Exception InvalidCallingContext(
            ServiceType currentCommandType,
            ServiceType commandType,
            string accepted) =>
            new InvalidOperationException(@$"
>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
The current calling service context is {currentCommandType} and the
target command is {commandType}.

Only exection of {accepted} services are allowed from {commandType} services.
>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
");

        public static Exception NoRegisteredService<TService>() =>
            typeof(TService).FullName switch
            {
                "Microsoft.Extensions.Configuration.IConfiguration" => NoConfigurationService(),
                _ => NoGeneralRegisteredService<TService>()
            };

        private static Exception NoConfigurationService() =>
            new InvalidOperationException($@"
>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
No service has been registered for type Microsoft.Extensions.Configuration.IConfiguration.

If you are in a windows app you can fix it with the following steps:

Install Microsoft.Extensions.Hosting package.

Add an appsettings.json file to the project root, set its build action to Content and Copy to output directory to Always.

Modify the program class:

-----------------------------------------------------------------------------------

static class Program
{{
    public static IConfiguration Configuration;
    static void Main(string[] args)
    {{
        ApplicationConfiguration.Initialize();
        Service.Register<IConfiguration, ConfigurationManager>();
        LazyAppInitializer.Initialize(Service.ServiceCollection);            
        Application.Run(new Form1());
    }}
}}
-----------------------------------------------------------------------------------
");

        private static Exception NoGeneralRegisteredService<TService>() =>
            new InvalidOperationException($@"
>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
no service has been registered for type {typeof(TService).FullName}.

Add this by using the Initialization utility and registering on startup with is code: services.AddSingleton<{typeof(TService).GetFriendlyName()}, MyImplementation>();.

This is done on application initialization, usually in a class that inherits from 'IInitialize'

example:

-----------------------------------------------------------------------------------
internal sealed class MyLazyInitializer : IInitialize
{{
    public void Initialize(IServiceCollection services, bool testing = false)
    {{
        services.AddSingleton<{typeof(TService).GetFriendlyName()}, MyImplementation>();

        // more initialization here
    }}
}}
-----------------------------------------------------------------------------------
");

        private static string GetFriendlyName(this Type type, bool includeNamespace = false)
        {
            string friendlyName;
            if (_typeToFriendlyName.TryGetValue(type, out friendlyName))
            {
                return friendlyName;
            }

            friendlyName = includeNamespace? type.FullName : type.Name;

            if (type.IsGenericType)
            {
                int backtick = friendlyName.IndexOf('`');

                if (backtick > 0)
                {
                    friendlyName = friendlyName.Remove(backtick);
                }

                friendlyName += "<";

                Type[] typeParameters = type.GetGenericArguments();

                for (int i = 0; i < typeParameters.Length; i++)
                {
                    string typeParamName = typeParameters[i].GetFriendlyName(includeNamespace);
                    friendlyName += i == 0 ? typeParamName : ", " + typeParamName;
                }

                friendlyName += ">";
            }

            if (type.IsArray)
            {
                return type.GetElementType().GetFriendlyName() + "[]";
            }

            return friendlyName;
        }

        public static Exception NoDefinedService<TArg, TResult>() =>
            new InvalidOperationException($@"
>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
no handler is defined for executing a command 
for type {typeof(TArg).GetFriendlyName(true)}:IReturns<{typeof(TResult).GetFriendlyName(true)}>{{}}.

You need to define a handler for the command that resides
in the same directory as the other assemblies (*.dll files).

-----------------------------------------------------------------------------------
[{typeof(ServiceAttribute).GetFriendlyName()}({typeof(ServiceType).GetFriendlyName()}.{nameof(ServiceType.Orchestration)})]
internal class MyHandler : IExecuteCommand<{typeof(TArg).GetFriendlyName()}, {typeof(TResult).GetFriendlyName()}>
{{

    public Task<{typeof(TResult).GetFriendlyName()}> ExecuteAsync(
            {typeof(TArg).GetFriendlyName()} arg,
            CancellationToken cancellationToken = default)
            {{ 

                // implementation

            }}
}}
-----------------------------------------------------------------------------------

If you are in unit tests and need to mock you have two options:

1) you can either define an emulator or simulator either in you test 
assembly or an assembly directly referenced by your test assembly with the signature:

-----------------------------------------------------------------------------------
[{typeof(ServiceAttribute).GetFriendlyName()}({typeof(ServiceType).GetFriendlyName()}.{nameof(ServiceType.Orchestration)})]
internal class MyEmulator : IExecuteCommand<{typeof(TArg).GetFriendlyName()}, {typeof(TResult).GetFriendlyName()}>
{{

    public Task<{typeof(TResult).GetFriendlyName()}> ExecuteAsync(
        {typeof(TArg).GetFriendlyName()} arg,
        CancellationToken cancellationToken = default)
        {{ 

            // implementation

        }}
}}
-----------------------------------------------------------------------------------

2) Alternatively, you can set the behavior for the duration of the test using the TestHelper.

-----------------------------------------------------------------------------------
pubic class MyTest
{{
    [Fact]
    public void DoesWhatIExpect()
    {{
        // set up mock
        TestHelper.MockCommandForTestDuration<{typeof(TArg).GetFriendlyName()}, {typeof(TResult).GetFriendlyName()}>(arg =>
        {{
                return new {typeof(TResult).GetFriendlyName()}(); // return mocked value 
        }});

        // execute test
    
        // ...

    }}
}}          
-----------------------------------------------------------------------------------
");


    }
}