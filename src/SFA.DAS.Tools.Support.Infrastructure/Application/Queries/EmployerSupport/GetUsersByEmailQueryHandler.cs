using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Encoding;
using SFA.DAS.Tools.Support.Infrastructure.Services;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport;

public class GetUsersByEmailQueryHandler(IToolsSupportApimService employerSupportApiClient, IEncodingService encodingService) : IRequestHandler<GetUsersByEmailQuery, GetUsersByEmailQueryResult>
{
    public async Task<GetUsersByEmailQueryResult> Handle(GetUsersByEmailQuery query, CancellationToken cancellationToken)
    {
        var result = new GetUsersByEmailQueryResult {Users = null};

        if (!string.IsNullOrWhiteSpace(query.Email))
        {
            var response = await employerSupportApiClient.GetMatchingUsers(query.Email, cancellationToken);
            result.Users = response.Users;
        }

        return result;
    }
}
    