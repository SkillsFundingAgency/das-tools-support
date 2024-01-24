using System;

namespace SFA.DAS.Tools.Support.Core.Models
{
    public abstract class ResultBase
    {
        public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
        public string ErrorMessage { get; set; }
    }
}