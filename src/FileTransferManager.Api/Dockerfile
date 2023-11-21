FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/FileTransferManager.Api/FileTransferManager.Api.csproj", "src/FileTransferManager.Api/"]
RUN dotnet restore "./src/FileTransferManager.Api/./FileTransferManager.Api.csproj"
COPY . .
WORKDIR "/src/src/FileTransferManager.Api"
RUN dotnet build "./FileTransferManager.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FileTransferManager.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .


# SSL sertifikalarini kopyala
# host.docker.internal - ssl start
COPY ["/src/FileTransferManager.Api/ssl/aspnetapp.pfx", "/app/ssl/aspnetapp.pfx"]
COPY ["/src/FileTransferManager.Api/ssl/docker-host.crt", "/etc/x509/https/docker-host.crt"]
COPY ["/src/FileTransferManager.Api/ssl/docker-host.key", "/etc/x509/https/docker-host.key"]
# host.docker.internal - ssl end




ENTRYPOINT ["dotnet", "FileTransferManager.Api.dll"]
