using System;
using SFA.DAS.Tools.Support.Core.Models.Enums;

namespace SFA.DAS.Tools.Support.Core.Models.EmployerSupport;

public class Agreement
{
    public long Id { get; set; }
    public DateTime? SignedDate { get; set; }
    public string SignedByName { get; set; }
    public EmployerAgreementStatus Status { get; set; }
    public int TemplateVersionNumber { get; set; }
    public AgreementType AgreementType { get; set; }
}
