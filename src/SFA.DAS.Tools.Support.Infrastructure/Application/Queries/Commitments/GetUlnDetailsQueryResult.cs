using SFA.DAS.Tools.Support.Core.Models;
using System;
using System.Collections.Generic;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.Commitments;

public class GetUlnDetailsQueryResult
{
    public List<ApprovedApprenticeshipUlnSummary> Apprenticeships { get; set; } = new();
}