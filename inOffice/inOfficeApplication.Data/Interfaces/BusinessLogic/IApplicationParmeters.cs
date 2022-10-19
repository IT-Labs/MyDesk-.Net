namespace inOfficeApplication.Data.Interfaces.BusinessLogic
{
    public interface IApplicationParmeters
    {
        string GetJwtIssuer();
        string GetJwtAudience();
        string GetMetadataAddress();
        string GetSentimentEndpoint();
        string GetCustomBearerTokenSigningKey();
        string GetTenantClaimKey();
        Dictionary<string, string> GetTenants();
    }
}
