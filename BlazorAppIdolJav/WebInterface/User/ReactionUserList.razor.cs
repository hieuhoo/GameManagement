using GameManagement.Share.ClassData;
using Microsoft.AspNetCore.Components;

namespace GameManagement.WebInterface.User
{
    public partial class ReactionUserList
    {
        [Parameter] public IEnumerable<ReactionPersonInfoData> ReactionUsers { get; set; } = Enumerable.Empty<ReactionPersonInfoData>();

        [Parameter] public string EmptyText { get; set; } = "Chưa có lượt cảm xúc nào";

        List<ReactionPersonInfoData> DisplayUsers => ReactionUsers.Take(5).ToList();

        int RemainingCount => Math.Max(ReactionUsers.Count() - 5, 0);


        string GetUserInitial(string? userName, bool anonymous)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                userName = "?";
            }
            var stringResult = anonymous == true ? "?" : userName
                                                                            .Trim()
                                                                            .Substring(0, 1)
                                                                            .ToUpper();
            return stringResult;
        }
    }
}
