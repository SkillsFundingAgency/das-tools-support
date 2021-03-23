using System;
using System.Collections.Generic;
using SFA.DAS.Tools.Support.Core.Models;

namespace SFA.DAS.Tools.Support.Web.Models
{
    public class UserViewModel
    {
        public string HashedAccountId { get; set; }
        public long? InternalAccountId { get; set; }
        public IEnumerable<AccountUserDto> Users { get; set; } = new List<AccountUserDto>();
    }
}