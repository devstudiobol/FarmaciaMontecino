# Etapa 1: Compilar
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Render envía el puerto en la variable PORT
ENV ASPNETCORE_URLS=http://+:${PORT}

ENTRYPOINT ["dotnet", "Farmacia.dll"]
