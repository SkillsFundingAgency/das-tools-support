using MediatR;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetPayeSchemeLevyDeclarations;

public class GetPayeSchemeLevyDeclarationsQuery : IRequest<GetPayeSchemeLevyDeclarationsResult>
{
    public string HashedAccountId { get; set; }
    public string HashedPayeRef { get; set; }
}
