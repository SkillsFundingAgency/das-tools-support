using MediatR;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport;

public class GetCohortDetailsQuery : IRequest<GetCohortDetailsQueryResult>
{
    public string CohortRef { get; set; }
}
