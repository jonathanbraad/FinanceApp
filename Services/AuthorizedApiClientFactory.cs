using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace FinanceApp.Web.Services {
    public class AuthorizedApiClientFactory {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthorizedApiClientFactory(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor) {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public HttpClient CreateClientWithJwt() {
            var client = _httpClientFactory.CreateClient();
            var jwt = _httpContextAccessor.HttpContext?.Request.Cookies["jwt"];

            if (!string.IsNullOrEmpty(jwt)) {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            }

            return client;
        }
    }
}
