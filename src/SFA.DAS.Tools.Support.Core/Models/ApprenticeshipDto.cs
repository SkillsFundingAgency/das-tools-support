﻿using System;

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

    /// <summary>
    /// Used as a DTO for the Commitments Api,
    /// Depending on the Api Call and the Mapper, not all of the below properties may be populated
    /// Check each api response object to confirm
    /// </summary>
    public class ApprenticeshipDto
    {
        public string ProviderRef { get; set; }
        public string EmployerRef { get; set; }
        public decimal? TotalAgreedPrice { get; set; }
        public ApprenticeshipStatus ApprenticeshipStatus { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime? PauseDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime StartDate { get; set; }
        public string CourseName { get; set; }
        public string ProviderName { get; set; }
        public long Ukprn { get; set; }
        public string EmployerName { get; set; }
        public string Uln { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public long Id { get; set; }
        public string CohortReference { get; set; }
        public string EndpointAssessorName { get; set; }
        public DateTime? StopDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public long AccountLegalEntityId { get; set; }
        public long EmployerAccountId { get; set; }
        public long CohortId { get; set; }
    }
}