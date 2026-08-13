FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY egl-kinexa.sln ./
COPY src/EGL.Kinexa.Domain/EGL.Kinexa.Domain.csproj src/EGL.Kinexa.Domain/
COPY src/EGL.Kinexa.Application/EGL.Kinexa.Application.csproj src/EGL.Kinexa.Application/
COPY src/EGL.Kinexa.Infrastructure/EGL.Kinexa.Infrastructure.csproj src/EGL.Kinexa.Infrastructure/
COPY src/EGL.Kinexa.Persistence/EGL.Kinexa.Persistence.csproj src/EGL.Kinexa.Persistence/
COPY src/EGL.Kinexa.API/EGL.Kinexa.API.csproj src/EGL.Kinexa.API/

# Restore
RUN dotnet restore

# Copy everything and build
COPY . .
RUN dotnet publish src/EGL.Kinexa.API/EGL.Kinexa.API.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "EGL.Kinexa.API.dll"]
