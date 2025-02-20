namespace SFA.DAS.Tools.Support.Web.Configuration;

public class ClaimsConfiguration(string nameClaim, string emailClaim, string nameIdentifierClaim)
{
    public ClaimsConfiguration(bool useDfeSignIn) : this(
        useDfeSignIn ? "given_name" : "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname", 
        useDfeSignIn ? "email" : "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn", 
        useDfeSignIn ? "family_name" : "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname")
    {
    }

    public string NameClaim { get; } = nameClaim;
    public string EmailClaim { get; } = emailClaim;
    public string NameIdentifierClaim { get; } = nameIdentifierClaim;
}