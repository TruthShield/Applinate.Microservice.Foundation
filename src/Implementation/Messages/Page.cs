// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    public sealed class Page
    {
        public Page(int pageSize, int pageNumber, int totalRecords)
        {
            PageSize = pageSize;
            PageNumber = pageNumber;
            TotalRecords = totalRecords;
        }

        public static readonly Page None = new(0, 0, 0);

        public int PageSize { get; }
        public int PageNumber { get; }
        public int TotalRecords { get; }
    }
}