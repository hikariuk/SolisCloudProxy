FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src
COPY ["src/Directory.Build.props", "src/"]
COPY ["src/Directory.Build.targets", "src/"]
COPY ["src/SolisCloudProxy/SolisCloudProxy.csproj", "src/SolisCloudProxy/"]
RUN dotnet restore "./src/SolisCloudProxy/SolisCloudProxy.csproj"

COPY . .

WORKDIR "/src/src/SolisCloudProxy"
RUN dotnet build "./SolisCloudProxy.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SolisCloudProxy.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SolisCloudProxy.dll"]