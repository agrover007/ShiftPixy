using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
//using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace OpenAPITest
{
    public class ApiKeyMiddleWare
    {
        private readonly RequestDelegate _next;
        private const string APIKEYNAME = "ApiKey";
        public ApiKeyMiddleWare(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Api Key was not provided. (Using ApiKeyMiddleware) ");
                return;
            }

            var appSettings = context.RequestServices.GetRequiredService<IConfiguration>();

            var apiKey = appSettings.GetValue<string>(APIKEYNAME);
            
            //plain cmparison -- can be improved with HASH compare
            if (!apiKey.Equals(extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized client. (Using ApiKeyMiddleware)");
                return;
            }

            await _next(context);
        }

        #region -- HASHING attempt
        //var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("thisisasecretkey@123"));
        //var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        //var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(apiKey));
        //var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);


        //var storedSecretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(extractedApiKey));
        //var storedsigninCredentials = new SigningCredentials(storedSecretKey, SecurityAlgorithms.HmacSha256);

        //using (HashAlgorithm hashAlg = HashAlgorithm.Create())
        //SHA1Managed hashProvider = new SHA1Managed();

        //var hashAlg = HashAlgorithm.Create();
        //byte[] hashBytesA = hashAlg.ComputeHash(Encoding.UTF8.GetBytes(extractedApiKey));

        //    byte[] hashBytesB = hashAlg.ComputeHash(Encoding.UTF8.GetBytes(apiKey));


        //    if (BitConverter.ToString(hashBytesA) == BitConverter.ToString(hashBytesB))
        //    {
        //        context.Response.StatusCode = 401;
        //        await context.Response.WriteAsync("Unauthorized client. (Using ApiKeyMiddleware)");
        //        return;
        //    }
        //}

        //private static byte[] HashHMACHex(string keyHex, string messageHex)
        //{
        //    var key = Convert.FromHexString(hexKey);
        //    var message = Convert.FromHexString(messageHex);
        //    var hash = new HMACSHA256(key);
        //    return hash.ComputeHash(message);
        //}
        //private static byte[] StringEncode(string text)
        //{
        //    var encoding = new System.Text.ASCIIEncoding();
        //    return encoding.GetBytes(text);
        //}

        //private static string HashEncode(byte[] hash)
        //{
        //    return BitConverter.ToString(hash).Replace("-", "").ToLower();
        //}
        //private static byte[] HexDecode(string hex) => System.Convert.FromHexString(hex);

        //private static string HashHMACHex(string keyHex, string message)
        //{
        //    byte[] hash = HashHMAC(HexDecode(keyHex), StringEncode(message));
        //    return HashEncode(hash);
        //}
        #endregion

    }
}