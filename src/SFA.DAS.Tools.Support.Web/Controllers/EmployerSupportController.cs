using FluentValidation;
using MediatR;
using Microsoft.Rest;
using SFA.DAS.Encoding;
using SFA.DAS.Tools.Support.Core.Models.EmployerSupport;
using SFA.DAS.Tools.Support.Core.Models.Enums;
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Models.EmployerSupport;
using StructureMap.Query;

namespace SFA.DAS.Tools.Support.Web.Controllers;

[Route("Employer")]
public class EmployerSupportController(IMediator mediator, IEncodingService encodingService) : Controller
{
    [HttpGet]
    [Route(RouteNames.EmployerSupport_UserSearch)]
    public async Task<IActionResult> EmployerUserSearch(string email)
    {
        var model = new EmployerUserSearchModel
        {
            Email = email,
        };

        var result = await mediator.Send(new GetUsersByEmailQuery {Email = email});

        if (result.Users != null)
        {
            model.Users = result.Users.Select(x => new MatchedUser
                {Id = x.Id.ToString(), DisplayName = x.DisplayName, Email = x.Email}).ToList();
        }

        return View(model);
    }

    [HttpGet]
    [Route(RouteNames.EmployerSupport_AccountSearch)]
    public async Task<IActionResult> EmployerAccountSearch(string publicHashedId, string payeRef)
    {
        var result = await mediator.Send(new GetEmployerAccountsQuery
            {PublicHashedAccountId = publicHashedId, PayeRef = payeRef});
        var model = new EmployerAccountSearchModel {PublicHashedId = publicHashedId, PayeRef = payeRef};
        if (result.Accounts != null)
        {
            model.Accounts = result.Accounts.Select(x => new MatchedAccount
            {
                AccountId = x.AccountId, DasAccountName = x.DasAccountName, HashedAccountId = x.HashedAccountId,
                PublicHashedAccountId = x.PublicHashedAccountId
            }).ToList();
        }
        return View(model);
    }

    [HttpGet]
    [Route(RouteNames.EmployerSupport_AccountDetails)]
    public async Task<IActionResult> AccountDetails(string hashedAccountId, AccountFieldsSelection accountFieldsSelection)
    {
        var query = new GetAccountDetailsQuery
        {
            HashedAccountId = hashedAccountId,
            AccountFieldsSelection = accountFieldsSelection
        };

        var result = await mediator.Send(query);

        var viewmodel = AccountDetailsViewModel.MapFrom(result);

        viewmodel.SelectedTab = accountFieldsSelection;

        return View(viewmodel);
    }

    [HttpGet]
    [Route("{hashedAccountId}/commitments")]
    public async Task<IActionResult> CommitmentSearch(string hashedAccountId, string searchTerm, ApprenticeshipSearchType? searchType, MatchFailure? failure, [FromServices] IValidator<CommitmentSearchViewModel> validator)
    {
        var model = new CommitmentSearchViewModel
        {
            SearchTerm = searchTerm,
            SearchType = searchType ?? ApprenticeshipSearchType.SearchByUln
        };

        if (failure != null)
        {
            string error = failure switch
            {
                MatchFailure.NoneFound => "Not found",
                MatchFailure.AccessDenied => "Account is unauthorised to access this Cohort",
                _ => "Unknown failure"
            };

            ModelState.AddModelError("SearchTerm", error);
        }
        return View(model);
    }

    [HttpPost]
    [Route("{hashedAccountId}/commitments")]
    public async Task<IActionResult> CommitmentSearch(string hashedAccountId, CommitmentSearchViewModel model) //, IValidator<CommitmentSearchViewModel> validator)
    {
        if (ModelState.IsValid)
        {
            if (model.SearchType == ApprenticeshipSearchType.SearchByUln)
            {
                return RedirectToAction("CommitmentUlnSearch", new { hashedAccountId , uln = model.SearchTerm });
            }
            
            if (model.SearchType == ApprenticeshipSearchType.SearchByCohort)
            {
                return RedirectToAction("CommitmentCohortSearch", new { hashedAccountId, uln = model.SearchTerm });
            }
        }
        return View(model);
    }

    [HttpGet]
    [Route("{hashedAccountId}/commitments/uln/{uln}")]
    public async Task<IActionResult> CommitmentUlnSearch(string hashedAccountId, string uln)
    {
        var ulnsResult = await mediator.Send(new GetUlnDetailsQuery { Uln = uln });

        if (!ulnsResult.Apprenticeships.Any())
        {
            return RedirectToAction("CommitmentSearch", new {hashedAccountId, searchTerm = uln, SearchType = ApprenticeshipSearchType.SearchByUln, failure = MatchFailure.NoneFound });
        }

        var model = new CommitmentUlnSearchViewModel
        {
            Uln = uln,
            HashedAccountId = hashedAccountId,
            Apprenticeships = ulnsResult.Apprenticeships.Select(x=> ApprenticeshipUlnSummary.MapFrom(x, encodingService)).ToList()
        };

        return View(model);
    }

    [HttpGet]
    [Route("{hashedAccountId}/commitments/{cohortRef}")]
    public async Task<IActionResult> ViewCohortDetails(string hashedAccountId, string cohortRef)
    {
        //var ulnsResult = await mediator.Send(new GetUlnDetailsQuery { Uln = uln });

        //if (!ulnsResult.Apprenticeships.Any())
        //{
        //    return RedirectToAction("CommitmentSearch", new { hashedAccountId, searchTerm = uln, SearchType = ApprenticeshipSearchType.SearchByUln, failure = MatchFailure.NoneFound });
        //}

        //var model = new CommitmentUlnSearchViewModel
        //{
        //    Uln = uln,
        //    HashedAccountId = hashedAccountId,
        //    Apprenticeships = ulnsResult.Apprenticeships.Select(x => ApprenticeshipUlnSummary.MapFrom(x, encodingService)).ToList()
        //};

        return View();
    }



    [HttpGet]
    [Route("{hashedAccountId}/apprenticeships/{hashedId}")]
    public async Task<IActionResult> ViewApprenticeshipDetails(string hashedAccountId, string hashedId)
    {
 
        return View();
    }

}