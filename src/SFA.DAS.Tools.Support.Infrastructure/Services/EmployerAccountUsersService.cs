using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.Tools.Support.Core.Models;

namespace SFA.DAS.Tools.Support.Infrastructure.Services;

public interface IEmployerAccountUsersService
{
    Task<GetAccountUsersResult> GetAccountUsers(GetAccountUsersRequest request);
}

public class EmployerAccountUsersService(IToolsSupportApimService toolsSupportApimService, IAccountApiClient accountsApi, IMapper mapper, ILogger<EmployerAccountUsersService> logger)
    : IEmployerAccountUsersService
{
    private readonly ILogger _logger = logger;

    public async Task<GetAccountUsersResult> GetAccountUsers(GetAccountUsersRequest request)
    {
        try
        {
            _logger.LogInformation("Getting account users for HashedAccountId: {HashedAccountId}, InternalAccountId: {InternalAccountId}", 
                request.HashedAccountId, request.InternalAccountId);

            if (!request.Validate())
            {
                throw new InvalidRequestException("Request failed validation");
            }

            ICollection<TeamMemberViewModel> employerAccountTeamMembers;

            if (!string.IsNullOrEmpty(request.HashedAccountId))
            {
                employerAccountTeamMembers = await accountsApi.GetAccountUsers(request.HashedAccountId);
            }
            else
            {
                employerAccountTeamMembers = await accountsApi.GetAccountUsers(request.InternalAccountId.Value);
            }

            _logger.LogInformation("Found {TeamMemberCount} team members for account", employerAccountTeamMembers.Count);

            var userTasks = employerAccountTeamMembers
                .Select(accUser =>
                {
                    return toolsSupportApimService.GetUserByUserRef(accUser.UserRef)
                        .ContinueWith(
                            t => t.IsFaulted
                                ? new ResultOrException<GetUserByUserRefResponse>(t.Exception, accUser.UserRef)
                                : new ResultOrException<GetUserByUserRefResponse>(t.Result, accUser.UserRef));
                });

            var mappedUsers = mapper.Map<ICollection<TeamMemberViewModel>, IEnumerable<AccountUserDto>>(employerAccountTeamMembers);
                
            var allUserResults = await Task.WhenAll(userTasks);
            
            var failedResults = allUserResults.Where(ut => !ut.IsSuccess).ToList();
            if (failedResults.Any())
            {
                _logger.LogWarning("Failed to retrieve user details for {FailedCount} out of {TotalCount} users in account {AccountId}", 
                    failedResults.Count, allUserResults.Length, request.HashedAccountId ?? request.InternalAccountId?.ToString());
                
                foreach (var failed in failedResults)
                {
                    _logger.LogError(failed.Exception, 
                        "Failed to get user details for UserRef: {UserRef}. Error: {ErrorMessage}", 
                        failed.UserRef, failed.Exception?.Message);
                }
            }
            
            var userAccounts = allUserResults
                .Where(ut => ut.IsSuccess && ut.Result.User != null)
                .Select(ut => ut.Result.User!);

            var accountUserDtos = mappedUsers.ToList();
            MapUserAccountStatus(userAccounts, accountUserDtos);

            var successCount = allUserResults.Count(ut => ut.IsSuccess);
            _logger.LogInformation("Successfully retrieved user details for {SuccessCount} out of {TotalCount} users", 
                successCount, allUserResults.Length);

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

    private static void MapUserAccountStatus(IEnumerable<Core.Models.UserProfile> userAccounts, IEnumerable<AccountUserDto> mappedUsers)
    {
        foreach (var user in mappedUsers)
        {
            var userMatch = userAccounts.FirstOrDefault(u => u.GovUkIdentifier == user.UserRef || u.Id.ToString() == user.UserRef);

            if (userMatch == null)
            {
                continue;
            }
            
            user.AccountStatus = userMatch.IsLocked ? "Locked" : userMatch.IsSuspended ? "Suspended" : "Active";
        }
    }

    private class ResultOrException<T>
    {
        public ResultOrException(T result, string userRef)
        {
            IsSuccess = true;
            Result = result;
            UserRef = userRef;
        }

        public ResultOrException(Exception ex, string userRef)
        {
            IsSuccess = false;
            Exception = ex;
            UserRef = userRef;
        }

        public bool IsSuccess { get; }
        public T Result { get; }
        public Exception Exception { get; }
        public string UserRef { get; }
    }
}