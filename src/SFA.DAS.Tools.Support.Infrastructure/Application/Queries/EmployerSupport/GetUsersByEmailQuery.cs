using MediatR;
using SFA.DAS.Tools.Support.Core.Models.Enums;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport;

public class GetUsersByEmailQuery : IRequest<GetUsersByEmailQueryResult>
{
    public string Email { get; set; }
}
