using SFA.DAS.Tools.Support.Core.Models.EmployerSupport;
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetPayeSchemeLevyDeclarations;

namespace SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

public class PayeSchemeLevyDeclarationViewModel : AccountDetailsBaseViewModel
{
    public string PayeSchemeName { get; set; }
    public string PayeSchemeFormatedAddedDate { get; set; }
    public string PayeSchemeRef { get; set; }
    public List<Declaration> LevyDeclarations { get; set; }
    public bool UnexpectedError { get; set; }

    public static PayeSchemeLevyDeclarationViewModel MapFrom(GetPayeSchemeLevyDeclarationsResult source)
    {
        if (source == null)
        {
            return new PayeSchemeLevyDeclarationViewModel();
        }

        return new PayeSchemeLevyDeclarationViewModel
        {
            PayeSchemeName = source.PayeSchemeName,
            PayeSchemeFormatedAddedDate = source.PayeSchemeFormatedAddedDate,
            PayeSchemeRef = source.PayeSchemeRef,
            UnexpectedError = source.UnexpectedError,
            LevyDeclarations = source.LevyDeclarations
        };
    }
}