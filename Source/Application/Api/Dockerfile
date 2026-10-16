# Image de l'API. Le contexte de build est la racine du dépôt (Library est une dépendance du projet).
#   docker build -f Source/Application/Api/Dockerfile -t adanova-api .

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restauration d'abord (couche mise en cache tant que les csproj ne changent pas)
COPY Source/Library/Library.csproj Source/Library/
COPY Source/Application/Api/Api.csproj Source/Application/Api/
RUN dotnet restore Source/Application/Api/Api.csproj

COPY Source/ Source/
RUN dotnet publish Source/Application/Api/Api.csproj \
    -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_EnableDiagnostics=0
EXPOSE 8080

COPY --from=build /app/publish .

# Clés de chiffrement du cookie de session (volume nommé, à conserver entre deux démarrages)
RUN mkdir -p /keys && chown $APP_UID /keys

# Utilisateur non-root fourni par l'image (UID 1654)
USER $APP_UID

ENTRYPOINT ["dotnet", "Api.dll"]
