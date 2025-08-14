# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY BlogSystem/*.csproj ./
RUN dotnet restore

# Copy the remaining source code and build
COPY BlogSystem/ ./
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS runtime
WORKDIR /app

# Copy published app from build stage
COPY --from=build /app/publish .

# Copy environment file and static assets
COPY BlogSystem/.env /app/.env
COPY BlogSystem/wwwroot /app/wwwroot

# Copy Content/posts so migrations won't fail
COPY BlogSystem/Content/posts /app/Content/posts

# Expose port 8080 (default for minimal API in container)
EXPOSE 8080

# Set environment for ASP.NET Core
ENV ASPNETCORE_URLS=http://+:8080

# Start the app
ENTRYPOINT ["dotnet", "BlogSystem.dll"]
