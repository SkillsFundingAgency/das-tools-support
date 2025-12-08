namespace SFA.DAS.Tools.Support.Infrastructure.OuterApi.Requests;

public class ChangeUserStatusRequest
{
    public required string ChangedByUserId { get; set; }
    public required string ChangedByEmail { get; set; }
}

