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



//#region Kestrel docker-compose.yml
//builder.WebHost.ConfigureKestrel(serverOptions =>
//{
//    var urls = builder.Configuration["ASPNETCORE_URLS"];
//    if (builder.Environment.IsDevelopment())
//    {
//        var uri = new Uri(urls);
//        var port = uri.Port;
//        serverOptions.Listen(IPAddress.Any, port);
//    }
//    else
//    {
//        serverOptions.Listen(IPAddress.Any, 80);
//        // Development 4002
//        //serverOptions.Listen(IPAddress.Any, 443, listenOptions =>
//        //{
//        //    listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
//        //    listenOptions.UseHttps(builder.Configuration.GetSection("ASPNETCORE_Kestrel:Certificates:Default:Path").Value,
//        //        builder.Configuration.GetSection("ASPNETCORE_Kestrel:Certificates:Default:Password").Value);
//        //});
//    }
//});
//#endregion


#region Keycloak JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddJwtBearer(options =>
           {
               options.Authority = builder.Configuration.GetSection("Keycloak:Authority").Value;
               options.Audience = builder.Configuration.GetSection("Keycloak:Audience").Value;
               options.RequireHttpsMetadata = false;
               options.UseSecurityTokenValidators = true;
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = false,
                   ValidateAudience = false,
                   ValidateIssuerSigningKey = true,
                   RequireSignedTokens = false,
                   SignatureValidator = delegate (string token, TokenValidationParameters parameters)
                   {
                       var jwt = new JwtSecurityToken(token);

                       return jwt;
                   },
                   ValidateLifetime = false,
                   RequireExpirationTime = false,
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
builder.Services.AddTransient<IClaimsTransformation>(_ => new KeycloakRolesClaimsTransformation("role", builder.Configuration.GetSection("Keycloak:Audience").Value));
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
    client.BaseAddress = new Uri(builder.Configuration.GetSection("Keycloak:KeycloakResourceUrl").Value);
});
builder.Services.AddHttpClient<IdentityModel.Client.TokenClient>();
builder.Services.AddSingleton(builder.Configuration.GetSection("Keycloak:ClientCredentialsTokenRequest").Get<ClientCredentialsTokenRequest>());
builder.Services.AddScoped<IAuthorizationHandler, RptRequirementHandler>();
#endregion




var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseStaticFiles();
//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
