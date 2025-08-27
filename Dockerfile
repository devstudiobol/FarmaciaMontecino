#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat
# Etapa 1: Compilar
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos los archivos del proyecto
COPY *.csproj ./
RUN dotnet restore

# Copiamos el resto del código
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Etapa 2: Ejecutar
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
# Render expone el puerto con la variable PORT
ENV ASPNETCORE_URLS=http://+:${PORT}
ENTRYPOINT ["dotnet", "Farmacia.dll"]