// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// An error where the
    /// </summary>
    public sealed class Error
    {
        public Error(string key, IDictionary<string, string>? metadata = null, IDictionary<string, string>? technicalDetails = null)
        {
            Key = key;
            Metadata = new ReadOnlyDictionary<string, string>(metadata ?? new Dictionary<string, string>(StringComparer.Ordinal));
            TechnicalDetails = new ReadOnlyDictionary<string, string>(technicalDetails ?? new Dictionary<string, string>(StringComparer.Ordinal));
        }

        /// <summary>
        /// Gets the key used to look up presentation template.
        /// </summary>
        /// <value>The key.</value>
        public string Key { get; }

        /// <summary>
        /// Gets the metadata used for variables in presentation template plus any other details.
        /// </summary>
        /// <value>The metadata.</value>
        public IDictionary<string, string> Metadata { get; }

        /// <summary>
        /// Gets any technical details requred for triage by engineers.
        /// </summary>
        /// <value>The technical details.</value>
        public IDictionary<string, string> TechnicalDetails { get; }
    }
}