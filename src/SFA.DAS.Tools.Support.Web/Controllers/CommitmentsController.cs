using System.Globalization;
using FluentValidation;
using MediatR;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.Commitments;
using SFA.DAS.Tools.Support.Web.Models.EmployerSupport;
using StructureMap.Query;

namespace SFA.DAS.Tools.Support.Web.Controllers;

[Route("Account")]
public class CommitmentsController(IMediator mediator, IEncodingService encodingService) : Controller
{

    [HttpGet]
    [Route("{hashedAccountId}/commitments")]
    public async Task<IActionResult> CommitmentSearch(string hashedAccountId, string searchTerm, ApprenticeshipSearchType? searchType, MatchFailure? failure, [FromServices] IValidator<CommitmentSearchViewModel> validator)
    {
        var model = new CommitmentSearchViewModel
        {
            SearchTerm = string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm.ToUpper(),
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
    public async Task<IActionResult> CommitmentSearch(string hashedAccountId, CommitmentSearchViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (model.SearchType == ApprenticeshipSearchType.SearchByUln)
            {
                return RedirectToAction("CommitmentUlnSearch", new { hashedAccountId , uln = model.SearchTerm });
            }
            
            if (model.SearchType == ApprenticeshipSearchType.SearchByCohort)
            {
                return RedirectToAction("ViewCohortDetails", new { hashedAccountId, cohortRef = model.SearchTerm });
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
        var cohort = await mediator.Send(new GetCohortDetailsQuery() { CohortRef = cohortRef });

        if (cohort == null)
        {
            return RedirectToAction("CommitmentSearch", new { hashedAccountId, searchTerm = cohortRef, SearchType = ApprenticeshipSearchType.SearchByCohort, failure = MatchFailure.NoneFound });
        }

        if (!string.Equals(cohort.HashedAccountId, hashedAccountId, StringComparison.InvariantCultureIgnoreCase))
        {
            return RedirectToAction("CommitmentSearch", new { hashedAccountId, searchTerm = cohortRef, SearchType = ApprenticeshipSearchType.SearchByCohort, failure = MatchFailure.AccessDenied });
        }

        var model = new CohortDetailsViewModel
        {
            CohortId = cohort.CohortId,
            CohortReference = cohortRef,
            HashedAccountId = cohort.HashedAccountId,
            EmployerAccountName = cohort.EmployerAccountName,
            UkPrn = cohort.UkPrn,
            ProviderName = cohort.ProviderName,
            CohortStatus = cohort.CohortStatus,
            Apprenticeships = cohort.Apprenticeships.Select(x => ApprenticeshipCohortSummary.MapFrom(x, encodingService)).ToList()
        };

        return View(model);
    }

    [HttpGet]
    [Route("{hashedAccountId}/apprenticeships/{hashedId}")]
    public async Task<IActionResult> ViewApprenticeshipDetails(string hashedAccountId, string hashedId)
    {
        var apprenticeship = await mediator.Send(new GetApprenticeshipDetailsQuery { HashedApprenticeshipId = hashedId, HashedAccountId = hashedAccountId });

        var model = new ApprenticeshipDetailsViewModel
        {
            HashedApprenticeshipId = apprenticeship.HashedApprenticeshipId,
            PaymentStatus = apprenticeship.PaymentStatus,
            AgreementStatus = apprenticeship.AgreementStatus,
            ConfirmationStatusDescription = apprenticeship.ConfirmationStatusDescription,
            FirstName = apprenticeship.FirstName,
            LastName = apprenticeship.LastName,
            Email = apprenticeship.Email,
            Uln = apprenticeship.Uln,
            DateOfBirth = apprenticeship.DateOfBirth,
            CohortReference = apprenticeship.CohortReference,
            EmployerReference = apprenticeship.EmployerReference,
            LegalEntity = apprenticeship.LegalEntity,
            TrainingProvider = apprenticeship.TrainingProvider,
            UKPRN = apprenticeship.UKPRN,
            Trainingcourse = apprenticeship.Trainingcourse,
            ApprenticeshipCode = apprenticeship.ApprenticeshipCode,
            TrainingStartDate = apprenticeship.TrainingStartDate,
            TrainingEndDate = apprenticeship.TrainingEndDate,
            TrainingCost = $"�{apprenticeship.TrainingCost}:0n",
            Version = apprenticeship.Version,
            Option = apprenticeship.Option,
            PauseDate = apprenticeship.PauseDate,
            StopDate = apprenticeship.StopDate,
            CompletionDate = apprenticeship.CompletionDate, 
            MadeRedundant = apprenticeship.MadeRedundant,
            OverlappingTrainingDateRequestCreatedOn = apprenticeship.OverlappingTrainingDateRequestCreatedOn,
            PendingChangesDescription = apprenticeship.PendingChanges.Description,
            PendingChanges = apprenticeship.PendingChanges.Changes,
            ChangeOfProviderChain = apprenticeship.ChangeOfProviderChain
        };

        return View(model);
    }
}