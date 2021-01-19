using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Tools.Support.Core.Models
{
    public class GetAccountUsersResult : ResultBase
    {
        public IEnumerable<AccountUserDto> Users { get; set; }
    }
}
