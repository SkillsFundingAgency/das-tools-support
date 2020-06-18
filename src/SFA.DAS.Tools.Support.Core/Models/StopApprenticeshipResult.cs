using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Tools.Support.Core.Models
{
    public class StopApprenticeshipResult
    {
        public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
        public string ErrorMessage { get; set; }
    }
}
