namespace GameManagement.Share.ClassDB
{
    public class GameRedeemCode
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public int? Value { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public DateTime? RedeemedDate { get; set; }
        public string Status { get; set; }
        public string CreateBy { get; set; }
        public string? RedeemedBy { get; set; }
        public string GenerateType { get; set; }
        public string? BatchId { get; set; } // nhận biết những code nào đc gen cùng nhau
        public string RedeemType { get; set; }
        public string? GameId { get; set; }


    }
}
