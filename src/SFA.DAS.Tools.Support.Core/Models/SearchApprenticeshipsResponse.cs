using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Tools.Support.Core.Models
{
    public class SearchApprenticeshipsResponse : ResultBase
    {
        public List<Apprenticeship> Apprenticeships { get; set; }
        public int ResultCount { get; set; }
    }
}
