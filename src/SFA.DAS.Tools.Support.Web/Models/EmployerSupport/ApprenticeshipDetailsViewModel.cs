using SFA.DAS.Tools.Support.Core.Models;

namespace SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

public class ApprenticeshipDetailsViewModel : AccountDetailsBaseViewModel
{
    public string HashedApprenticeshipId { get; set; }
    public string PaymentStatus { get; set; }
    public string AgreementStatus { get; set; }
    public string? ConfirmationStatusDescription { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string DisplayName => $"{FirstName} {LastName}";

    public string Email { get; set; }
    public string Uln { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string CohortReference { get; set; }
    public string EmployerReference { get; set; }

    public string LegalEntity { get; set; }

    public string TrainingProvider { get; set; }
    public long UKPRN { get; set; }
    public string Trainingcourse { get; set; }
    public string ApprenticeshipCode { get; set; }

    public DateTime? TrainingStartDate { get; set; }
    public DateTime? TrainingEndDate { get; set; }
    public string TrainingCost { get; set; }

    public string? Version { get; set; }
    public string Option { get; set; }
    public DateTime? PauseDate { get; set; }
    public DateTime? StopDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public string PaymentStatusTagColour { get; set; }
    public bool? MadeRedundant { get; set; }

    public string PendingChangesDescription { get; set; }

    public DateTime? OverlappingTrainingDateRequestCreatedOn { get; set; }
    public List<PendingChange> PendingChanges { get; set; }
    public List<ChangeOfProviderLink> ChangeOfProviderChain { get; set; }
}