FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Lithium.Web/Lithium.Web.csproj", "src/Lithium.Web/"]
COPY ["src/Lucide/LucideBlazor/LucideBlazor.csproj", "src/Lucide/LucideBlazor/"]
COPY ["src/Lucide/LucideBlazor.Generator/LucideBlazor.Generator.csproj", "src/Lucide/LucideBlazor.Generator/"]
RUN dotnet restore "src/Lithium.Web/Lithium.Web.csproj"
COPY . .
WORKDIR "/src/src/Lithium.Web"
RUN dotnet build "./Lithium.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Lithium.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Lithium.Web.dll"]