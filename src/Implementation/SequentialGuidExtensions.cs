// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    using System.Buffers.Text;
    using System.Runtime.InteropServices;
    using System.Text;

    public static class SequentialGuidExtensions
    {
        private static readonly byte ForwardSlashByte = (byte)'/';
        private static readonly byte DashByte = (byte)'-';
        private static readonly byte PlusByte = (byte)'+';
        private static readonly byte UnderscoreByte = (byte)'_';

        public static string ToCompactString(this Guid guid) => new SequentialGuid(guid).ToCompactString();

        /// <summary>
        /// Base64 encoding for the <see cref="SequentialGuid"/>
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static string ToCompactString(this SequentialGuid guid)
        {
            Span<byte> guidBytes = stackalloc byte[16];
            Span<byte> encodedBytes = stackalloc byte[24];

            MemoryMarshal.TryWrite(guidBytes, ref guid); // write bytes from the Guid
            Base64.EncodeToUtf8(guidBytes, encodedBytes, out _, out _);

            // replace any characters which are not URL safe
            for (var i = 0; i < 22; i++)
            {
                if (encodedBytes[i] == ForwardSlashByte)
                    encodedBytes[i] = DashByte;

                if (encodedBytes[i] == PlusByte)
                    encodedBytes[i] = UnderscoreByte;
            }

            // skip the last two bytes as these will be '==' padding
            var final = Encoding.UTF8.GetString(encodedBytes[..22]);

            return final;
        }

        public static SequentialGuid FromCompactStringToSequentialGuid(this string compactGuidString)
        {
            compactGuidString ??= string.Empty;

            Span<byte> guidBytes = stackalloc byte[16];
            Span<byte> encodedBytes = stackalloc byte[24];

            var enc = Encoding.UTF8.GetBytes(compactGuidString + "==");

            for (var i = 0; i < enc.Length; i++)
            {
                encodedBytes[i] = enc[i];
            }

            // replace any characters which are not URL safe
            for (var i = 0; i < 22; i++)
            {
                if (encodedBytes[i] == DashByte)
                    encodedBytes[i] = ForwardSlashByte;

                if (encodedBytes[i] == UnderscoreByte)
                    encodedBytes[i] = PlusByte;
            }

            Base64.DecodeFromUtf8(encodedBytes, guidBytes, out _, out _);

            var final = new Guid(guidBytes);

            return final;
        }
    }
}