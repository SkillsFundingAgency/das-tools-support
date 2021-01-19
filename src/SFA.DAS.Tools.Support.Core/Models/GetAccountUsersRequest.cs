using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Tools.Support.Core.Models
{
    public class GetAccountUsersRequest : ResultBase
    {
        public string AccountId { get; set; }

        public bool Validate()
        {
            return !string.IsNullOrEmpty(AccountId);
        }
    }
}
