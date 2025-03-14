using SFA.DAS.Tools.Support.Core.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

public class PayeSchemeLevyDeclarationViewModel : AccountDetailsBaseViewModel
{
    public string PayeSchemeName { get; set; }
    public string PayeSchemeFormatedAddedDate { get; set; }
    public string PayeSchemeRef { get; set; }
    public List<Declaration> LevyDeclarations { get; set; }
    public bool UnexpectedError { get; set; }
}