FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/OakTech.FoodVansApi/OakTech.FoodVansApi.csproj", "src/OakTech.FoodVansApi/"]
COPY ["src/OakTech.FoodVansLib/OakTech.FoodVansLib.csproj", "src/OakTech.FoodVansLib/"]
RUN dotnet restore "src/OakTech.FoodVansApi/OakTech.FoodVansApi.csproj"
COPY src/OakTech.FoodVansApi ./src/OakTech.FoodVansApi
COPY src/OakTech.FoodVansLib ./src/OakTech.FoodVansLib
WORKDIR "/src/src/OakTech.FoodVansApi"
RUN dotnet build "OakTech.FoodVansApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OakTech.FoodVansApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OakTech.FoodVansApi.dll"]