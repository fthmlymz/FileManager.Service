using IdentityModel.Client;

namespace FileTransferManager.Api.Keycloak
{
    public class TokenClient
    {
        private readonly HttpClient _client;
        private readonly ClientCredentialsTokenRequest _tokenRequest;

        public TokenClient(HttpClient client, ClientCredentialsTokenRequest tokenRequest)
        {
            _client = client;
            _tokenRequest = tokenRequest;
        }

        public async Task<string> GetClientCredentialsToken()
        {
            var response = await _client.RequestClientCredentialsTokenAsync(_tokenRequest);
            return response.AccessToken;
        }
    }
}
