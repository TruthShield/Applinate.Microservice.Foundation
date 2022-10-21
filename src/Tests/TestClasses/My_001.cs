// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate.Foundation.Test
{
    [Service(ServiceType.Orchestration)]
    public class My_001_UnitTestCommand : IReturn<My_001_UnitTestCommandResult>
    {
        public int PostNumber { get; set; }
        public int PreNumber { get; set; }
    }

    public class My_001_UnitTestCommandResult : IHaveRequestStatus
    {
        public int PostNumber { get; set; }
        public int PreNumber { get; set; }

        public RequestStatus Status => throw new NotImplementedException();
    }

    [Service(ServiceType.Orchestration)]
    public class My_001_UntiTestCommandExecutor : IHandleRequest<My_001_UnitTestCommand, My_001_UnitTestCommandResult>
    {
        public Task<My_001_UnitTestCommandResult> ExecuteAsync(
            My_001_UnitTestCommand arg,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new My_001_UnitTestCommandResult
            {
                PreNumber = arg.PreNumber,
                PostNumber = arg.PostNumber
            });
        }
    }
}