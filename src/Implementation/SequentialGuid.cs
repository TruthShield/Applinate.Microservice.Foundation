﻿// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    using System.Buffers.Binary;
    using System.Buffers.Text;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// <see cref="SequentialGuid"/> generates sequential unique identifiers that are 128-bit (16-bytes)
    /// and fit nicely into a `Guid`.
    /// <para>
    /// The Problem:
    /// </para>
    /// <para>
    /// Many applications use unique identifiers to identify data.Common approaches applications
    /// use to generate unique identifiers in a relational database delegate identifier generation
    /// to the database, using an identity column or another similar auto-incrementing value.
    /// </para>
    /// <para>
    /// While this approach can be adequate for a small application, it quickly becomes a
    /// bottleneck at scale.And it's a common problem.
    /// </para>
    /// <para>
    /// A key use case, specifically related to distributed messaging systems, is applications
    /// that use messages to communicate between services – which is common in a service-based
    /// architecture. In these applications, sequential identifiers generated by <see cref="SequentialGuid"/> can serve
    /// dual purposes. First and foremost, it is a sequential unique identifier. Second, it is
    /// also a timestamp, as every <see cref="SequentialGuid"/> includes a UTC timestamp.
    /// </para>
    /// <para>
    /// Why does order matter?
    /// </para>
    /// <para>
    /// For a .NET developer, it is easy to use `Guid.NewGuid()`.  the problem is that the identifiers
    /// created are not sequential, and are completely randomized. And when it comes to data,
    /// being able to sort it matters. Using a _uniqueidentifier_ column as a primary key clustered
    /// index with SQL Server is frowned upon because it used to cause massive index fragmentation.
    /// This led developers to use an _int_ (or _bigint_ once they realized that four billion isn't a lot)
    /// primary key and create a separate unique index on the _uniqueidentifier_ column.
    /// </para>
    /// <para>
    /// The Solution
    /// </para>
    /// <para>
    /// <see cref="SequentialGuid"/> solves the problem because the sequential 128-bit identifiers are collation
    /// compatible with SQL Server as a clustered primary key. Using the host MAC address, along with
    /// an optional offset (in case multiple processes are on the same host), combined with a timestamp
    /// and an incrementing sequence number, generate identifiers are unique across a network of systems
    /// and can be safely inserted into a database without conflicts.
    /// </para>
    /// <para>
    /// <see cref="SequentialGuid"/>s can be generated using the factory method.
    /// </para>
    /// <code>
    /// SequentialGuid newId = SequantialGuid.New();
    /// </code>
    /// <para>
    /// You can wrap any guid in a <see cref="SequentialGuid"/>
    /// </para>
    /// <code>
    /// Guid id = Guid.New();
    /// SequentialGuid sid = new SequentialGuid(id);
    /// </code>
    /// <para>
    /// <see cref="SequentialGuid"/> can be implicitly converted to a Guid.
    /// </para>
    /// <code>
    /// Guid id = SequentialGuid.NewGuid();
    /// </code>
    /// </summary>
    public struct SequentialGuid : IEqualityComparer<SequentialGuid>, IEquatable<SequentialGuid>
    {
        public static readonly SequentialGuid Empty = SequentialGuid.From(0);

        private const int EmbedAtIndex = 10;
        private const int FixedNumDateBytes = 6;
        private const double IncrementMs = 1; // Note: An increment of 4ms is required to overcome the resolution of 1/300s of SqlDateTimeStrategy
        private const int RemainingBytesFromInt64 = 8 - FixedNumDateBytes;
        private const long TicksPerMillisecond = 10000;
        private static readonly object locker = new object();
        private static readonly DateTime MaxDateTimeValue = MinDateTimeValue.AddMilliseconds(2 ^ (8 * FixedNumDateBytes));
        private static readonly DateTime MinDateTimeValue = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        private static DateTime lastValue = DateTime.MinValue;

        internal SequentialGuid(Guid guid)
        {
            Guid = guid;
        }

        private Guid Guid { get; }

        public static SequentialGuid From(int value)
        {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes, 0);
            var guid = new Guid(bytes);
            var result = new SequentialGuid(guid);
            return result;
        }

        public static implicit operator Guid(SequentialGuid s) => s.Guid;

        public static implicit operator SequentialGuid(Guid guid) => new SequentialGuid(guid);

        public static SequentialGuid NewGuid()
        {
            var value = Guid.NewGuid();

            var timestamp = GetUniqueTimestamp();

            Span<byte> gbytes = stackalloc byte[16];
            _ = value.TryWriteBytes(gbytes);
            WriteDateTime(gbytes[EmbedAtIndex..], timestamp);

            return new SequentialGuid(new Guid(gbytes));
        }

        public static Boolean operator !=(SequentialGuid x, SequentialGuid y) => x.Guid != y.Guid;

        public static Boolean operator !=(Guid x, SequentialGuid y) => x != y.Guid;

        public static Boolean operator !=(SequentialGuid x, Guid y) => x.Guid != y;

        public static Boolean operator ==(SequentialGuid x, SequentialGuid y) => x.Guid == y.Guid;

        public static Boolean operator ==(Guid x, SequentialGuid y) => x == y.Guid;

        public static Boolean operator ==(SequentialGuid x, Guid y) => x.Guid == y;

        public static SequentialGuid Parse(string value) => new SequentialGuid(Guid.Parse(value));

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (!(obj is SequentialGuid))
            {
                return false;
            }

            return Equals((SequentialGuid)obj);
        }

        public bool Equals(SequentialGuid x, SequentialGuid y) => x.Guid == y.Guid;

        public bool Equals(SequentialGuid other) => Guid == other.Guid;

        public int GetHashCode([DisallowNull] SequentialGuid obj) => obj.Guid.GetHashCode();

        public override int GetHashCode() => Guid.GetHashCode();

        public override string ToString()
        {
            return Guid.ToString();
        }

        public string ToString(string? format)
        {
            return Guid.ToString(format);
        }

        public DateTime ToTimestamp(Guid comb)
        {
            Span<byte> gbytes = stackalloc byte[16];
            _ = comb.TryWriteBytes(gbytes);

            return ReadDateTime(gbytes[EmbedAtIndex..]);
        }

        private static DateTime FromMilliseconds(long ms) => MinDateTimeValue.AddTicks(ms * TicksPerMillisecond);

        private static DateTime GetUniqueTimestamp()
        {
            var now = DateTime.UtcNow;
            lock (locker)
            {
                var timeDifferenceDoesNotMeetIncrementThreshold = (now - lastValue).TotalMilliseconds < IncrementMs;

                if (timeDifferenceDoesNotMeetIncrementThreshold)
                {
                    now = lastValue.AddMilliseconds(IncrementMs);
                }

                lastValue = now;
            }

            return now;
        }

        private static DateTime ReadDateTime(ReadOnlySpan<byte> source)
        {
            Span<byte> msBytes = stackalloc byte[8];
            source[..FixedNumDateBytes].CopyTo(msBytes[RemainingBytesFromInt64..]);
            msBytes[..RemainingBytesFromInt64].Clear();
            var ms = BinaryPrimitives.ReadInt64BigEndian(msBytes);
            return FromMilliseconds(ms);
        }

        private static long ToMilliseconds(DateTime timestamp) => (timestamp.Ticks - MinDateTimeValue.Ticks) / TicksPerMillisecond;

        private static void WriteDateTime(Span<byte> destination, DateTime timestamp)
        {
            var ms = ToMilliseconds(timestamp);
            Span<byte> msBytes = stackalloc byte[8];
            BinaryPrimitives.WriteInt64BigEndian(msBytes, ms);
            msBytes[RemainingBytesFromInt64..].CopyTo(destination);
        }

        public Guid ToGuid() => this;
    }
}