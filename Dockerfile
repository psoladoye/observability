FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY observability.sln .
COPY ./src/monitoring/monitoring.csproj src/monitoring/
COPY ./src/worker/worker.csproj src/worker/
COPY ./src/web/web.csproj src/web/
RUN dotnet restore

# copy and build app and libraries
COPY ./src src
RUN dotnet build -c Release

FROM build AS publish
RUN dotnet publish -c Release

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app

COPY --from=publish /app/src/web/bin/Release/net7.0/publish ./web/
COPY --from=publish /app/src/worker/bin/Release/net7.0/publish ./worker/

EXPOSE 80
EXPOSE 443

WORKDIR /app/web
ENTRYPOINT ["dotnet", "web.dll"]