using System.Linq;

namespace SFA.DAS.Tools.Support.Web.Models;

public class ResumeUsersViewModel : UsersViewModelBase
{
    public bool UsersResumeCompleted => Users != null && Users.All(a => a.ApiSubmissionStatus == SubmissionStatus.Successful);
}