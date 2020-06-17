using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Tools.Support.Core.Services;

namespace SFA.DAS.Tools.Support.Core.Handlers
{
    public class StopApprenticeshipHandler : INotificationHandler<StopApprenticeshipMessage>
    {
        private readonly IEmployerCommitmentsService _employerCommitmentsService;

        public StopApprenticeshipHandler(IEmployerCommitmentsService employerCommitmentsService)
        {
            _employerCommitmentsService = employerCommitmentsService;
        }

        public async Task Handle(StopApprenticeshipMessage message, CancellationToken cancellationToken)
        {
            await _employerCommitmentsService.StopApprenticeship(message.EmployerAccountId, message.ApprenticeshipId, message.StopDate);

            await Task.CompletedTask;
        }
    }
}
