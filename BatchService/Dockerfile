FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Common/Common.csproj ./Common/
COPY BatchService/BatchService.csproj ./BatchService/

RUN dotnet restore ./BatchService/BatchService.csproj

COPY Common ./Common
COPY BatchService ./BatchService

WORKDIR /src/BatchService

RUN dotnet publish BatchService.csproj -c Release -o /app/publish /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

EXPOSE 8080

ENTRYPOINT ["dotnet", "BatchService.dll"]
