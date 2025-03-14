using MediatR;
using SFA.DAS.Tools.Support.Core.Models.Enums;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.Commitments;

public class GetUlnDetailsQuery : IRequest<GetUlnDetailsQueryResult>
{
    public string Uln { get; set; }
}
