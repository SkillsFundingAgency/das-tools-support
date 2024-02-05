namespace SFA.DAS.Tools.Support.Web.Models;

public class UserSearchResultsViewModel
{
    public string HashedAccountId { get; set; }
    public long? InternalAccountId { get; set; }
    public IEnumerable<AccountUserRow> Users { get; set; }
    public string SelectedIds { get; set; }
    public string UserData { get; set; }       
}