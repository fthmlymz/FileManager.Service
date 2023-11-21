using FileTransferManager.Api.Configurations;
using FileTransferManager.Api.Keycloak;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.Configure<FileSettings>(builder.Configuration.GetSection("FileSettings"));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));


#region Kestrel Docker
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    var httpPort = builder.Configuration["ASPNETCORE_HTTP_PORTS"];
    var httpsPort = builder.Configuration["ASPNETCORE_HTTPS_PORTS"];

    if (builder.Environment.IsDevelopment() || builder.Environment.IsProduction())
    {
        if (!string.IsNullOrEmpty(httpPort))
        {
            serverOptions.ListenAnyIP(Convert.ToInt32(httpPort));
        }

        if (!string.IsNullOrEmpty(httpsPort))
        {
            serverOptions.ListenAnyIP(Convert.ToInt32(httpsPort), listenOptions =>
            {
                listenOptions.UseHttps(builder.Configuration.GetSection("ASPNETCORE_Kestrel:Certificates:Default:Path").Value,
                    builder.Configuration.GetSection("ASPNETCORE_Kestrel:Certificates:Default:Password").Value);
            });
        }
    }
});
#endregion


#region Keycloak JWT
var keycloakSettings = builder.Configuration.GetSection("Keycloak").Get<KeycloakSettings>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddJwtBearer(options =>
           {
               options.Authority = keycloakSettings.Authority;
               options.Audience = keycloakSettings.Audience;
               options.RequireHttpsMetadata = false;
               options.UseSecurityTokenValidators = true;
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateIssuerSigningKey = true,
                   RequireSignedTokens = false,
                   SignatureValidator = delegate (string token, TokenValidationParameters parameters)
                   {
                       var jwt = new JwtSecurityToken(token);

                       return jwt;
                   },
                   ValidateLifetime = true,
                   RequireExpirationTime = true,
                   ClockSkew = TimeSpan.Zero,
               };
               options.Events = new JwtBearerEvents
               {
                   OnTokenValidated = context =>
                   {
                       return Task.CompletedTask;
                   },
               };
           });
builder.Services.AddTransient<IClaimsTransformation>(_ => new KeycloakRolesClaimsTransformation("role", keycloakSettings.Audience));
builder.Services.AddAuthorization(options =>
{
    #region File Transfer Permissions
    options.AddPolicy("FileTransferReadRole", builder => { builder.AddRequirements(new RptRequirement("res:filetransfer", "scopes:read")); });
    options.AddPolicy("FileTransferCreateRole", builder => { builder.AddRequirements(new RptRequirement("res:filetransfer", "scopes:create")); });
    options.AddPolicy("FileTransferDeleteRole", builder => { builder.AddRequirements(new RptRequirement("res:filetransfer", "scopes:delete")); });
    #endregion
});
builder.Services.AddHttpClient<KeycloakService>(client =>
{
    client.BaseAddress = new Uri(keycloakSettings.KeycloakResourceUrl);
});
builder.Services.AddHttpClient<IdentityModel.Client.TokenClient>();
builder.Services.AddSingleton(new ClientCredentialsTokenRequest
{
    Address = keycloakSettings?.ClientCredentialsTokenAddress
});
builder.Services.AddScoped<IAuthorizationHandler, RptRequirementHandler>();
#endregion




var app = builder.Build();
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
