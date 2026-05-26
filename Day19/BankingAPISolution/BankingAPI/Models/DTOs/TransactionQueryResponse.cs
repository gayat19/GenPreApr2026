namespace BankingAPI.Models.DTOs
{
    public class TransactionQueryResponse
    {
        public IEnumerable<TransactionResponse> Items { get; set; } = new List<TransactionResponse>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}
