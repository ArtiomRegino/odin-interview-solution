using BatchService;
using BatchService.Configuration;
using BatchService.Contracts;
using BatchService.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);


const string ipLookupName = "IPLookup";

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<IPLookupSettings>(builder.Configuration.GetSection(ipLookupName));

builder.Services.AddTransient<IIPLookupService, IPLookupService>();

builder.Services.AddHttpClient<IIPLookupService, IPLookupService>(
    (sp, client) =>
    {
        var settings = sp.GetRequiredService<IOptions<IPLookupSettings>>().Value;
    
        client.BaseAddress = new Uri(settings.BaseUrl);
        client.DefaultRequestHeaders.Add("Accept", "application/json");
    });

builder.Services.AddHostedService<BatchProcessorBackgroundService>();
var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();