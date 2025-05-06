using MediatR;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport;

public class GetEmployerAccountsQuery: IRequest<GetEmployerAccountsQueryResult>
{
    public string PublicHashedAccountId { get; set; }
    public string PayeRef { get; set; }
    public string EmployerName { get; set; }
}
