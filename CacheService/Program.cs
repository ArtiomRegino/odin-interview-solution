using CacheService;
using CacheService.CacheService;
using CacheService.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache(options =>
{
    options.SizeLimit = 10_000;
});
builder.Services.AddSingleton<ICacheStore, MemoryCacheStore>();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCacheEndpoints();

app.Run();