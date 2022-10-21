// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    using System.Text;

    public static class StringExtensions
    {
        private static readonly char[] Delimeters = { ' ', '-', '_' };

        private static char[] GetSymbols(char s, bool disableFrontDelimeter)
        {
            if (disableFrontDelimeter)
            {
                return new char[] { char.ToLowerInvariant(s) };
            }

            return new char[] { '-', char.ToLowerInvariant(s) };
        }

        public static string ToKebabCase(this string source)
        {
            var builder                = new StringBuilder();
            bool nextIsNewWord         = true;
            bool disableFrontDelimeter = true;

            for (var i = 0; i < source.Length; i++)
            {
                var currentChar = source[i];

                if (Delimeters.Contains(currentChar))
                {
                    if (currentChar == '-')
                    {
                        builder.Append(currentChar);
                        disableFrontDelimeter = true;
                    }

                    nextIsNewWord = true;
                    continue;
                }

                if (!char.IsLetterOrDigit(currentChar))
                {
                    builder.Append(currentChar);
                    disableFrontDelimeter = true;
                    nextIsNewWord = true;
                    continue;
                }

                if (nextIsNewWord || char.IsUpper(currentChar))
                {
                    builder.Append(GetSymbols(currentChar, disableFrontDelimeter));
                    disableFrontDelimeter = false;
                    nextIsNewWord = false;
                    continue;
                }

                builder.Append(currentChar);
            }

            return builder.ToString();
        }
    }
}