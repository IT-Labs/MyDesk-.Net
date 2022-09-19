namespace inOffice.BusinessLogicLayer.Interface
{
    public interface IApplicationParmeters
    {
        public string GetJwtIssuer();
        public string GetJwtAudience();
        public string GetMetadataAddress();
        public string GetSentimentEndpoint();
        public string GetCustomBearerTokenSigningKey();
    }
}
