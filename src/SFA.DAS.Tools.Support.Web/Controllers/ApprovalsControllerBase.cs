using AutoMapper;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Extensions;
using SFA.DAS.Tools.Support.Web.Models;

namespace SFA.DAS.Tools.Support.Web.Controllers;

public abstract class ApprovalsControllerBase : Controller
{
    protected readonly ILogger Logger;
    protected readonly IEmployerCommitmentsService EmployerCommitmentsService;
    protected readonly IMapper Mapper;
    
    private readonly IOptions<ClaimsConfiguration> _claimConfiguration;

    protected ApprovalsControllerBase(ILogger logger, IEmployerCommitmentsService employerCommitmentsService, IMapper mapper, IOptions<ClaimsConfiguration> claimConfiguration)
    {
        Logger = logger;
        EmployerCommitmentsService = employerCommitmentsService;
        Mapper = mapper;
        _claimConfiguration = claimConfiguration;
    }

    protected dynamic GetClaims()
    {
        return new 
        {
            UserEmail = HttpContext.User.Claims.GetClaim(_claimConfiguration.Value.EmailClaim),
            UserId = HttpContext.User.Claims.GetClaim(_claimConfiguration.Value.EmailClaim),
            DisplayName = $"{HttpContext.User.Claims.GetClaim(_claimConfiguration.Value.NameClaim)} {HttpContext.User.Claims.GetClaim(_claimConfiguration.Value.NameIdentifierClaim)}"
        };
    }

    protected static dynamic CreateSearchModel(ApprenticeshipSearchResultsViewModel model, string action)
    {
        return new
        {
            model.ApprenticeNameOrUln,
            model.CourseName,
            model.ProviderName,
            model.Ukprn,
            model.EmployerName,
            SelectedStatus = model.Status,
            EndDate = model.EndDate.GetUIFormattedDate(),
            StartDate = model.StartDate.GetUIFormattedDate(),
            act = action
        };
    }

    protected IEnumerable<Task<GetApprenticeshipResult>> GetApprenticeshipsFromApprovals(IEnumerable<string> ids)
    {
        var tasks = new List<Task<GetApprenticeshipResult>>();

        foreach (var id in ids)
        {
            if (int.TryParse(id, out var longId))
            {
                tasks.Add(EmployerCommitmentsService.GetApprenticeship(longId, new CancellationToken()));
            }
        }

        return tasks;
    }

    protected static IEnumerable<TOut> CreateApprenticeshipRows<TIn, TOut>(IEnumerable<TIn> results, IEnumerable<TOut> apprenticeshipsData) 
        where TIn : ApprenticeshipResult
        where TOut : ApprenticeshipRow
    {
        var apprenticeshipRows = apprenticeshipsData.ToList();
        
        foreach (var apprenticeship in apprenticeshipRows)
        {
            var result = results.FirstOrDefault(s => s.ApprenticeshipId == apprenticeship.Id);
            if (result == null)
            {
                continue;
            }

            if (!result.HasError)
            {
                apprenticeship.ApiSubmissionStatus = SubmissionStatus.Successful;
                apprenticeship.ApiErrorMessage = string.Empty;
            }
            else
            {
                apprenticeship.ApiSubmissionStatus = SubmissionStatus.Errored;
                apprenticeship.ApiErrorMessage = result.ErrorMessage;
            }
        }

        return apprenticeshipRows;
    }
}