using System.ComponentModel;

namespace SFA.DAS.Tools.Support.Core.Models.Enums;
public enum AgreementType : byte
{
    [Description("Levy")]
    Levy,
    [Description("Expression of Interest")]
    NonLevyExpressionOfInterest,
    [Description("Combined")]
    Combined
}
