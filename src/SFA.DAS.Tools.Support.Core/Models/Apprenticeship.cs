using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Tools.Support.Core.Models
{
    public enum ApprenticeshipStatus : short
    {
        WaitingToStart = 0,
        Live = 1,
        Paused = 2,
        Stopped = 3,
        Completed = 4,
        Unknown = 5
    }
    public enum PaymentStatus : short
    {
        Active = 1,
        Paused = 2,
        Withdrawn = 3,
        Completed = 4
    }

    public class Apprenticeship
    {
        public string ProviderRef { get; set; }
        public string EmployerRef { get; set; }
        public decimal? TotalAgreedPrice { get; set; }
        public ApprenticeshipStatus ApprenticeshipStatus { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime PauseDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime StartDate { get; set; }
        public string CourseName { get; set; }
        public string ProviderName { get; set; }
        public string EmployerName { get; set; }
        public string Uln { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public long Id { get; set; }
        public string CohortReference { get; set; }
        public long AccountLegalEntityId { get; set; }
    }
}
