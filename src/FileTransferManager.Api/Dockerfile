FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/FileTransferManager.Api/FileTransferManager.Api.csproj", "src/FileTransferManager.Api/"]
RUN dotnet restore "src/FileTransferManager.Api/FileTransferManager.Api.csproj"
COPY . .
WORKDIR "/src/src/FileTransferManager.Api"
RUN dotnet build "FileTransferManager.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FileTransferManager.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FileTransferManager.Api.dll"]
