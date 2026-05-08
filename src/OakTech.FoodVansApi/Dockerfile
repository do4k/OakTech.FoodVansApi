FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OakTech.FoodVansApi/OakTech.FoodVansApi.csproj", "OakTech.FoodVansApi/"]
COPY ["OakTech.FoodVansLib/OakTech.FoodVansLib.csproj", "OakTech.FoodVansLib/"]
RUN dotnet restore "OakTech.FoodVansApi/OakTech.FoodVansApi.csproj"
COPY . .
WORKDIR "/src/OakTech.FoodVansApi"
RUN dotnet build "OakTech.FoodVansApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OakTech.FoodVansApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OakTech.FoodVansApi.dll"]