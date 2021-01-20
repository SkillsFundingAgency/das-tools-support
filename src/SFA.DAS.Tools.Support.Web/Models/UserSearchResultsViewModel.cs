using System.Collections.Generic;
using SFA.DAS.Tools.Support.Core.Models;

namespace SFA.DAS.Tools.Support.Web.Models
{
    public class UserSearchResultsViewModel
    {
       public string AccountId { get; set; }
       public IEnumerable<AccountUserRow> Users { get; set; }
       public string SelectedIds { get; set; }
       public string UserData { get; set; }       
    }
}