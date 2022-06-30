using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OpenAPITest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BankAccountController : ControllerBase
    {
        private readonly ILogger<BankAccountController> _logger ;

        public static IBankAccount _bankAct; // to avoid trips to get token again if valid

        private DetailDb _detailDbContext;
        private Token _token;
        public BankAccountController(ILogger<BankAccountController> logger, IConfiguration config, DetailDb context, Token token) 
        {
            _logger = logger;
            _detailDbContext = context;
            _token = token;

            _bankAct = new BankAccount(logger, config, context, token);

            //_bankAct = BankAccount.GetInstance(); // for singleton attempt
        }

        //[Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        //[HttpGet(Name = "BankAccounts"), Authorize] //
        [HttpGet(Name = "BankAccounts")]
        public async Task<IActionResult> BankAccounts()
        {

            _logger.LogInformation("{DT} GetAllAccounts", DateTime.UtcNow.ToLongTimeString());
            var accounts = await _bankAct.GetAllAccounts(); //_logger
            return Ok(accounts);
        }

        [HttpGet("{AccountSubType}")] 
        public async Task<IActionResult> BankAccounts(string AccountSubType)
        {
            _logger.LogInformation("{DT} GetSingleAccountDetails", DateTime.UtcNow.ToLongTimeString());
            var account = await _bankAct.GetSingleAccountDetails(AccountSubType);

            return Ok(account);
        }
    }
}
