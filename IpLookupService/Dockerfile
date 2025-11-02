FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Common/Common.csproj ./Common/
COPY IpLookupService/IpLookupService.csproj ./IpLookupService/

RUN dotnet restore ./IpLookupService/IpLookupService.csproj

COPY Common ./Common
COPY IpLookupService ./IpLookupService

WORKDIR /src/IpLookupService

RUN dotnet publish IpLookupService.csproj -c Release -o /app/publish /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

EXPOSE 8080

ENTRYPOINT ["dotnet", "IpLookupService.dll"]
