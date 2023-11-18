using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Json;

namespace FileTransferManager.Api.Keycloak
{
    public class KeycloakRolesClaimsTransformation : IClaimsTransformation
    {
        private readonly string _roleClaimType;
        private readonly string _audience;

        public KeycloakRolesClaimsTransformation(string roleClaimType, string audience)
        {
            _roleClaimType = roleClaimType;
            _audience = audience;
        }

        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var result = principal.Clone();
            if (result.Identity is not ClaimsIdentity identity)
            {
                return Task.FromResult(result);
            }

            var resourceAccessValue = principal.FindFirst("resource_access")?.Value;
            if (string.IsNullOrWhiteSpace(resourceAccessValue))
            {
                return Task.FromResult(result);
            }

            using var resourceAccess = JsonDocument.Parse(resourceAccessValue);
            try
            {
                var clientRoles = resourceAccess
                    .RootElement
                    .GetProperty(_audience)
                    .GetProperty("roles");

                foreach (var role in clientRoles.EnumerateArray())
                {
                    var value = role.GetString();
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        identity.AddClaim(new Claim(_roleClaimType, value));
                    }
                }
            }
            catch (Exception)
            {
                // Handle exception if needed
            }

            return Task.FromResult(result);
        }
    }

}
