# ==============================================================================
# Dockerfile Multi-etapa para Glow & Glamour - MakeupInventory (ASP.NET Core .NET 10)
# Universidad Don Bosco
# ==============================================================================

# Etapa 1: Imagen base de ejecución (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
ENV ASPNETCORE_URLS=http://+:8080

# Etapa 2: Imagen del SDK para compilación y publicación
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar csproj y restaurar dependencias
COPY ["MaquillajeInventory.csproj", "./"]
RUN dotnet restore "MaquillajeInventory.csproj"

# Copiar el resto del código y compilar
COPY . .
RUN dotnet build "MaquillajeInventory.csproj" -c Release -o /app/build

# Etapa 3: Publicación optimizada (Release)
FROM build AS publish
RUN dotnet publish "MaquillajeInventory.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 4: Imagen final ligera para producción
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MaquillajeInventory.dll"]
