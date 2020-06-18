using MediatR;
using System;

namespace SFA.DAS.Tools.Support.Core.Handlers
{
    public class StopApprenticeshipMessageResult
    {
        public bool HasSuccess => string.IsNullOrWhiteSpace(ErrorMessage);
        public string ErrorMessage { get; set; }
    }
}