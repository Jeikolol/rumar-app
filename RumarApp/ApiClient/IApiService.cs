using System.Threading.Tasks;

namespace RumarApp.ApiClient
{
    public interface IApiService<TRequest, TResponse>
    {
        Task<TResponse> ExecuteAsync(TRequest request);
    }
}
