using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Tools.Support.Core.Models;
using AutoMapper;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.Tools.Support.Infrastructure.Services
{
    public interface IEmployerAccountsService
    {
        Task<GetAccountUsersResult> GetAccountUsers(Core.Models.GetAccountUsersRequest request, CancellationToken token);
    }

    public class EmployerAccountsService : IEmployerAccountsService
    {
        private readonly IAccountApiClient _accountsApi;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public EmployerAccountsService(IAccountApiClient accountsApi, IMapper mapper, ILogger<EmployerAccountsService> logger)
        {
            _accountsApi = accountsApi;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<GetAccountUsersResult> GetAccountUsers(Core.Models.GetAccountUsersRequest request, CancellationToken token)
        {
            try
            {
                if(!request.Validate())
                {
                    throw new InvalidRequestException("Request failed validation");
                }

                ICollection<SFA.DAS.EAS.Account.Api.Types.TeamMemberViewModel> result;

                if(!string.IsNullOrEmpty(request.AccountId))
                {
                    result = await _accountsApi.GetAccountUsers(request.AccountId);
                } 
                
                else
                {
                    result = await _accountsApi.GetAccountUsers(request.InternalAccountId);
                }

                return new GetAccountUsersResult
                {
                    Users = _mapper.Map<ICollection<SFA.DAS.EAS.Account.Api.Types.TeamMemberViewModel>, IEnumerable<AccountUserDto>>(result)
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failure to get account users.");
                return new GetAccountUsersResult
                {
                    ErrorMessage = e.Message
                };
            }
        }
    }
}
