// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    public static class ErrorExtensions
    {
        public static Error ToError(this Exception ex, string key, IDictionary<string, string>? metadata = null) => new Error(key, metadata, GetTechnicalDetail(ex));

        private static IDictionary<string, string> GetTechnicalDetail(Exception ex) => GetTechnicalDetailValues(ex).ToDictionary(x => x.Item1, x => x.Item2, StringComparer.OrdinalIgnoreCase);

        private static IEnumerable<(string, string)> GetTechnicalDetailValues(this Exception ex)
        {
            var i = 0;

            yield return ("exception-message", ex.Message);

            if (ex.StackTrace is not null)
            {
                yield return ("stack-trace", ex.StackTrace);
            }

            var inner = ex.InnerException;

            while (inner is not null)
            {
                i++;

                yield return ("exception-message-" + i, inner.Message);

                if (inner.StackTrace is not null)
                {
                    yield return ("stack-trace-" + i, inner.StackTrace);
                }

                inner = inner.InnerException;
            }
        }
    }
}