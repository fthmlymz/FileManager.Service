using FileTransferManager.Api.Configurations;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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


var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
