using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Tools.Support.Core.Models;

namespace SFA.DAS.Tools.Support.Infrastructure.OuterApi.EmployerSupport;

public class EmployerSupportApiClient(IOuterApiClient client) : IEmployerSupportApiClient
{
    public async Task<GetAccountDetailsResponse> GetAccountDetails(string accountHashedId, string accountFieldSelection, CancellationToken cancellationToken = default)
    {
        return await client.Get<GetAccountDetailsResponse>($"Account/{accountHashedId}/AccountDetails?accountFieldSelection={accountFieldSelection}");

        //var acc = new Account
        //{
        //    AccountId = 2,
        //    HashedAccountId = accountHashedId,
        //    PublicHashedAccountId = "hghghgh",
        //    DasAccountName = "DasAccountName",
        //    DateRegistered = DateTime.UtcNow.AddDays(-133),
        //    OwnerEmail = "email@email.com",
        //    ApprenticeshipEmployerType = "LEVY",
        //    LegalEntities = [ new LegalEntity
        //    {
        //        Address = "address goes, here, postcode",
        //        Name = "NAME GOES HERE",
        //        Source = "Companies House",
        //        AgreementStatus = SFA.DAS.Tools.Support.Core.Models.Enums.EmployerAgreementStatus.Signed,
        //    } ]
        //};

        //return new GetAccountDetailsResponse { Account = acc };
    }
}
