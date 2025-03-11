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
    public async Task<ChallengeEntryCommandResult> Handle(ChallengeEntryCommand command, CancellationToken cancellationToken)
    {
        var accountId = encodingService.Decode(command.HashedAccountId, EncodingType.AccountId);
        var request = new ChallengeEntryRequest
        {
            Id = command.HashedAccountId,
            Challenge1 = command.Challenge1,
            Challenge2 = command.Challenge2,
            Balance = command.Balance,
            FirstCharacterPosition = command.FirstCharacterPosition,
            SecondCharacterPosition = command.SecondCharacterPosition
        };

        var challengeResponse = await employerSupportApiClient.ChallengeEntry(request, accountId, cancellationToken);

        return (ChallengeEntryCommandResult)challengeResponse;
    }
}