namespace BankingAPI.Models.DTOs
{
    public class TransactionQueryRequest
    {
        // Optional filters
        public string? FromAccountNumber { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Sorting: e.g. SortBy = "TransactionDate" or "Amount", SortDirection = "asc"|"desc"
        public string SortBy { get; set; } = "TransactionDate";
        public string SortDirection { get; set; } = "desc";
    }
}