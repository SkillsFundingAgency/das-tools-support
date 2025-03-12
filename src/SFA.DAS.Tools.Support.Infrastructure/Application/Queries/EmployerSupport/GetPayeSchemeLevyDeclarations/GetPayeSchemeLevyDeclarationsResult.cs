using System.Collections.Generic;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Core.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetPayeSchemeLevyDeclarations;

public class GetPayeSchemeLevyDeclarationsResult
{
    public string PayeSchemeName { get; set; }
    public string PayeSchemeFormatedAddedDate { get; set; }
    public string PayeSchemeRef { get; set; }
    public List<Declaration> LevyDeclarations { get; set; }
    public bool UnexpectedError { get; set; }

    public static explicit operator GetPayeSchemeLevyDeclarationsResult(GetPayeSchemeLevyDeclarationsResponse source)
    {
        if (source == null)
        {
            return new GetPayeSchemeLevyDeclarationsResult();
        }

        return new GetPayeSchemeLevyDeclarationsResult
        {
            PayeSchemeName = source.PayeSchemeName,
            PayeSchemeFormatedAddedDate = source.PayeSchemeFormatedAddedDate,
            PayeSchemeRef = source.PayeSchemeRef,
            UnexpectedError = source.UnexpectedError,
            LevyDeclarations = source.LevyDeclarations
        };
    }
}
