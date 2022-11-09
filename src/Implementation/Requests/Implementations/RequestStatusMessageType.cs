// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    public record RequestStatusMessageType
    {
        public string Name { get; }

        public static readonly RequestStatusMessageType Information = new("Info");
        public static readonly RequestStatusMessageType Warning = new("Warn");
        public static readonly RequestStatusMessageType Error = new("Err");

        public RequestStatusMessageType(string name)
        {
            Name = name;
        }
    }
}