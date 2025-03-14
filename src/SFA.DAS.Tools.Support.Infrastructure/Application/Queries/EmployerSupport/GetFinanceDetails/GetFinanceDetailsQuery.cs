using MediatR;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetFinanceDetails;

public class GetFinanceDetailsQuery : IRequest<GetFinanceDetailsQueryResult>
{
    public string HashedAccountId { get; set; } = "";
}
