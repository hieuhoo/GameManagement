namespace GameManagement.Share.ClassData
{
	public class PurchaseGameResultData
	{
		public bool IsSuccess { get; set; }
		public string? FullName { get; set; }
        public string? GameName { get; set; }
        public string? TransactionId { get; set; }
        public string? PurchaseDate { get; set; } // show chuỗi 
        public string? OriginalPrice { get; set; }
        public string? DiscountPercent { get; set; }
        public string? PurchasePrice { get; set; }

    }
}
