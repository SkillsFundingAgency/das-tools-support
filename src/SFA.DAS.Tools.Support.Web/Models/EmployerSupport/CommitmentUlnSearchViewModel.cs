﻿using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Encoding;

namespace SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

public class CommitmentUlnSearchViewModel : AccountDetailsBaseViewModel
{
    public string Uln { get; set; }
    public string HashedAccountId { get; set; }
    public List<ApprenticeshipUlnSummary> Apprenticeships { get; set; } = null;
}

public class ApprenticeshipUlnSummary
{
    public long Id { get; set; }
    public string HashedId { get; set; }
    public string HashedAccountId { get; set; }
    public long ProviderId { get; set; }
    public string EmployerName { get; set; }
    public string DisplayName { get; set; }
    public string TrainingDates { get; set; }
    public string StatusDescription { get; set; }
}