FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY NavegaStudio.csproj .
COPY nuget.config .
RUN dotnet restore NavegaStudio.csproj

# Copy everything else and publish
COPY . .
RUN dotnet publish NavegaStudio.csproj -c Release -o /app/publish --no-restore

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Railway sets PORT env var at runtime - use shell form to expand it
CMD ASPNETCORE_URLS=http://0.0.0.0:${PORT:-3000} dotnet NavegaStudio.dll
