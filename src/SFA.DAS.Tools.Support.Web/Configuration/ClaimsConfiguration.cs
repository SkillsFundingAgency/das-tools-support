using System;

namespace SFA.DAS.Tools.Support.Web.Configuration
{
    public class ClaimsConfiguration
    {
        public string NameClaim { get; set; }
        public string NameIdentifierClaim { get; set; }
        public string EmailClaim { get; set; }

        public void ValidateConfiguration()
        {
            if(string.IsNullOrWhiteSpace(NameClaim) || string.IsNullOrWhiteSpace(NameIdentifierClaim) || string.IsNullOrWhiteSpace(EmailClaim))
            {
                throw new ArgumentException("ClaimsConfiguration must be configured with a Name, NameIdentifier & Email Claim Type");
            }
        }
    }
}
