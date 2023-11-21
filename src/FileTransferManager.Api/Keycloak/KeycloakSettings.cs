namespace FileTransferManager.Api.Keycloak
{
    public sealed class KeycloakSettings
    {
        public string? Authority { get; set; }
        public string? Audience { get; set; }
        public string? KeycloakResourceUrl { get; set; }
        public string? ClientCredentialsTokenAddress { get; set; }
    }
}
