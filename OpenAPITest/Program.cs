
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Configuration;
using System.Text;
using OpenAPITest;
using Microsoft.Extensions.Configuration;
using Serilog;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// For Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// In-memory EF - db context
builder.Services.AddDbContext<DetailDb>(options => options.UseInMemoryDatabase("items"));

builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Plaid-OpenAPI Test", Version = "v1" });
    option.OperationFilter<AddRequiredHeaderParameter>(); // To Add ApiKey Header param with every Demo API call - can improve for bearer token
});

// Add your logging handler
//builder.Logging.AddLog4Net();
//builder.Services.AddLogging();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(); // DI for Serilog


// Add Token into middleware like EF DbContext
//var logger = builder.Logging.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
var logger = builder.Logging.Services.BuildServiceProvider().GetService<ILogger<Program>>();
var config = builder.Configuration;
//IWebHostEnvironment environment = builder.Environment;
var token = new Token(logger, config);
token._accessToken = await token.GetAccessToken();
builder.Services.AddSingleton<Token>(token);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

//for API simple key -- DI for API KEY
app.UseMiddleware<ApiKeyMiddleWare>();

app.MapControllers();

// Get Details
app.MapGet("/details", async (DetailDb db) => await db.Details.ToListAsync());

// Post manually Details
app.MapPost("/detail", async (DetailDb db, Detail detail) =>
{
    await db.Details.AddAsync(detail);
    await db.SaveChangesAsync();
    return Results.Created($"/detail/{detail.Id}", detail);
});

app.MapDelete("/detail", async (DetailDb db) =>
{
    foreach (var detail in db.Details)
        db.Details.Remove(detail);

    await db.SaveChangesAsync();
    return Results.Ok();
});


app.Run();


#region  JWT Authentication attempt
/*
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//});

//var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);
//// Add services to the container.
//builder.Services.AddJWTTokenServices(builder.Configuration);
var bindJwtSettings = new JwtSettings();

//Microsoft.Extensions.Configuration.ConfigurationBinder.B();

//Microsoft.Extensions.Configuration.ConfigurationBinder.Bind()"JsonWebTokenKeys", bindJwtSettings);
builder.Services.AddSingleton(bindJwtSettings);

//builder.Services.AddJWTTokenServices(builder.Configuration);
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options => {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
        {
            //ValidateIssuerSigningKey = bindJwtSettings.ValidateIssuerSigningKey,
            //IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(bindJwtSettings.IssuerSigningKey)),
            //ValidateIssuer = bindJwtSettings.ValidateIssuer,
            //ValidIssuer = bindJwtSettings.ValidIssuer,
            //ValidateAudience = bindJwtSettings.ValidateAudience,
            //ValidAudience = bindJwtSettings.ValidAudience,
            //RequireExpirationTime = bindJwtSettings.RequireExpirationTime,
            //ValidateLifetime = bindJwtSettings.RequireExpirationTime,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("64A63153-11C1-4919-9133-EFAF99A9B456")),
            ValidateIssuer = true,
            ValidIssuer = "http://localhost:7088",
            ValidateAudience = true,
            ValidAudience = "http://localhost:7088",
            RequireExpirationTime = true,
            ValidateLifetime = true,

            ClockSkew = TimeSpan.FromDays(1),
        };
});
*/

//issuer: "ABCXYZ",
//                                            audience: "http://localhost:7088",
//                                            claims: new List<Claim>(),
//                                            expires: DateTime.Now.AddMinutes(10),
//                                            signingCredentials: signinCredentials

// Add services to the container.
#endregion 

#region -- JWT in OpenAPI
//builder.Services.AddSwaggerGen(option =>
//{
//    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Plaid API test", Version = "v1" });

//    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        In = ParameterLocation.Header,
//        Description = "Please enter a valid token",
//        Name = "Authorization",
//        Type = SecuritySchemeType.Http,
//        BearerFormat = "JWT",
//        Scheme = "Bearer"
//    });
//    option.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference {
//                    Type=ReferenceType.SecurityScheme,
//                    Id="Bearer"
//                }
//            },
//            new string[] {}
//        }
//    });

//});


//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => Configuration.Bind("JwtSettings", options))
//.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => Configuration.Bind("CookieSettings", options));


//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => new { d })
//.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);


//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
//})// Adding Jwt Bearer
//.AddJwtBearer(options =>
//{
//    options.SaveToken = true;
//    options.RequireHttpsMetadata = false;
//    options.TokenValidationParameters = new TokenValidationParameters()
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidAudience = "http://localhost:7088",
//        ValidIssuer = "ABCXYZ",
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("thisisasecretkey@123"))
//    };
//});


//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//});


//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = DefaultAuthenticationTypes.ApplicationCookie;
//})

//builder.Services.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, configureOptions);
#endregion

