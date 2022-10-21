// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    public sealed record RequestContextChangeType
    {
        public string Name { get; }

        public RequestContextChangeType(string name)
        {
            Name = name;
        }

        public static readonly RequestContextChangeType Entry = new("Entry");
        public static readonly RequestContextChangeType Exit = new("Exit");
        public static readonly RequestContextChangeType Fault = new("Fault");

        public override string ToString()
        {
            return Name;
        }
    }
}