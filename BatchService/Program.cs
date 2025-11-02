using System.Reflection;
using System.Threading.Channels;
using BatchService;
using BatchService.Configuration;
using BatchService.Contracts;
using BatchService.Models;
using BatchService.Services;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


const string ipLookupName = "IPLookup";

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Batch Service API",
        Version = "v1",
        Description = "Handles asynchronous batch IP processing."
    });
    
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

builder.Services.Configure<IPLookupSettings>(builder.Configuration.GetSection(ipLookupName));

builder.Services.AddTransient<IIPLookupService, IPLookupService>();
builder.Services.AddTransient<IBatchScheduler, BatchScheduler>();
builder.Services.AddSingleton<IBatchStore, BatchStore>();
builder.Services.AddSingleton<IBatchQueue, BatchQueue>();

builder.Services.AddHttpClient<IIPLookupService, IPLookupService>(
    (sp, client) =>
    {
        var settings = sp.GetRequiredService<IOptions<IPLookupSettings>>().Value;
    
        client.BaseAddress = new Uri(settings.BaseUrl);
        client.DefaultRequestHeaders.Add("Accept", "application/json");
    });

builder.Services.AddHostedService<BatchProcessor>();
builder.Services.AddSingleton<Channel<BatchJob>>(_ => Channel.CreateUnbounded<BatchJob>(
    new UnboundedChannelOptions
{
    SingleReader = false,
    SingleWriter = false
}));

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