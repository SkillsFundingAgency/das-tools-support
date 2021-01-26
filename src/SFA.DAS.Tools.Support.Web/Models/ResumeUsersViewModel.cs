using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Tools.Support.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;

namespace SFA.DAS.Tools.Support.Web.Models
{
    public class ResumeUsersViewModel : UsersViewModelBase
    {
        public bool UsersResumeCompleted => Users != null && Users.All(a => a.ApiSubmissionStatus == SubmissionStatus.Successful);
    }
}
