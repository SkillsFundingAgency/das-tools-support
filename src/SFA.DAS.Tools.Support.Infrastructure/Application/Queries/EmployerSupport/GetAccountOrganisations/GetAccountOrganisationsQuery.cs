using MediatR;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetAccountOrganisations;

public class GetAccountOrganisationsQuery : IRequest<GetAccountOrganisationsQueryResult>
{
    public string HashedAccountId { get; set; } = "";
}