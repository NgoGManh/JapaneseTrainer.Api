using Microsoft.AspNetCore.Http;

namespace JapaneseTrainer.Api.Common
{
    /// <summary>
    /// Service to provide CommonsDependencies via dependency injection
    /// </summary>
    public interface ICommonsDependenciesService
    {
        CommonsDependencies GetCommonsDependencies();
    }

    /// <summary>
    /// Implementation of ICommonsDependenciesService
    /// </summary>
    public class CommonsDependenciesService : ICommonsDependenciesService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CommonsDependenciesService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public CommonsDependencies GetCommonsDependencies()
        {
            var httpContext = _httpContextAccessor.HttpContext 
                ?? throw new InvalidOperationException("HttpContext is not available");
            
            return new CommonsDependencies(httpContext);
        }
    }
}

