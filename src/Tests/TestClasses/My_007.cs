// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    public class My_007_InterceptorFactory : InterceptorFactoryBase
    {
        public override Task<TResult> ExecuteAsync<TArg, TResult>(ExecuteDelegate<TArg, TResult> next, TArg arg, CancellationToken cancellationToken)
        {
            var newArg = Update(arg);
            return base.ExecuteAsync(next, newArg, cancellationToken);
        }

        private static My_007_UnitTestCommand Update(My_007_UnitTestCommand val)
        {
            return new My_007_UnitTestCommand
            {
                PostNumber = val.PostNumber + 1,
                PreNumber = val.PreNumber + 1
            };
        }

        private static TArg Update<TArg>(TArg arg)
        {
            if (arg is My_007_UnitTestCommand z)
            {
                return Update(z).As<TArg>();
            }

            return arg;
        }
    }

    [Service(ServiceType.Calculation)]
    public class My_007_UnitTestCommand : IReturn<My_007_UnitTestCommandResult>
    {
        public int PostNumber { get; set; }
        public int PreNumber { get; set; }
    }

    public class My_007_UnitTestCommandResult:IHaveRequestStatus
    {
        public int PostNumber { get; set; }
        public int PreNumber { get; set; }

        public RequestStatus Status => throw new System.NotImplementedException();
    }

    [Service(ServiceType.Calculation)]
    public class My_007_UntiTestCommandExecutor : IHandleRequest<My_007_UnitTestCommand, My_007_UnitTestCommandResult>
    {
        public Task<My_007_UnitTestCommandResult> ExecuteAsync(
            My_007_UnitTestCommand arg,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new My_007_UnitTestCommandResult
            {
                PreNumber = arg.PreNumber,
                PostNumber = arg.PostNumber
            });
        }
    }
}