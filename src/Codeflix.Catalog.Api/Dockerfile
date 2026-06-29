FROM mcr.microsoft.com/dotnet/aspnet:10 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Codeflix.Catalog.Api/Codeflix.Catalog.Api.csproj", "src/Codeflix.Catalog.Api/"]
COPY ["src/Codeflix.Catalog.Application/Codeflix.Catalog.Application.csproj", "src/Codeflix.Catalog.Application/"]
COPY ["src/Codeflix.Catalog.Domain/Codeflix.Catalog.Domain.csproj", "src/Codeflix.Catalog.Domain/"]
COPY ["src/Codeflix.Catalog.Infra.Data.EF/Codeflix.Catalog.Infra.Data.EF.csproj", "src/Codeflix.Catalog.Infra.Data.EF/"]
COPY ["src/Codeflix.Catalog.Infra.Messaging/Codeflix.Catalog.Infra.Messaging.csproj", "src/Codeflix.Catalog.Infra.Messaging/"]
COPY ["src/Codeflix.Catalog.Infra.Storage/Codeflix.Catalog.Infra.Storage.csproj", "src/Codeflix.Catalog.Infra.Storage/"]
RUN dotnet restore "src/Codeflix.Catalog.Api/Codeflix.Catalog.Api.csproj"
COPY . .
WORKDIR "/src/src/Codeflix.Catalog.Api"
RUN dotnet build "./Codeflix.Catalog.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Codeflix.Catalog.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Codeflix.Catalog.Api.dll"]
