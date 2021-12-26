using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Jellyfin.Data.Entities;
using MediaBrowser.Common;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Authentication;
using MediaBrowser.Controller.Net;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Jellyfin.Plugin.OIDC
{
    /// <summary>
    /// Ldap Authentication Provider Plugin.
    /// </summary>
    public class OidcAuthenticationProviderPlugin : IAuthenticationProvider
    {
        private readonly ILogger<OidcAuthenticationProviderPlugin> _logger;
        private readonly IApplicationHost _applicationHost;
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="OidcAuthenticationProviderPlugin"/> class.
        /// </summary>
        /// <param name="applicationHost">Instance of the <see cref="IApplicationHost"/> interface.</param>
        /// <param name="logger">Instance of the <see cref="ILogger{OidcAuthenticationProviderPlugin}"/> interface.</param>
        /// <param name="httpClientFactory">Instance of the <see cref="IHttpClientFactory"/> interface.</param>
        public OidcAuthenticationProviderPlugin(IApplicationHost applicationHost, ILogger<OidcAuthenticationProviderPlugin> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _applicationHost = applicationHost;
            _httpClientFactory = httpClientFactory;
        }

        private string? Authority => Plugin.Instance?.Configuration.Authority;

        private string? ApiClient => Plugin.Instance?.Configuration.ApiClient;

        private string? ClientId => Plugin.Instance?.Configuration.ClientId;

        private string? ClientSecret => Plugin.Instance?.Configuration.ClientId;

        private string? RedirectUri => Plugin.Instance?.Configuration.RedirectUri;

        private string[]? Scopes => Plugin.Instance?.Configuration.Scopes;

        /// <summary>
        /// Gets plugin name.
        /// </summary>
        public string Name => "OIDC-Authentication";

        /// <summary>
        /// Gets a value indicating whether gets plugin enabled.
        /// </summary>
        public bool IsEnabled => true;

        /// <summary>
        /// AUTH METHOD.
        /// </summary>
        /// <param name="username">username.</param>
        /// /// <param name="password">Password.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<ProviderAuthenticationResult> Authenticate(string username, string password)
        {
            var client = _httpClientFactory.CreateClient(NamedClient.Default);
            var discovery = await Discover().ConfigureAwait(true);

            if (Scopes != null)
            {
                var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = discovery.TokenEndpoint,
                    ClientId = ClientId,
                    ClientSecret = ClientSecret,
                    Scope = Scopes.IsNullOrEmpty() ? string.Empty : string.Join(" ", Scopes),
                }).ConfigureAwait(true);
            }

            var authRequest = await client.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest()
            {
                Address = discovery.AuthorizeEndpoint,
                ClientId = ClientId,
                ClientSecret = ClientSecret,
                RedirectUri = RedirectUri,
                GrantType = "code"
            }).ConfigureAwait(true);

            return new ProviderAuthenticationResult { Username = username };
        }

        /// <inheritdoc />
        public bool HasPassword(User user)
        {
            return true;
        }

        /// <inheritdoc />
        public Task ChangePassword(User user, string newPassword)
        {
            throw new NotImplementedException();
        }

        private async Task<DiscoveryDocumentResponse> Discover()
        {
            if (string.IsNullOrEmpty(Authority))
            {
                throw new NotSupportedException("Authority shouldn't be empty.");
            }
            else
            {
                var client = _httpClientFactory.CreateClient(NamedClient.Default);

                var disco = await client.GetDiscoveryDocumentAsync(Authority + "/.well-known/openid-configuration").ConfigureAwait(true);

                if (disco.IsError)
                {
                    throw new SecurityException(disco.Error);
                }

                return disco;
            }
        }
    }
}
