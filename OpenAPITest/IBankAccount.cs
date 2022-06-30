namespace OpenAPITest
{
    public interface IBankAccount
    {
        public Task<List<accounts>> GetAllAccounts();
        public Task<accounts> GetSingleAccountDetails(string AccountSubType);
    }
}
