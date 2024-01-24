using System;
using System.Collections.Generic;

namespace SFA.DAS.Tools.Support.Core.Models
{
    public class SearchApprenticeshipsResult : ResultBase
    {
        public List<ApprenticeshipDto> Apprenticeships { get; set; }
        public int ResultCount { get; set; }
    }
}