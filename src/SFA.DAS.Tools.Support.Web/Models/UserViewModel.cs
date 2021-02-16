using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Tools.Support.Core.Models;

namespace SFA.DAS.Tools.Support.Web.Models
{
    public class UserViewModel
    {
        public string AccountId { get; set; }
        public IEnumerable<AccountUserDto> Users { get; set; } = new List<AccountUserDto>();
    }
}