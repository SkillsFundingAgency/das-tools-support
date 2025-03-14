using MediatR;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.Commitments;

public class GetCohortDetailsQuery : IRequest<GetCohortDetailsQueryResult>
{
    public string CohortRef { get; set; }
}
