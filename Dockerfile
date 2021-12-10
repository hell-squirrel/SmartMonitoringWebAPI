FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["SmartMonitoringWebAPI/SmartMonitoringWebAPI.csproj", "SmartMonitoringWebAPI/"]
RUN dotnet restore "SmartMonitoringWebAPI/SmartMonitoringWebAPI.csproj"
COPY . .
WORKDIR "/src/SmartMonitoring.Tests"
RUN dotnet test
WORKDIR "/src/SmartMonitoringWebAPI"
RUN dotnet build "SmartMonitoringWebAPI.csproj" -c Release -o /app/build


FROM build AS publish
RUN dotnet publish "SmartMonitoringWebAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartMonitoringWebAPI.dll"]