// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    internal sealed class My_011_Service : IMy_011_Service
    {
        public Task<My_011_UnitTestResponse> HandleRequestAsync(My_011_UnitTestRequestParams arg, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new My_011_UnitTestResponse()
            {
                Value = arg.Value
            });
        }
    }
}