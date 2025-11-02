FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Common/Common.csproj ./Common/
COPY CacheService/CacheService.csproj ./CacheService/

RUN dotnet restore ./CacheService/CacheService.csproj

COPY Common ./Common
COPY CacheService ./CacheService

WORKDIR /src/CacheService

RUN dotnet publish CacheService.csproj -c Release -o /app/publish /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

EXPOSE 8080

ENTRYPOINT ["dotnet", "CacheService.dll"]
