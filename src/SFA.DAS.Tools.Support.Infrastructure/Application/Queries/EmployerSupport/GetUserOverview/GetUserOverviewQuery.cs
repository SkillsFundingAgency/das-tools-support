using System;
using MediatR;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetUserOverview;

public class GetUserOverviewQuery : IRequest<GetUserOverviewQueryResult>
{
    public Guid UserId { get; set; }
}
