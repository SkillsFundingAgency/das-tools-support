namespace SFA.DAS.Tools.Support.Web.Configuration;

public class ClaimsConfiguration
{
    public ClaimsConfiguration(bool useDfeSignIn)
    {
        NameClaim = useDfeSignIn ? "given_name" : "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname";
        EmailClaim = useDfeSignIn ? "email" : "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn";
        NameIdentifierClaim = useDfeSignIn ? "family_name" : "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname";
    }

    public ClaimsConfiguration(string nameClaim, string emailClaim, string nameIdentifierClaim)
    {
        NameClaim = nameClaim;
        EmailClaim = emailClaim;
        NameIdentifierClaim = nameIdentifierClaim;
    }

    public string NameClaim { get; }
    public string EmailClaim { get; }
    public string NameIdentifierClaim { get; }
}