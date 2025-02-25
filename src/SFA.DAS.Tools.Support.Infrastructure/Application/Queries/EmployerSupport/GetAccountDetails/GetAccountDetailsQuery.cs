using MediatR;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetAccountDetails;

public class GetAccountDetailsQuery : IRequest<GetAccountDetailsQueryResult>
{
    public string HashedAccountId { get; set; }
}
