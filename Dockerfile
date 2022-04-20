FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /build

# Copy csproj and restore as distinct layers
COPY observability.sln .
COPY ./src/monitoring/monitoring.csproj src/monitoring/
COPY ./src/worker/worker.csproj src/worker/
COPY ./src/web/web.csproj src/web/
RUN dotnet restore

# copy and build app and libraries
COPY ./src src
RUN dotnet build -c Release -o /app/build

FROM build as publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "web.dll"]