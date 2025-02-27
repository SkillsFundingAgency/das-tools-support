using MediatR;
using SFA.DAS.Tools.Support.Core.Models.Enums;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport;

public class GetUlnDetailsQuery : IRequest<GetUlnDetailsQueryResult>
{
    public string Uln { get; set; }
}
