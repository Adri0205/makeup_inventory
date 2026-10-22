# ==============================================================================
# Dockerfile Multi-etapa para Glow & Glamour - MakeupInventory (ASP.NET Core .NET 10)
# Universidad Don Bosco
# ==============================================================================

# Etapa 1: Imagen base de ejecución (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Etapa 2: Imagen del SDK para compilar y publicar
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar el .csproj y restaurar dependencias
COPY ["MaquillajeInventory.csproj", "./"]
RUN dotnet restore "MaquillajeInventory.csproj"

# Copiar el resto del código y publicar en modo Release
COPY . .
RUN dotnet publish "MaquillajeInventory.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 3: Imagen final ligera
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MaquillajeInventory.dll"]