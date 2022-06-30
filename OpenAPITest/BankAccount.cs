using RestSharp;
using Newtonsoft.Json;

namespace OpenAPITest
{
    public class BankAccount : IBankAccount
    {
        private readonly string _url_balance;
        private readonly string _client_id;
        private readonly string _secret;

       // private string _publicToken = String.Empty;
       // private string _accessToken = String.Empty;

        private Token _token;
        private ILogger _logger;
        private IConfiguration _config;
        private DetailDb _detailDbContext;

        public BankAccount(ILogger logger, IConfiguration config, DetailDb context, Token token)
        {
            _logger = logger;
            _config = config;
            _detailDbContext = context;
            _token = token;

            _client_id = _config.GetValue<string>("client_id");
            _secret = _config.GetValue<string>("secret");
            _url_balance = _config.GetValue<string>("url_balance");
        }

        // For Singleton pattern
        //private static BankAccount? _instance;
        //public static readonly BankAccount instance = null;// new BankAccount();
        //public static BankAccount GetInstance() // makes sure single instance..
        //{
        //    if (_instance == null)
        //    {
        //        _instance = new BankAccount();//logger
        //        _logger = logger;
        //    }
        //    return _instance;
        //}

        public async Task<List<accounts>> GetAllAccounts()
        {
            try
            {
                var client = new RestClient(_url_balance);
                var request = new RestRequest(_url_balance, Method.Post);
                request.AddHeader("Content-Type", "application/json");
                var body = new
                {
                    client_id = _client_id,
                    secret = _secret,
                    access_token = _token._accessToken

                };
                var bodyy = JsonConvert.SerializeObject(body);
                request.AddBody(bodyy, "application/json");
                RestResponse response = await client.ExecuteAsync(request);
                if (!response.IsSuccessful)
                {
                    _logger.LogError("Plaid service acccount call failed.");
                    return null;
                }

                root output = JsonConvert.DeserializeObject<root>(response.Content);

                //dynamic output = JsonConvert.DeserializeObject(response.Content);
                //var accounts = output.accounts; //images[0].report.nemo;
                
                //Masking PII info like account ID before logging
                var maskedJson = JsonHelper.MaskJson(response.Content);
                _logger.LogInformation("{DT} Accounts Fetched: {account}", DateTime.UtcNow.ToLongTimeString(), maskedJson);

                // STORE List into EF In-Mem
                if (output?.accounts?.Count > 0)
                {
                    List<Detail> details = new List<Detail>();
                    foreach (var item in output.accounts)
                    {
                        // Add into in-memory db for test
                        Detail detail = new Detail()
                        {
                            account_id = item.account_id,
                            current = item.balances.current,
                            mask = item.mask,
                            name = item.name,
                            type = item.type
                        };
                        details.Add(detail);
                        await _detailDbContext.AddAsync(detail);
                    }
                    await _detailDbContext.SaveChangesAsync();
                    return output.accounts;
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception: Calling GetAllAccounts");
                Console.WriteLine(ex.Message);
                _logger.LogError(ex.Message);
            }
            return null;
        }

        public async Task<accounts> GetSingleAccountDetails(string AccountSubType) 
        {
            try
            {
                var client = new RestClient(_url_balance);
                var request = new RestRequest(_url_balance, Method.Post);
                request.AddHeader("Content-Type", "application/json");
                var body = new
                {
                    client_id = _client_id,
                    secret = _secret,
                    access_token = _token._accessToken

                };
                var bodyy = JsonConvert.SerializeObject(body);
                request.AddBody(bodyy, "application/json");
                RestResponse response = await client.ExecuteAsync(request);
                if (!response.IsSuccessful)
                {
                    _logger.LogError("Plaid service acccount service.");
                    return null;
                }

                root output = JsonConvert.DeserializeObject<root>(response.Content);

                var act = output?.accounts.Where(p => p.subtype.Trim().ToLower() == AccountSubType.Trim().ToLower()).FirstOrDefault();
                if (act == null)
                    return null;

                //Masking PII info like account ID before logging
                var maskedJson = JsonHelper.MaskJson(JsonConvert.SerializeObject(act));
                _logger.LogInformation("{DT} Account Fetched: {account}", DateTime.UtcNow.ToLongTimeString(), maskedJson);

                // Add into in-memory db for test
                Detail detail = new Detail()
                {
                    account_id = act.account_id,
                    current = act.balances.current,
                    mask = act.mask,
                    name = act.name,
                    type = act.type
                };

                await _detailDbContext.AddAsync(detail);
                await _detailDbContext.SaveChangesAsync();

                return act;
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception: Calling GetSingleAccountDetails");
                Console.WriteLine(ex.Message);
                _logger.LogError(ex.Message);
            }
            return null;
        }
    }
}