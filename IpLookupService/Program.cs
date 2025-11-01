using IpLookupService;
using IpLookupService.Middleware;
using IpLookupService.Services;
using Microsoft.Extensions.Options;

const string externalIPProviderName = "IpStack";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<IPStackSettings>(builder.Configuration.GetSection(externalIPProviderName));


builder.Services.AddHttpClient<IExternalIPService, ExternalIPService>(
    (sp, client) =>
{
    var settings = sp.GetRequiredService<IOptions<IPStackSettings>>().Value;
    
    client.BaseAddress = new Uri(settings.BaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseIpAddressEndpoints();

app.Run();