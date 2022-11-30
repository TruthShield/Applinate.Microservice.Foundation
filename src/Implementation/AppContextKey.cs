// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    using System.Runtime.Serialization;

    /// <summary>
    /// This is the version of the contract
    /// defining the version of the 
    /// view-model being used
    /// </summary>
    [DataContract]
    public sealed record AppContextKey
    {
        public AppContextKey(SequentialGuid id, int major, int minor, int build) :
            this(id, new Version(major, minor, build)) { }

        public AppContextKey(SequentialGuid id, Version appModelVersion)
        {
            AppId = id;
            Version = appModelVersion.ToString();
        }

        [Newtonsoft.Json.JsonConstructor]
        [System.Text.Json.Serialization.JsonConstructor]
        internal AppContextKey(SequentialGuid id, string version)
        {
            AppId = id;
            Version = version;
        }

        public static readonly AppContextKey Empty = new(SequentialGuid.Empty, new Version(0, 0, 0));

        [DataMember]
        private string Version { get; }

        /// <summary>
        /// This is the version of the application
        /// </summary>
        public Version AppModelVersion => System.Version.Parse(Version);

        /// <summary>
        /// This is the unique identifier for the applicatoin
        /// </summary>
        [DataMember]
        public SequentialGuid AppId { get; init; }
    }
}