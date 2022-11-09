// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    using System;
    using System.Globalization;

    public record RequestStatusMessage
    {
        public RequestStatusMessage(string value, RequestStatusMessageType messageTyp)
        {
            Value = value ?? String.Empty;
            MessageType = messageTyp;
        }

        public string Value { get; }
        public RequestStatusMessageType MessageType { get; }

        public override string ToString() => $"{MessageType.Name.ToUpper(CultureInfo.CurrentCulture)}: {Value}";

        public static RequestStatusMessage Information(string message) => new(message, RequestStatusMessageType.Information);
        public static RequestStatusMessage Warning(string message) => new(message, RequestStatusMessageType.Warning);
        public static RequestStatusMessage Error(string message) => new(message, RequestStatusMessageType.Error);
    }
}