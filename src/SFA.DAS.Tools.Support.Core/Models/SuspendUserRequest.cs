namespace SFA.DAS.Tools.Support.Core.Models
{
    public class SuspendUserRequest : ResultBase
    {
        public string UserId { get; private set; }
        public string CurrentUserId { get; private set; }
        public string CurrentUserEmail { get; private set; }

        public SuspendUserRequest(string userId, string currentUserId, string currentUserEmail)
    {
        UserId = userId;
        CurrentUserId = currentUserId;
        CurrentUserEmail = currentUserEmail;
    }
    }
}