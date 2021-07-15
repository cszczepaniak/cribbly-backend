using System.Net.Http;
using Xunit;

namespace CribblyBackend.Api.Tests.Common
{
    public abstract class ApiTestBase : IClassFixture<ApiWebApplicationFactory>
    {
        protected readonly ApiWebApplicationFactory _factory;

        public ApiTestBase(ApiWebApplicationFactory fixture)
        {
            _factory = fixture;
        }
    }
}