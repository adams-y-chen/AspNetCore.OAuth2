using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ImageGallery.Client.Controllers.HttpHandlers
{
    public class BearerTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;

        public BearerTokenHandler(IHttpContextAccessor httpContextAccessor,
            IHttpClientFactory httpClientFactory)
        {
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));

            _httpClientFactory = httpClientFactory ??
                throw new ArgumentNullException(nameof(httpClientFactory));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var accessToken = await GetAccessTokenAsync();

            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                request.SetBearerToken(accessToken);
            }

            return await base.SendAsync(request, cancellationToken);
        }

        /**
         * Return a valid access token. Check if cached access token has expired. If so, request a new one from IDP.
         */
        public async Task<string> GetAccessTokenAsync()
        {
            var expiresAt = await _httpContextAccessor
                .HttpContext.GetTokenAsync("expires_at"); // the univeral time saved by the middleware

            var expiresAtAsDatetimeOffset =
                DateTimeOffset.Parse(expiresAt, CultureInfo.InvariantCulture);

            var expresAtAsUtc = (expiresAtAsDatetimeOffset.AddSeconds(-60)).ToUniversalTime();

            if (expresAtAsUtc > DateTime.UtcNow)
            {
                // still valid. return it.
                var accessToken = await _httpContextAccessor
                    .HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

                return accessToken;
            }

            var idpClient = _httpClientFactory.CreateClient("IDPClient");

            // get discovery document from IDP
            var discoveryResponse = await idpClient.GetDiscoveryDocumentAsync();

            // we need the refresh token to get new access token
            var refreshToken = await _httpContextAccessor
                .HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            var refreshResponse = await idpClient.RequestRefreshTokenAsync(
                new RefreshTokenRequest()
                {
                    Address = discoveryResponse.TokenEndpoint,
                    ClientId = "imagegalleryclient",
                    ClientSecret = "secret",
                    RefreshToken = refreshToken
                });

            // store the tokens             
            var updatedTokens = new List<AuthenticationToken>();
            updatedTokens.Add(new AuthenticationToken
            {
                Name = OpenIdConnectParameterNames.IdToken,
                Value = refreshResponse.IdentityToken
            });
            updatedTokens.Add(new AuthenticationToken
            {
                Name = OpenIdConnectParameterNames.AccessToken,
                Value = refreshResponse.AccessToken
            });
            updatedTokens.Add(new AuthenticationToken
            {
                Name = OpenIdConnectParameterNames.RefreshToken,
                Value = refreshResponse.RefreshToken
            });
            updatedTokens.Add(new AuthenticationToken
            {
                Name = "expires_at",
                Value = (DateTime.UtcNow + TimeSpan.FromSeconds(refreshResponse.ExpiresIn)).
                        ToString("o", CultureInfo.InvariantCulture)
            });

            // get authenticate result, containing the current principal & 
            // properties
            var currentAuthenticateResult = await _httpContextAccessor
                .HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // store the updated tokens
            currentAuthenticateResult.Properties.StoreTokens(updatedTokens);

            // sign in
            await _httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                currentAuthenticateResult.Principal,
                currentAuthenticateResult.Properties);

            return refreshResponse.AccessToken;
        }
    }
}
