using System.Collections.Generic;
using SFA.DAS.Tools.Support.Core.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.Core.Models;

public class GetPayeSchemeLevyDeclarationsResponse
{
    public string PayeSchemeName { get; set; }
    public string PayeSchemeFormatedAddedDate { get; set; }
    public string PayeSchemeRef { get; set; }
    public List<Declaration> LevyDeclarations { get; set; }
    public bool UnexpectedError { get; set; }

}
