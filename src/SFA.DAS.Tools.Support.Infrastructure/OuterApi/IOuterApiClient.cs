using System.Threading.Tasks;

namespace SFA.DAS.Tools.Support.Infrastructure.OuterApi;

public interface IOuterApiClient
{
    Task<TResponse> Get<TResponse>(string url);
}