using RestSharp;
using Newtonsoft.Json;

namespace OpenAPITest
{
    public class Token : IToken  
    {
        private readonly string _client_id;
        private readonly string _secret;

        private readonly string _url_token_create;
        private readonly string _url_token_exchange;

        private string _publicToken = String.Empty;
        public string _accessToken = String.Empty;

        private IConfiguration _config;
        private ILogger _logger;

        public Token(ILogger logger, IConfiguration config)
        {
            _config = config;
            _logger = logger;

            _client_id = _config.GetValue<string>("client_id");
            _secret = _config.GetValue<string>("secret");
            _url_token_create = _config.GetValue<string>("url_token_create");
            _url_token_exchange = _config.GetValue<string>("url_token_exchange");

        }

        public async Task<string> GetPublicToken() 
        {
            try
            {
                _logger.LogInformation("Calling GetPublicToken");

                var client = new RestClient(_url_token_create);
                var request = new RestRequest(_url_token_create, Method.Post);
                request.AddHeader("Content-Type", "application/json");

                var body = new
                {
                    client_id = _client_id,
                    secret = _secret,
                    institution_id = "ins_3",
                    initial_products = new string[] { "auth" },
                    options = new
                    {
                        webhook = "https://www.genericwebhookurl.com/webhook",
                    }
                };

                var bodyy = JsonConvert.SerializeObject(body);
                request.AddBody(bodyy, "application/json");
                RestResponse response = await client.ExecuteAsync(request);

                if (!response.IsSuccessful)
                {
                    _logger.LogError("GetPublicToken: Response failed");
                    return null;
                }

                var output = JsonConvert.DeserializeObject<PublicToken>(response.Content);

                if (output?.public_token != null)
                    return output.public_token;
                
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception: Calling GetPublicToken");
                Console.WriteLine(ex.Message);
                _logger.LogError(ex.Message);
            }
            return null;
        }

        public async Task<string> GetAccessToken() // connect Plaid exchange token
        {
            try
            {
                if (string.IsNullOrEmpty(_publicToken))
                    _publicToken = await GetPublicToken(); // ID token

                _logger.LogInformation("Calling GetAccessToken");
                var client = new RestClient(_url_token_exchange);
                var request = new RestRequest(_url_token_exchange, Method.Post);
                request.AddHeader("Content-Type", "application/json");
                var body = new
                {
                    client_id = _client_id,
                    secret = _secret,
                    public_token = _publicToken

                };
                var bodyy = JsonConvert.SerializeObject(body);
                request.AddBody(bodyy, "application/json");
                RestResponse response = await client.ExecuteAsync(request);
                if (!response.IsSuccessful)
                {
                    _logger.LogError("GetAccessToken: Response failed");
                    return null;
                }

                var output = JsonConvert.DeserializeObject<AccessToken>(response.Content);
                if (output?.access_token != null)
                    return output.access_token;

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception: Calling GetAccessToken");
                Console.WriteLine(ex.Message);
                _logger.LogError(ex.Message);
            }
            return null;
        }
    }

}
