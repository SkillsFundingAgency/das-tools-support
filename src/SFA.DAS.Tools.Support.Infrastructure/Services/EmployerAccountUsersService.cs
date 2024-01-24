using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EmployerUsers.Api.Client;
using SFA.DAS.EmployerUsers.Api.Types;
using SFA.DAS.Tools.Support.Core.Models;

namespace SFA.DAS.Tools.Support.Infrastructure.Services;

public interface IEmployerAccountUsersService
{
    Task<GetAccountUsersResult> GetAccountUsers(GetAccountUsersRequest request, CancellationToken token);
}

public class EmployerAccountUsersService : IEmployerAccountUsersService
{
    private readonly IEmployerUsersApiClient _employerUsersApiClient;
    private readonly IAccountApiClient _accountsApi;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;

    public EmployerAccountUsersService(IEmployerUsersApiClient employerUsersApiClient, IAccountApiClient accountsApi, IMapper mapper, ILogger<EmployerAccountUsersService> logger)
    {
        _employerUsersApiClient = employerUsersApiClient;
        _accountsApi = accountsApi;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<GetAccountUsersResult> GetAccountUsers(GetAccountUsersRequest request, CancellationToken token)
    {
        try
        {
            if (!request.Validate())
            {
                throw new InvalidRequestException("Request failed validation");
            }

            ICollection<TeamMemberViewModel> employerAccountTeamMembers;

            if (!string.IsNullOrEmpty(request.HashedAccountId))
            {
                employerAccountTeamMembers = await _accountsApi.GetAccountUsers(request.HashedAccountId);
            }

            else
            {
                employerAccountTeamMembers = await _accountsApi.GetAccountUsers(request.InternalAccountId.Value);
            }

            var userTasks = employerAccountTeamMembers
                .ToList()
                .Select(accUser =>
                {
                    return _employerUsersApiClient.GetUserById(accUser.UserRef)
                        .ContinueWith(
                            t => t.IsFaulted
                                ? new ResultOrException<UserViewModel>(t.Exception)
                                : new ResultOrException<UserViewModel>(t.Result));
                });

            var mappedUsers = _mapper.Map<ICollection<TeamMemberViewModel>, IEnumerable<AccountUserDto>>(employerAccountTeamMembers);
                
            var userAccounts = (await Task.WhenAll(userTasks))
                .Where(ut => ut.IsSuccess)
                .Select(ut => ut.Result);

            var accountUserDtos = mappedUsers.ToList();
            MapUserAccountStatus(userAccounts, accountUserDtos);

            return new GetAccountUsersResult
            {
                Users = accountUserDtos
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

    private static void MapUserAccountStatus(IEnumerable<UserViewModel> userAccounts, IEnumerable<AccountUserDto> mappedUsers)
    {
        foreach (var user in mappedUsers)
        {
            var userMatch = userAccounts.FirstOrDefault(u => u.Id == user.UserRef);

            if (userMatch == null)
            {
                continue;
            }
            
            user.AccountStatus = userMatch.IsSuspended ? "Suspended" : userMatch.IsLocked ? "Locked" : "Active";
            user.LastSuspendedDate = userMatch.LastSuspendedDate;
        }
    }

    private class ResultOrException<T>
    {
        public ResultOrException(T result)
        {
            IsSuccess = true;
            Result = result;
        }

        public ResultOrException(Exception ex)
        {
            IsSuccess = false;
            Exception = ex;
        }

        public bool IsSuccess { get; }
        public T Result { get; }
        public Exception Exception { get; }
    }
}