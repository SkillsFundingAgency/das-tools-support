using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Tools.Support.Core.Models;
using AutoMapper;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace SFA.DAS.Tools.Support.Infrastructure.Services
{
    public class EmployerUsersService : IEmployerUsersService
    {
        private readonly IEmployerUsersApiClient _employerUsersApi;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public EmployerUsersService(IEmployerUsersApiClient employerUsersApi, IMapper mapper, ILogger<EmployerUsersService> logger)
        {
            _employerUsersApi = employerUsersApi;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<SuspendUsersResult> SuspendUsers(Core.Models.SuspendUsersRequest request, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
