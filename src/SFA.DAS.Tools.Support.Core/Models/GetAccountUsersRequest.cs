namespace SFA.DAS.Tools.Support.Core.Models
{
    public class GetAccountUsersRequest : ResultBase
    {
        public string HashedAccountId { get; set; }
        public long? InternalAccountId { get; set; }

        public bool Validate()
    {
        return !string.IsNullOrEmpty(HashedAccountId) ^ (InternalAccountId.HasValue && InternalAccountId > 0);
    }
    }
}