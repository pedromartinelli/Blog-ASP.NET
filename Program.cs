using Blog;
using Blog.Data;
using Blog.Models;
using Blog.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
LoadConfiguration(builder);
ConfigureAuthentication(builder);
ConfigureMvc(builder);
ConfigureServices(builder);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();
app.Run();

void LoadConfiguration(WebApplicationBuilder builder)
{
    var configuration = builder.Configuration;

    Configuration.ApiUrl = configuration.GetValue<string>("ApiUrl");
    Configuration.JwtKey = configuration.GetValue<string>("JwtKey");
    Configuration.ApiKeyName = configuration.GetValue<string>("ApiKeyName");
    Configuration.ApiKey = configuration.GetValue<string>("ApiKey");

    var smtp = new Configuration.SmtpConfiguration();
    configuration.GetSection("Smtp").Bind(smtp);
    Configuration.Smtp = smtp;
}

void ConfigureAuthentication(WebApplicationBuilder builder)
{
    var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);
    builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(x =>
    {
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
}

void ConfigureMvc(WebApplicationBuilder builder)
{
    builder.Services.AddMemoryCache();
    builder
        .Services
        .AddControllers()
        .ConfigureApiBehaviorOptions(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        })
        .AddJsonOptions(x =>
        {
            x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
        });
}

void ConfigureServices(WebApplicationBuilder builder)
{
    //AddTransient - toda vez que for invocado o objeto será inicializado
    //AddScoped - o objeto será inicializado uma vez no começo da requisição e finalizado ao final
    //AddSingleton - o objeto será inicializado apenas uma vez e assim ficará por toda a aplicação
    builder.Services.AddDbContext<BlogDataContext>();
    builder.Services.AddTransient<TokenService>();
    builder.Services.AddTransient<PasswordHasher<User>>();
    builder.Services.AddTransient<EmailService>();
}