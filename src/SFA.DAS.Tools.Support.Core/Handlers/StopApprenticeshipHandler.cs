using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Tools.Support.Core.Services;
using AutoMapper;

namespace SFA.DAS.Tools.Support.Core.Handlers
{
    public class StopApprenticeshipHandler : IRequestHandler<StopApprenticeshipMessage, StopApprenticeshipMessageResult>
    {
        private readonly IEmployerCommitmentsService _employerCommitmentsService;
        private readonly IMapper _mapper;

        public StopApprenticeshipHandler(IEmployerCommitmentsService employerCommitmentsService, IMapper mapper)
        {
            _employerCommitmentsService = employerCommitmentsService;
            _mapper = mapper;
        }

        public async Task<StopApprenticeshipMessageResult> Handle(StopApprenticeshipMessage message, CancellationToken cancellationToken)
        {
            var result = await _employerCommitmentsService.StopApprenticeship(message.EmployerAccountId, message.ApprenticeshipId, message.UserId, message.StopDate);
            return _mapper.Map<StopApprenticeshipMessageResult>(result);
            
        }
    }
}
