FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/OakTech.FoodVansSlackNotifier/OakTech.FoodVansSlackNotifier.csproj", "src/OakTech.FoodVansSlackNotifier/"]
COPY ["src/OakTech.FoodVansLib/OakTech.FoodVansLib.csproj", "src/OakTech.FoodVansLib/"]
RUN dotnet restore "src/OakTech.FoodVansSlackNotifier/OakTech.FoodVansSlackNotifier.csproj"
COPY src/OakTech.FoodVansSlackNotifier ./src/OakTech.FoodVansSlackNotifier
COPY src/OakTech.FoodVansLib ./src/OakTech.FoodVansLib
COPY src/OakTech.FoodVansApi ./src/OakTech.FoodVansApi
WORKDIR "/src/src/OakTech.FoodVansSlackNotifier"
RUN dotnet build "OakTech.FoodVansSlackNotifier.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OakTech.FoodVansSlackNotifier.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OakTech.FoodVansSlackNotifier.dll"]