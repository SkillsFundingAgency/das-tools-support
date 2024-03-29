namespace SFA.DAS.Tools.Support.Web.Models;

public abstract class UsersViewModelBase
{
    public string HashedAccountId { get; set; }
    public long? InternalAccountId { get; set; }
    public IEnumerable<AccountUserRow> Users { get; set; }
    public string UserData { get; set; }
    public bool HasError { get; set; }
    public string GetEmployerUsersTableData() => JsonSerializer.Serialize(Users, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

    public static bool TryDeserialise(string json, out IEnumerable<AccountUserRow> users) 
    {
        users = null;
        try
        {
            users = JsonSerializer.Deserialize<IEnumerable<AccountUserRow>>(json, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            return true;
        }
        catch
        {
            return false;
        }
    } 
}