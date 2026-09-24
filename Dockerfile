# syntax=docker/dockerfile:1

# ---- Etapa 1: build ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["WeatherApiDio.csproj", "./"]
RUN dotnet restore "WeatherApiDio.csproj"
COPY . .
RUN dotnet build "WeatherApiDio.csproj" -c Release -o /app/build

# ---- Etapa 2: publish ----
FROM build AS publish
RUN dotnet publish "WeatherApiDio.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ---- Etapa 3: runtime (imagem final, enxuta) ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WeatherApiDio.dll"]
