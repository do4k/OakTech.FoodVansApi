FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OakTech.FoodVansSlackNotifier/OakTech.FoodVansSlackNotifier.csproj", "OakTech.FoodVansSlackNotifier/"]
COPY ["OakTech.FoodVansLib/OakTech.FoodVansLib.csproj", "OakTech.FoodVansLib/"]
COPY ["OakTech.FoodVansApi/OakTech.FoodVansApi.csproj", "OakTech.FoodVansApi/"]
RUN dotnet restore "OakTech.FoodVansSlackNotifier/OakTech.FoodVansSlackNotifier.csproj"
COPY . .
WORKDIR "/src/OakTech.FoodVansSlackNotifier"
RUN dotnet build "OakTech.FoodVansSlackNotifier.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OakTech.FoodVansSlackNotifier.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OakTech.FoodVansSlackNotifier.dll"]