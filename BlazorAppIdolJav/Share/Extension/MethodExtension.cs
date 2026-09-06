using GameManagement.CoreConfig.Extensions;
using GameManagement.Share.ClassData;
using GameManagement.Share.Extension;
using static GameManagement.Share.Extension.EnumExtension;

namespace GameManagement.Share.Extension
{
	// chứa các hàm dùng chung của các class data ở nhiều nơi, ko bị lặp lại
	public static class MethodExtension
	{
		public static UserLockHistoryData FillLockHistoryData(
		  this string userId,
		  AccountOperation action,
		  LockPerson performedBy,
		  AccountLockReason? reason = null)
		{
			return new UserLockHistoryData
			{
				Id = ObjectExtentions.GenerateGuid(),
				UserId = userId,
				CreateDate = DateTime.Now,
				Action = action.ToString(),
				PerformedBy = performedBy.ToString(),
				Reason = reason?.ToString()
			};
		}

        public static UserPasswordHistoryData FillPasswordHistoryData(
          this string userId,
          string currentPassword ,
          string? previousPassword = null)
        {
            return new UserPasswordHistoryData
            {
                Id = ObjectExtentions.GenerateGuid(),
                UserId = userId,
                CreateDate = DateTime.Now,
                CurrentPassword = currentPassword,
                CurrentPasswordHash = StringExtension.GetCharacterHash(currentPassword),
                PreviousPassword = previousPassword
            };
        }

    }
}