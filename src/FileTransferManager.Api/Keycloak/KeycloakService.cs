using IdentityModel.Client;

namespace FileTransferManager.Api.Keycloak
{
    public class KeycloakService
    {
        private readonly HttpClient _client;
        private readonly TokenClient _tokenClient;

        public KeycloakService(HttpClient client, TokenClient tokenClient)
        {
            _client = client;
            _tokenClient = tokenClient;
        }

        public async Task CreateResource(Resource resource)
        {
            var token = await _tokenClient.GetClientCredentialsToken();
            _client.SetBearerToken(token);
            await _client.PostAsJsonAsync("", resource);
        }
    }
}
