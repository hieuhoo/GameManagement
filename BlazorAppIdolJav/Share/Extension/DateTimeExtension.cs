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
	}
}
