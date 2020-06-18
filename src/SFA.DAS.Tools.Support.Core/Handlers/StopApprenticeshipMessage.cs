using MediatR;
using System;

namespace SFA.DAS.Tools.Support.Core.Handlers
{
    public class StopApprenticeshipMessage : IRequest<StopApprenticeshipMessageResult>
    {
        public long EmployerAccountId { get; set; }
        public long ApprenticeshipId { get; set; }
        public DateTime StopDate { get; set; }
    }
}