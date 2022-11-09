// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    public class My_006_CommandState
    {
        public RequestContext CommandContext { get; set; } = RequestContext.Empty;
        public bool Entry { get; set; }
        public string ExecutorFullName { get; set; } = string.Empty;

        public override string ToString()
        {
            return $@"{CommandContext.RequestCallCount} {(Entry ? "Entry" : "Exit")} {ExecutorFullName} ";
        }
    }
}