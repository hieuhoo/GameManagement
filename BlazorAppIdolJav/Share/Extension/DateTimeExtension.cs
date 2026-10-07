namespace GameManagement.Share.Extension
{
	public static class DateTimeExtension
	{
		public static string GetFormatDateWithMinute (this DateTime dateTime)
		{
			return dateTime.ToString("dd/MM/yyyy HH:mm");
		}

		public static string GetFormatDateWithSecond (this DateTime dateTime)
		{
			return dateTime.ToString("dd/MM/yyyy HH:mm:ss");
		}

		public static string GetFormatBasicDate (this DateTime dateTime)
		{
			return dateTime.ToString("dd/MM/yyyy");
		}

		public static string GetRelativeTime(DateTime date)
		{
			var diff = DateTime.Now - date;
			if (diff.TotalSeconds < 60)
			{
				return "Vừa xong";
			}
			if (diff.TotalMinutes < 60)
			{
				return $"{(int)diff.TotalMinutes} phút trước";
			}
			if (diff.TotalHours < 24)
			{
				return $"{(int)diff.TotalHours} giờ trước";
			}
			if (diff.TotalDays < 7)
			{
				return $"{(int)diff.TotalDays} ngày trước";
			}
			if (diff.TotalDays < 30)
			{
				return $"{(int)(diff.TotalDays / 7)} tuần trước";
			}
			if (diff.TotalDays < 365)
			{
				return $"{(int)(diff.TotalDays / 30)} tháng trước";
			}
			return $"{(int)(diff.TotalDays / 365)} năm trước";
		}
	}
}
