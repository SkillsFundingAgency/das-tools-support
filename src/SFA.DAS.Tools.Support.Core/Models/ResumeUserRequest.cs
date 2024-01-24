namespace SFA.DAS.Tools.Support.Core.Models
{
    public class ResumeUserRequest : ResultBase
    {
        public string UserId { get; private set; }
        public string CurrentUserId { get; private set; }
        public string CurrentUserEmail { get; private set; }

        public ResumeUserRequest(string userId, string currentUserId, string currentUserEmail)
    {
        UserId = userId;
        CurrentUserId = currentUserId;
        CurrentUserEmail = currentUserEmail;
    }
    }
}