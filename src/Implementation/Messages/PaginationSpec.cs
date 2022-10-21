// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    public sealed class PaginationSpec : IEquatable<PaginationSpec>
    {
        
        public static readonly PaginationSpec SingleItem = new(1, 1);
        public static readonly PaginationSpec None = new(0, 0);

        public PaginationSpec(int pageSize, int pageNumber)
        {
            PageSize = pageSize;
            PageNumber = pageNumber;
        }

        public int PageNumber { get; }

        public int PageSize { get; }

        public static bool operator !=(PaginationSpec left, PaginationSpec right)
        {
            return !(left == right);
        }

        public static bool operator ==(PaginationSpec left, PaginationSpec right)
        {
            return ReferenceEquals(left, right) || (left is not null && left.Equals(right));
        }

        public bool Equals(PaginationSpec? other) =>
            other is not null &&
            PageSize == other.PageSize &&
            PageNumber == other.PageNumber;

        public override bool Equals(object? obj) =>
            base.Equals(obj as PaginationSpec);

        public override int GetHashCode()
        {
            return PageSize.GetHashCode() ^ PageNumber.GetHashCode();
        }
    }
}