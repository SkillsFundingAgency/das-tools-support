using System.Linq;

namespace SFA.DAS.Tools.Support.Web.Models
{
    public class SuspendUsersViewModel : UsersViewModelBase
    {
        public bool UsersSuspendedCompleted => Users != null && Users.All(a => a.ApiSubmissionStatus == SubmissionStatus.Successful);
    }
}
