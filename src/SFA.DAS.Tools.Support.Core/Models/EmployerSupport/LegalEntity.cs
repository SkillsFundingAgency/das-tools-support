﻿using System;
using System.Collections.Generic;
using SFA.DAS.Tools.Support.Core.Models.Enums;

namespace SFA.DAS.Tools.Support.Core.Models.EmployerSupport;

public class LegalEntity
{
    public List<Agreement> Agreements { get; set; }
    public string Code { get; set; }
    public string DasAccountId { get; set; }
    public DateTime? DateOfInception { get; set; }
    public string Address { get; set; }
    public long LegalEntityId { get; set; }
    public string Name { get; set; }
    public string PublicSectorDataSource { get; set; }
    public string Sector { get; set; }
    public string Source { get; set; }
    public short SourceNumeric { get; set; }
    public string Status { get; set; }
    public long AccountLegalEntityId { get; set; }
    public string AccountLegalEntityPublicHashedId { get; set; }
    public EmployerAgreementStatus AgreementStatus { get; set; }
}
