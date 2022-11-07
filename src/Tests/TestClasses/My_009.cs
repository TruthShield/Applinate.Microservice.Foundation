// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    public class My_009_InterceptorFactory : InterceptorFactoryBase
    {
        public override Task<TResult> ExecuteAsync<TArg, TResult>(ExecuteDelegate<TArg, TResult> next, TArg arg, CancellationToken cancellationToken)
        {
            var newArg = Update(arg);
            return base.ExecuteAsync(next, newArg, cancellationToken);
        }

        private static My_009_UnitTestCommand Update(My_009_UnitTestCommand val)
        {
            return new My_009_UnitTestCommand
            {
                PostNumber = val.PostNumber + 1,
                PreNumber = val.PreNumber + 1
            };
        }

        private static TArg Update<TArg>(TArg arg)
        {
            if (arg is My_009_UnitTestCommand z)
            {
                return Update(z).As<TArg>();
            }

            return arg;
        }
    }

    [ServiceRequest(ServiceType.Integration)]
    public class My_009_UnitTestCommand : IReturn<My_009_UnitTestCommandResult>
    {
        public int PostNumber { get; set; }
        public int PreNumber { get; set; }
    }

    public class My_009_UnitTestCommandResult: IHaveRequestStatus
    {
        public int PostNumber { get; set; }
        public int PreNumber { get; set; }

        public RequestStatus Status => throw new System.NotImplementedException();
    }

    [ServiceRequest(ServiceType.Integration)]
    public class My_009_UntiTestCommandExecutor : IHandleRequest<My_009_UnitTestCommand, My_009_UnitTestCommandResult>
    {
        public Task<My_009_UnitTestCommandResult> ExecuteAsync(
            My_009_UnitTestCommand arg,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new My_009_UnitTestCommandResult
            {
                PreNumber = arg.PreNumber,
                PostNumber = arg.PostNumber
            });
        }
    }
}