namespace OpenAPITest
{
    public class accounts
    {
        public string account_id { get; set; }
        public balances balances { get; set; }
        public string mask { get; set; }
        public string name { get; set; }
        public string official_name { get; set; }
        public string subtype { get; set; }
        public string type { get; set; }
    }

    public class balances
    {
        public int? available { get; set; }
        public double current { get; set; }
        public string iso_currency_code { get; set; }
        public int? limit { get; set; }
        public object unofficial_currency_code { get; set; }
    }

    public class item
    {
        public List<string> available_products { get; set; }
        public List<string> billed_products { get; set; }
        public object consent_expiration_time { get; set; }
        public object error { get; set; }
        public string institution_id { get; set; }
        public string item_id { get; set; }
        public object optional_products { get; set; }
        public List<string> products { get; set; }
        public string update_type { get; set; }
        public string webhook { get; set; }
    }

    public class root
    {
        public List<accounts> accounts { get; set; }
        //public account[] accounts { get; set; }
        public item item { get; set; }
        public string request_id { get; set; }
    }


    public class AccessToken
    {
        public string? access_token { get; set; }
        public string? item_id { get; set; }
        public string? request_id { get; set; }
    }

    public class PublicToken
    {
        public string? public_token { get; set; }
        public string? request_id { get; set; }
    }

    //public class JwtAuthenticationDefaults
    //{
    //    public const string AuthenticationScheme = "JWT";
    //    public const string HeaderName = "Authorization";
    //    public const string BearerPrefix = "Bearer";
    //}

    //public class LoginDTO
    //{
    //    public string UserName { get; set; }
    //    public string Password { get; set; }
    //}
}


