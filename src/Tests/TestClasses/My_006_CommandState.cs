// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    public class My_006_CommandState
    {
        public RequestContext CommandContext { get; set; }
        public bool Entry { get; set; }
        public string ExecutorFullName { get; set; }

        public override string ToString()
        {
            return $@"{CommandContext.RequestCallCount} {(Entry ? "Entry" : "Exit")} {ExecutorFullName} ";
        }
    }
}