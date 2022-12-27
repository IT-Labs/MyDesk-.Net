namespace MyDesk.Data.Interfaces.BusinessLogic
{
    public interface IApplicationParmeters
    {
        string GetAdminEmail();
        string GetAdminPassword();
        string GetConnectionString();
        string GetSentimentEndpoint();
        string GetTenantClaimKey();
        Dictionary<string, string> GetTenants();
        string GetCustomBearerTokenSigningKey(bool isDevelopment);
        string GetAzureAdIssuer(bool isDevelopment);
        string GetAzureAdAudience(bool isDevelopment);
        string GetAzureAdMetadataAddress(bool isDevelopment);
        string GetGoogleIssuer(bool isDevelopment);
        string GetGoogleClientId(bool isDevelopment);
    }
}