// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    public sealed class Result<T> 
    {
        public Result(T value, bool isSuccess, string[] messages)
        {
            Value = value;
            IsSuccess = isSuccess;
            Messages = messages;
        }

        public T Value { get; }
        public bool IsSuccess { get; }
        public string[] Messages { get; }
    }
}