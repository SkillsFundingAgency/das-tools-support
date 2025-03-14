using MediatR;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.Commitments;

public class GetApprenticeshipDetailsQuery : IRequest<GetApprenticeshipDetailsQueryResult>
{
    public string HashedApprenticeshipId { get; set; }
    public string HashedAccountId { get; set; }
}
