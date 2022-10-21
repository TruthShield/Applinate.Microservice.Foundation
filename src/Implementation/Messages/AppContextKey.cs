// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    /// <summary>
    /// This is the version of the contract
    /// defining the version of the 
    /// view-model being used
    /// </summary>
    [Serializable]
    public sealed class AppContextKey:IEquatable<AppContextKey>
    {
        public AppContextKey(SequentialGuid id, int major, int minor, int build) :
            this(id, new Version(major, minor, build)) { }
  

        public AppContextKey(SequentialGuid id, Version appModelVersion)
        {
            AppId = id;
            AppModelVersion = appModelVersion;
        }

        public static readonly AppContextKey Empty = new(SequentialGuid.Empty, new Version(0, 0, 0));

        /// <summary>
        /// This is the version of the application
        /// </summary>
        public Version AppModelVersion { get; }

        /// <summary>
        /// This is the unique identifier for the applicatoin
        /// </summary>
        public SequentialGuid AppId { get; }

        public bool Equals(AppContextKey? other) =>
            ReferenceEquals(this, other) ||
            (other is not null &&
            this.AppModelVersion.Equals(other.AppModelVersion) &&
            this.AppId.Equals(other.AppId));


        public override bool Equals(object? obj) => 
            Equals(obj as AppContextKey);


        public override int GetHashCode() => AppId.GetHashCode() ^ AppModelVersion.GetHashCode();
    }

}