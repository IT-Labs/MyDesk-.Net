namespace inOffice.BusinessLogicLayer.Interface
{
    public interface IApplicationParmeters
    {
        public string GetJwtIssuer();
        public string GetJwtAudience();
        public string GetSettingsMetadataAddress();
        public string GetSettingsSentimentEndpoint();
        public string GetSettingsUseCustomBearerToken();
        public string GetSettingsCustomBearerTokenSigningKey();
    }
}
