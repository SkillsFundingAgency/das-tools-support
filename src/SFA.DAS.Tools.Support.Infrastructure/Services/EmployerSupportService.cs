using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Core.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.Infrastructure.Services;

public class EmployerSupportService(HttpClient httpClient) : IEmployerSupportService
{
    public async Task<GetAccountDetailsResponse> GetAccountDetails(string accountHashedId, string accountFieldSelection, CancellationToken cancellationToken = default)
    {
        //var response = await httpClient.GetAsync($"resource/organisations?accountId={accountHashedId}", cancellationToken);
        //response.EnsureSuccessStatusCode();

        //return JsonConvert.DeserializeObject<GetAccountDetailsResponse>(await response.Content.ReadAsStringAsync(cancellationToken));

        var acc = new Account
        {
            AccountId = 2,
            HashedAccountId = accountHashedId,
            PublicHashedAccountId = "hghghgh",
            DasAccountName = "DasAccountName",
            DateRegistered = DateTime.UtcNow.AddDays(-133),
            OwnerEmail = "email@email.com",
            ApprenticeshipEmployerType = "LEVY",
            LegalEntities = [ new LegalEntity
            {
                Address = "address goes, here, postcode",
                Name = "NAME GOES HERE",
                Source = "SOURCE???",
                AgreementStatus = SFA.DAS.Tools.Support.Core.Models.Enums.EmployerAgreementStatus.Signed,
            } ]
        };

        return new GetAccountDetailsResponse { Account = acc};
    }
}
