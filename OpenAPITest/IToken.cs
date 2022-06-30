namespace OpenAPITest
{
    public interface IToken
    {
        public Task<string> GetPublicToken();
        public Task<string> GetAccessToken();
    }
}
