// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    internal sealed class My_010_Service : IRequestHandler<My_010_UnitTestRequestParams, My_010_UnitTestResponse>
    {
        public Task<My_010_UnitTestResponse> ExecuteAsync(My_010_UnitTestRequestParams arg, CancellationToken cancellationToken = default)
        {
             return Task.FromResult( new My_010_UnitTestResponse()
             { 
                 
             });
        }
    }
}