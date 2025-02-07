using MediatR;
using SFA.DAS.Tools.Support.Core.Models.Enums;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport;

public class GetAccountDetailsQuery : IRequest<GetAccountDetailsQueryResult>
{
    public string AccountHashedId { get; set; }
    public AccountFieldsSelection AccountFieldsSelection { get; set; }
}
