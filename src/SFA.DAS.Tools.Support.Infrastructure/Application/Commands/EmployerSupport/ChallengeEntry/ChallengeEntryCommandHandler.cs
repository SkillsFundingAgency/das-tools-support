using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Encoding;
using SFA.DAS.Tools.Support.Infrastructure.OuterApi.Requests;
using SFA.DAS.Tools.Support.Infrastructure.Services;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Commands.EmployerSupport.ChallengeEntry;
public class ChallengeEntryCommandHandler(IToolsSupportApimService employerSupportApiClient, IEncodingService encodingService)
    : IRequestHandler<ChallengeEntryCommand, ChallengeEntryCommandResult>
{
    public async Task<ChallengeEntryCommandResult> Handle(ChallengeEntryCommand query, CancellationToken cancellationToken)
    {
        var accountId = encodingService.Decode(query.Id, EncodingType.AccountId);
        var request = new ChallengeEntryRequest
        {
            Id = query.Id,
            Challenge1 = query.Challenge1,
            Challenge2 = query.Challenge2,
            Balance = query.Balance,
            FirstCharacterPosition = query.FirstCharacterPosition,
            SecondCharacterPosition = query.SecondCharacterPosition
        };

        var challengeResponse = await employerSupportApiClient.ChallengeEntry(request, accountId, cancellationToken);

        return (ChallengeEntryCommandResult)challengeResponse;
    }
}