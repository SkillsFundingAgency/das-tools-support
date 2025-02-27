using System;
using System.Collections.Generic;

namespace SFA.DAS.Tools.Support.Core.Models;

public class GetMatchingApprenticeshipsResponse
{
    public List<ApprovedApprenticeshipUlnSummary> ApprovedApprenticeships { get; set; } = new();
}

public class ApprovedApprenticeshipUlnSummary
{
    public long Id { get; set; }
    public long EmployerAccountId { get; set; }
    public long ProviderId { get; set; }
    public string EmployerName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; }
}