namespace GameManagement.Share.ClassDB
{
    public class RefreshToken
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;

        // Chỉ lưu hash của refresh token, không lưu token thật trong DB.
        public string TokenHash { get; set; } = string.Empty;

        public DateTime CreateDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public DateTime? RevokedDate { get; set; }
        public string? ReplacedByTokenHash { get; set; }

        public bool IsRevoked => RevokedDate.HasValue;
        public bool IsExpired => DateTime.UtcNow >= ExpiredDate;
        public bool IsActive => !IsRevoked && !IsExpired;
    }
}
