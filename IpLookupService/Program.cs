using System.Net;
using IpLookupService;
using IpLookupService.Configuration;
using IpLookupService.Contracts;
using IpLookupService.Exceptions;
using IpLookupService.Middleware;
using IpLookupService.Services;
using Microsoft.Extensions.Options;
using Polly;

const string externalIPProviderName = "IpStack";
const string ipCacheProviderName = "Cache";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<IPStackSettings>(builder.Configuration.GetSection(externalIPProviderName));
builder.Services.Configure<IPCacheSettings>(builder.Configuration.GetSection(ipCacheProviderName));

var retryPolicy = Policy
    .Handle<IPServiceNotAvailableException>()
    .Or<HttpRequestException>()
    .WaitAndRetryAsync(new[]
    {
        TimeSpan.FromSeconds(1),
        TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(10)
    });

builder.Services.AddHttpClient<IExternalIPService, ExternalIPService>((sp, client) =>
{
    var settings = sp.GetRequiredService<IOptions<IPStackSettings>>().Value;

    client.BaseAddress = new Uri(settings.BaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
}).AddPolicyHandler(Policy<HttpResponseMessage>
    .Handle<IPServiceNotAvailableException>()
    .WaitAndRetryAsync([
        TimeSpan.FromSeconds(1),
        TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(10)
    ]));

builder.Services.AddHttpClient<IIPCacheService, IPCacheService>(
    (sp, client) =>
    {
        var settings = sp.GetRequiredService<IOptions<IPCacheSettings>>().Value;
    
        client.BaseAddress = new Uri(settings.BaseUrl);
        client.DefaultRequestHeaders.Add("Accept", "application/json");
    });

builder.Services.AddTransient<IIPDetailsProvider, IPDetailsProvider>();

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