using GameManagement.CoreConfig.Extensions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GameManagement.Share.Extension
{
	public static class StringExtension
	{
		public static string GetCharacterHash(string code)
		{
			var codeHash = BCrypt.Net.BCrypt.HashPassword(code);
			return codeHash;
		}

		public static string GetEnumDescription<TEnum>(this string value)
	where TEnum : struct, Enum
		{
			if (string.IsNullOrWhiteSpace(value))
				return string.Empty;

			if (Enum.TryParse<TEnum>(value, true, out var enumValue))
			{
				return enumValue.GetDescription();
			}

			return value;
		}

		public static string TruncateText(string? text, int maxLength = 50)
		{
			if (string.IsNullOrEmpty(text))
				return string.Empty;

			return text.Length > maxLength
				? text.Substring(0, maxLength) + "..."
				: text;
		}
	}
}
