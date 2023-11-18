using System.Text.Json.Serialization;

namespace FileTransferManager.Api.Keycloak
{
    public class Resource
    {
        public Resource(string name, string[] scopes)
        {
            Name = name;
            Scopes = scopes;
        }

        public string Name { get; }
        public string? Type { get; set; }

        [JsonPropertyName("resource_scopes")]
        public string[] Scopes { get; }

        public Dictionary<string, string> Attributes { get; } = new Dictionary<string, string>();
    }
}
