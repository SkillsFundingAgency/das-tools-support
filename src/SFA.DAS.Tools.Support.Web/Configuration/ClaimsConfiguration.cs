namespace SFA.DAS.Tools.Support.Web.Configuration;

public class ClaimsConfiguration(string nameClaim, string emailClaim, string nameIdentifierClaim)
{
    public ClaimsConfiguration() : this(
        "given_name",
        "email",
        "family_name")
    {
    }

    public string NameClaim { get; } = nameClaim;
    public string EmailClaim { get; } = emailClaim;
    public string NameIdentifierClaim { get; } = nameIdentifierClaim;
}